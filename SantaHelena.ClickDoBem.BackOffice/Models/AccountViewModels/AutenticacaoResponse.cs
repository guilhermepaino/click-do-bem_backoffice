using System;
using System.Collections.Generic;

namespace SantaHelena.ClickDoBem.BackOffice.Models.AccountViewModels
{
    public class AutenticacaoResponse
    {

        /// <summary>
        /// Flag indicando o sucesso da autenticação
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensgaem do resultado do processamento
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Token gerado de autenticação
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Data de validade do token
        /// </summary>
        public DateTime? DataValidade { get; set; }

        /// <summary>
        /// Lista de perfis (roles)
        /// </summary>
        public IEnumerable<string> Perfis { get; set; }

    }
}
