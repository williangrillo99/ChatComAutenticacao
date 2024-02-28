using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request
{
    public class TokenRequest
    {
        public string AcessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
