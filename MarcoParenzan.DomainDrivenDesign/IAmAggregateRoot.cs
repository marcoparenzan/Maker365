using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface IAmAggregateRoot<TAggregate, TKey> : IAmEntity<TAggregate, TKey>
        where TAggregate : IAmAggregateRoot<TAggregate, TKey>
    {
    }
}
