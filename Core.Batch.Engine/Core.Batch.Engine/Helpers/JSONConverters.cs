using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
