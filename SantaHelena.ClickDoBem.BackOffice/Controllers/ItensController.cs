using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Exceptions;
using SantaHelena.ClickDoBem.BackOffice.Models;
using SantaHelena.ClickDoBem.BackOffice.Models.ApiDto;
using SantaHelena.ClickDoBem.BackOffice.Models.Itens;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{

    public class ItensController : Controller
    {

        #region Objetos/Variáveis Locais

        protected readonly string _url;
        protected readonly HttpClient _client;
        protected readonly string _token;

        #endregion

        #region Construtores

        public ItensController()
        {
            _url = Environment.GetEnvironmentVariable("API_SERVER");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_url);
        }

        #endregion

        #region Métodos Locais

        /// <summary>
        /// Obter o token do usuário autenticado
        /// </summary>
        protected string ObterToken()
        {
            IEnumerable<Claim> claims = User.Claims;

            Claim token = User.Claims.Where(x => x.Type.Equals("Token")).FirstOrDefault();

            if (token == null)
                return string.Empty;

            return token.Value;
        }

        /// <summary>
        /// Executa a chamada a API para efetivar o match
        /// </summary>
        /// <param name="necessidadeId">Id do item de necessidade</param>
        /// <param name="doacaoId">Id do item de doação</param>
        protected async Task<SimpleResponse> ExecutaMatch(Guid necessidadeId, Guid doacaoId)
        {

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.PostAsJsonAsync("/api/v1/Item/match", new { DoacaoId = doacaoId, NecessidadeId = necessidadeId });
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();

            SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(conteudo);

            return response;

        }

        /// <summary>
        /// Executar a pesquisa com fase nos filtros
        /// </summary>
        /// <param name="filtro">Objeto modelo de filtragem</param>
        protected async Task ExecutaPesquisa(FiltroItensViewModel filtro)
        {

            Guid? tipoItemId = null;
            Guid? categoriaId = null;

            if (!string.IsNullOrWhiteSpace(filtro.TipoItem) && !filtro.TipoItem.Equals("-"))
                tipoItemId = Guid.Parse(filtro.TipoItem);

            if (!string.IsNullOrWhiteSpace(filtro.Categoria) && !filtro.Categoria.Equals("-"))
                categoriaId = Guid.Parse(filtro.Categoria);

            FiltroItemRequest request = new FiltroItemRequest()
            {
                DataInicial = filtro.DataInicial,
                DataFinal = filtro.DataFinal,
                TipoItemId = tipoItemId,
                CategoriaId = categoriaId
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.PostAsJsonAsync("/api/v1/Item/pesquisar", request);
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();

            filtro.Itens = JsonConvert.DeserializeObject<List<ItemDetalheViewModel>>(conteudo);


        }

        /// <summary>
        /// Carregar tipos de itens disponíveis
        /// </summary>
        protected async Task<IEnumerable<SelectListItem>> CarregarTipoItem(string tipoItemSelecionado)
        {

            IList<SelectListItem> itens = new List<SelectListItem>();
            itens.Add(new SelectListItem("Todos", "-"));

            tipoItemSelecionado = (string.IsNullOrWhiteSpace(tipoItemSelecionado) ? "-" : tipoItemSelecionado);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.GetAsync("/api/v1/TipoItem");
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            List<TipoItemResponse> responseApi = JsonConvert.DeserializeObject<List<TipoItemResponse>>(conteudo);
            if (responseApi != null)
            {
                responseApi.ForEach(i =>
                {
                    itens.Add(new SelectListItem(i.Descricao, i.Id.ToString()));
                });
            }

            return new SelectList(itens, "Value", "Text", tipoItemSelecionado);

        }

        /// <summary>
        /// Carregar categorias disponíveis
        /// </summary>
        private async Task<IEnumerable<SelectListItem>> CarregarCategoria(string categoriaSelecionada)
        {

            IList<SelectListItem> itens = new List<SelectListItem>();
            itens.Add(new SelectListItem("Todas", "-"));

            categoriaSelecionada = (string.IsNullOrWhiteSpace(categoriaSelecionada) ? "-" : categoriaSelecionada);

            HttpResponseMessage resultApi = await _client.GetAsync("/api/v1/Categoria");
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            List<CategoriaResponse> responseApi = JsonConvert.DeserializeObject<List<CategoriaResponse>>(conteudo);
            if (responseApi != null)
            {
                responseApi.ForEach(i =>
                {
                    itens.Add(new SelectListItem(i.Descricao, i.Id.ToString()));
                });
            }

            return new SelectList(itens, "Value", "Text", categoriaSelecionada);

        }

        /// <summary>
        /// Carregar as doações e necessidades
        /// </summary>
        private async Task CarregarDoacoesNecessidades(FiltroMatchViewModel filtro)
        {

            // Carregando Doações
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.GetAsync("/api/v1/Item");
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();

            List<ItemResponse> itens = JsonConvert.DeserializeObject<List<ItemResponse>>(conteudo);

            List<ItemDetalheMatchViewModel> doacoes = new List<ItemDetalheMatchViewModel>();
            List<ItemDetalheMatchViewModel> necessidades = new List<ItemDetalheMatchViewModel>();

            foreach (var item in itens)
            {

                ItemDetalheMatchViewModel tmpItem = new ItemDetalheMatchViewModel()
                {
                    Id = item.Id,
                    DataInclusao = item.DataInclusao,
                    Nome = (item.Anonimo ? "** ANÕNIMO **" : item.Usuario.Nome),
                    Titulo = item.Titulo,
                    Descricao = item.Descricao,
                    Categoria = item.Categoria.Descricao,
                    Peso = item.Categoria.Pontuacao,
                    GerenciadaRh = item.Categoria.GerenciadaRh
                };

                if (item.TipoItem.ToLower().Equals("necessidade"))
                    necessidades.Add(tmpItem);
                else
                    doacoes.Add(tmpItem);

            }

            filtro.ItensDoacao = doacoes;
            filtro.ItensNecessidade = necessidades;

        }

        /// <summary>
        /// Carregar os dados para alimentar as DropDown de TipoItem e Categoria
        /// </summary>
        /// <param name="tipoItemSelecionado">Id do tipo de item selecionado</param>
        /// <param name="categoriaSelecionada">Id da categoria selecionada</param>
        private async Task CarregarDropDown(string tipoItemSelecionado, string categoriaSelecionada)
        {
            // SelectLists
            ViewBag.TipoItem = await CarregarTipoItem(tipoItemSelecionado);
            ViewBag.Categoria = await CarregarCategoria(categoriaSelecionada);
        }

        #endregion


        #region ActionResults

        public async Task<IActionResult> Pesquisar()
        {

            try
            {
                await CarregarDropDown(null, null);
                return View(new FiltroItensViewModel());
            }
            catch (SessaoExpiradaException)
            {
                return RedirectToAction("Logout", "Account");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Filtrar(FiltroItensViewModel model)
        {

            await CarregarDropDown(model.TipoItem, model.Categoria);

            // Validações
            if (!(model.DataInicial == null && model.DataFinal == null))
            {
                if (model.DataInicial == null || model.DataFinal == null)
                {
                    model.Criticas = "Para filtrar por período é necessário preencher os dois campos";
                }
                else
                {
                    if (model.DataInicial.Value > model.DataFinal.Value)
                    {
                        model.Criticas = "A data inicial não pode ser superior a data final";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(model.Criticas))
            {
                try
                {
                    await ExecutaPesquisa(model);
                }
                catch (SessaoExpiradaException)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }

            return View("Pesquisar", model);

        }

        public async Task<IActionResult> Matches(FiltroMatchViewModel model)
        {
            try
            {
                await CarregarDropDown(null, null);
                return View(new FiltroMatchViewModel());
            }
            catch (SessaoExpiradaException)
            {
                return RedirectToAction("Logout", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FiltrarMatches(FiltroMatchViewModel model)
        {

            await CarregarDropDown(null, model.Categoria);

            // Validações
            if (!(model.DataInicial == null && model.DataFinal == null))
            {
                if (model.DataInicial == null || model.DataFinal == null)
                {
                    model.Criticas = "Para filtrar por período é necessário preencher os dois campos";
                }
                else
                {
                    if (model.DataInicial.Value > model.DataFinal.Value)
                    {
                        model.Criticas = "A data inicial não pode ser superior a data final";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(model.Criticas))
            {
                try
                {
                    await CarregarDoacoesNecessidades(model);
                }
                catch (SessaoExpiradaException)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }

            return View("Matches", model);

        }

        [HttpPost]
        public async Task<IActionResult> RealizarMatch([FromBody]MatchRequest request)
        {
            //await CarregarDropDown(null, null);
            //return View(new FiltroMatchViewModel());

            SimpleResponse response = new SimpleResponse();

            if (request.DoacaoId == null || request.DoacaoId == null)
            {
                response.Sucesso = false;
                response.Mensagem = "É necessário informar o id do item de necesside e de doação";
            }

            try
            {
                response = await ExecutaMatch(request.NecessidadeId.Value, request.DoacaoId.Value);
            }
            catch (SessaoExpiradaException)
            {
                return RedirectToAction("Logout", "Account");
            }
            
            response.Sucesso = true;


            return StatusCode(StatusCodes.Status200OK, response);

        }

        #endregion

    }
}