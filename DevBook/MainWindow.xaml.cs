using DevBook.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Lang = DevBook.Data.Language;
using DevBook.Services;

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
                new Command() { Name = "ReadText", Action = ReadText },
                new Command() { Name = "OpenDictionary", Action = OpenDictionary }
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
            Loader loader = new Loader(@"C:\Users\Xiaomi\Documents\01.txt", 512);
            
            xMain.Children.Clear();

            ReadTextControl control = new ReadTextControl(_vocabulary, _targetLanguage, _nativeLanguage, loader.LoadText());
            xMain.Children.Add(control);
        }

        private void OpenDictionary()
        {
            var list = _vocabulary.GetTranslationsByLanguagePair(_targetLanguage, _nativeLanguage);

            xMain.Children.Clear();

            //TODO: choose dictionary language
            VocabularyControl control = new VocabularyControl(list);
            xMain.Children.Add(control);
        }
    }
}
