using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface ICanLog: ICan
    {
        ICanLog Log<TCommand, TCommandResponse>(TCommand command, TCommandResponse response)
            where TCommand : IAmCommand<TCommand>
            where TCommandResponse : IAmCommandResponse<TCommand, TCommandResponse>
        ;
    }
}
