using OfficeOpenXml.Style;
using System;

namespace SantaHelena.ClickDoBem.BackOffice.Helpers.Planilha
{

    public class ExcelColumnAttribute : Attribute
    {

        public int Order { get; set; }

        public string Title { get; set; }

        public ExcelVerticalAlignment TitleVerticalAlignment { get; set; }

        public ExcelHorizontalAlignment TitleHorizontalAlignment { get; set; }

        public ExcelVerticalAlignment ContentVerticalAlignment { get; set; }

        public ExcelHorizontalAlignment ContentHorizontalAlignment { get; set; }

        public bool WrapText { get; set; }

        public string NumberFormat { get; set; }

        public bool BooleanoSimNao { get; set; }

    }

}
