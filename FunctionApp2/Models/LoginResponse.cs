using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp2.Models {
    public class LoginResponse {
        public string odatametadata { get; set; }
        public string SessionId { get; set; }
        public string Version { get; set; }
        public int SessionTimeout { get; set; }
    }
}
