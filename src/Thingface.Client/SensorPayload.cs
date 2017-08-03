#if !(NETMF44 || NETMF43)
using Newtonsoft.Json;

namespace Thingface.Client
{
    public class SensorPayload
    {
        public SensorPayload(object value)
        {
            Value = value;
        }

        public SensorPayload()
        {
        }

        [JsonProperty(PropertyName = "v")]
        public object Value { get; set; }
    }
}
#endif
