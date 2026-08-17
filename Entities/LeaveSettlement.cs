using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LeaveSettlement
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public decimal AdjustmentDays { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public DateTime SettlementDate { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;

        public LeaveType LeaveType { get; set; } = null!;
    }
}
