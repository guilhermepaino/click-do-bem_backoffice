using SantaHelena.ClickDoBem.BackOffice.Models.ApiDto;
using System;
using System.Collections.Generic;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class EfetivaMatchViewModel
    {
        public EfetivaMatchViewModel()
        {
            Matches = new List<PesquisaMatchResponse>();
        }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public string Categoria { get; set; }

        public int Efetivacao { get; set; }

        public IEnumerable<PesquisaMatchResponse> Matches { get; set; }

        public string Criticas { get; set; }

    }
}
