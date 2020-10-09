using System;
using System.Windows;

namespace DevBook
{
    public partial class App : Application
    {
        public static void UpdateUi(Action action) =>
            Application.Current.Dispatcher.Invoke(action);
    }
}
