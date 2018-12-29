#if !(NETMF44 || NETMF43)
using Newtonsoft.Json;

namespace Thingface.Client
{
    class CommandDeliveryStateLog
    {
        [JsonProperty("t")]
        public string Token { get; set; }

        [JsonProperty("s")]
        public CommandDeliveryState State { get; set; }

        [JsonProperty("r")]
        public int? ResultCode { get; set; }
    }
}
#endif

public enum CommandDeliveryState
{
    Pending,
    Sent,
    Received,
    Executed,
    Cancelled
}