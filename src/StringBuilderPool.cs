using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class StringBuilderPool
    {
        private readonly ConcurrentBag<StringBuilder> _stringBuilders;

        internal int InitialCapacity { get; set; } = 5000;

        internal int MaximumRetainedCapacity { get; set; } = 3 * 5000;

        public StringBuilderPool()
        {
            _stringBuilders = new ConcurrentBag<StringBuilder>();
        }

        internal StringBuilder Get()
        {
            if (_stringBuilders.TryTake(out StringBuilder? sb) && sb is not null)
            {
                return sb;
            }
            else
            {
                return new StringBuilder();
            }
        }

        internal bool Return(StringBuilder sb)
        {
            if(sb.Capacity > MaximumRetainedCapacity)
            {
                // Too big. Discard this one.
                return false;
            }

            sb.Clear();

            _stringBuilders.Add(sb);

            return true;
        }

    }
}
