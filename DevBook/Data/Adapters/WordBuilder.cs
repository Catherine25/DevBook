using System.Collections.Generic;
using System.Linq;

namespace DevBook.Data.Adapters
{
    public class WordBuilder
    {
        private Dictionary<string, Stack<string>> _japaneseWordsDictionary;
        private List<string> _englishWords;

        public WordBuilder(List<Data> dataList)
        {
            _japaneseWordsDictionary = new Dictionary<string, Stack<string>>();
            _englishWords = new List<string>();

            foreach (Data item in dataList)
            {
                foreach (JWord jWord in item.Japanese)
                {
                    string s = jWord.Word;

                    if (s == null)
                        continue;

                    int index = s.IndexOf('-');
                    if(index > 0)
                        s = s.Remove(index);

                    if (_japaneseWordsDictionary.ContainsKey(s))
                        _japaneseWordsDictionary[s].Push(jWord.Reading);
                    else
                    {
                        Stack<string> stack = new Stack<string>();
                        stack.Push(jWord.Reading);

                        _japaneseWordsDictionary.Add(s, new Stack<string>(stack));
                    }
                }

                foreach (Sense sense in item.Senses)
                    foreach (string definition in sense.EnglishDefinitions)
                        _englishWords.Add(definition);
            }
        }

        public List<JapaneseWord> GetJapaneseWords()
        {
            var keys = _japaneseWordsDictionary.Keys;
            var japaneseWords = new List<JapaneseWord>();
            foreach (string key in keys)
                japaneseWords.Add(new JapaneseWord(key, _japaneseWordsDictionary[key]));
            return japaneseWords;
        }

        public List<Word> GetEnglishWords() =>
            _englishWords.Select(w => new Word(w, Language.English)).ToList();
    }
}
