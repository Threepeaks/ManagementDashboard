using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManagementDashboard
{
    public enum ColumnType
    {
        String,
        Decimal,
        DecimanNoThoundSep,
        Percentage
    }

    public class ColumnTypeItems : List<ColumnTypePair>
    {
        public void Add(string columnName, ColumnType columnType)
        {
            var c = new ColumnTypePair();
            c.ColumnName = columnName;
            c.Type= columnType;
            this.Add(c);
        }
    }

    public class ColumnTypePair
    {
        public string ColumnName { get; set; }
        public ColumnType Type { get; set; }
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


        public static string ConvertDataTableToHTML(this DataTable dt, ColumnTypeItems colTypeItems = null)
        {

            var dataTable = dt;

            string html = "<table class='table table-striped table-sm '>";

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
                    var fieldType = ColumnType.String;

                    if (colTypeItems != null)
                    {
                        var col = colTypeItems.FirstOrDefault(x => x.ColumnName == dataTable.Columns[j].ColumnName);
                        if (col != null)
                        {
                            fieldType = col.Type;
                        }


                    }
                    if (fieldType == ColumnType.String)
                        html += "<td>" + dataTable.Rows[i][j].ToString() + "</td>";
                    if (fieldType == ColumnType.Decimal)
                        html += "<td class='text-right'>" + ToDecimalValue(dataTable.Rows[i][j]) + "</td>";                    
                    if (fieldType == ColumnType.Percentage)
                        html += "<td class='text-right'>" + dataTable.Rows[i][j] + "</td>";

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
            catch (Exception e )
            {


                return "";
            }

        }

        private static string ToDecimalValue(object obj,bool thousandSeperator = true)
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