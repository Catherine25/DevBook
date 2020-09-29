using Newtonsoft.Json;
using System.Collections.Generic;

namespace DevBook.Data.Adapters
{
    public class JishoAdapter
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("data")]
        public List<Data> Data { get; set; }
    }

    public class Meta
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Data
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("is_common")]
        public string IsCommon { get; set; }

        [JsonProperty("japanese")]
        public List<JapaneseWord> Japanese { get; set; }

        [JsonProperty("senses")]
        public List<Sense> Senses { get; set; }
    }

    public class JapaneseWord
    {
        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("reading")]
        public string Reading { get; set; }
    }

    public class Sense
    {
        [JsonProperty("english_definitions")]
        public string[] EnglishDefinitions { get; set; }

        [JsonProperty("parts_of_speech")]
        public string[] PartsOfSpeech { get; set; }
    }
}
