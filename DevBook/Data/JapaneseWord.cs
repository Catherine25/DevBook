using System.Collections.Generic;

namespace DevBook.Data
{
    public class JapaneseWord : Word
    {
        public Stack<string> Readings { get; set; }

        public JapaneseWord(string value, Stack<string> readings) : base(value, Language.Japanese)
        {
            Value = value;
            Readings = readings;
        }
    }
}