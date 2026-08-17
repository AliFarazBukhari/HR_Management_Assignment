using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LeaveType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int DefaultAllocationDays { get; set; }

        public bool IsAccrued { get; set; }

        public decimal MonthlyAccrualRate { get; set; }

        public bool IsActive { get; set; }

        // Navigation
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

        public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
    }
}
