using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.Office365.API.Models
{
    public class Message
    {
        public DateTime DateTimeReceived { get; set; }
        public Recipient From { get; set; }
        public string Subject { get; set; }
    }
}
