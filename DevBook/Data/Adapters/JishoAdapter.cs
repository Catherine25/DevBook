using DevBook.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DevBook.Data.Adapters
{
    public class JishoAdapter
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("data")]
        public List<Data> Data { get; set; }
        
        public string BuildEnglishWords()
        {    
            WordBuilder wordBuilder = new WordBuilder(Data);
            List<Word> eWords = wordBuilder.GetEnglishWords();

            string englishWordsText = CollectionStringBuilder.BuildString(eWords.Select(w => w.Value).ToList(), ",\t");

            return englishWordsText;
        }

        public string BuildJapaneseWordsReadings()
        {
            WordBuilder wordBuilder = new WordBuilder(Data);
            List<JapaneseWord> jWords = wordBuilder.GetJapaneseWords();

            string japaneseWordsReadingText = CollectionStringBuilder.BuildString(jWords.SelectMany(w => w.Readings.Select(r => r)).ToList(), ", ");

            return japaneseWordsReadingText;
        }
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
        public JWord[] Japanese { get; set; }

        [JsonProperty("senses")]
        public Sense[] Senses { get; set; }
    }

    public class JWord
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
