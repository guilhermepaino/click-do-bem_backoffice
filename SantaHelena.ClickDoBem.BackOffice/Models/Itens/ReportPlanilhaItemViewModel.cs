﻿using OfficeOpenXml.Style;
using SantaHelena.ClickDoBem.BackOffice.Helpers.Planilha;
using System;

namespace SantaHelena.ClickDoBem.BackOffice.Models.Itens
{
    public class ReportPlanilhaItemViewModel
    {


        [ExcelColumn(
            Order = 1,
            Title = "Tipo",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center
        )]
        public string TipoItem { get; set; }

        [ExcelColumn(
            Order = 2,
            Title = "Criação",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center,
            NumberFormat = "dd/MM/yyyy"
        )]
        public DateTime DataInclusao { get; set; }

        [ExcelColumn(
            Order = 3,
            Title = "Efetivação",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center,
            NumberFormat = "dd/MM/yyyy"
        )]
        public DateTime? DataEfetivacao { get; set; }

        [ExcelColumn(
            Order = 4,
            Title = "Contato do Doador",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string Doador { get; set; }

        [ExcelColumn(
            Order = 5,
            Title = "Contato do Receptor",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Left
        )]
        public string Receptor { get; set; }

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
            Title = "Peso",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center
        )]
        public int Peso { get; set; }

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
            Title = "Ger.RH.",
            TitleVerticalAlignment = ExcelVerticalAlignment.Center,
            TitleHorizontalAlignment = ExcelHorizontalAlignment.Center,
            ContentVerticalAlignment = ExcelVerticalAlignment.Center,
            ContentHorizontalAlignment = ExcelHorizontalAlignment.Center,
            BooleanoSimNao = true
        )]
        public bool GerenciadaRh { get; set; }

    }
}
