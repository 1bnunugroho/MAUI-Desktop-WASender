using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender.Models
{
    public class WAMessage
    {
        public int ID { get; set; }
        public string WANO { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime SendDate { get; set; }
    }
}
