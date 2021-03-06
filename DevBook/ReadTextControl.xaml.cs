﻿using DevBook.Data;
using DevBook.Data.Adapters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DevBook
{
    public partial class ReadTextControl : UserControl
    {
        private Vocabulary _vocabulary;
        private readonly Language _targetLanguage;
        private readonly Language _nativeLanguage;
        private string _text;

        public ReadTextControl() => InitializeComponent();

        public ReadTextControl(Vocabulary vocabulary, Language target, Language native, string text)
        {
            InitializeComponent();

            _vocabulary = vocabulary;
            _targetLanguage = target;
            _nativeLanguage = native;
            _text = text;

            ShowText();
        }

        public void ShowText()
        {
            // split by blocks
            List<string> blocks = SplitToBlocks(_text);

            // split by sentences
            var paragraphs = SplitToSentences(blocks, _vocabulary.Dot);

            foreach (Paragraph item in paragraphs)
                xText.Document.Blocks.Add(item);

            xText.SelectionChanged += Textbox_SelectionChanged;
            xSave.Click += XSave_Click;
        }

        private void XSave_Click(object sender, RoutedEventArgs e)
        {
            Word target = new Word(xTargetWord.Text, _targetLanguage);
            Word native = new Word(xNativeWord.Text, _nativeLanguage);

            Translation translation = new Translation(target, native);

            _vocabulary.AddWord(translation.Target);
            _vocabulary.AddWord(translation.Native);

            _vocabulary.AddTranslation(translation);
        }

        private void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string selectedText = (sender as RichTextBox).Selection.Text;
            xTargetWord.Text = selectedText;

            // todo: hightlight the text

            if (selectedText != "" && selectedText != " ")
                new Thread(a => MakeRequest(selectedText)).Start();
        }

        private void MakeRequest(string selectedText)
        {
            Word word = _vocabulary.GetWord(selectedText, _targetLanguage);
            Translation translation = _vocabulary.GetTranslation(selectedText, _targetLanguage, _nativeLanguage);

            if(translation != null)
                App.UpdateUi(() => { xNativeWord.Text = translation.Native.Value; });

            var httpWebRequest = WebRequest.Create($"https://jisho.org/api/v1/search/words?keyword=\"{selectedText}\"");
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using var streamReader = new StreamReader(httpResponse.GetResponseStream());

            JishoAdapter data = JsonConvert.DeserializeObject<JishoAdapter>(streamReader.ReadToEnd());

            App.UpdateUi(() => ShowWords(selectedText, data.BuildJapaneseWordsReadings(), data.BuildEnglishWords()));
        }

        private void ShowWords(string text, string reading, string english)
        {
            xTargetWord.Text = text;
            xTargetWordReading.Text = reading;
            xNativeWord.Text = english;
        }

        private List<Run> FindKnownTranslations(string text)
        {
            List<Translation> knownTranslations = _vocabulary.GetKnownTranslations(text);
            List<bool> indexes = new List<bool>();

            for (int i = 0; i < text.Length; i++)
                indexes.Add(false);

            foreach (Translation translation in knownTranslations)
            {
                int index = text.IndexOf(translation.Target.Value);
                int length = translation.Target.Value.Length;

                for (int i = index; i < index + length; i++)
                    indexes[i] = true;
            }

            return CreateRuns(text, indexes);
        }

        //todo: it's a kind of shit
        private List<Run> CreateRuns(string text, List<bool> indexesToMark)
        {
            List<Run> runs = new List<Run>();

            while (indexesToMark.Count != 0)
            {
                Run run = new Run();

                bool initialValue = indexesToMark[0];

                int counter = 1;
                for (int i = 1; i < indexesToMark.Count; i++)
                    if (indexesToMark[i] == initialValue)
                        counter++;
                    else break;

                if (initialValue)
                    run.Foreground = SystemColors.MenuHighlightBrush;

                run.Text = text.Substring(0, counter);
                text = text.Remove(0, counter);
                indexesToMark.RemoveRange(0, counter);

                runs.Add(run);
            }

            return runs;
        }

        // TODO: Fix
        private List<Paragraph> SplitToSentences(List<string> textBlocks, char dot)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();

            foreach (string item in textBlocks)
            {
                Paragraph paragraph = new Paragraph();

                List<string> sentences = SplitSingleBlock(item, dot);

                foreach (string s in sentences)
                    paragraph.Inlines.AddRange(FindKnownTranslations(s));

                paragraphs.Add(paragraph);
            }

            return paragraphs;
        }

        private List<string> SplitSingleBlock(string block, char separator)
        {
            List<string> sentences = new List<string>();
            int count = block.Count(b => b == separator);

            if (count == 0)
            {
                sentences.Add(block);
                return sentences;
            }

            for (int i = 0; i < count; i++)
            {
                int dotIndex = block.IndexOf(separator) + 1;
                string sentence = new string(block.Take(dotIndex).ToArray());
                block = block.Remove(0, dotIndex);
                sentences.Add(sentence);
            }

            return sentences;
        }

        private List<string> SplitToBlocks(string text)
        {
            List<string> blocks = text.Split('\n').ToList();
            blocks.RemoveAll(x => x == "\r");
            return blocks;
        }
    }
}
