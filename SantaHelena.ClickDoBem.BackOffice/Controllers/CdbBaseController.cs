using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{
    public abstract class CdbBaseController : Controller
    {

        #region Objetos/Variáveis Locais

        protected readonly string _url;
        protected readonly HttpClient _client;
        protected readonly string _token;
        protected readonly IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Construtores

        public CdbBaseController(IHostingEnvironment hostingEnvironment)
        {

            _url = Environment.GetEnvironmentVariable("API_SERVER");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_url);
            _hostingEnvironment = hostingEnvironment;

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

        #endregion

    }
}
