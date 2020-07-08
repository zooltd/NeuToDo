using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IBackupService
    {
        Task ImportAsync(List<FileType> allowedTypes = null);

        Task<string> ExportAsync();
    }

    public static class PlatformFileTypeDict
    {
        public static Dictionary<FileType, string> UWPTypes = new Dictionary<FileType, string>
            {{FileType.Sqlite, ".sqlite3"}};

        public static Dictionary<FileType, string> DroidTypes = new Dictionary<FileType, string>
            {{FileType.Sqlite, null}};
    }

    public enum FileType
    {
        Sqlite
    }
}