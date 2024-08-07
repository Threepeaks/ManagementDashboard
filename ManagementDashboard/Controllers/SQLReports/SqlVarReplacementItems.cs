using System.Collections.Generic;

namespace ManagementDashboard.Controllers
{
    internal class SqlVarReplacementItems : List<SqlVarReplacementItem>
    {
        public void Add(string name,string value)
        {
            var item = new SqlVarReplacementItem();
            item.Name = name;
            item.Value = value;
            this.Add(item);
        }
    }
}