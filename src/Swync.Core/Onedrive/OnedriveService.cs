using System.Collections.Generic;

namespace Swync.Core.Onedrive
{
    public class OnedriveService
    {
        public IList<OnedriveFile> GetFiles()
        {
            return new[]
            {
                new OnedriveFile("one/file"),
                new OnedriveFile("another/file")
            };
        }
    }

    public class OnedriveFile
    {
        public OnedriveFile(string path)
        {
            Path = path;
        }
        
        public string Path { get; }
    }
}