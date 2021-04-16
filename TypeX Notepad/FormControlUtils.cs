using System.IO;
using System.Windows.Forms;
using TypeX_Notepad.Models;
namespace TypeX_Notepad
{
    public class FormControlUtils
    {
        private OpenFileDialog openFileDialog;
        private PrintPreviewDialog printPreviewDialog;
        private PageSetupDialog pageSetupDialog;
        private FontDialog fontDialog;
        private SaveFileDialog saveFileDialog;
        private PrintDialog printDialog;
        private DocumentModel Document;
        public FormControlUtils(ref DocumentModel Document)
        {
            this.Document = Document;
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
        }
        public bool ShowOpenFileDialog()
        {
            openFileDialog.Filter = "Text Documents(*.txt)|*.txt|All Files(*.*)|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.DefaultExt = ".txt";
            if (openFileDialog.ShowDialog()==DialogResult.OK)
            {
                StreamReader objReader = new StreamReader(openFileDialog.FileName,true);
                Document.Content = objReader.ReadToEnd();
                Document.Encoding = objReader.CurrentEncoding;
                objReader.Close();
                Document.FilePath = openFileDialog.FileName;
                Document.FileName = Path.GetFileName(Document.FilePath);
                Document.Extension = Path.GetExtension(Document.FilePath);
                Document.IsSaved = true;
                return true;
            }
            return false;
        }
        public bool ShowSaveFileDialog()
        {
            saveFileDialog.Filter = "Text Documents(*.txt)|*.txt|All Files(*.*)|*.*";
            saveFileDialog.DefaultExt = Document.Extension;
            saveFileDialog.FileName = "*.txt";
            if(saveFileDialog.ShowDialog()==DialogResult.OK)
            {
                Document.FilePath = saveFileDialog.FileName;
                Document.FileName = Path.GetFileName(Document.FilePath);
                Document.Extension = Path.GetExtension(Document.FilePath);
                File.WriteAllText(Document.FilePath,Document.Content,Document.Encoding);
                Document.IsSaved = true;
                return Document.IsInvalidFile==false;
            }
            return false;
        }
    }
}
