using Newtonsoft.Json;

namespace Thingface.Client
{
    class CommandPayload
    {
        [JsonProperty("c")]
        public string Name { get; set; }

        [JsonProperty("a")]
        public string[] Args { get; set; }
    }
}
