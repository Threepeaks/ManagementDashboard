using ManagementDashboard.Models;
using System.IO;

namespace ManagementDashboard.Controllers
{
    internal class SQLReportManager
    {
        private string GetFileContent(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                return "";

            string fileContent = "";
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                fileContent = streamReader.ReadToEnd();
            }
            return fileContent;

        }


        public SQLReportTableViewModel GetSqlReportTableViewModel(string sqlFileFullPath,
            SqlVarReplacementItems varItems )
        {
            if (varItems == null)
                varItems = new SqlVarReplacementItems();

            var vm = new Models.SQLReportTableViewModel();

            var db = new DBConnect();

            string query = GetFileContent(sqlFileFullPath);

            if (string.IsNullOrEmpty(query))
                return vm;

            foreach (var varItem in varItems)
            {
                query = query.Replace("{{" + varItem.Name + "}}", varItem.Value);
            }



            var result = db.Query(query);
            string htmlTable = result.Tables[0].ConvertDataTableToHTML();
            vm.HtmlTable = htmlTable;

            return vm;

        }




    }
}