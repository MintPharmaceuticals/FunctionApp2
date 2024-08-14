using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp2.Models {
    public class LoginRequest {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CompanyDB { get; set; }
    }
}
