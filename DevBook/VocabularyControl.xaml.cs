using DevBook.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DevBook
{
    public partial class VocabularyControl : UserControl
    {
        private const int _paging = 10;
        List<Translation> _list;
        List<(TextBox, TextBox)> _textBoxes;

        public VocabularyControl()
        {
            InitializeComponent();
        }

        public VocabularyControl(List<Translation> list)
        {
            InitializeComponent();

            _list = list;
            _textBoxes = new List<(TextBox, TextBox)>();

            _previousButton.Click += _previousButton_Click;
            _nextButton.Click += _nextButton_Click;
            _currentPageNumberTextBox.TextChanged += _currentPageNumberTextBox_TextChanged;

            int count = list.Count < _paging ? list.Count : _paging;

            for (int i = 0; i < count; i++)
            {
                TextBox targetTextBox = new TextBox()
                {
                    Text = list[i].Target.Value,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(targetTextBox, 0);
                Grid.SetRow(targetTextBox, i);
                xWordGrid.Children.Add(targetTextBox);

                TextBox nativeTextBox = new TextBox()
                {
                    Text = list[i].Native.Value,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(nativeTextBox, 1);
                Grid.SetRow(nativeTextBox, i);
                xWordGrid.Children.Add(nativeTextBox);

                _textBoxes.Add((targetTextBox, nativeTextBox));
            }

            _currentPageNumberTextBox.Text = "0";
        }

        private void _currentPageNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(_currentPageNumberTextBox.Text, out int result))
                _currentPageNumberTextBox.Text = "0";

            ShowPage(result);

            // int currentPosition = int.Parse(_currentPageNumberTextBox.Text);
            // currentPosition--;
            // Debug.WriteLine($"currentPosition = {currentPosition}");
            // int items = _list.Count;
            // Debug.WriteLine($"items = {items}");

            // if (currentPosition * _paging < items)
            // {
            //     int nextPosition = currentPosition + 1;
            //     Debug.WriteLine($"nextPosition = {nextPosition}");
            //     int maxItems = nextPosition * _paging;
            //     Debug.WriteLine($"maxItems = {maxItems}");
            //     int newCount = maxItems > items ? items : maxItems;
            //     Debug.WriteLine($"newCount = {newCount}");
            //     int afterNextPosition = nextPosition + 1;
            //     Debug.WriteLine($"afterNextPosition = {afterNextPosition}");

            //     Debug.WriteLine($"for (int i = {nextPosition * _paging}; i < {newCount * afterNextPosition}; i++)");
            //     for (int i = nextPosition * _paging; i < newCount * afterNextPosition; i++)
            //     {
            //         Debug.WriteLine($"_textBoxes[{i - nextPosition * _paging}].Item1.Text = _list[{i}].Target.Value");
            //         _textBoxes[i - nextPosition * _paging].Item1.Text = _list[i].Target.Value;
            //         _textBoxes[i - nextPosition * _paging].Item2.Text = _list[i].Native.Value;
            //     }
            // }
        }

        private void _nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(_currentPageNumberTextBox.Text, out int result))
                _currentPageNumberTextBox.Text = (result + 1).ToString();
        }

        private void _previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(_currentPageNumberTextBox.Text, out int result))
                _currentPageNumberTextBox.Text = (result - 1).ToString();
        }

        private void ShowPage(int page)
        {
            page = ValidatePage(page);

            _currentPageNumberTextBox.Text = page.ToString();

            int start;
            int end;
            
            (start, end) = GetRange(page);

            for (int i = start; i < end; i++)
            {
                _textBoxes[i % _paging].Item1.Text = _list[i].Target.Value;
                _textBoxes[i % _paging].Item2.Text = _list[i].Native.Value;
            }

            int filledCount = end - start;

            if (filledCount < 10)
            {
                for (int i = filledCount; i > 0; i--)
                {
                    _textBoxes[_paging - i].Item1.Text = "";
                    _textBoxes[_paging - i].Item2.Text = "";
                }
            }
        }

        private int ValidatePage(int page)
        {
            int pages = _list.Count / _paging;

            if (page > pages)
                page = 0;
            else if (page < 0)
                page = pages;
            
            return page;
        }

        private (int, int) GetRange(int page)
        {
            int indexStart = 0;
            int indexEnd = 10;

            int paging = page * _paging;
            
            indexStart += paging;
            indexEnd += paging;

            if(indexEnd > _list.Count)
                indexEnd = _list.Count;

            return (indexStart, indexEnd);
        }
    }
}
