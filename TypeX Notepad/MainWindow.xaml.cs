using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Forms;
using TypeX_Notepad.Properties;
using System.Windows.Controls;
using System.Text;
using System;
using System.Drawing;

namespace TypeX_Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FlowDocument flowDocument;
        private System.Windows.Controls.PrintDialog printDialog;
        private PrintPreviewDialog printPreviewDialog;
        private PageSetupDialog pageSetupDialog;
        private FontDialog fontDialog;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private Encoding encoding;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TextBox.CanUndo;
        }
        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox.Undo();
        }
        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TextBox.CanRedo;
        }
        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TextBox.Redo();
        }
        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            InstantiateControls();
            TextBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
            UpdateStatusBar();
            //flowDocument = new FlowDocument();
            //printDialog = new System.Windows.Controls.PrintDialog();
            //printPreviewDialog = new PrintPreviewDialog();
            //pageSetupDialog = new PageSetupDialog();
            //flowDocument.PagePadding = new Thickness(40);
            //flowDocument.Blocks.Add(new Paragraph(new Run(TextBox.Text)));
            //printDialog.PrintDocument((((IDocumentPaginatorSource)flowDocument).DocumentPaginator),"Using Paginator");
        }
        private void InstantiateControls()
        { 
            SpellCheck.IsChecked = Settings.SpellCheckSet;
            WordWrap.IsChecked = TextWrappingToBool(Settings.WordWrapSet);
            StatusBar.IsChecked = Settings.StatusBarSet;
            SetUpCustomCommands();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            fontDialog = new FontDialog();
            fontDialog.ShowEffects = false;
            fontDialog.ShowColor = false;
            TextBox.FontFamily = Settings.DefaultFont;
        }
        private void SetUpCustomCommands()
        {
            RoutedUICommand AutoSaveCmd = new RoutedUICommand("Used for AutoSave", "AutoSaveCommand", typeof(MainMenu),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.T,ModifierKeys.Control)
                });
            RoutedUICommand SaveAsCmd = new RoutedUICommand("Used for SaveAs", "SaveAsCommand", typeof(MainMenu),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.S,ModifierKeys.Alt)
                });
            RoutedUICommand PageSetupCmd = new RoutedUICommand("Used for Page Setup", "PageSetupCommand", typeof(MainMenu),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.P,ModifierKeys.Alt)
                });
            RoutedUICommand ExitCmd = new RoutedUICommand("Used for Exit", "ExitCommand", typeof(MainMenu),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.Escape,ModifierKeys.Shift)
                });
            CommandBindings.Add(new CommandBinding(AutoSaveCmd,new ExecutedRoutedEventHandler(AutoSave_Executed),
                new CanExecuteRoutedEventHandler(AutoSave_CanExecute)));
            CommandBindings.Add(new CommandBinding(SaveAsCmd,new ExecutedRoutedEventHandler(SaveAs_Executed),
                new CanExecuteRoutedEventHandler(SaveAs_CanExecute)));
            CommandBindings.Add(new CommandBinding(PageSetupCmd, new ExecutedRoutedEventHandler(PageSetup_Executed),
                new CanExecuteRoutedEventHandler(PageSetup_CanExecute)));
            CommandBindings.Add(new CommandBinding(ExitCmd, new ExecutedRoutedEventHandler(Exit_Executed),
                new CanExecuteRoutedEventHandler(Exit_CanExecute)));
            AutoSave.Command = AutoSaveCmd;
            SaveAs.Command = SaveAsCmd;
            PageSetup.Command = PageSetupCmd;
            Exit.Command = ExitCmd;
        }

        private void PageSetup_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PageSetup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Page Setup");
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Save As");
        }
        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Font_Click(object sender, RoutedEventArgs e)
        {
            fontDialog.Font = new Font(new System.Drawing.FontFamily(Settings.DefaultFont.Source), 23);
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily(fontDialog.Font.Name);
                if (!fontFamily.Equals(Settings.DefaultFont))
                {
                    Settings.DefaultFont = fontFamily;
                    Settings.Save();
                }
            }
            
        }
        private static Settings Settings { get { return Settings.Default; } }
        private void UpdateStatusBar()
        {
            int row = TextBox.GetLineIndexFromCharacterIndex(TextBox.CaretIndex);
            int col = TextBox.GetLineLength(row);
            int words = GetWordCount();
            if(LineLabel!=null)
                LineLabel.Text = "Line : " + (row + 1);
            if(ColLabel!=null)
                ColLabel.Text = "Col : " + (col + 1);
            if (WordsLabel != null)
                WordsLabel.Text = "Words : " + words;
            if (Settings.AutoSaveSet&&AutosaveLabel!=null)
                AutosaveLabel.Visibility = Visibility.Visible;
        }
        private int GetWordCount()
        {
            string temp = TextBox.Text.Trim();
            if (string.IsNullOrEmpty(temp))
                return 0;
            int count = 0;
            bool word = false;
            foreach(char letter in temp)
            {
                if(!char.IsLetterOrDigit(letter))
                {
                    word = false;
                    continue;
                }
                if (word)
                    continue;
                count++;
                word = true;
            }
            return count;
        }
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateStatusBar();
        }
        private void SpellCheck_Checked(object sender, RoutedEventArgs e)
        {
            Settings.SpellCheckSet = true;
            Settings.Save();
        }
        private void SpellCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.SpellCheckSet = false;
            Settings.Save();
        }
        private void WordWrap_Checked(object sender, RoutedEventArgs e)
        {
            Settings.WordWrapSet = TextWrapping.Wrap;
            Settings.Save();
        }        
        private void WordWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.WordWrapSet = TextWrapping.NoWrap;
            Settings.Save();
        }
        private bool TextWrappingToBool(TextWrapping textWrapping)
        {
            return textWrapping == TextWrapping.Wrap;
        }
        private void StatusBar_Checked(object sender, RoutedEventArgs e)
        {
            Settings.StatusBarSet = true;
            Settings.Save();
        }
        private void StatusBar_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.StatusBarSet = false;
            Settings.Save();
        }
        private void AutoSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ToggleAutoSave()
        {
            Settings.AutoSaveSet = !Settings.AutoSaveSet;
            Settings.Save();
        }
        private void AutoSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleAutoSave();
        }
        private void AutoSave_Click(object sender,RoutedEventArgs e)
        {
            ToggleAutoSave();
        }
    }
}
