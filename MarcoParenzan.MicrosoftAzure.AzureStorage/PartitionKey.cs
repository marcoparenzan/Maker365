using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.MicrosoftAzure.AzureStorage
{
    public class PartitionKey
    {
        private Func<string> _f;

        public static implicit operator PartitionKey(Func<string> f)
        {
            return new PartitionKey
            {
                _f = f
            };
        }

        public static implicit operator string(PartitionKey pk)
        {
            return pk._f();
        }
    }
}
