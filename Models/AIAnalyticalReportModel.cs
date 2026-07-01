using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkMonitor_360.Models
{
    public  class AIAnalyticalReportModel
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public decimal TotalTimeSpent  { get; set; }
        public decimal IdleTime{ get; set; }       

        public AIAnalyticalReportModel()
        {   
            //
        }
    }
}
