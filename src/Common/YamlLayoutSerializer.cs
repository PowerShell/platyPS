using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Microsoft.PowerShell.PlatyPS
{
    public class LayoutSequenceStyle : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            bool fromString = typeof(IEnumerable<string>).IsAssignableFrom(type);
            bool fromObject = typeof(IEnumerable<object>).IsAssignableFrom(type);
            return fromString || fromObject;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException("no");
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            if (value is null)
            {
                throw new ArgumentException("object to serialize must not be null");
            }

            IEnumerable<string> sequence;
            if (value is IEnumerable<object> collection)
            {
                List<string> stringCollection = new();
                foreach (var item in collection)
                {
                    if (item is not null)
                    {
                        stringCollection.Add(item.ToString());
                    }
                    else
                    {
                        stringCollection.Add(string.Empty);
                    }
                }
                sequence = stringCollection;
            }
            else
            {
                sequence = (IEnumerable<string>)value;
            }

            emitter.Emit(new SequenceStart(default, default, false, SequenceStyle.Flow));

            foreach (var item in sequence)
            {
                emitter.Emit(new Scalar(default, item));
            }

            emitter.Emit(new SequenceEnd());
        }
    }
}
