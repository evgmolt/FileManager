using fileman2.Messages;
using System;
using System.IO;

namespace fileman2
{
    public class Utils
    {
        public static string GetDirFileString(int level, System.IO.FileSystemInfo dirfile)
        {
            string indent = level == 0 ? "" : "  ";
            char horizLine = Properties.Settings1.Default.HorizLines ? '-' : ' ';
            int mode = Properties.Settings1.Default.ViewMode;
            int maxFileNameLen = FMConstants.GetMaxFileNameLen(mode);
            string s = indent + dirfile.Name;
            if (s.Length > maxFileNameLen)
                s = s.Substring(0, maxFileNameLen - 3) + FMStrings.tooLongString;//если длина строки больше ширины колонки
            s = s.PadRight(maxFileNameLen, horizLine);//колонка 1
            if (mode > 0)
            {
                string s1;
                if (dirfile.GetType() == typeof(System.IO.FileInfo))
                {
                    System.IO.FileInfo f = (System.IO.FileInfo)dirfile;
                    s1 = FMStrings.GetSizeString(f.Length);
                }
                else
                    s1 = FMStrings.directory;
                s1 = s1.PadRight(FMConstants.sizeStringLen); //колонка 2
                s += s1;
            }
            if (mode > 1)
            {
                string s1 = FMStrings.GetFileAttributesString(dirfile.Attributes);
                s1 = s1.PadRight(FMConstants.attrStringLen); //колонка 3
                s += s1;
            }
            if (mode > 2)
            {
                string s1 = dirfile.CreationTime.ToString(FMStrings.dateTimePatt);
                s1 = s1.PadRight(FMConstants.timeStringLen); //колонка 4
                s += s1;
            }
            if (mode > 3)
            {
                string s1 = dirfile.LastAccessTime.ToString(FMStrings.dateTimePatt);
                s1 = s1.PadRight(FMConstants.timeStringLen); //колонка 5
                s += s1;
            }
            if (mode > 4)
            {
                string s1 = dirfile.LastWriteTime.ToString(FMStrings.dateTimePatt);
                s1 = s1.PadRight(FMConstants.timeStringLen); //колонка 6
                s += s1;
            }
            return s;
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            FileInfo[] files = d.GetFiles();
            foreach (FileInfo fi in files)
            {
                size += fi.Length;
            }
            DirectoryInfo[] dirs = d.GetDirectories();
            foreach (DirectoryInfo di in dirs)
            {
                size += DirSize(di);
            }
            return size;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, IMessager messager)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                messager.ShowAndSaveError(sourceDirName + FMStrings.dirNotExist, false);
                return;
            }
            try
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                Directory.CreateDirectory(destDirName);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string tempPath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, tempPath, copySubDirs, messager);
                    }
                }
            }
            catch (Exception e)
            {
                messager.ShowAndSaveError(e.Message, true);
            }
        }

        public static void FileCopyOrMove(bool move, string source, string dest, IMessager messager)
        {
            if (source.IndexOf("*") >= 0)
            {
                if (!Directory.Exists(dest))
                {
                    try
                    {
                        Directory.CreateDirectory(dest);
                    }
                    catch (Exception e)
                    {
                        messager.ShowAndSaveError(e.Message, true);
                    }
                }
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), source);
                foreach (string s in files)
                {
                    string fname = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(dest, fname);
                    FileCopyOrMovePrim(move, s, destFile, messager);
                }
            }
            else
                FileCopyOrMovePrim(move, source, dest, messager);

        }
        static void FileCopyOrMovePrim(bool move, string sourceFile, string destFile, IMessager messager)
        {
            if (!File.Exists(sourceFile))
            {
                messager.ShowAndSaveError(sourceFile + FMStrings.fileNotExist, false);
                return;
            }
            if (File.Exists(destFile))
            {
                Console.SetCursorPosition(2, FMConstants.messPosition + 1);
                Console.WriteLine(destFile + FMStrings.fileExist);
                Console.SetCursorPosition(2, FMConstants.messPosition + 2);
                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key != ConsoleKey.Y)
                    return;
            }
            try
            {
                if (move)
                {
                    File.Move(sourceFile, destFile);
                }
                else
                {
                    File.Copy(sourceFile, destFile, true);
                }
            }
            catch (Exception e)
            {
                messager.ShowAndSaveError(e.Message, true);
            }
        }

    }
}
