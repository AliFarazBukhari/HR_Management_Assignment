using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum LeaveRequestStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
    public enum ResponseCode
    {
        LeaveApplied,
        OverlappingLeave,
        LeaveExhausted,
    }
}
