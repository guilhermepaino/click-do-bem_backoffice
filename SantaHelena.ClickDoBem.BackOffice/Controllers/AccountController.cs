using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Helpers.Security;
using SantaHelena.ClickDoBem.BackOffice.Models;
using SantaHelena.ClickDoBem.BackOffice.Models.AccountViewModels;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{
    public class AccountController : Controller
    {

        #region Objetos/Variáveis Locais

        string url;

        #endregion

        #region Construtores

        public AccountController()
        {
            url = Environment.GetEnvironmentVariable("API_SERVER");
        }

        #endregion

        #region Métodos Locais

        private async Task<AutenticacaoResponse> Autenticar(LoginViewModel model)
        {


            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(url);

                /*
                Requisição
                {
                    "nome": "admin",
                    "senha": "202cb962ac59075b964b07152d234b70",
                }

                Resposta
                { 
                    "sucesso" = "true",
                    "mensagem" = "mensagem do resultado da operação",
                    "token" = "hash informado se sucesso, caso contrário será nulo",
                    "dataValidade = "data de validade do token quando sucesso, em caso de falha será informado nulo"
                }
                */

                AutenticacaoRequest request = new AutenticacaoRequest()
                {
                    Nome = model.Login,
                    Senha = MD5.ByteArrayToString(MD5.HashMD5(model.Senha))
                };

                HttpResponseMessage result = await client.PostAsJsonAsync("/api/v1/usuario/autenticar", request);
                string conteudo = result.Content.ReadAsStringAsync().Result;
                AutenticacaoResponse response = JsonConvert.DeserializeObject<AutenticacaoResponse>(conteudo);

                return response;

            }

        }

        #endregion

        #region ActionsResults


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //var tmp = User.Identity.IsAuthenticated;
                // Chamar a API
                try
                {
                    AutenticacaoResponse resposta = await Autenticar(model);
                    if (resposta.Sucesso)
                    {
                        // Registrar o Cookie
                        IList<Claim> claims = new List<Claim>
                        {
                            new Claim("Login", model.Login),
                            new Claim("Token", resposta.Token),
                        };

                        foreach (string perfil in resposta.Perfis)
                            claims.Add(new Claim(ClaimTypes.Role, perfil));

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                            IsPersistent = true,
                            RedirectUri = "/itens/pesquisar"
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity), authProperties);

                        if (string.IsNullOrWhiteSpace(returnUrl))
                            return RedirectToAction("Pesquisar", "Itens");
                        return RedirectToPage(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, resposta.Mensagem);
                        return View(model);
                    }

                }
                catch (Exception ex)
                {
                    return View("Error", new ErrorViewModel() { Details = $"Erro: 500 - Internal Server Error => {ex.Message} - {ex.StackTrace}" });
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        #endregion

    }
}