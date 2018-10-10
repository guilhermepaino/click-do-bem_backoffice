using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantaHelena.ClickDoBem.BackOffice.Models.ApiDto
{

    /// <summary>
    /// Objeto de rsposta de categoria
    /// </summary>
    public class CategoriaResponse
    {

        /// <summary>
        /// Id do registro
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data de inclusão do registro
        /// </summary>
        public DateTime DataInclusao { get; set; }

        /// <summary>
        /// Data da última alteração do registro
        /// </summary>
        public DateTime? DataAlteracao { get; set; }

        /// <summary>
        /// Descrição da categoria
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Pontuação da categoria
        /// </summary>
        public int Pontuacao { get; set; }

        /// <summary>
        /// Flag indicando se a categoria é gerida pelo RH
        /// </summary>
        public bool GerenciadaRh { get; set; }

    }

}