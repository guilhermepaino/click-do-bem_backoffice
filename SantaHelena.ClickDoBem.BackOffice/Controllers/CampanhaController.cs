using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Exceptions;
using SantaHelena.ClickDoBem.BackOffice.Models.ApiDto;
using SantaHelena.ClickDoBem.BackOffice.Models.Campanha;
using System;
using System.Collections.Generic;
using sys = System.IO;
using System.Linq;
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
            CampanhaModel campanha = JsonConvert.DeserializeObject<CampanhaModel>(conteudo);
            if (!string.IsNullOrWhiteSpace(campanha.Imagem))
                campanha.Imagem = $"{_url}{campanha.Imagem}";
            return campanha;
        }

        protected async Task<bool> ValidarArquivo(IFormFile arquivo)
        {
            // Salvar em pasta temporária
            string nomeArquivoBase = ContentDispositionHeaderValue.Parse(arquivo.ContentDisposition).FileName.Trim('"');

            // Validando tipo de arquivo pela extenção
            string extensao = nomeArquivoBase.Split('.').LastOrDefault().ToLower();
            if (string.IsNullOrWhiteSpace(extensao) || !"jpg|jpeg|png".Contains(extensao.ToLower()))
                return false;
            return true;
        }

        protected async Task<SimpleResponse> CarregarArquivo(Guid id, IFormFile arquivo)
        {

            // Salvar em pasta temporária
            string hashId = $"{(DateTime.Now.Ticks * (new Random().Next(2, 16))).ToString().Substring(0, 6)}".Replace("-", string.Empty);
            string dataArquivo = DateTime.Now.ToString("yyyyMMddhhmmss");
            string nomeArquivoFileServer = $"{hashId}-{dataArquivo}_{id.ToString()}.jpg";
            string nomeCompleto = sys.Path.Combine(_caminho, nomeArquivoFileServer);

            // Validando tamanho do arquivo
            using (sys.FileStream stream = new sys.FileStream(nomeCompleto, sys.FileMode.Create))
            {
                arquivo.CopyTo(stream);
            }

            // Converter em Base64
            byte[] bytes = sys.File.ReadAllBytes(nomeCompleto);
            string arquivoImagemBase64 = Convert.ToBase64String(bytes);

            try { System.IO.File.Delete(nomeCompleto); }
            finally { /* Nada a fazer */ }

            // Enviar para Api
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
            HttpResponseMessage resultApi = await _client.PostAsJsonAsync($"/api/v1/Campanha/Imagem", new { CampanhaId = id.ToString(), ImagemBase64 = arquivoImagemBase64 });
            if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
            string conteudo = await resultApi.Content.ReadAsStringAsync();

            SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(conteudo);
            return response;

        }

        #endregion

        #region Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View("List", await CarregarCampanhas());
            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }
        }

        [HttpGet]
        [ActionName("Editar")]
        public async Task<IActionResult> Editar(Guid id)
        {
            try
            {
                int? prioridade = 1;
                CampanhaModel campanha = await LocalizarCampanha(id);
                if (campanha != null)
                    prioridade = campanha.Prioridade;
                ViewBag.Prioridade = CarregarPrioridades(prioridade);

                return View("Editar", campanha);
            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }
        }

        [HttpPost]
        [ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Guid id, CampanhaModel model)
        {

            try
            {

                ViewBag.Prioridade = CarregarPrioridades(model.Prioridade);

                if (!ModelState.IsValid)
                    return View("Editar", model);

                if (model.Prioridade.Value < 0 || model.Prioridade.Value > 3)
                    ModelState.AddModelError("Prioridade", "A prioridade de estar dentro da lista de escolha");

                if (model.ImgCampanha != null && model.ImgCampanha.Length > 524288)
                    ModelState.AddModelError("ImgCampanha", "Tamanho máximo de arquivo excedido (500KBytes)");

                if (ModelState.ErrorCount > 0)
                    return View("Editar", model);

                SimpleResponseObj resposta = await EditarRegistro(model);
                if (resposta.Sucesso)
                {

                    if (model.ImgCampanha != null)
                    {

                        SimpleResponse respImg = await CarregarArquivo(model.Id.Value, model.ImgCampanha);
                        if (!respImg.Sucesso)
                        {
                            ViewBag.Prioridade = CarregarPrioridades(model.Prioridade);
                            model.Criticas = "Falha ao carregar arquivo";
                            return View("Editar", model);
                        }
                    }

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, $"Ops, algo deu errado, {resposta.Mensagem.ToString()}");
                return View("Editar", model);

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }

        [HttpGet]
        [ActionName("Adicionar")]
        public IActionResult Adicionar()
        {
            try
            {
                ViewBag.Prioridade = CarregarPrioridades(null);
                return View("Adicionar", new CampanhaModel());
            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }
        }

        [HttpPost]
        [ActionName("Adicionar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(Guid id, CampanhaModel model)
        {

            try
            {

                if (!ModelState.IsValid)
                    return View("Adicionar", model);

                if (model.Prioridade.Value < 0 || model.Prioridade.Value > 3)
                    ModelState.AddModelError("Prioridade", "A prioridade de estar dentro da lista de escolha");

                if (model.ImgCampanha == null || model.ImgCampanha.Length.Equals(0))
                {
                    ModelState.AddModelError("ImgCampanha", "Arquivo de imagem inválido ou não informado");
                }
                else if (model.ImgCampanha.Length > 524288)
                {
                    ModelState.AddModelError("ImgCampanha", "Tamanho máximo de arquivo excedido (500KBytes)");
                }
                else
                {
                    if (!(await ValidarArquivo(model.ImgCampanha)))
                        ModelState.AddModelError("ImgCampanha", "Formato de arquivo inválido");
                }

                if (ModelState.ErrorCount > 0)
                {
                    ViewBag.Prioridade = CarregarPrioridades(model.Prioridade);
                    return View("Adicionar", model);
                }

                SimpleResponseObj resposta = await AdicionarRegistro(model);
                if (resposta.Sucesso)
                {
                    CampanhaModel campanha = JsonConvert.DeserializeObject<CampanhaModel>(resposta.Mensagem.ToString());
                    SimpleResponse respImg = await CarregarArquivo(campanha.Id.Value, model.ImgCampanha);

                    if (respImg.Sucesso)
                        return RedirectToAction("Index");

                    campanha = await LocalizarCampanha(campanha.Id.Value);
                    ViewBag.Prioridade = CarregarPrioridades(campanha.Prioridade);
                    model.Criticas = "Falha ao carregar arquivo";

                    return View("Editar", model);


                }

                ModelState.AddModelError(string.Empty, $"Ops, algo deu errado, {resposta.Mensagem.ToString()}");
                return View("Adicionar", model);

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }


        [HttpGet]
        [ActionName("Upload")]
        public async Task<IActionResult> Upload(Guid id)
        {

            try
            {

                CampanhaModel campanha = await LocalizarCampanha(id);
                ViewBag.Prioridade = CarregarPrioridades(campanha.Prioridade);
                return View("Upload", campanha);

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }

        [HttpPost, DisableRequestSizeLimit]
        [ActionName("Upload")]
        public async Task<IActionResult> Upload(Guid id, IFormFile arquivo)
        {

            try
            {

                CampanhaModel campanha = await LocalizarCampanha(id);
                ViewBag.Prioridade = CarregarPrioridades(campanha.Prioridade);

                if (arquivo == null || arquivo.Length.Equals(0))
                {
                    campanha.Criticas = "Arquivo inválido ou vazio";
                    return View("Upload", campanha);
                }

                // Salvar em pasta temporária
                string nomeArquivoBase = ContentDispositionHeaderValue.Parse(arquivo.ContentDisposition).FileName.Trim('"');

                string hashId = $"{(DateTime.Now.Ticks * (new Random().Next(2, 16))).ToString().Substring(0, 6)}".Replace("-", string.Empty);
                string dataArquivo = DateTime.Now.ToString("yyyyMMddhhmmss");

                string nomeArquivoFileServer = $"{hashId}-{dataArquivo}_{id.ToString()}.jpg";

                string nomeCompleto = sys.Path.Combine(_caminho, nomeArquivoFileServer);

                // Validando tipo de arquivo pela extenção
                string extensao = nomeArquivoBase.Split('.').LastOrDefault().ToLower();
                if (string.IsNullOrWhiteSpace(extensao) || !"jpg|jpeg|png".Contains(extensao.ToLower()))
                {
                    campanha.Criticas = "Formato de arquivo inválido";
                    return View("Upload", campanha);
                }

                // Validando tamanho do arquivo
                using (sys.FileStream stream = new sys.FileStream(nomeCompleto, sys.FileMode.Create))
                {
                    arquivo.CopyTo(stream);
                }

                // Converter em Base64
                byte[] bytes = sys.File.ReadAllBytes(nomeCompleto);
                string arquivoImagemBase64 = Convert.ToBase64String(bytes);

                try { System.IO.File.Delete(nomeCompleto); }
                finally { /* Nada a fazer */ }

                // Enviar para Api
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ObterToken());
                HttpResponseMessage resultApi = await _client.PostAsJsonAsync($"/api/v1/Campanha/Imagem", new { CampanhaId = id.ToString(), ImagemBase64 = arquivoImagemBase64 });
                if (resultApi.StatusCode.Equals(HttpStatusCode.Unauthorized)) throw new SessaoExpiradaException();
                string conteudo = await resultApi.Content.ReadAsStringAsync();

                SimpleResponse response = JsonConvert.DeserializeObject<SimpleResponse>(conteudo);
                if (!response.Sucesso)
                {
                    campanha.Criticas = response.Mensagem;
                    return View("Upload", campanha);
                }

                // Redirecionar para Listagem
                return View("List", await CarregarCampanhas());

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }

        #endregion

    }
}