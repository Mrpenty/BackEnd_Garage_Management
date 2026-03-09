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
        WaitingMechanic = 2,
        Inspection = 3,
        WaitingApproval = 4,
        WaitingParts = 5,
        InProgress = 6,
        WaitingPickup = 7,
        Completed = 8,
        Rejected = 9
    }
}
