using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class ItemDetalheMatchViewModel
    {

        public Guid Id { get; set; }

        public DateTime DataInclusao { get; set; }

        public string Nome { get; set; } // aparecer Anonônimo se aplicável

        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public string Categoria { get; set; }

        public int Peso { get; set; }

        public bool GerenciadaRh { get; set; }

        public String Telefone { get; set; }

        public String Celular { get; set; }

        public String Email { get; set; }

    }
}
