using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Models
{
    public class JwtConfiguration
    {
        public string Key { get; init; } = null!;
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public required string AuthProvider { get; set; }
        public required string AuthTokenName { get; set; }
        public int AccessTokenMins { get; init; }
    }
}