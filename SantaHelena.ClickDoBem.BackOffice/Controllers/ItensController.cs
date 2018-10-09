using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SantaHelena.ClickDoBem.BackOffice.Models;

namespace SantaHelena.ClickDoBem.BackOffice.Controllers
{
    public class ItensController : Controller
    {
        public IActionResult Pesquisar()
        {
            return View();
        }

    }
}