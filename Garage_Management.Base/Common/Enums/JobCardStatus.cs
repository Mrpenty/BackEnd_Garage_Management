using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Enums
{
    public enum JobCardStatus
    {
        Created = 1,
        OnwaitingList= 2,
        WaitingInspection = 3,
        Inspection = 4,
        WaitingSupervisorApproval = 5,
        WaitingCustomerApproval = 6,
        InProgress = 7,
        WaitingPickup = 8,
        Completed = 9,
        Rejected = 10
    }
}
