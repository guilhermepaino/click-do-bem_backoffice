using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            List<TipoItemResponse> responseApi = JsonConvert.DeserializeObject<List<TipoItemResponse>>(conteudo);
            responseApi.ForEach(i =>
            {
                itens.Add(new SelectListItem(i.Descricao, i.Id.ToString()));
            });

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
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            List<CategoriaResponse> responseApi = JsonConvert.DeserializeObject<List<CategoriaResponse>>(conteudo);
            responseApi.ForEach(i =>
            {
                itens.Add(new SelectListItem(i.Descricao, i.Id.ToString()));
            });

            return new SelectList(itens, "Value", "Text", categoriaSelecionada);

        }

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

            await CarregarDropDown(null, null);
            return View(new FiltroItensViewModel());

        }

        [HttpPost]
        public async Task<IActionResult> Filtrar(FiltroItensViewModel model)
        {

            await CarregarDropDown(model.TipoItem, model.Categoria);

            // Validações
            if (!(model.DataInicial == null && model.DataFinal == null ))
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
                await ExecutaPesquisa(model);

            return View("Pesquisar", model);

        }

        #endregion


    }
}