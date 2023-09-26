using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle.Aplication.CustomExceptions
{
    public class Error
    {
        public Error(string code, string message)
        { 
            this.Code = code;
            this.Message = message;            
        }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
