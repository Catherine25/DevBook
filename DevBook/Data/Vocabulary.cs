using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Diagnostics.Debug;

namespace DevBook.Data
{
    public class Vocabulary
    {
        public char Dot { get; private set; }

        public Vocabulary(string directory)
        {
            _wordsPath = directory + "\\words.txt";
            _words = new List<Word>();

            _translationsPath = directory + "\\translations.txt";
            _translations = new List<Translation>();

            _separatorsPath = directory + "\\separators.txt";
            _separators = new List<string>();

            LoadVocabulary(_wordsPath, _translationsPath);
            LoadSeparators(_separatorsPath);
        }

        public Word GetWord(string meaning, Language language)
        {
            return _words.FirstOrDefault(a => a.Value == meaning && a.Language == language);
        }

        public void AddWord(Word word)
        {
            if (_words.Any(w => w.Language == word.Language && w.Value == word.Value))
            {
                _words.Add(word);
                SaveVocabulary();
            }
            //todo: else show message
        }

        public void AddTranslation(Translation translation)
        {
            //if (_translations.Any(t => t.Word1.Language == word.Language && w.Value == word.Value))
            //todo: check if exists
            _translations.Add(translation);
            SaveVocabulary();
        }

        public Translation GetTranslation(string selectedText, Language language1, Language language2)
        {
            var translations = GetTranslationsByLanguagePair(language1, language2);
            var translation = translations.FirstOrDefault(
                t => t.Target.Value == selectedText || t.Native.Value == selectedText);
            WriteLine(translation);
            return translation;
        }

        public List<Translation> GetTranslationsByLanguagePair(Language language1, Language language2)
        {
            return _translations
            .Where(t => (t.Target.Language == language1 && t.Native.Language == language2)
                || (t.Target.Language == language2 && t.Native.Language == language1))
            .ToList();
        }

        public List<Translation> GetKnownTranslations(string text)
        {
            return _translations.Where(t => text.Contains(t.Target.Value)).ToList();
        }

        private void LoadVocabulary(string wPath, string tPath)
        {
            //todo: handle
            _words = JsonConvert.DeserializeObject<List<Word>>(File.ReadAllText(wPath)).ToList();
            WriteLine($"{_words.Count} words read.");
            
            _translations = JsonConvert.DeserializeObject<List<Translation>>(File.ReadAllText(tPath)).ToList();
            WriteLine($"{_translations.Count} translations read.");
        }

        private void LoadSeparators(string path)
        {
            Dot = File.ReadAllText(path)[0];
        }

        private void SaveVocabulary()
        {
            string words = JsonConvert.SerializeObject(_words);
            string translations = JsonConvert.SerializeObject(_translations);

            File.WriteAllText(_wordsPath, words);
            File.WriteAllText(_translationsPath, translations);
        }

        private string _wordsPath;
        private string _translationsPath;
        private string _separatorsPath;

        private List<Word> _words;
        private List<Translation> _translations;
        private List<string> _separators;
    }
}