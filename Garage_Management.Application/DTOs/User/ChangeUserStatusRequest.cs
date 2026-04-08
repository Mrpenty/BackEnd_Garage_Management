using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.User
{
    public class ChangeUserStatusRequest
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
