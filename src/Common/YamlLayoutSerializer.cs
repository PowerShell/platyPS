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
            return typeof(IEnumerable<string>).IsAssignableFrom(type);
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

            var sequence = (IEnumerable<string>)value;
            emitter.Emit(new SequenceStart(default, default, false, SequenceStyle.Flow));

            foreach (var item in sequence)
            {
                emitter.Emit(new Scalar(default, item));
            }

            emitter.Emit(new SequenceEnd());
        }
    }
}
