using System.Windows;
using System.Windows.Input;
namespace TypeX_Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void New_CanExecute(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void New_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("New command executed");
        }
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Open Command executed");
        }
        public static class TypeXCommands
        {
            public static readonly RoutedUICommand AutoSave = new RoutedUICommand("AutoSave","AutoSave",typeof(TypeXCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.T,ModifierKeys.Control)
                });
        }
    }
}
