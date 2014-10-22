using MarcoParenzan.DomainDrivenDesign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Maker365.Customers365.Commands
{
    public class NewCustomerOrderCommand : IAmCommand<NewCustomerOrderCommand>
    {
        public string ModelReferenceName { get; set; }
        public Stream ModelStream { get; set; }
        public string ModelFileName { get; set; }

    }
}