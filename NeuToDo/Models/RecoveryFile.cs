using NeuToDo.ViewModels;

namespace NeuToDo.Models
{
    public class RecoveryFile
    {
        public FileSource FileSource { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public enum FileSource
    {
        Local,
        Server
    }
}