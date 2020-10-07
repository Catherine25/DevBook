namespace DevBook.Data
{
    public struct Word
    {
        public Word(string value, Language language)
        {
            Value = value;
            Language = language;
        }

        public string Value { get; set; }
        public Language Language { get; set; }
    }
}