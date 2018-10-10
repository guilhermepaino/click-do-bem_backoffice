using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.ApiDto
{

    /// <summary>
    /// Objeto de resposta de tipo de item
    /// </summary>
    public class TipoItemResponse
    {

        /// <summary>
        /// Id do registro
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data de inclusão
        /// </summary>
        public DateTime DataInclusao { get; set; }

        /// <summary>
        /// Data da última alteração do registro
        /// </summary>
        public DateTime? DataAlteracao { get; set; }

        /// <summary>
        /// Descrição do tipo de registro
        /// </summary>
        public string Descricao { get; set; }

    }
}
