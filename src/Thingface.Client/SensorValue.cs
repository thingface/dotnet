using Newtonsoft.Json;

namespace Thingface.Client
{
    public class SensorValue
    {
        public SensorValue(double value)
        {
            Value = value;
        }

        public SensorValue()
        {
        }

        [JsonProperty(PropertyName = "v")]
        public double Value { get; set; }
    }
}