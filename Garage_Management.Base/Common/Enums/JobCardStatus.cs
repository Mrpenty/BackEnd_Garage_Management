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
        WaitingMechanic = 3,
        Inspection = 4,
        WaitingApproval = 5,
        WaitingParts = 6,
        InProgress = 7,
        WaitingPickup = 8,
        Completed = 9,
        Rejected = 10
    }
}
