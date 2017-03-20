using Newtonsoft.Json.Converters;
using System;

namespace Core.Batch.Engine.Helpers
{
    public class InterfaceConverter<TInterface, TConcrete> : CustomCreationConverter<TInterface>
    where TConcrete : TInterface, new()
    {
        public override TInterface Create(Type objectType)
        {
            return new TConcrete();
        }
    }
}
