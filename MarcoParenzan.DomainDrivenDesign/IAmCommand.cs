using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface IAmCommand<TCommand> : IAm
        where TCommand: IAmCommand<TCommand>
    {
    }
}
