using LibMdx.Controller.MdxController;
using System.Collections.Generic;
using System.Data;

namespace LibMdx
{
    public class QueryMdx
    {
        public DataTable GetClientsQueryMdx()
        {
            MdxController mdxController = new MdxController();
            string dimension = "[Dim Cliente].[Dim Cliente Nombre]";
            var mdxQuery = $@"SELECT NON EMPTY {{ [Measures].[Fact Ventas Netas] }} ON COLUMNS, NON EMPTY {{ ( {dimension}.Children) }}ON ROWS FROM [DWH Northwind]";
            DataTable dataTable = mdxController.GetDataTable(mdxQuery);
            dataTable.Columns[0].ColumnName = "ClientesNombre";
            dataTable.Columns[1].ColumnName = "FactVentas";
            return dataTable;
        }

        public DataTable GetChartDataPieQueryMdx(string[] clients, string[] months, string[] years)
        {
            MdxController mdxController = new MdxController();
            List<string> lstSelectedClientMonthsYears = mdxController.GetSelectedClientsMonthsYearAsFilterMdx(clients, months, years);
            var mdxQuery = $@"
 SELECT NON EMPTY {{
 (
  [Dim Tiempo].[Dim Tiempo Año].[Dim Tiempo Año].ALLMEMBERS *
  [Dim Tiempo].[Dim Tiempo Mes].[Dim Tiempo Mes].ALLMEMBERS
 )
 }}
 ON COLUMNS, NON EMPTY {{ (
 (
  [Measures].[Fact Ventas Netas],
 [Dim Cliente].[Dim Cliente Nombre].[Dim Cliente Nombre].ALLMEMBERS
 )
 )
 }}  ON ROWS FROM  ( SELECT ( {{
{lstSelectedClientMonthsYears[0]}
 }}) ON COLUMNS FROM ( SELECT ( {{
{lstSelectedClientMonthsYears[1]}
 }} ) ON COLUMNS FROM ( SELECT ( {{
{lstSelectedClientMonthsYears[2]} 
 }} ) ON COLUMNS 
 FROM [DWH Northwind]))) 
";
            DataTable dataTable = mdxController.GetDataTable(mdxQuery);
            dataTable.Columns.RemoveAt(0);
            dataTable.Columns[0].ColumnName = "ClientesNombre";
            dataTable.Columns.Add("Total", typeof(double));
            dataTable = mdxController.AddSumOfRowsOnTheLastRow(dataTable);

            List<int> numeros = new List<int>();
            for (int i = 1; i < dataTable.Columns.Count - 1; i++)
            {
                numeros.Add(i);
            }

            foreach (var item in numeros)
            {
                if (dataTable.Columns.Count == 2)
                {
                    break;
                }
                dataTable.Columns.RemoveAt(dataTable.Columns.Count - 2);

            }

            return dataTable;
        }

        public List<dynamic> GetChartDataBarQueryMdx(string[] clients, string[] months, string[] years) //
        {
            MdxController mdxController = new MdxController();
            List<string> lstSelectedClientMonthsYears = mdxController.GetSelectedClientsMonthsYearAsFilterMdx(clients, months, years);
            var mdxQuery = $@"SELECT NON EMPTY {{([Dim Tiempo].[Dim Tiempo Año].[Dim Tiempo Año].ALLMEMBERS *  [Dim Tiempo].[Dim Tiempo Mes].[Dim Tiempo Mes].ALLMEMBERS )  }} ON COLUMNS, NON EMPTY {{ ( (  [Measures].[Fact Ventas Netas], [Dim Cliente].[Dim Cliente Nombre].[Dim Cliente Nombre].ALLMEMBERS  ) ) }}  ON ROWS FROM  ( SELECT ( {{ {lstSelectedClientMonthsYears[0]}
 }}) ON COLUMNS FROM ( SELECT ( {{
{lstSelectedClientMonthsYears[1]}
 }} ) ON COLUMNS FROM ( SELECT ( {{
{lstSelectedClientMonthsYears[2]} 
 }} ) ON COLUMNS 
 FROM [DWH Northwind]))) 
";
            DataTable dataTable = mdxController.GetDataTable(mdxQuery);
            dataTable.Columns.RemoveAt(0);
            return mdxController.GetNameColumnsAndRowsValues(dataTable);
        }

        public List<string> GetChartLabelsDataBarQueryMdx(string[] clients, string[] months, string[] years) //
        {
            List<dynamic> dlist = new List<dynamic>();
            List<string> lstColumNames = new List<string>();
            MdxController mdxController = new MdxController();
            List<string> lstSelectedClientMonthsYears = mdxController.GetSelectedClientsMonthsYearAsFilterMdx(clients, months, years);
            var mdxQuery = $@"

 SELECT NON EMPTY {{

 (
  [Dim Tiempo].[Dim Tiempo Año].[Dim Tiempo Año].ALLMEMBERS *
  [Dim Tiempo].[Dim Tiempo Mes].[Dim Tiempo Mes].ALLMEMBERS
 )
 }}
 ON COLUMNS, NON EMPTY {{ (
 (
  [Measures].[Fact Ventas Netas],
 [Dim Cliente].[Dim Cliente Nombre].[Dim Cliente Nombre].ALLMEMBERS
 )
 )
 }}  ON ROWS FROM  ( SELECT ( {{
{lstSelectedClientMonthsYears[0]}
 }}) ON COLUMNS FROM ( SELECT ( {{
{lstSelectedClientMonthsYears[1]}
 }} ) ON COLUMNS FROM ( SELECT ( {{
{lstSelectedClientMonthsYears[2]} 
 }} ) ON COLUMNS 
 FROM [DWH Northwind]))) 
";
            DataTable dataTable = mdxController.GetDataTable(mdxQuery);

            string tmpNombre = "Cliente";
            dataTable.Columns.RemoveAt(0);
            dataTable.Columns[0].ColumnName = "Cliente";
            foreach (DataColumn item in dataTable.Columns)
            {
                tmpNombre = item.ColumnName;
                tmpNombre = tmpNombre.Replace("[Dim Tiempo].[Dim Tiempo Año].&[", " ");
                tmpNombre = tmpNombre.Replace("].[Dim Tiempo].[Dim Tiempo Mes].&[", " ");
                tmpNombre = tmpNombre.Replace("[", " ");
                tmpNombre = tmpNombre.Replace("]", " ");
                dataTable.Columns[item.Ordinal].ColumnName = tmpNombre;
                lstColumNames.Add(tmpNombre);
                tmpNombre = "";
            }
            lstColumNames.RemoveAt(0);
            return lstColumNames;
        }

    }
}
