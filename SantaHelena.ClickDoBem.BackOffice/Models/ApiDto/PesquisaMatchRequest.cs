using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.ApiDto
{

    /// <summary>
    /// Objeto de requisição de filtro de item
    /// </summary>
    public class PesquisaMatchRequest
    {

        /// <summary>
        /// Data inicial do período
        /// </summary>
        public DateTime? DataInicial { get; set; }

        /// <summary>
        /// Data final do período
        /// </summary>
        public DateTime? DataFinal { get; set; }

        /// <summary>
        /// Id da categoria
        /// </summary>
        public Guid? CategoriaId { get; set; }

        /// <summary>
        /// Flag indicando se serão exibidos matches efetivados ou não (null = exibe todos)
        /// </summary>
        public bool? Efetivados { get; set; }

    }
}
