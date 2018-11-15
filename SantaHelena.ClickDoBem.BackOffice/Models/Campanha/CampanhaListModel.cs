using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Campanha
{
    public class CampanhaListModel
    {

        public Guid Id { get; set; }

        public string Descricao { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int Prioridade { get; set; }

        public string Situacao
        {
            get
            {
                if (DataFinal < DateTime.Now)
                    return "Encerrada";

                if (DataInicial > DateTime.Now)
                    return "Futura";

                return "Vigente";
            }
        }

        public string PrioridadeDescricao
        {
            get
            {
                if (Prioridade.Equals(0)) return "Baixa";
                if (Prioridade.Equals(1)) return "Normal";
                if (Prioridade.Equals(2)) return "Alta";
                return "Altíssima";
            }
        }

    }
}
