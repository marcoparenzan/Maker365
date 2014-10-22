using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface IAmValue<TValue> : IAm
        where TValue : IAmValue<TValue>
    {
    }
}
