using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class ItemDetalheViewModel
    {

        public string TipoItem { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime? DataEfetivacao { get; set; }

        public string Doador { get; set; } // aparecer Anonônimo se aplicável
        public string TelefoneDoador { get; set; }
        public string CelularDoador { get; set; }
        public string EmailDoador { get; set; }

        public string Receptor { get; set; } // aparecer Anonônimo se aplicável
        public string TelefoneReceptor { get; set; }
        public string CelularReceptor { get; set; }
        public string EmailReceptor { get; set; }
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public string Categoria { get; set; }

        public int Peso { get; set; }

        public string ValorFaixa { get; set; }

        public bool GerenciadaRh { get; set; }


    }
}
