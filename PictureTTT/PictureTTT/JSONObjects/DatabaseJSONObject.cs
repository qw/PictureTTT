using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureTTT.JSONObjects
{
    public class DatabaseJSONObject
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "From")]
        public string From { get; set; }

        [JsonProperty(PropertyName = "To")]
        public string To { get; set; }

        [JsonProperty(PropertyName = "OriginalText")]
        public string OriginalText { get; set; }

        [JsonProperty(PropertyName = "TranslatedText")]
        public string TranslatedText { get; set; }

        public string[] ToStringArray()
        {
            string[] text = { OriginalText, TranslatedText };
            return text;
        }
    }
}
