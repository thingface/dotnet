#if !(NETMF44 || NETMF43)
using Newtonsoft.Json;

namespace Thingface.Client
{
    class CommandPayload
    {
        [JsonProperty("c")]
        public string Name { get; set; }

        [JsonProperty("a")]
        public string[] Args { get; set; }

        [JsonProperty("t")]
        public string Token { get; set; }
    }
}
#endif
