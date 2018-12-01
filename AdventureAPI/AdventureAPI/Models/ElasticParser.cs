using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureAPI.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ElasticParser
    {
        [JsonProperty("_index")]
        public string Index { get; set; }

        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_version")]
        public long Version { get; set; }

        [JsonProperty("found")]
        public bool Found { get; set; }

        [JsonProperty("_source")]
        public Source Source { get; set; }
    }

    public partial class Source
    {
        [JsonProperty("planCostShares")]
        public JsonParser PlanCostShares { get; set; }

        [JsonProperty("linkedPlanServices")]
        public LinkedPlanService[] LinkedPlanServices { get; set; }

        [JsonProperty("_org")]
        public string Org { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("planType")]
        public string PlanType { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }
    }

    public partial class LinkedPlanService
    {
        [JsonProperty("linkedService")]
        public LinkedService LinkedService { get; set; }

        [JsonProperty("planserviceCostShares")]
        public JsonParser PlanserviceCostShares { get; set; }

        [JsonProperty("_org")]
        public string Org { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }
    }

    public partial class LinkedService
    {
        [JsonProperty("_org")]
        public string Org { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class JsonParser
    {
        [JsonProperty("deductible")]
        public long Deductible { get; set; }

        [JsonProperty("_org")]
        public string Org { get; set; }

        [JsonProperty("copay")]
        public long Copay { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }
    }

    public partial class ElasticParser
    {
        public static ElasticParser FromJson(string json) => JsonConvert.DeserializeObject<ElasticParser>(json, AdventureAPI.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ElasticParser self) => JsonConvert.SerializeObject(self, AdventureAPI.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
