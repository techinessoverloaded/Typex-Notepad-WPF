using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using TypeX_Notepad.Properties;
using System.Windows.Controls;
using System.Text;
using System.IO;
using TypeX_Notepad.Models;

namespace TypeX_Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FlowDocument flowDocument;
        private DocumentModel CurrentDocument;
        private Encoding encoding;
        private static readonly string DefaultText = "Drag and drop a file here or start typing";
        private static readonly string DefaultTitle = "TypeX Notepad - Untitled"; 
        private static readonly string AutoSaveMessage = "Saving the file as you are typing...";
        private FormControlUtils FormControlUtils;
        public MainWindow()
        {
            InitializeComponent();
            CurrentDocument = new DocumentModel();
            FormControlUtils = new FormControlUtils(ref CurrentDocument);
        }
        public MainWindow(DocumentModel document)
        {
            InitializeComponent();
            CurrentDocument = document;
            FormControlUtils = new FormControlUtils(ref CurrentDocument);
            TextBox.Text = CurrentDocument.Content;
            TextBox.Text = CurrentDocument.Content;
            EncodingSelector.SelectedItem = EncodingSelector.Items.GetItemAt(EncodingToInt(CurrentDocument.Encoding));
            SetTitle(0);
            UpdateStatusBar();
        }
        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!CurrentDocument.IsSaved&&!CurrentDocument.Content.Equals(DefaultText))
            {
                if (MessageBox.Show("Do you want to save the Untitled file before opening a New File ?", "TypeX Notepad - Save Untitled File", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFileThroughDialog();
                }
            }
            NewFile();
        }
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(!CurrentDocument.IsSaved)
            {
                if(MessageBox.Show("Do you want to save the Untitled file before opening another File ?", "TypeX Notepad - Save Untitled File",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    SaveFileThroughDialog();
                }
            }
            OpenFileThroughDialog();
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
            TextBox.KeyUp += new System.Windows.Input.KeyEventHandler(TextBox_KeyUp);
            EncodingSelector.SelectionChanged += new SelectionChangedEventHandler(Encoding_SelectionChanged);
            UpdateStatusBar();
            TextBox.Focus();
            TextBox.SelectAll();
            //flowDocument = new FlowDocument();
            //printDialog = new System.Windows.Controls.PrintDialog();
            //printPreviewDialog = new PrintPreviewDialog();
            //pageSetupDialog = new PageSetupDialog();
            //flowDocument.PagePadding = new Thickness(40);
            //flowDocument.Blocks.Add(new Paragraph(new Run(TextBox.Text)));
            //printDialog.PrintDocument((((IDocumentPaginatorSource)flowDocument).DocumentPaginator),"Using Paginator");
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(AutosaveLabel!=null)
            {
                if (AutosaveLabel.Text.Equals(AutoSaveMessage))
                    AutosaveLabel.Text = string.Empty;
            }
        }

        private void InstantiateControls()
        {
            SpellCheck.IsChecked = Settings.SpellCheckSet;
            WordWrap.IsChecked = TextWrappingToBool(Settings.WordWrapSet);
            StatusBar.IsChecked = Settings.StatusBarSet;
            SetUpCustomCommands();
            TextBox.FontFamily = Settings.DefaultFont;
        }
        private void SetUpCustomCommands()
        {
            RoutedUICommand AutoSaveCmd = new RoutedUICommand("Used for AutoSave", "AutoSaveCommand", typeof(MainWindow),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.T,ModifierKeys.Control)
                });
            RoutedUICommand SaveAsCmd = new RoutedUICommand("Used for SaveAs", "SaveAsCommand", typeof(MainWindow),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.S,ModifierKeys.Alt)
                });
            RoutedUICommand PageSetupCmd = new RoutedUICommand("Used for Page Setup", "PageSetupCommand", typeof(MainWindow),
               new InputGestureCollection()
                {
                    new KeyGesture(Key.P,ModifierKeys.Alt)
                });
            RoutedUICommand ExitCmd = new RoutedUICommand("Used for Exit", "ExitCommand", typeof(MainWindow),
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
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentDocument.IsInvalidFile)
                SaveFileThroughDialog();
            else
                File.WriteAllText(CurrentDocument.FilePath, CurrentDocument.Content, CurrentDocument.Encoding);
            SetTitle(0);
        }
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileThroughDialog();
            SetTitle(0);
        }
        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void PrintPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Print Preview");
        }
        private void PrintPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Print");
        }
        private void Print_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Font_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private static Settings Settings { get { return Settings.Default; } }
        private void UpdateStatusBar()
        {
            int row = TextBox.GetLineIndexFromCharacterIndex(TextBox.CaretIndex);
            int col = TextBox.GetLineLength(row);
            int words = GetWordCount();
            if (LineLabel != null)
                LineLabel.Text = "Line : " + (row + 1);
            if(ColLabel!=null)
                ColLabel.Text = "Col : " + (col + 1);
            if (WordsLabel != null)
                WordsLabel.Text = "Words : " + words;
            if (AutosaveLabel != null)
            {
                if (Settings.AutoSaveSet&& CurrentDocument.FilePath !=null)
                { 
                    File.WriteAllText(CurrentDocument.FilePath, TextBox.Text);
                    AutosaveLabel.Text = AutoSaveMessage;
                }
                else
                {
                    AutosaveLabel.Text = string.Empty;
                }
            }   
        }
        private void Encoding_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            Encoding old = CurrentDocument.Encoding;
            CurrentDocument.Encoding = TextToEncoding(((sender as System.Windows.Controls.ComboBox).SelectedItem as ComboBoxItem).Content.ToString());
            if(old!=CurrentDocument.Encoding)
            {
                if(!Settings.AutoSaveSet)
                    SetTitle(1);
                else
                    File.WriteAllText(CurrentDocument.FilePath, CurrentDocument.Content, CurrentDocument.Encoding);
            }
        }
        private string EncodingToText(Encoding encoding)
        {
            if (encoding.Equals(Encoding.UTF8))
                return "UTF-8";
            if (encoding.Equals(Encoding.Unicode))
                return "UTF-16 LE";
            if (encoding.Equals(Encoding.BigEndianUnicode))
                return "UTF-16 BE";
            return string.Empty;
        }
        private Encoding TextToEncoding(string text)
        {
            if (text.Equals("UTF-8"))
                return Encoding.UTF8;
            if (text.Equals("UTF-16 LE"))
                return Encoding.Unicode;
            if (text.Equals("UTF-16 BE"))
                return Encoding.BigEndianUnicode;
            return Encoding.UTF8;
        }
        private int EncodingToInt(Encoding encoding)
        {
            if (encoding.Equals(Encoding.UTF8))
                return 0;
            if (encoding.Equals(Encoding.Unicode))
                return 1;
            if (encoding.Equals(Encoding.BigEndianUnicode))
                return 2;
            return -1;
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
        private void TextBox_TextChanged(object sender,TextChangedEventArgs e)
        {
            if (CurrentDocument != null)
            {
                CurrentDocument.Content = TextBox.Text;
                if (!Settings.AutoSaveSet)
                {
                    SetTitle(1);
                    CurrentDocument.IsSaved = false;
                }
                else 
                {
                    if(!CurrentDocument.IsInvalidFile)
                    {
                        File.WriteAllText(CurrentDocument.FilePath, CurrentDocument.Content, CurrentDocument.Encoding);
                        CurrentDocument.IsSaved = true;
                    }
                }
                UpdateStatusBar();
            }
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
            SetTitle(0);
        }
        private void AutoSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleAutoSave();
        }
        private void AutoSave_Click(object sender,RoutedEventArgs e)
        {
            ToggleAutoSave();
        }
        private void SetTitle(int mode)
        {
            Title = GetTitle(mode);
        }
        private string GetTitle(int mode)
        {
            switch(mode)
            {
                case 0:
                    if (!CurrentDocument.IsInvalidFile)
                    {
                        if (Settings.AutoSaveSet)
                        {
                            return DefaultTitle.Replace("Untitled", CurrentDocument.FilePath + " (AutoSave Enabled)");
                        }
                        else
                        {
                            return DefaultTitle.Replace("Untitled", CurrentDocument.FilePath);
                        }
                    }
                    else
                    {
                        return DefaultTitle;
                    }
                case 1:
                    if (!CurrentDocument.IsInvalidFile)
                    {
                        return DefaultTitle.Replace("Untitled",CurrentDocument.FilePath+"*");
                    }
                    else
                    {
                        return DefaultTitle.Replace("Untitled","Untitled*");
                    }
                default:
                    return string.Empty;
            }
        }
        private void NewFile()
        {
            CurrentDocument.Content = DefaultText;
            CurrentDocument.Encoding = Encoding.UTF8;
            CurrentDocument.Extension = ".txt";
            CurrentDocument.FilePath = null;
            CurrentDocument.FileName = null;
            CurrentDocument.IsSaved = false;
            SetTitle(0);
            UpdateStatusBar();
        }
        private void OpenFileThroughDialog()
        {
            if (FormControlUtils.ShowOpenFileDialog())
            {
                TextBox.Text = CurrentDocument.Content;
                EncodingSelector.SelectedItem = EncodingSelector.Items.GetItemAt(EncodingToInt(CurrentDocument.Encoding));
                SetTitle(0);
                UpdateStatusBar();
            }
        }
        private void SaveFileThroughDialog()
        {
            if(FormControlUtils.ShowSaveFileDialog())
                SetTitle(0);
        }
    }
}
