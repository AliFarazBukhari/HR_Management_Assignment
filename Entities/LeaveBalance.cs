using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LeaveBalance
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public decimal Balance { get; set; }

        public Employee Employee { get; set; } = null!;

        public LeaveType LeaveType { get; set; } = null!;
    }
}
