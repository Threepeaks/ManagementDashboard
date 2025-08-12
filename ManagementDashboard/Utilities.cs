using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace ManagementDashboard
{
    public enum ColumnType
    {
        String,
        Decimal,
        DecimanNoThoundSep,
        Percentage,
        ClientReference,
        Date,
        DateTime,
        Hidden
    }

    public class ColumnTypeItems : List<ColumnTypePair>
    {
        public void Add(string columnName, ColumnType columnType)
        {
            var c = new ColumnTypePair();
            c.ColumnName = columnName;
            c.Type = columnType;
            this.Add(c);
        }

    }

    public class ColumnTypePair
    {
        public string ColumnName { get; set; }
        public ColumnType Type { get; set; }
        public bool IsAccountReference { get; set; }
    }

    public static class Utilities
    {
        public static string IsActive(this HtmlHelper html,
                                      string control,
                                      string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // both must match
            var returnActive = control == routeControl &&
                               action == routeAction;

            return returnActive ? "active" : "";
        }
        public static string IsActive(this HtmlHelper html, string control)
        {
            var routeData = html.ViewContext.RouteData;
            var routeControl = (string)routeData.Values["controller"];
            var returnActive = control == routeControl;
            return returnActive ? "active" : "";
        }
        public static string IsOpen(this HtmlHelper html, string control)
        {
            var routeData = html.ViewContext.RouteData;
            var routeControl = (string)routeData.Values["controller"];
            var returnActive = control == routeControl;
            return returnActive ? "menu-open" : "";
        }


        static string ToHtmlAccountReference(string clientReference)
        {
            if (String.IsNullOrEmpty(clientReference))
            {
                return "<td></td>";
            }
            //class="reference" data-reference="@item.Ref"
            var html = "<td  class='account-reference reference text-nowrap' data-reference=\"" + clientReference + "\">";
            html += "<div>";

            html += "<a class='mr-2' href='https://tpmsweb.threepeaks.co.za/Clients/Profile/ByRef/" + clientReference + "' target='_blank'><i class=\"bi bi-box-arrow-up-right\"></i></i></a>";
            html += clientReference;
            
            html += "</div></td>";

            return html;
        
        }

        static string ToHtmlDateTime(object obj)
        {
            if (obj == null)
                return "<td></td>";
            var date = DateTime.Parse(obj.ToString());
            var html = "<td class='text-nowrap'>" + date.ToString("dd/MM/yyyy hh:mm:ss tt") + "</td>";
            return html;
        }
        static string ToHtmlDate(object obj)
        {
            if (obj == null)
                return "<td></td>";
            var dateString = obj.ToString();
            if (string.IsNullOrEmpty(dateString))
                return "<td></td>";

            var date = DateTime.Parse(obj.ToString());
            var html = "<td class='text-nowrap'>" + date.ToString("dd/MM/yyyy") + "</td>";
            return html;
        }
        static string ToHtmlString(string obj)
        {
            //class="reference" data-reference="@item.Ref"
            var html = "<td><div>";
            html += obj.Trim();
            html += "</div></td>";
            return html;
        }
        static string ToHtmlDecimal(object obj)
        {
            var html = "<td class='text-right'>" + ToDecimalValue(obj) + "</td>";
            return html;
        }
        static string ToHtmlPercentage(object obj)
        {
            return "<td class='text-right'>" + ToPercentageValue(obj, 2) + "</td>";
        }
        public static string ConvertDataTableToHTML(this DataTable dt, ColumnTypeItems colTypeItems = null)
        {

            var dataTable = dt;

            string html = "<table class='table table-striped table-sm text-sm'>";

            //add header row
            html += "<thead>";
            html += "<tr>";
            for (int i = 0; i < dataTable.Columns.Count; i++)
                html += "<th>" + dataTable.Columns[i].ColumnName + "</th>";
            html += "</tr>";
            html += "</thead>";
            //add rows
            html += "<tbody>";
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    var isAccountReference = false;
                    var fieldType = ColumnType.String;

                    if (colTypeItems != null)
                    {
                        var col = colTypeItems.FirstOrDefault(x => x.ColumnName == dataTable.Columns[j].ColumnName);
                        if (col != null)
                        {
                            fieldType = col.Type;
                            isAccountReference = col.IsAccountReference;
                        }


                    }
                    if (fieldType == ColumnType.String)
                        html += ToHtmlString(dataTable.Rows[i][j].ToString());
              

                    if (fieldType == ColumnType.ClientReference)
                        html += ToHtmlAccountReference(dataTable.Rows[i][j].ToString().Trim());


                    if (fieldType == ColumnType.Decimal)
                        html += ToHtmlDecimal(dataTable.Rows[i][j]);

                    if (fieldType == ColumnType.Percentage)
                        html += "<td class='text-right'>" + dataTable.Rows[i][j] + "</td>";


                    if (fieldType == ColumnType.Percentage)
                        html += ToHtmlPercentage(dataTable.Rows[i][j]);

                    if (fieldType == ColumnType.Date)
                        html += ToHtmlDate(dataTable.Rows[i][j]);

                    //Date Time
                    if (fieldType == ColumnType.DateTime)
                        html += ToHtmlDateTime(dataTable.Rows[i][j]);

                    if (fieldType == ColumnType.Hidden)
                        html += "<td data-value='" + dataTable.Rows[i][j] + "' ></td>";



                }
                html += "</tr>";
            }
            html += "</tbody>";
            html += "</table>";
            return html;
        }

        private static string ToPercentageValue(object obj, int v2)
        {
            try
            {
                if (obj == null)
                    return "";
                var v = decimal.Parse(obj.ToString());
                return v.ToString();
            }
            catch (Exception e)
            {


                return "";
            }

        }

        private static string ToDecimalValue(object obj, bool thousandSeperator = true)
        {
            try
            {
                if (obj == null)
                    return "";
                var v = decimal.Parse(obj.ToString());
                return v.ToString("N");

            }
            catch (Exception e)
            {

                return "";
            }

        }
    }
}