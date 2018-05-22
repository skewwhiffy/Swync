using System;
using System.IO;

namespace Swync.Test.Common.TestFiles
{
    public class TestFolder : IDisposable
    {
        private TestFolder(string prefix)
        {
            prefix = prefix.EndsWith("_") ? prefix : $"{prefix}_";
            var name = $"{prefix}{Guid.NewGuid()}";
            Info = Directory.CreateDirectory(name);
        }

        public DirectoryInfo Info { get; }

        public FileInfo CreateRandomFile(long bytes)
        {
            var fileName = $"{Guid.NewGuid()}.swync.test";
            var fullFileName = Path.Combine(Info.FullName, fileName);
            var randomBytes = new byte[bytes];
            new Random().NextBytes(randomBytes);
            File.WriteAllBytes(fullFileName, randomBytes);
            return new FileInfo(fullFileName);
        }

        public static TestFolder WithPrefix(string prefix)
        {
            return new TestFolder(prefix);
        }
        
        public void Dispose()
        {
            void DeleteDirectory(string path)
            {
                foreach (var file in Directory.EnumerateFiles(path))
                {
                    File.Delete(file);
                }

                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    Directory.Delete(directory, true);
                }

                Directory.Delete(path);
            }

            try
            {
                if (!Directory.Exists(Info.FullName))
                {
                    return;
                }

                DeleteDirectory(Info.FullName);
            }
            catch
            {
                // Don't care.
            }
        }
    }
}