// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.Remoting.Metadata.W3cXsd2001;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

public class QuotedNullStringEvenEmitter : ChainedEventEmitter
{
    public QuotedNullStringEvenEmitter(IEventEmitter nextEmitter) : base(nextEmitter)
    {
    }

    public override void Emit(ScalarEventInfo eventInfo, YamlDotNet.Core.IEmitter emitter)
    {
       if (eventInfo.Source.Type == typeof(string) && string.Equals(eventInfo.Source.Value?.ToString() , "NULL", System.StringComparison.OrdinalIgnoreCase))
        {
            emitter.Emit(new Scalar(@"'NULL'"));
        }
        else
        {
            // For all other values, use the default emitter
            base.Emit(eventInfo, emitter);
        }
    }
}