using DevBook.Data;
using DevBook.Data.Adapters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using RichTextBox = System.Windows.Controls.RichTextBox;
using Lang = DevBook.Data.Language;

namespace DevBook
{
    public partial class MainWindow : Window
    {
        private Vocabulary _vocabulary;
        private List<Command> _commands;

        // todo: ask about it at start
        private readonly Lang _nativeLanguage;
        // todo: ask about it as loading file
        private readonly Lang _targetLanguage;

        public MainWindow()
        {
            _nativeLanguage = Lang.Russian;
            _targetLanguage = Lang.Japanese;

            _commands = new List<Command>
            {
                new Command() { Name = "ReadText", Action = ReadText }
            };

            InitializeComponent();
            xcomboBox.ItemsSource = _commands;
            xcomboBox.MouseWheel += XcomboBox_MouseWheel;
            xOkButton.Click += xOkButtonClicked;

            _vocabulary = new Vocabulary(Environment.CurrentDirectory);
        }

        private void XcomboBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int next = _commands.IndexOf(xcomboBox.SelectedItem as Command);

            if (e.Delta > 0)
            {
                next++; // up
                if(next >= _commands.Count)
                    next = 0;
            }
            else if(e.Delta < 0)
            {
                next--; // down
                if(next <= -1)
                    next = _commands.Count - 1;
            }

            xcomboBox.SelectedItem = _commands[next];
        }

        private void xOkButtonClicked(object sender, RoutedEventArgs e)
        {
            Command command = (xcomboBox.SelectedItem as Command);
            if(command != null)
                command.Action();
        }

        private void ReadText()
        {
            // RequestWindow requestWindow = new RequestWindow();
            // if (requestWindow.ShowDialog() == true) { }

            //todo: get the file using dialog
            string path = @"C:\Users\Xiaomi\Documents\01.txt";
            string text = "";

            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[512];
                
                UTF8Encoding temp = new UTF8Encoding();
                //Encoding temp = Encoding.GetEncoding(932);

                while (fs.Read(b, 0, b.Length) > 0)
                    text += temp.GetString(b);
            }

            Debug.WriteLine(Environment.CurrentDirectory);
            ShowText(text);
        }

        public void ShowText(string text)
        {
            // split by blocks
            List<string> blocks = SplitToBlocks(text);

            // split by sentences
            var paragraphs = SplitToSentences(blocks, _vocabulary.Dot);

            foreach (Paragraph item in paragraphs)
                xText.Document.Blocks.Add(item);
            
            xText.SelectionChanged += Textbox_SelectionChanged;
            xSave.Click += XSave_Click;
        }

        private void XSave_Click(object sender, RoutedEventArgs e)
        {
            Translation translation = new Translation
            {
                Target = new Word
                {
                    Value = xWord1.Text,
                    Language = _targetLanguage
                },

                Native = new Word
                {
                    Value = xWord2.Text,
                    Language = _nativeLanguage
                }
            };

            _vocabulary.AddWord(translation.Target);
            _vocabulary.AddWord(translation.Native);
            _vocabulary.AddTranslation(translation);
        }
        
        private void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string selectedText = (sender as RichTextBox).Selection.Text;
            xWord1.Text = selectedText;

            // todo: hightlight the text

            if (selectedText != "" && selectedText != " ")
            {
                Word word = _vocabulary.GetWord(selectedText, _targetLanguage);
                Translation translation = _vocabulary.GetTranslation(selectedText, _targetLanguage, _nativeLanguage);

                //check if found
                //if (word != null)
                xWord2.Text = translation.Native.Value;

                var httpWebRequest = WebRequest.Create($"https://jisho.org/api/v1/search/words?keyword=\"{selectedText}\"");
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                
                using var streamReader = new StreamReader(httpResponse.GetResponseStream());

                JishoAdapter data = JsonConvert.DeserializeObject<JishoAdapter>(streamReader.ReadToEnd());

                if (xWord2.Text == "" || xWord2.Text == null)
                    if (data.Data.Count > 0)
                    {
                        //then build string
                        string toShow = "";

                        foreach (Sense sense in data.Data[0].Senses)
                        {
                            foreach (string definition in sense.EnglishDefinitions)
                            {
                                toShow += definition + "\t";
                            }
                            toShow += "\n";
                        }

                        xWord2.Text = toShow;
                    }
            }
        }

        private List<Paragraph> SplitToSentences(List<string> textBlocks, char dot)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();

            foreach (string item in textBlocks)
            {
                Paragraph paragraph = new Paragraph();

                List<string> sentences = item.Split(dot).ToList();

                sentences.RemoveAll(x => x == dot.ToString() + "\r");
                sentences.RemoveAll(x => x == "\r" + dot.ToString());

                foreach (string s in sentences)
                {
                    paragraph.Inlines.AddRange(FindKnownTranslations(s + dot));
                }

                paragraphs.Add(paragraph);
            }
            
            return paragraphs;
        }

        private List<string> SplitToBlocks(string text)
        {
            List<string> blocks = text.Split('\n').ToList();
            blocks.RemoveAll(x => x == "\r");
            return blocks;
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

            while(indexesToMark.Count != 0)
            {
                Run run = new Run();
                
                bool initialValue = indexesToMark[0];

                int counter = 1;
                for (int i = 1; i < indexesToMark.Count; i++)
                    if(indexesToMark[i] == initialValue)
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
    }
}
