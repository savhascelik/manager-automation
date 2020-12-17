using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TarikGuney.DevOpsAutomation
{
    public class WorkItemMessage
    {
        [JsonPropertyName("ids")]
        public List<long> Ids { get; set; }

        [JsonPropertyName("$expand")]
        public string Expand => "fields";
    }
}