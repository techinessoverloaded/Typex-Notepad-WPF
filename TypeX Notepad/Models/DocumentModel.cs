using System.IO;
using System.Text;

namespace TypeX_Notepad.Models
{
    public class DocumentModel
    {
        private string _fileName;
        private string _filePath;
        private string _content;
        private Encoding _encoding;
        private string _extension;
        private bool _isSaved;
        private static readonly string DefaultContent="Drag and drop a file here or start typing";
        private static readonly string DefaultExtension = ".txt";
        public DocumentModel()
        {
            _fileName = null;
            _filePath = null;
            _content = DefaultContent;
            _encoding = Encoding.UTF8;
            _extension = DefaultExtension;
            _isSaved = false;
        }
        public DocumentModel(string filepath, string content, Encoding encoding)
        {
            _filePath = filepath;
            _fileName = Path.GetFileName(_filePath);
            _extension = Path.GetExtension(_filePath);
            _content = content;
            _encoding = encoding;
            _isSaved = IsInvalidFile ? false : true;
        }
        public string FileName{ get { return _fileName; } set { _fileName=value; } }
        public string FilePath { get { return _filePath; } 
            set 
            { 
                _filePath=value;
                _fileName = Path.GetFileName(_filePath);
                _extension = Path.GetExtension(_filePath);
            } 
        }
        public string Content { get { return _content; } set { _content=value; } }
        public string Extension { get { return _extension; } set { _extension=value; } }
        public Encoding Encoding { get { return _encoding; } set { _encoding=value; } }
        public bool IsInvalidFile { get { return (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileName)); }}
        public bool IsSaved { get { return _isSaved; } set { _isSaved = value; } }
    }
}
