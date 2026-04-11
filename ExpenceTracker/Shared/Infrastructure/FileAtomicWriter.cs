using System.IO;
using System.Threading.Tasks;

namespace ExpenceTracker.Shared.Infrastructure
{
    public static class FileAtomicWriter
    {
        public static async Task WriteAllTextAsync(string filePath, string content)
        {
            string tempPath = filePath + ".tmp";
            try
            {
                await File.WriteAllTextAsync(tempPath, content);
                File.Move(tempPath, filePath, overwrite: true);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
    }
}
