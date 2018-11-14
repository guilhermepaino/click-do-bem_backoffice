using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.ApiDto
{

    /// <summary>
    /// Objeto de requisição de filtro de item
    /// </summary>
    public class PesquisaMatchResponse
    {

        /// <summary>
        /// Id do Match
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data do Match
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Tipo de match
        /// </summary>
        public string TipoMatch { get; set; }

        /// <summary>
        /// Nome do doador
        /// </summary>
        public string NomeDoador { get; set; }

        /// <summary>
        /// Nome do receptor
        /// </summary>
        public string NomeReceptor { get; set; }

        /// <summary>
        /// Título do item
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descrição do item
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Categoria do item
        /// </summary>
        public string Categoria { get; set; }

        /// <summary>
        /// Faixa de valor do item
        /// </summary>
        public string ValorFaixa { get; set; }

        /// <summary>
        /// Pontuação da categoria
        /// </summary>
        public int Pontuacao { get; set; }

        /// <summary>
        /// Flag de gerenciamento pelo RH
        /// </summary>
        public bool GerenciadaRh { get; set; }

        /// <summary>
        /// Flag de efetivação
        /// </summary>
        public bool Efetivado { get; set; }

        /// <summary>
        /// Caminho da primeira imagem, se disponível
        /// </summary>
        public string Imagem { get; set; }

    }
}
