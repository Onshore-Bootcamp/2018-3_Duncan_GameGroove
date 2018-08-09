using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGrooveDAL.Models
{
    public class RequestDO
    {
        public int RequestID { get; set; }

        public string RequestText { get; set; }
        
        public string Username { get; set; }

        public string Date { get; set; }
    }
}
