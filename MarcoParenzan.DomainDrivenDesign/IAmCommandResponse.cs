using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface IAmCommandResponse<TCommand, TCommandResponse> : IAm
        where TCommand: IAmCommand<TCommand>
        where TCommandResponse : IAmCommandResponse<TCommand, TCommandResponse>
    {
    }
}
