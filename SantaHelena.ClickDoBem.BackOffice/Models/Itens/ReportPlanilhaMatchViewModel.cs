using OfficeOpenXml.Style;
using SantaHelena.ClickDoBem.BackOffice.Helpers.Planilha;
using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class ReportPlanilhaMatchViewModel
    {

        [ExcelColumn(
            Order = 1,
            Title = "Id",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center
        )]
        public Guid Id { get; set; }

        [ExcelColumn(
            Order = 2,
            Title = "Data",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center,
            NumberFormat = "dd/MM/yyyy"
        )]
        public DateTime Data { get; set; }

        [ExcelColumn(
            Order = 3,
            Title = "Tipo Match",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center
        )]
        public string TipoMatch { get; set; }

        [ExcelColumn(
            Order = 4,
            Title = "Doador",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string NomeDoador { get; set; }

        [ExcelColumn(
            Order = 5,
            Title = "Receptor",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string NomeReceptor { get; set; }

        [ExcelColumn(
            Order = 6,
            Title = "Título",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string Titulo { get; set; }

        [ExcelColumn(
            Order = 7,
            Title = "Descrição",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string Descricao { get; set; }

        [ExcelColumn(
            Order = 8,
            Title = "Categoria",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string Categoria { get; set; }

        [ExcelColumn(
            Order = 9,
            Title = "Pontuação",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center
        )]
        public int Pontuacao { get; set; }

        [ExcelColumn(
            Order = 10,
            Title = "Faixa Valor",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string ValorFaixa { get; set; }

        [ExcelColumn(
            Order = 11,
            Title = "Ger.RH",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center,
            BooleanoSimNao = true
        )]
        public bool GerenciadaRh { get; set; }

        [ExcelColumn(
            Order = 12,
            Title = "Efetivado",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center,
            BooleanoSimNao = true
        )]
        public bool Efetivado { get; set; }

    }
}
