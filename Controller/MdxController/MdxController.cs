using Microsoft.AnalysisServices.AdomdClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LibMdx.Controller.MdxController
{
    public class MdxController
    {
        public DataTable GetDataTable(string queryMdx)
        {
            DataTable dataTable = new DataTable();
            using (AdomdConnection cnn = new AdomdConnection($@"Provider=MSOLAP; Data Source=localhost;Catalog=CuboNorthwindISSC811; User ID=sa; Password = roverto; Persist Security Info = True; Impersonation Level = Impersonate"))
            {
                AdomdDataAdapter adomdDataAdapter;
                cnn.Open();
                using (AdomdCommand cmd = new AdomdCommand(queryMdx, cnn))
                {
                    adomdDataAdapter = new AdomdDataAdapter(cmd);
                    adomdDataAdapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public List<string> GetSelectedClientsMonthsYearAsFilterMdx(string[] clients, string[] months, string[] years)
        {
            List<string> selectedCMY = new List<string>();
            string SelectedClients = "", SelectedMonths = "", SelectedYears = "";
            for (int i = 0; i < clients.Count(); i++)
            {
                SelectedClients += $@"[Dim Cliente].[Dim Cliente Nombre].&[{clients[i]}],";
            }
            
            for (int i = 0; i < months.Count(); i++)
            {
                SelectedMonths += $@"[Dim Tiempo].[Dim Tiempo Mes].&[{months[i]}],";
            }
            
            for (int i = 0; i < years.Count(); i++)
            {
                SelectedYears += $@"[Dim Tiempo].[Dim Tiempo Año].&[{years[i]}],";
            }
            SelectedClients = SelectedClients.Remove(SelectedClients.Length - 1);
            SelectedMonths = SelectedMonths.Remove(SelectedMonths.Length - 1);
            SelectedYears = SelectedYears.Remove(SelectedYears.Length - 1);
            selectedCMY.Add(SelectedClients);
            selectedCMY.Add(SelectedMonths);
            selectedCMY.Add(SelectedYears);
            return selectedCMY;
        }

        public List<dynamic> GetNameColumnsAndRowsValues(DataTable dataTable)
        {
            List<dynamic> lstChartDataForBar = new List<dynamic>();
            List<double> dataList = null;
            for (int rows = 0; rows < dataTable.Rows.Count; rows++)
            {
                dataList = new List<double>();
                string label = "";
                for (int columns = 0; columns < dataTable.Columns.Count; columns++)
                {
                    double number1 = 0;
                    var dataToConvert = "";

                    if (dataTable.Rows[rows][columns].ToString().Equals(""))
                    {
                        dataToConvert = "0";
                    }
                    else
                    {
                        dataToConvert = dataTable.Rows[rows][columns].ToString();
                    }
                    bool canConvert = double.TryParse(dataToConvert, out number1);
                    if (canConvert == true)
                    {
                        dataList.Add(number1);
                    }

                    else
                    {
                        label = dataTable.Rows[rows][columns].ToString();
                    }
                }
                dynamic itemChartDataForBar = new
                {
                    label = label,
                    data = dataList
                };
                lstChartDataForBar.Add(itemChartDataForBar);
            }
            return lstChartDataForBar;
        }

        public DataTable AddSumOfRowsOnTheLastRow(DataTable dataTable)
        {
            double r = 0;
            double sum = 0;
            for (int renglones = 0; renglones < dataTable.Rows.Count; renglones++)
            {
                for (int columnas = 0; columnas < dataTable.Columns.Count; columnas++)
                {
                    double.TryParse(dataTable.Rows[renglones][columnas].ToString(), out r);
                    sum = sum + r;
                }
                dataTable.Rows[renglones][dataTable.Columns.Count - 1] = sum;
                sum = 0;
            }
            return dataTable;
        }
    }
}
