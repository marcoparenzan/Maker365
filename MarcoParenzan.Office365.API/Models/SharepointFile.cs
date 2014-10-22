using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.Office365.API.Models
{
    public class SharePointFile
    {
        public string Name { get; set; }
        public SharePointUser Author { get; set; }
        public DateTime TimeLastModified { get; set; }
    }
}
