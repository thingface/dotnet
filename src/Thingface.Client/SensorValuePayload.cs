using Newtonsoft.Json;

namespace Thingface.Client
{
    public class SensorValuePayload
    {
        public SensorValuePayload(double value)
        {
            Value = value;
        }

        public SensorValuePayload()
        {
        }

        [JsonProperty(PropertyName = "v")]
        public double Value { get; set; }
    }
}