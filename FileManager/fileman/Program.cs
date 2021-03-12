using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            Console.Title = Str.appTitle;
            InitSize();
            InitSettings();
            Walls walls = new Walls(Const.fieldWidth - 1, Const.fieldHeight - 1);
            Const.messPosition = walls.messPos;
            do
            {
                ShowDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()), walls);
                Console.SetCursorPosition(Const.left, Const.promptPosition);
                Console.Write(Directory.GetCurrentDirectory().ToString() + Str.prompt);
                string enterStr = Console.ReadLine();
                List<string> enters = Commands.ParseCommand(enterStr);
                if (enters == null)
                {
                    Console.WriteLine(Str.syntaxErr);
                }
                else
                if (enters[0] != String.Empty)
                switch (Commands.GetCommandNum(enters[0]))
                {
                    case 0:
                        walls.Draw();
                        ShowHelp();
                        ShowAndSaveError("", false);
                        break;
                    case 1: //exit
                        Properties.Settings.Default.DefaultPath = Directory.GetCurrentDirectory();
                        Properties.Settings.Default.Save();
                        return;
                    case 2: //создание папки
                        if (!Directory.Exists(enters[1]))
                        {
                           try
                           {
                               Directory.CreateDirectory(enters[1]);
                           }
                           catch (Exception e)
                           {
                               ShowAndSaveError(e.Message, true);
                           }
                        }
                        else
                        {
                            ShowAndSaveError(enters[1] + Str.dirExist, false);
                        }
                        break;
                    case 3: //смена текущей папки
                        if (enters.Count < 2)
                        {
                            ShowAndSaveError(Str.syntaxErr, false);
                            break;
                        };
                        if (Directory.Exists(enters[1]))
                            Directory.SetCurrentDirectory(enters[1]);
                        else
                            ShowAndSaveError(enters[1] +Str.dirNotExist, false);
                        break;
                    case 4: //удаление файла 
                        if (!File.Exists(enters[1]))
                        {
                            ShowAndSaveError(enters[1] +Str.fileNotExist, false);
                            break;
                        }
                        try
                        {
                            File.Delete(enters[1]);
                        }
                        catch (Exception e) 
                        {
                            ShowAndSaveError(e.Message, true);
                        }
                        break;
                    case 5: //удаление папки
                        if (enters.Count != 2)
                        {
                                ShowAndSaveError(Str.syntaxErr, false);
                                break;
                        }
                        try
                        {
                            Directory.Delete(enters[1], true);
                        }
                        catch (Exception e)
                        {
                            ShowAndSaveError(e.Message, true);
                        }
                        break;
                    case 6://COPY копирование файла 
                        if (enters.Count < 3)
                        {
                            ShowAndSaveError(Str.syntaxErr, false);
                            break;
                        }
                        if (enters[1].IndexOf("*") >= 0)
                        {
                            if (!Directory.Exists(enters[2]))
                            {
                               try
                               {
                                    Directory.CreateDirectory(enters[2]);
                               }
                               catch (Exception e)
                               {
                                    ShowAndSaveError(e.Message, true);
                                }
                            }
                            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), enters[1]);
                            foreach (string s in files)
                            {
                                string fname = System.IO.Path.GetFileName(s);
                                string destFile = System.IO.Path.Combine(enters[2], fname);
                                FileCopy(s, destFile);
                            }
                        }
                        else
                            FileCopy(enters[1], enters[2]);
                        break;
                    case 7://COPYD копирование папки
                        bool recurse = enters[1].ToUpper() == Commands.keyRecurs;
                        if (recurse)
                        {
                                if (enters.Count == 4)
                                    DirectoryCopy(enters[2], enters[3], recurse); //есть параметр -r
                                else
                                    ShowAndSaveError(Str.syntaxErr, false);
                        }
                        else
                        {
                                if (enters.Count == 3)
                                    DirectoryCopy(enters[1], enters[2], false);// нет параметра
                                else
                                    ShowAndSaveError(Str.syntaxErr, false);
                        }
                        break;
                    case 8: //режим отображения
                        if (enters.Count == 1)
                        {
                            Properties.Settings.Default.ViewMode = 0;
                            break;
                        }
                        byte res;
                        bool err = false;
                        if (Byte.TryParse(enters[1], out res))
                        {
                            if (res > 0 && res < Commands.numOfViewMode + 1)
                            {
                                Properties.Settings.Default.ViewMode = res;
                            }
                            else
                            {
                                err = true;
                            }
                        }
                        else
                        {
                            err = true;
                        }
                        if (err)
                        {
                            Console.WriteLine(Str.syntaxErr);
                        }
                        break;
                        case 9: //количество строк на странице, без параметров 10
                            if (enters.Count == 1)
                            {
                                Properties.Settings.Default.StringsOnPage = 10;
                                break;
                            }
                            byte res9;
                            bool err9 = false;
                            if (Byte.TryParse(enters[1], out res9))
                            {
                                if (res9 > 0 && res9 < byte.MaxValue)
                                {
                                    Properties.Settings.Default.StringsOnPage = res9;
                                }
                                else
                                {
                                    err9 = true;
                                }
                            }
                            else
                            {
                                err9 = true;
                            }
                            if (err9)
                            {
                                Console.WriteLine(Str.syntaxErr);
                            }
                            break;
                        case 10://Lines 
                            Properties.Settings.Default.HorizLines = !Properties.Settings.Default.HorizLines;
                            break;
                        case 11://DirSize 
                            if (enters.Count != 2)
                            {
                                ShowAndSaveError(Str.syntaxErr, false);
                                break;
                            }
                            long size;
                            try
                            {
                                size = DirSize(new DirectoryInfo(enters[1]));
                                ShowInfo(enters[1] + " : " + Str.GetSizeString(size));
                            }
                            catch (Exception e)
                            {
                                ShowAndSaveError(e.Message, true);
                            }
                            break;
                        case 12: //DI информация о диске
                            if (enters.Count == 1)
                            {
                                ShowDrives(walls);
                            }
                            break;
                        case 13: //отображение содержимого папки без смены текущей
                            if (enters.Count != 2)
                            {
                                ShowAndSaveError(Str.syntaxErr, false);
                                break;
                            }
                            ShowDirectory(new DirectoryInfo(enters[1]), walls);
                            ShowInfo("");
                            break;
                        case 14://Уровень отображения дерева
                            Properties.Settings.Default.Level = !Properties.Settings.Default.Level;
                            break;
                        case 15://Цвет фона
                            Properties.Settings.Default.BColor = !Properties.Settings.Default.BColor;
                            Const.backColor = Properties.Settings.Default.BColor ? ConsoleColor.Black : ConsoleColor.Blue;
                            break;
                        default:
                            walls.Draw();
                            ShowHelp();
                            ShowAndSaveError(enters[0] + Str.notCommand, false);
                            break;
                }
                Console.WriteLine();
            } 
            while (true);
        }

        static string[,] GetDrivesInfo()
        {
            int nName = 0;
            int nType = 1;
            int nFormat = 2;
            int nLabel = 3;
            int nTotal = 4;
            int nFree = 5;
            int nAvailable = 6;
            string[,] drivesInfoArr = null;
            DriveInfo[] drives = DriveInfo.GetDrives();
            try
            {
                drivesInfoArr = new string[Const.numOfDrivesParameters, drives.Length + 1];
                for (int i = 0; i < Const.numOfDrivesParameters; i++) //Заголовки столбцов
                {
                    drivesInfoArr[i, 0] = Str.driveInfoTitle[i];
                }
                for (int i = 0; i < drives.Length; i++)
                {
                    drivesInfoArr[nName, i + 1] = drives[i].Name;
                    drivesInfoArr[nType, i + 1] = drives[i].DriveType.ToString();
                    if (!drives[i].IsReady)
                    {
                        drivesInfoArr[nFormat, i + 1] = Str.driveNotReady;
                        drivesInfoArr[nLabel, i + 1] = String.Empty;
                        drivesInfoArr[nTotal, i + 1] = String.Empty;
                        drivesInfoArr[nFree, i + 1] = String.Empty;
                        drivesInfoArr[nAvailable, i + 1] = String.Empty;
                    }
                    else
                    {
                        drivesInfoArr[nFormat, i + 1] = drives[i].DriveFormat;
                        drivesInfoArr[nLabel, i + 1] = drives[i].VolumeLabel;
                        drivesInfoArr[nTotal, i + 1] = Str.GetSizeString(drives[i].TotalSize);
                        drivesInfoArr[nFree, i + 1] = Str.GetSizeString(drives[i].TotalFreeSpace);
                        drivesInfoArr[nAvailable, i + 1] = Str.GetSizeString(drives[i].AvailableFreeSpace);
                    }
                }
            }
            catch (Exception e)
            {
                ShowAndSaveError(e.Message, true);
            }
            return drivesInfoArr;

        }

        static void ShowDrives(Walls walls)
        {
            walls.Draw();
            string[,] dInfo = GetDrivesInfo();
            if (dInfo == null)
            {
                return;
            }
            int[] colW = Str.GetColumnsWidth(dInfo);
            Console.SetCursorPosition(Const.left, Const.top);
            for (int i = 0; i < dInfo.GetLength(1); i++)
            {
                if (i == 1)//Пустая строка после заголовка
                {
                    Console.WriteLine();
                    Console.SetCursorPosition(Const.left, Console.CursorTop);
                }
                for (int k = 0; k < dInfo.GetLength(0); k++)
                {
                    Console.Write(dInfo[k, i].PadRight(colW[k] + 2));
                }
                Console.WriteLine();
                Console.SetCursorPosition(Const.left, Console.CursorTop);
            }
            ShowInfo("");
        }

        static void FileCopy(string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile))
            {
                ShowAndSaveError(sourceFile + Str.fileNotExist, false);
                return;
            }
            if (File.Exists(destFile))
            {
                Console.SetCursorPosition(2, Const.messPosition + 1);
                Console.WriteLine(destFile + Str.fileExist);
                Console.SetCursorPosition(2, Const.messPosition + 2);
                ConsoleKeyInfo cki = Console.ReadKey();
                if (cki.Key != ConsoleKey.Y)
                    return;
            }
            try
            {
                File.Copy(sourceFile, destFile, true);
            }
            catch (Exception e)
            {
                ShowAndSaveError(e.Message, true);
            }
        }

        static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                ShowAndSaveError(sourceDirName + Str.dirNotExist, false);
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
                        DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                    }
                }
            }
            catch (Exception e)
            {
                ShowAndSaveError(e.Message, true);
            }
        }

        static void InitSettings()
        {
            Properties.Settings.Default.StringsOnPage = Const.messPosition - 5;
            if (Properties.Settings.Default.DefaultPath != String.Empty)
            {
                Directory.SetCurrentDirectory(Properties.Settings.Default.DefaultPath);
                Const.backColor = Properties.Settings.Default.BColor ? ConsoleColor.Black : ConsoleColor.Blue;
            }
            else
            {
                Properties.Settings.Default.DefaultPath = Directory.GetCurrentDirectory();
                Properties.Settings.Default.HelpFile = Path.Combine(Directory.GetCurrentDirectory(), Str.helpFileName);
                if (!Directory.Exists(Str.errorsDirName))
                    Directory.CreateDirectory(Str.errorsDirName);
                Properties.Settings.Default.ErrorsFile = Path.Combine(Directory.GetCurrentDirectory(),
                    Str.errorsDirName, Str.errorsFileName);
            }
        }

        static void InitSize()
        {
            
            int w = Console.LargestWindowWidth;
            int h = Console.LargestWindowHeight;
            Const.fieldWidth =  (int)(w * Const.scale);
            Const.fieldHeight = (int)(h * Const.scale);
            Console.SetBufferSize(Const.fieldWidth, Const.fieldHeight);
            Console.SetWindowSize(Const.fieldWidth, Const.fieldHeight);
            Const.maxFileNameLen = Const.fieldWidth / 3;
            Const.promptPosition = Const.fieldHeight - 2;
            Const.messPosition = Const.promptPosition - 2;

//Запрещаем изменение размеров окна
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
        }

        static void ShowInfo(string mess)
        {
            Console.SetCursorPosition(2, Const.messPosition + 1);
            Console.WriteLine(mess);
            Console.SetCursorPosition(2, Const.messPosition + 2);
            Console.WriteLine(Str.pressAnyKey);
            Console.ReadKey();
        }

        static void ShowAndSaveError(string mess, bool save )
        {
            ConsoleColor defColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(2, Const.messPosition + 1);
            DateTime dt = DateTime.Now;
            if (save)
            try
            {
                File.AppendAllText(Properties.Settings.Default.ErrorsFile, dt.ToString() + " " + mess + "\n");
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.Message);
            }
            Console.WriteLine(mess);
            Console.ForegroundColor = defColor;
            Console.SetCursorPosition(2, Const.messPosition + 2);
            Console.WriteLine(Str.pressAnyKey);
            Console.ReadKey();
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

        static void ShowHelp()
        {
            try
            {
                Console.SetCursorPosition(Const.left, Const.top);
                string[] txt = File.ReadAllLines(Properties.Settings.Default.HelpFile, Encoding.Default);
                foreach (string s in txt)
                {
                    Console.WriteLine(s);
                    Console.SetCursorPosition(Const.left, Console.CursorTop);
                }
            }
            catch (Exception e)
            {
                ShowAndSaveError(e.Message, true);
            }
        }

        static void ShowList(int pageNum, int totalPages, List<string> ls, List<bool> listIsDir, int max, Walls walls)
        {
            int total = 0;
            for (int i = 0; i < ls.Count; i++)
            {
                if (total == max)
                {
                    pageNum++;
                    Console.SetCursorPosition(2, Const.messPosition + 1);
                    Console.WriteLine($"Страница {pageNum} из {totalPages}");
                    Console.SetCursorPosition(2, Const.messPosition + 2);
                    Console.WriteLine(Str.pressAnyKey);
                    Console.SetCursorPosition(2, Const.messPosition + 2);
                    ConsoleKeyInfo cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Escape)
                    {
                        Console.SetCursorPosition(2, Const.messPosition + 2);
                        Console.WriteLine(Str.viewStopped);
                        return;
                    }
                    walls.Draw();
                    Console.SetCursorPosition(Const.left, Const.top);
                    Console.WriteLine(Str.GetTitle());
                    Console.SetCursorPosition(Const.left, Console.CursorTop);

                    total = 1;
                }
                Console.SetCursorPosition(Const.left, Console.CursorTop);
                if (listIsDir[i])
                {
                    Console.ForegroundColor = ConsoleColor.White;
                };
                Console.WriteLine(ls[i]);
                Console.ForegroundColor= ConsoleColor.Gray;
                total++;
            }
        }

        static void ShowDirectory(DirectoryInfo startPathInfo, Walls walls)
        {
            walls.Draw();
            Console.SetCursorPosition(Const.left, Const.top);
            Console.WriteLine(Str.GetTitle());
            int linesOnPage = Properties.Settings.Default.StringsOnPage;
            List<bool> listIsDir = new List<bool>();//Признак папки для выделения цветом
            List<string> listForShow = new List<string>();
            try
            {
                System.IO.DirectoryInfo[] subDirs = startPathInfo.GetDirectories();
                System.IO.FileInfo[] files = startPathInfo.GetFiles("*.*");
                for (int i = 0; i < subDirs.Count(); i++)
                {
                    listForShow.Add(GetDirFileString(0, subDirs[i]));
                    listIsDir.Add(true);
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
                    if (Properties.Settings.Default.Level)
                    {
                        FillDirList(listForShow, listIsDir, subDirs2, files2, walls);
                    }
                }
                for (int i = 0; i < files.Length; i++)
                {
                    listForShow.Add(GetDirFileString(0, files[i]));
                    listIsDir.Add(false);
                }
                int totalPages = listForShow.Count / linesOnPage;
                if ((listForShow.Count + listForShow.Count) % linesOnPage != 0)
                    totalPages++;
                Console.ForegroundColor = ConsoleColor.White;
                int pNum = 0;
                ShowList(pNum, totalPages, listForShow, listIsDir, linesOnPage, walls);
            }
            catch (Exception e)
            {
                ShowAndSaveError(e.Message, true);
            }

        }

        static string GetDirFileString(int level, System.IO.FileSystemInfo dirfile)
        {
            //char l0 = '+'; Пробовал с символами псвдографики, без них понравилось больше
            //char l1 = '\u251C';// ├
            //char l2 = '\u2500';// ─
            //char l02 = '\u2502';// │
            //char l3 = '\u2514'; //└
            string indent = level == 0 ? "" : "  ";
            char horizLine = Properties.Settings.Default.HorizLines ? '-' : ' ';
            int mode = Properties.Settings.Default.ViewMode;
            int maxFileNameLen = Const.GetMaxFileNameLen(mode);
            string s = indent + dirfile.Name;
            if (s.Length > maxFileNameLen)
                s = s.Substring(0, maxFileNameLen - 3) + Str.tooLongString;//если длина строки больше ширины колонки
            s = s.PadRight(maxFileNameLen, horizLine);//колонка 1
            if (mode > 0)
            {
                string s1;
                if (dirfile.GetType() == typeof(System.IO.FileInfo))
                {
                    System.IO.FileInfo f = (System.IO.FileInfo)dirfile;
                    s1 = Str.GetSizeString(f.Length);
                }
                else
                    s1 = Str.directory;
                s1 = s1.PadRight(Const.sizeStringLen); //колонка 2
                s += s1;
            }
            if (mode > 1)
            {
                string s1 = Str.GetFileAttributesString(dirfile.Attributes);
                s1 = s1.PadRight(Const.attrStringLen); //колонка 3
                s += s1;
            }
            if (mode > 2)
            {
                string s1 = dirfile.CreationTime.ToString(Str.dateTimePatt);
                s1 = s1.PadRight(Const.timeStringLen); //колонка 4
                s += s1;
            }
            if (mode > 3)
            {
                string s1 = dirfile.LastAccessTime.ToString(Str.dateTimePatt);
                s1 = s1.PadRight(Const.timeStringLen); //колонка 5
                s += s1;
            }
            if (mode > 4)
            {
                string s1 = dirfile.LastWriteTime.ToString(Str.dateTimePatt);
                s1 = s1.PadRight(Const.timeStringLen); //колонка 6
                s += s1;
            }
            return s;
        }

        static void FillDirList(List<string> listForShow, List<bool> listIsDir, System.IO.DirectoryInfo[] dirs, System.IO.FileInfo[] files, Walls walls)
        {
            int mode = Properties.Settings.Default.ViewMode;
            int linesOnPage = Properties.Settings.Default.StringsOnPage;
            for (int i = 0; i < dirs.Length; i++)
            {
                listForShow.Add(GetDirFileString(1, dirs[i]));
                listIsDir.Add(true);//Это папка
            }
            for (int i = 0; i < files.Length; i++)
            {
                listForShow.Add(GetDirFileString(1, files[i]));
                listIsDir.Add(false);//Это файл
            }
        }

    }
}
