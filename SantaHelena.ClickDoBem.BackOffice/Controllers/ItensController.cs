using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Exceptions;
using SantaHelena.ClickDoBem.BackOffice.Helpers.Planilha;
using SantaHelena.ClickDoBem.BackOffice.Models.ApiDto;
using SantaHelena.ClickDoBem.BackOffice.Models.Itens;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{

    public class ItensController : CdbBaseController
    {

        #region Construtores

        public ItensController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }

        #endregion

        #region Métodos Locais

        /// <summary>
        /// Efetiva o match informado
        /// </summary>
        /// <param name="id">Id do match</param>
        protected async Task<SimpleResponse> EfetivaMatch(Guid id)
        {

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.PostAsJsonAsync($"/api/v1/Item/match/efetivar/{id}", new { Id = id });
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();

            SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(conteudo);

            return response;

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
        /// Carregar as opções de filtragem de efetivação
        /// </summary>
        /// <param name="efetivacaoEscolhida">Efetivação escolhida</param>
        private IEnumerable<SelectListItem> CarregarEfetivacao(int? efetivacaoEscolhida)
        {
            IList<SelectListItem> selItens = new List<SelectListItem>();

            selItens.Add(new SelectListItem("Todos", "0"));
            selItens.Add(new SelectListItem("Pendentes", "1"));
            selItens.Add(new SelectListItem("Efetivados", "2"));

            return new SelectList(selItens, "Value", "Text", efetivacaoEscolhida?.ToString());
        }

        /// <summary>
        /// Carregar as doações e necessidades
        /// </summary>
        private async Task CarregarDoacoesNecessidades(FiltroMatchViewModel filtro)
        {

            Guid? categoriaId = null;

            if (!string.IsNullOrWhiteSpace(filtro.Categoria) && !filtro.Categoria.Equals("-"))
                categoriaId = Guid.Parse(filtro.Categoria);

            FiltroItemRequest request = new FiltroItemRequest()
            {
                DataInicial = filtro.DataInicial,
                DataFinal = filtro.DataFinal,
                TipoItemId = null,
                CategoriaId = categoriaId
            };

            // Carregando Doações
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.PostAsJsonAsync("/api/v1/Item/livres", request);
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
                    Nome = item.Usuario.Nome,
                    Titulo = item.Titulo,
                    Descricao = item.Descricao,
                    Categoria = item.Categoria.Descricao,
                    Peso = item.Categoria.Pontuacao,
                    GerenciadaRh = item.Categoria.GerenciadaRh
                };

                if (item.TipoItem.Descricao.ToLower().Equals("necessidade"))
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
        private async Task CarregarDropDown(string tipoItemSelecionado, string categoriaSelecionada, int? efetivacao)
        {
            // SelectLists
            ViewBag.TipoItem = await CarregarTipoItem(tipoItemSelecionado);
            ViewBag.Categoria = await CarregarCategoria(categoriaSelecionada);
            ViewBag.Efetivacao = CarregarEfetivacao(efetivacao);
        }

        /// <summary>
        /// Carregar os matches
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task CarregarMatches(EfetivaMatchViewModel filtro)
        {

            Guid? categoriaId = null;

            if (!string.IsNullOrWhiteSpace(filtro.Categoria) && !filtro.Categoria.Equals("-"))
                categoriaId = Guid.Parse(filtro.Categoria);

            bool? efetivados;
            switch (filtro.Efetivacao)
            {
                case 1:
                    efetivados = false;
                    break;
                case 2:
                    efetivados = true;
                    break;
                default:
                    efetivados = null;
                    break;
            }

            PesquisaMatchRequest request = new PesquisaMatchRequest()
            {
                DataInicial = filtro.DataInicial,
                DataFinal = filtro.DataFinal,
                CategoriaId = categoriaId,
                Efetivados = efetivados
            };

            // Carregando Doações
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.PostAsJsonAsync("/api/v1/Item/match/listar", request);

            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();

            List<PesquisaMatchResponse> matches = JsonConvert.DeserializeObject<List<PesquisaMatchResponse>>(conteudo);

            filtro.Matches = matches;

        }

        #endregion

        #region Actions de Itens

        public async Task<IActionResult> Pesquisar()
        {

            try
            {
                await CarregarDropDown(null, null, null);
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

            await CarregarDropDown(model.TipoItem, model.Categoria, null);

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

        #endregion

        #region Actions de Matches

        public async Task<IActionResult> Matches(FiltroMatchViewModel model)
        {
            try
            {
                await CarregarDropDown(null, null, null);
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

            await CarregarDropDown(null, model.Categoria, null);

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

            SimpleResponse response = new SimpleResponse();

            if (request.DoacaoId == null || request.DoacaoId == null)
            {
                response.Sucesso = false;
                response.Mensagem = "É necessário informar o id do item de necesside e de doação";
            }
            else
            {
                try
                {
                    response = await ExecutaMatch(request.NecessidadeId.Value, request.DoacaoId.Value);
                }
                catch (SessaoExpiradaException)
                {
                    return RedirectToAction("Logout", "Account");
                }

                response.Sucesso = true;

            }


            return StatusCode(StatusCodes.Status200OK, response);

        }

        public async Task<IActionResult> EfetivarMatch()
        {
            try
            {
                await CarregarDropDown(null, null, null);
                return View(new EfetivaMatchViewModel());
            }
            catch (SessaoExpiradaException)
            {
                return RedirectToAction("Logout", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PesquisarMatches(EfetivaMatchViewModel model)
        {

            await CarregarDropDown(null, model.Categoria, model.Efetivacao);

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
                    await CarregarMatches(model);
                    return View("EfetivarMatch", model);
                }
                catch (SessaoExpiradaException)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }

            return View("EfetivarMatch", model);

        }

        [HttpPost]
        public async Task<IActionResult> EfetivarMatch([FromBody]EfetivarMatchRequest request)
        {

            SimpleResponse response = new SimpleResponse();

            if (request.Id == null)
            {
                response.Sucesso = false;
                response.Mensagem = "O id do item deve ser informado";
            }
            else
            {

                try
                {
                    response = await EfetivaMatch(request.Id.Value);
                }
                catch (SessaoExpiradaException)
                {
                    return RedirectToAction("Logout", "Account");
                }

                response.Sucesso = true;

            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpGet]
        public IActionResult DownloadRelatorioItem([FromQuery]DateTime? dataInicial, DateTime? dataFinal, string tipoItem, string categoria)
        {

            try
            {

                FiltroItensViewModel model = new FiltroItensViewModel()
                {
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    TipoItem = tipoItem,
                    Categoria = categoria
                };

                ExecutaPesquisa(model).Wait();

                IList<ReportPlanilhaItemViewModel> registros = new List<ReportPlanilhaItemViewModel>();

                model.Itens.ToList().ForEach(i =>
                {
                    registros.Add
                    (
                        new ReportPlanilhaItemViewModel()
                        {
                            TipoItem = i.TipoItem,
                            DataInclusao = i.DataInclusao,
                            DataEfetivacao = i.DataEfetivacao,
                            Doador = i.Doador,
                            Receptor = i.Receptor,
                            Titulo = i.Titulo,
                            Descricao = i.Descricao,
                            Categoria = i.Categoria,
                            Peso = i.Peso,
                            ValorFaixa = i.ValorFaixa,
                            GerenciadaRh = i.GerenciadaRh
                        }
                    );
                });

                string caminho = Directory.GetDirectories(_hostingEnvironment.WebRootPath).Where(x => x.EndsWith("tmp")).SingleOrDefault();
                string nomeBase = $"relatorio-itens-{Guid.NewGuid()}.xlsx";

                string nomeArquivo = string.Empty;
                using (Excel excel = new Excel(caminho))
                {
                    nomeArquivo = Path.GetFileName(excel.Exportar(nomeBase, "Relatorio", registros, true, OfficeOpenXml.Table.TableStyles.Medium16, "TabRelatorio"));
                }

                IFileProvider provider = new PhysicalFileProvider(Path.Combine(caminho));
                IFileInfo fileInfo = provider.GetFileInfo(nomeArquivo);
                Stream readStream = fileInfo.CreateReadStream();
                string mimeType = "application/vnd.ms-excel";

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(10000);
                    System.IO.File.Delete(Path.Combine(caminho, nomeArquivo));
                });

                return File(readStream, mimeType, nomeArquivo);

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }

        [HttpGet]
        public IActionResult DownloadRelatorioMatch([FromQuery]DateTime? dataInicial, DateTime? dataFinal, string categoria, int efetivacao)
        {

            try
            {

                EfetivaMatchViewModel model = new EfetivaMatchViewModel()
                {
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    Categoria = categoria,
                    Efetivacao = efetivacao
                };

                CarregarMatches(model).Wait();

                IList<ReportPlanilhaMatchViewModel> registros = new List<ReportPlanilhaMatchViewModel>();

                model.Matches.ToList().ForEach(m =>
                {
                    registros.Add
                    (
                        new ReportPlanilhaMatchViewModel()
                        {
                            Id = m.Id,
                            Data = m.Data,
                            TipoMatch = m.TipoMatch,
                            ContatoDoador = $"{m.NomeDoador}\n{m.TelefoneDoador}\n{m.CelularDoador}\n{m.EmailDoador}",
                            ContatoReceptor = $"{m.NomeReceptor}\n{m.TelefoneReceptor}\n{m.CelularReceptor}\n{m.EmailReceptor}",
                            DadosItem = $"Título: {m.Titulo}\nDescrição: {m.Descricao}\nCategoria: {m.Categoria}\nValor: {m.ValorFaixa}\nGerenciada RH: {(m.GerenciadaRh ? "Sim" : "Não")}",
                            // Titulo = m.Titulo,
                            // Descricao = m.Descricao,
                            // Categoria = m.Categoria,
                            Pontuacao = m.Pontuacao,
                            // ValorFaixa = m.ValorFaixa,
                            // GerenciadaRh = m.GerenciadaRh,
                            Efetivado = m.Efetivado
                        }
                    );
                });

                string caminho = Directory.GetDirectories(_hostingEnvironment.WebRootPath).Where(x => x.EndsWith("tmp")).SingleOrDefault();
                string nomeBase = $"relatorio-matches-{Guid.NewGuid()}.xlsx";

                string nomeArquivo = string.Empty;
                using (Excel excel = new Excel(caminho))
                {
                    nomeArquivo = Path.GetFileName(excel.Exportar(nomeBase, "Relatorio", registros, true, OfficeOpenXml.Table.TableStyles.Medium16, "TabRelatorio"));
                }

                IFileProvider provider = new PhysicalFileProvider(Path.Combine(caminho));
                IFileInfo fileInfo = provider.GetFileInfo(nomeArquivo);
                Stream readStream = fileInfo.CreateReadStream();
                string mimeType = "application/vnd.ms-excel";

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(10000);
                    System.IO.File.Delete(Path.Combine(caminho, nomeArquivo));
                });

                return File(readStream, mimeType, nomeArquivo);

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }

        #endregion

    }
}