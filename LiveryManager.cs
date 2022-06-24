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
            WalkFilesAndDirectories(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Documents/iRacing/paint");
        }

        void WalkFilesAndDirectories(string path)
        {
            // process all sub-folders (in any)
            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                WalkFilesAndDirectories(directory);
            }
            // process all files (in any)
            foreach (var file in Directory.EnumerateFiles(path))
            {
                ProcessFile(file);
            }
        }

        void ProcessFile(string file)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastAccessTime <= DateTime.Now.AddDays(this.deleteOlderThanDays * -1))
            {
                foreach (int exceptedDriverTeamID in this.exceptedDriverIDs)
                {
                    if (fileInfo.Name.EndsWith(exceptedDriverTeamID + ".tga") || fileInfo.Name.EndsWith(exceptedDriverTeamID + ".mip"))
                    {
                        return;
                    }
                }
                Console.WriteLine(fileInfo.FullName + " hasn't been accessed within the last " + this.deleteOlderThanDays + " days and will be deleted.");
                File.Delete(file);
            }
        }
    }
}