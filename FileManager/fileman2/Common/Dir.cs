using fileman2.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fileman2.Common
{
    public class Dir
    {
        private readonly List<Item> _dirList = new List<Item>();
        public List<Item> GetList() => _dirList;
        public Dir(DirectoryInfo startPathInfo, IMessager messager)
        {
            try
            {
                System.IO.DirectoryInfo[] subDirs = startPathInfo.GetDirectories();
                System.IO.FileInfo[] files = startPathInfo.GetFiles("*.*");
                for (int i = 0; i < subDirs.Count(); i++)
                {
                    _dirList.Add(new Item() { Content = Utils.GetDirFileString(0, subDirs[i]), IsDirectory = true });
                    System.IO.DirectoryInfo[] subDirs2;
                    try
                    {
                        subDirs2 = subDirs[i].GetDirectories();
                    }
                    catch (Exception)
                    {
                        subDirs2 = new System.IO.DirectoryInfo[0];
                    }
                    System.IO.FileInfo[] files2;
                    try
                    {
                        files2 = subDirs[i].GetFiles("*.*");
                    }
                    catch (Exception)
                    {
                        files2 = new System.IO.FileInfo[0];
                    }
                    if (Properties.Settings1.Default.Level)
                    {
                        FillDirList(subDirs2, files2);
                    }
                }
                for (int i = 0; i < files.Length; i++)
                {
                    _dirList.Add(new Item() { Content = Utils.GetDirFileString(0, files[i]), IsDirectory = false });
                }
            }
            catch (Exception e)
            {
                messager.ShowAndSaveError(e.Message, true);
            }
        }

        void FillDirList(System.IO.DirectoryInfo[] dirs, System.IO.FileInfo[] files)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                _dirList.Add(new Item() { Content = Utils.GetDirFileString(1, dirs[i]), IsDirectory = true });
            }
            for (int i = 0; i < files.Length; i++)
            {
                _dirList.Add(new Item() { Content = Utils.GetDirFileString(1, files[i]), IsDirectory = false });
            }
        }
    }
}
