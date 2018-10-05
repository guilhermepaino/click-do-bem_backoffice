namespace SantaHelena.ClickDoBem.BackOffice.Models.AccountViewModels
{
    public class AutenticacaoRequest
    {

        /// <summary>
        /// Nome de login (CPF)
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Senha de acesso MD5
        /// </summary>
        public string Senha { get; set; }

    }
}
