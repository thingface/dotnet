#if !(NETMF44 || NETMF43)
using Newtonsoft.Json;

namespace Thingface.Client
{
    public class TelemetryPayload
    {
        public TelemetryPayload(object value)
        {
            Value = value;
        }

        public TelemetryPayload()
        {
        }

        [JsonProperty(PropertyName = "v")]
        public object Value { get; set; }
    }
}
#endif
