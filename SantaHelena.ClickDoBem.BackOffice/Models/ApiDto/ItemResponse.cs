using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.ApiDto
{

    public class ItemResponse
    {

        public Guid Id { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string TipoItem { get; set; }
        public CategoriaResponse Categoria { get; set; }
        public UsuarioResponse Usuario { get; set; }
        public bool Anonimo { get; set; }

    }

}
