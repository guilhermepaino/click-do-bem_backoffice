using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SantaHelena.ClickDoBem.BackOffice.Helpers.Planilha
{

    /// <summary>
    /// Objeto helper de importação e exportação de dados em excel
    /// </summary>
    public class Excel : IDisposable
    {

        #region Objetos/Variáveis Locais

        private string _caminho;
        private IDictionary<string, ExcelColumnAttribute> _attributos;
        private IDictionary<int, string> _colunas;
        private IDictionary<int, string> _titulos;

        #endregion

        #region Construtores

        /// <summary>
        /// Cria uma nova instância de Helper.Excel
        /// </summary>
        /// <param name="caminho">Caminho da pasta alvo de trabalho para importação e exportação</param>
        public Excel(string caminho)
        {
            _caminho = caminho;
            _attributos = new Dictionary<string, ExcelColumnAttribute>();
            _colunas = new Dictionary<int, string>();
            _titulos = new Dictionary<int, string>();
        }

        #endregion

        #region Métodos Locais

        /// <summary>
        /// Ler os attributos das propriedades do objeto de modelo
        /// </summary>
        /// <typeparam name="T">Tipo do objeto de odelo</typeparam>
        protected void LerAtributosModelo<T>()
        {

            _attributos.Clear();
            _colunas.Clear();
            _titulos.Clear();

            // Lendo attributos do modelo
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    ExcelColumnAttribute exAttr = attr as ExcelColumnAttribute;
                    if (exAttr != null)
                    {
                        _attributos.Add(prop.Name, exAttr);
                        _colunas.Add(exAttr.Order, prop.Name);
                        _titulos.Add(exAttr.Order, exAttr.Title);
                    }
                }
            }

        }

        /// <summary>
        /// Preparar uma string com base na lista de críticas
        /// </summary>
        /// <param name="criticas">Lista de críticas</param>
        /// <returns>Expressão contendo a relação de críticas</returns>
        protected string LerTodasCriticas(IList<string> criticas)
        {

            StringBuilder sb = new StringBuilder();

            foreach (string c in criticas)
            {
                sb.Append($"{(sb.Length.Equals(0) ? string.Empty : "|")}{c}");
            }

            return sb.ToString();

        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Exportar os dados do modelo para uma planilha excel
        /// </summary>
        /// <typeparam name="T">Tipo do objeto de modelo</typeparam>
        /// <param name="nomeArquivo">Nome do arquivo, incluindo a extensão, que será gerado</param>
        /// <param name="nomePlanilha">Nome da planilha (aba) a ser utilizada como fonte de dados</param>
        /// <param name="registros">Lista de registros (modelo) a ser exportado</param>
        /// <returns>O caminho+nome do arquivo gerado</returns>
        public string Exportar<T>(string nomeArquivo, string nomePlanilha, IList<T> registros)
            where T : class
        {
            return Exportar<T>(nomeArquivo, nomePlanilha, registros, true, TableStyles.None, string.Empty);
        }

        /// <summary>
        /// Exportar os dados do modelo para uma planilha excel
        /// </summary>
        /// <typeparam name="T">Tipo do objeto de modelo</typeparam>
        /// <param name="nomeArquivo">Nome do arquivo, incluindo a extensão, que será gerado</param>
        /// <param name="nomePlanilha">Nome da planilha (aba) a ser utilizada como fonte de dados</param>
        /// <param name="registros">Lista de registros (modelo) a ser exportado</param>
        /// <param name="estiloTabela">Estilo da tabela (Excel Table)</param>
        /// <param name="nomeTabela">Nome da tabela (Excel Table)</param>
        /// <returns>O caminho+nome do arquivo gerado</returns>
        public string Exportar<T>(string nomeArquivo, string nomePlanilha, IList<T> registros, bool sobrescreverArquivo, TableStyles estiloTabela, string nomeTabela)
            where T : class
        {

            // Abrindo arquivo, excluindo se existir
            FileInfo arqInfo = new FileInfo(Path.Combine(_caminho, nomeArquivo));
            if (arqInfo.Exists && sobrescreverArquivo)
            {
                arqInfo.Delete();
                arqInfo = new FileInfo(Path.Combine(_caminho, nomeArquivo));
            }

            // Lendo attributos do modelo
            LerAtributosModelo<T>();

            using (ExcelPackage pkg = new ExcelPackage(arqInfo))
            {

                // Adicionando a planilha à pasta de trabalho
                ExcelWorksheet ws = pkg.Workbook.Worksheets.Add(nomePlanilha);

                if (ws != null && registros != null && registros.Count() > 0)
                {

                    // Adicionando o cabeçalho
                    foreach (KeyValuePair<string, ExcelColumnAttribute> item in _attributos)
                    {
                        string columnTitle = (item.Value.Title ?? item.Key);
                        ws.Cells[1, item.Value.Order].Value = columnTitle;
                        ws.Cells[1, item.Value.Order].Style.Font.Bold = true;
                        ws.Cells[1, item.Value.Order].Style.VerticalAlignment = item.Value.TitleVerticalAlignment;
                        ws.Cells[1, item.Value.Order].Style.HorizontalAlignment = item.Value.TitleHorizontalAlignment;
                    }

                    // Populando a planilha
                    for (int pos = 0; pos < registros.Count; pos++)
                    {

                        T reg = registros[pos];

                        int linha = (pos + 2);

                        foreach (var item in _attributos)
                        {

                            Type t = reg.GetType();
                            PropertyInfo[] props = t.GetProperties();
                            PropertyInfo prop = props.Where(x => x.Name == item.Key).First();
                            Type propType = prop.GetType();

                            var value = prop.GetValue(reg);

                            if (item.Value.BooleanoSimNao)
                            {
                                if (value.GetType().ToString().ToLower().Equals("system.boolean"))
                                {
                                    if (bool.Parse(value.ToString()))
                                        value = "Sim";
                                    else
                                        value = "Não";
                                }

                            }

                            ws.Cells[linha, item.Value.Order].Value = value;

                        }
                    }

                    // Formatando a planilha
                    int ultimaLinha = ws.Dimension.End.Row;
                    int ultimaColuna = ws.Dimension.End.Column;

                    foreach (KeyValuePair<string, ExcelColumnAttribute> item in _attributos)
                    {
                        int coluna = item.Value.Order;
                        using (ExcelRange cells = ws.Cells[2, coluna, ultimaLinha, coluna])
                        {
                            cells.Style.VerticalAlignment = item.Value.ContentVerticalAlignment;
                            cells.Style.HorizontalAlignment = item.Value.ContentHorizontalAlignment;
                            cells.Style.Numberformat.Format = item.Value.NumberFormat;
                            cells.Style.WrapText = item.Value.WrapText;
                        }
                    }

                    // Atribuindo tabela
                    if (estiloTabela != TableStyles.None)
                    {
                        var handle = ws.Tables.Add(new ExcelAddressBase(1, 1, ultimaLinha, ultimaColuna), nomeTabela);
                        handle.TableStyle = estiloTabela;
                    }

                }

                pkg.Save(); // Salvando a planilha

            }
            return arqInfo.FullName;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _attributos.Clear();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _caminho = null;
                _attributos = null;
                _colunas = null;

                disposedValue = true;
            }
        }

        ~Excel()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

    }

}
