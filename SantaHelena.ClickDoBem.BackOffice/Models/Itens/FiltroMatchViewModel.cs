using System;
using System.Collections.Generic;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class FiltroMatchViewModel
    {
        public FiltroMatchViewModel()
        {
            ItensDoacao = new List<ItemDetalheMatchViewModel>();
            ItensNecessidade = new List<ItemDetalheMatchViewModel>();
        }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public string Categoria { get; set; }

        public IEnumerable<ItemDetalheMatchViewModel> ItensDoacao { get; set; }

        public IEnumerable<ItemDetalheMatchViewModel> ItensNecessidade { get; set; }

        public string Criticas { get; set; }

    }
}
