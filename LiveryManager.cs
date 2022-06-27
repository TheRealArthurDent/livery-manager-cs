using System.Configuration;

namespace LiveryManager
{
    class FileWalker
    {
        private int deleteOlderThanDays;
        private ICollection<int> exceptedDriverIDs;

        public FileWalker(int deleteOlderThanDays, ICollection<int> exceptedDriverIDs)
        {
            this.deleteOlderThanDays = deleteOlderThanDays;
            this.exceptedDriverIDs = exceptedDriverIDs;
        }

        public void Walk()
        {
            string workingDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Documents/iRacing/paint";
            long freedSpace = WalkFilesAndDirectories(workingDir);
            Console.WriteLine($"FileWalker freed up {FormatToHumanReadableSize(freedSpace)} in {workingDir} and its subfolders.");
        }

        long WalkFilesAndDirectories(string path)
        {
            long freedSpace = 0;
            // process all sub-folders (in any)
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                freedSpace += WalkFilesAndDirectories(directory);
            }
            // process all files (in any)
            foreach (var file in Directory.EnumerateFiles(path))
            {
                freedSpace += ProcessFile(file);
            }
            return freedSpace;
        }

        long ProcessFile(string file)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastAccessTime <= DateTime.Now.AddDays(this.deleteOlderThanDays * -1))
            {
                foreach (int exceptedDriverTeamID in this.exceptedDriverIDs)
                {
                    if (fileInfo.Name.EndsWith(exceptedDriverTeamID + ".tga") || fileInfo.Name.EndsWith(exceptedDriverTeamID + ".mip"))
                    {
                        return 0;
                    }
                }
                Console.WriteLine(fileInfo.FullName + " hasn't been accessed within the last " + this.deleteOlderThanDays + " days and will be deleted.");
                File.Delete(file);
                return fileInfo.Length;
            }
            return 0;
        }

        string FormatToHumanReadableSize(long bytes)
        {
            if (bytes > 1024 * 1024 * 1024)
            {
                return String.Format("{0:0.##}", bytes / (1024.0 * 1024.0 * 1024.0)) + " GB";
            }
            if (bytes > 1024 * 1024)
            {
                return String.Format("{0:0.##}", bytes / (1024.0 * 1024.0)) + " MB";
            }
            if (bytes > 1024)
            {
                return String.Format("{0:0.##}", bytes / 1024.0) + " KB";
            }
            return bytes + " bytes";
        }
    }
}