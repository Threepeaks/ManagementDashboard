using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class UserActionLog
    {
        public int Id { get; set; }
        public DateTime DateTimeUtc { get; internal set; }
        public string UserName { get; internal set; }
        public string ControllerName { get; internal set; }
        public string ActionName { get; internal set; }
        public string HttpMethod { get; internal set; }
        public string UrlAccessed { get; internal set; }
        public DateTime AccessedAt { get; internal set; }
    }
}