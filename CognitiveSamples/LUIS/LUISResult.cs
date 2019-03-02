using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveSamples.LUIS
{
    public class LUISResult
    {

        [JsonProperty("query")]
        public string query { get; set; }

        [JsonProperty("topScoringIntent")]
        public TopScoringIntent topScoringIntent { get; set; }

        [JsonProperty("intents")]
        public IList<Intent> intents { get; set; }

        [JsonProperty("entities")]
        public IList<Entity> entities { get; set; }
    }

    public class TopScoringIntent
    {

        [JsonProperty("intent")]
        public string intent { get; set; }

        [JsonProperty("score")]
        public double score { get; set; }
    }

    public class Intent
    {

        [JsonProperty("intent")]
        public string intent { get; set; }

        [JsonProperty("score")]
        public double score { get; set; }
    }

    public class Resolution
    {

        [JsonProperty("values")]
        public IList<string> values { get; set; }
    }

    public class Entity
    {

        [JsonProperty("entity")]
        public string entity { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("startIndex")]
        public int startIndex { get; set; }

        [JsonProperty("endIndex")]
        public int endIndex { get; set; }

        [JsonProperty("resolution")]
        public Resolution resolution { get; set; }
    }
}
