using System;

namespace WorkMonitor_360.Models
{
    namespace WorkMonitor_360.Models
    {
        public class WorkSessionModel
        {
            public int SerialNo { get; set; }
            public string? MachineName { get; set; }
            public string? UserName { get; set; }

            public DateTime? CheckInTime { get; set; }

            public DateTime? CheckOutTime { get; set; }

            public string? Status { get; set; }

            public bool? Synced { get; set; }

        }
    }
}
