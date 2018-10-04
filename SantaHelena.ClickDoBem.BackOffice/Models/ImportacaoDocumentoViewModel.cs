using System.Collections.Generic;

namespace SantaHelena.ClickDoBem.BackOffice.Models
{
    public class ImportacaoDocumentoViewModel
    {

        public string NomeArquivo { get; set; }

        public bool Sucesso { get; set; }

        public string Detalhe { get; set; }

        public IList<LinhaDocumentoViewModel> Linhas { get; set; }

    }

    public class LinhaDocumentoViewModel
    {

        public string Linha { get; set; }
        public string Conteudo { get; set; }
        public bool Sucesso { get; set; }
        public string Detalhe { get; set; }

    }

}
