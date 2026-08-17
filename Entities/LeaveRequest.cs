using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal DaysRequested { get; set; }

        public string Reason { get; set; } = string.Empty;

        public LeaveRequestStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ReviewedOn { get; set; }

        public int? ReviewedById { get; set; }

        public string? RejectionComment { get; set; }

        // Navigation
        public Employee? Employee { get; set; } = null!;

        public LeaveType? LeaveType { get; set; } = null!;
    }
}
