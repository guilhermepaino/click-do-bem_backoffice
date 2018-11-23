using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Exceptions;
using SantaHelena.ClickDoBem.BackOffice.Models;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{
    public class ImportacoesController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportarArquivo()
        {

            try
            {

                IFormFile arquivo = null;
                string url = Environment.GetEnvironmentVariable("API_SERVER");

                try { arquivo = Request.Form.Files.FirstOrDefault(); }
                finally { /* Nada a fazer */ }

                if (arquivo == null || arquivo.Length.Equals(0))
                    return View("Error", new ErrorViewModel() { Details = "Arquivo inválido ou não enviado" });


                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(url);

                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", arquivo.ContentType);


                        byte[] data;
                        using (var br = new BinaryReader(arquivo.OpenReadStream()))
                            data = br.ReadBytes((int)arquivo.OpenReadStream().Length);

                        ByteArrayContent bytes = new ByteArrayContent(data);


                        MultipartFormDataContent multiContent = new MultipartFormDataContent();

                        multiContent.Add(bytes, "file", arquivo.FileName);

                        var result = client.PostAsync("api/v1/colaborador/upload", multiContent).Result;
                        string conteudo = result.Content.ReadAsStringAsync().Result;

                        ImportacaoDocumentoViewModel detalheResultado = JsonConvert.DeserializeObject<ImportacaoDocumentoViewModel>(conteudo);

                        return View("UpLoadResult", detalheResultado);

                    }
                    catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }
                    catch (Exception ex)
                    {
                        return View("Error", new ErrorViewModel() { Details = $"Erro: 500 - Internal Server Error => {ex.Message} - {ex.StackTrace}" });
                    }
                }

            }
            catch (SessaoExpiradaException) { return RedirectToAction("Logout", "Account"); }

        }

    }
}