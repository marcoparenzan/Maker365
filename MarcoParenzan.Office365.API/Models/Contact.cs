using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.Office365.API.Models
{
    public class Contact
    {
        public string GivenName { get; set; }
        public string DisplayName { get; set; }
        public EMailAddress[] EmailAddresses { get; set; }
        public string[] BusinessPhones { get; set; }
    }
}
