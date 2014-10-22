using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface IAmRepository<TAggregate, TKey>
        where TAggregate: IAmAggregateRoot<TAggregate, TKey>
    {
        IAmRepository<TAggregate, TKey> Insert(TAggregate xx);
        IAmRepository<TAggregate, TKey> Update(TAggregate xx);
        IAmRepository<TAggregate, TKey> Delete(TAggregate xx);
        IAmRepository<TAggregate, TKey> Delete(TKey id);
        TAggregate Get(TKey id);
    }
}
