using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Exceptions;
using SantaHelena.ClickDoBem.BackOffice.Models.ApiDto;
using SantaHelena.ClickDoBem.BackOffice.Models.Campanha;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{
    public class CampanhaController : CdbBaseController
    {

        #region Objetos/Variáveis Locais

        #endregion

        #region Construtores

        public CampanhaController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }

        #endregion

        #region Métodos Locais

        /// <summary>
        /// Carregar as campanhas cadastradas
        /// </summary>
        protected async Task<IEnumerable<CampanhaListModel>> CarregarCampanhas()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.GetAsync("/api/v1/Campanha/todas");
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CampanhaListModel>>(conteudo);
        }

        /// <summary>
        /// Prepara SelectList de prioridade
        /// </summary>
        protected IEnumerable<SelectListItem> CarregarPrioridades(int? prioridadeSelecionada)
        {

            prioridadeSelecionada = (!prioridadeSelecionada.HasValue ? 1 : prioridadeSelecionada);

            IList<SelectListItem> campanhas = new List<SelectListItem>();
            campanhas.Add(new SelectListItem("Baixa", "0"));
            campanhas.Add(new SelectListItem("Normal", "1"));
            campanhas.Add(new SelectListItem("Alta", "2"));
            campanhas.Add(new SelectListItem("Altíssima", "3"));

            return new SelectList(campanhas, "Value", "Text", prioridadeSelecionada);

        }

        protected async Task<SimpleResponseObj> EditarRegistro(CampanhaModel model)
        {

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            var resultApi = await _client.PutAsJsonAsync("/api/v1/Campanha", model);
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SimpleResponseObj>(conteudo);

        }

        protected async Task<SimpleResponseObj> AdicionarRegistro(CampanhaModel model)
        {

            model.DataInicial = new DateTime(model.DataInicial.Value.Year, model.DataInicial.Value.Month, model.DataInicial.Value.Day, 0, 0, 0);
            model.DataFinal = new DateTime(model.DataFinal.Value.Year, model.DataFinal.Value.Month, model.DataFinal.Value.Day, 23, 59, 59);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            var resultApi = await _client.PostAsJsonAsync("/api/v1/Campanha", model);
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SimpleResponseObj>(conteudo);
        }

        /// <summary>
        /// Localizar uma campanha pelo Id
        /// </summary>
        /// <param name="id">Id da campanha</param>
        protected async Task<CampanhaModel> LocalizarCampanha(Guid id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.GetAsync($"/api/v1/Campanha/{id.ToString()}");
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CampanhaModel>(conteudo);
        }

        #endregion

        #region Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("List", await CarregarCampanhas());
        }

        [HttpGet]
        [ActionName("Editar")]
        public async Task<IActionResult> Editar(Guid id)
        {

            int? prioridade = 1;
            CampanhaModel campanha = await LocalizarCampanha(id);
            if (campanha != null)
                prioridade = campanha.Prioridade;
            ViewBag.Prioridade = CarregarPrioridades(prioridade);

            return View("Editar", campanha);
        }

        [HttpPost]
        [ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Guid id, CampanhaModel model)
        {

            ViewBag.Prioridade = CarregarPrioridades(model.Prioridade);

            if (!ModelState.IsValid)
                return View("Editar", model);

            if (model.Prioridade.Value < 0 || model.Prioridade.Value > 3)
                ModelState.AddModelError("Prioridade", "A prioridade de estar dentro da lista de escolha");

            if (ModelState.ErrorCount > 0)
                return View("Editar", model);

            SimpleResponseObj resposta = await EditarRegistro(model);
            if (resposta.Sucesso)
                return RedirectToAction("Index");

            ModelState.AddModelError(string.Empty, $"Ops, algo deu errado, {resposta.Mensagem.ToString()}");
            return View("Editar", model);


        }

        [HttpGet]
        [ActionName("Adicionar")]
        public IActionResult Adicionar()
        {
            ViewBag.Prioridade = CarregarPrioridades(null);
            return View("Adicionar", new CampanhaModel());
        }

        [HttpPost]
        [ActionName("Adicionar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(Guid id, CampanhaModel model)
        {

            ViewBag.Prioridade = CarregarPrioridades(model.Prioridade);

            if (!ModelState.IsValid)
                return View("Adicionar", model);

            if (model.Prioridade.Value < 0 || model.Prioridade.Value > 3)
                ModelState.AddModelError("Prioridade", "A prioridade de estar dentro da lista de escolha");

            if (ModelState.ErrorCount > 0)
                return View("Adicionar", model);

            SimpleResponseObj resposta = await AdicionarRegistro(model);
            if (resposta.Sucesso)
                return RedirectToAction("Index");

            ModelState.AddModelError(string.Empty, $"Ops, algo deu errado, {resposta.Mensagem.ToString()}");
            return View("Adicionar", model);


        }

        #endregion

    }
}