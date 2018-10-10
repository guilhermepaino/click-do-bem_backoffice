using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class FiltroItensViewModel
    {
        public FiltroItensViewModel() { Itens = new List<ItemDetalheViewModel>(); }

        public FiltroItensViewModel(IEnumerable<ItemDetalheViewModel> itens) { Itens = itens; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public string TipoItem { get; set; }

        public string Categoria { get; set; }

        public IEnumerable<ItemDetalheViewModel> Itens { get; set; }

        public string Criticas { get; set; }

    }
}
