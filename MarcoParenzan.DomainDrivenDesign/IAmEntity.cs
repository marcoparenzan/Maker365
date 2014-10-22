using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.DomainDrivenDesign
{
    public interface IAmEntity<T, TKey> : IAm
        where T: IAmEntity<T, TKey>
    {
        TKey Id { get; }
    }

    public interface IAmEntity<T, TKey, TKey2> : IAmEntity<T, TKey>
        where T : IAmEntity<T, TKey, TKey2>
    {
        TKey2 Id2 { get; }
    }
}
