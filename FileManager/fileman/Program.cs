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
            Console.Title = FMStrings.appTitle;
            InitSize();
            InitSettings();
            Walls walls = new Walls(FMConstants.fieldWidth - 1, FMConstants.fieldHeight - 1);
            FMConstants.messPosition = walls.messPos;
            do
            {
                ShowDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()), walls);
                Console.SetCursorPosition(FMConstants.left, FMConstants.promptPosition);
                Console.Write(Directory.GetCurrentDirectory().ToString() + FMStrings.prompt);
                string enterStr = Console.ReadLine();
                List<string> enters = FMCommands.ParseCommand(enterStr);
                if (enters == null)
                {
                    Console.WriteLine(FMStrings.syntaxErr);
                }
                else
                if (enters[0] != String.Empty)
                switch (FMCommands.GetCommandNum(enters[0]))
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
                            ShowAndSaveError(enters[1] + FMStrings.dirExist, false);
                        }
                        break;
                    case 3: //смена текущей папки
                        if (enters.Count < 2)
                        {
                            ShowAndSaveError(FMStrings.syntaxErr, false);
                            break;
                        };
                        if (Directory.Exists(enters[1]))
                            Directory.SetCurrentDirectory(enters[1]);
                        else
                            ShowAndSaveError(enters[1] +FMStrings.dirNotExist, false);
                        break;
                    case 4: //удаление файла 
                        if (!File.Exists(enters[1]))
                        {
                            ShowAndSaveError(enters[1] +FMStrings.fileNotExist, false);
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
                                ShowAndSaveError(FMStrings.syntaxErr, false);
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
                            ShowAndSaveError(FMStrings.syntaxErr, false);
                            break;
                        }
                        FileCopyOrMove(false, enters[1], enters[2]);
                        break;
                    case 7://перемещение файла(-ов)
                        if (enters.Count < 3)
                        {
                            ShowAndSaveError(FMStrings.syntaxErr, false);
                            break;
                        }
                        FileCopyOrMove(true, enters[1], enters[2]);
                        break;

                    case 8://COPYD копирование папки
                        bool recurse = enters[1].ToUpper() == FMCommands.keyRecurs;
                        if (recurse)
                        {
                                if (enters.Count == 4)
                                    DirectoryCopy(enters[2], enters[3], recurse); //есть параметр -r
                                else
                                    ShowAndSaveError(FMStrings.syntaxErr, false);
                        }
                        else
                        {
                                if (enters.Count == 3)
                                    DirectoryCopy(enters[1], enters[2], false);// нет параметра
                                else
                                    ShowAndSaveError(FMStrings.syntaxErr, false);
                        }
                        break;
                        case 9://перемещение папки
                            if (enters.Count < 3)
                            {
                                ShowAndSaveError(FMStrings.syntaxErr, false);
                                break;
                            }
                            DirectoryInfo dirInfo = new DirectoryInfo(enters[1]);
                            if (dirInfo.Exists && Directory.Exists(enters[2]) == false)
                            {
                                try
                                {
                                    dirInfo.MoveTo(enters[2]);
                                }
                                catch (Exception e)
                                {
                                    ShowAndSaveError(e.Message, true);
                                }
                            }
                            else
                            {
                                ShowAndSaveError(FMStrings.dirNameError, false);
                            }
                            break;
                        case 10: //режим отображения
                        if (enters.Count == 1)
                        {
                            Properties.Settings.Default.ViewMode = 0;
                            break;
                        }
                        byte res;
                        bool err = false;
                        if (Byte.TryParse(enters[1], out res))
                        {
                            if (res > 0 && res < FMCommands.numOfViewMode + 1)
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
                            Console.WriteLine(FMStrings.syntaxErr);
                        }
                        break;
                        case 11: //количество строк на странице, без параметров 10
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
                                Console.WriteLine(FMStrings.syntaxErr);
                            }
                            break;
                        case 12://Lines 
                            Properties.Settings.Default.HorizLines = !Properties.Settings.Default.HorizLines;
                            break;
                        case 13://DirSize 
                            if (enters.Count != 2)
                            {
                                ShowAndSaveError(FMStrings.syntaxErr, false);
                                break;
                            }
                            long size;
                            try
                            {
                                size = DirSize(new DirectoryInfo(enters[1]));
                                ShowInfo(enters[1] + " : " + FMStrings.GetSizeString(size));
                            }
                            catch (Exception e)
                            {
                                ShowAndSaveError(e.Message, true);
                            }
                            break;
                        case 14: //DI информация о диске
                            if (enters.Count == 1)
                            {
                                ShowDrives(walls);
                            }
                            break;
                        case 15: //отображение содержимого папки без смены текущей
                            if (enters.Count != 2)
                            {
                                ShowAndSaveError(FMStrings.syntaxErr, false);
                                break;
                            }
                            ShowDirectory(new DirectoryInfo(enters[1]), walls);
                            ShowInfo("");
                            break;
                        case 16://Уровень отображения дерева
                            Properties.Settings.Default.Level = !Properties.Settings.Default.Level;
                            break;
                        case 17://Цвет фона
                            Properties.Settings.Default.BColor = !Properties.Settings.Default.BColor;
                            FMConstants.backColor = Properties.Settings.Default.BColor ? ConsoleColor.Black : ConsoleColor.Blue;
                            break;
                        case 18://Создание файла отчета
                            CreateReport();
                            break;
                        default:
                            walls.Draw();
                            ShowHelp();
                            ShowAndSaveError(enters[0] + FMStrings.notCommand, false);
                            break;
                }
                Console.WriteLine();
            } 
            while (true);
        }

        private static void CreateReport()
        {
            string[,] driveInfoArray = GetDrivesInfo();
            ReportService reportService = new ReportService();
            if (reportService.GenerateReport(driveInfoArray))
            {
                ShowInfo(FMStrings.reportOk);
            }
            else
            {
                ShowAndSaveError(FMStrings.reportError, true);
            }
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
                drivesInfoArr = new string[FMConstants.numOfDrivesParameters, drives.Length + 1];
                for (int i = 0; i < FMConstants.numOfDrivesParameters; i++) //Заголовки столбцов
                {
                    drivesInfoArr[i, 0] = FMStrings.driveInfoTitle[i];
                }
                for (int i = 0; i < drives.Length; i++)
                {
                    drivesInfoArr[nName, i + 1] = drives[i].Name;
                    drivesInfoArr[nType, i + 1] = drives[i].DriveType.ToString();
                    if (!drives[i].IsReady)
                    {
                        drivesInfoArr[nFormat, i + 1] = FMStrings.driveNotReady;
                        drivesInfoArr[nLabel, i + 1] = String.Empty;
                        drivesInfoArr[nTotal, i + 1] = String.Empty;
                        drivesInfoArr[nFree, i + 1] = String.Empty;
                        drivesInfoArr[nAvailable, i + 1] = String.Empty;
                    }
                    else
                    {
                        drivesInfoArr[nFormat, i + 1] = drives[i].DriveFormat;
                        drivesInfoArr[nLabel, i + 1] = drives[i].VolumeLabel;
                        drivesInfoArr[nTotal, i + 1] = FMStrings.GetSizeString(drives[i].TotalSize);
                        drivesInfoArr[nFree, i + 1] = FMStrings.GetSizeString(drives[i].TotalFreeSpace);
                        drivesInfoArr[nAvailable, i + 1] = FMStrings.GetSizeString(drives[i].AvailableFreeSpace);
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
            int[] colW = FMStrings.GetColumnsWidth(dInfo);
            Console.SetCursorPosition(FMConstants.left, FMConstants.top);
            for (int i = 0; i < dInfo.GetLength(1); i++)
            {
                if (i == 1)//Пустая строка после заголовка
                {
                    Console.WriteLine();
                    Console.SetCursorPosition(FMConstants.left, Console.CursorTop);
                }
                for (int k = 0; k < dInfo.GetLength(0); k++)
                {
                    Console.Write(dInfo[k, i].PadRight(colW[k] + 2));
                }
                Console.WriteLine();
                Console.SetCursorPosition(FMConstants.left, Console.CursorTop);
            }
            ShowInfo("");
        }

        static void FileCopyOrMove(bool move, string source, string dest)
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
                        ShowAndSaveError(e.Message, true);
                    }
                }
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), source);
                foreach (string s in files)
                {
                    string fname = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(dest, fname);
                    FileCopyOrMovePrim(move, s, destFile);
                }
            }
            else
                FileCopyOrMovePrim(move, source, dest);

        }
        static void FileCopyOrMovePrim(bool move, string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile))
            {
                ShowAndSaveError(sourceFile + FMStrings.fileNotExist, false);
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
                ShowAndSaveError(e.Message, true);
            }
        }

        static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                ShowAndSaveError(sourceDirName + FMStrings.dirNotExist, false);
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
            Properties.Settings.Default.StringsOnPage = FMConstants.messPosition - 5;
            if (Properties.Settings.Default.DefaultPath != String.Empty)
            {
                Directory.SetCurrentDirectory(Properties.Settings.Default.DefaultPath);
                FMConstants.backColor = Properties.Settings.Default.BColor ? ConsoleColor.Black : ConsoleColor.Blue;
            }
            else
            {
                Properties.Settings.Default.DefaultPath = Directory.GetCurrentDirectory();
                Properties.Settings.Default.HelpFile = Path.Combine(Directory.GetCurrentDirectory(), FMStrings.helpFileName);
                if (!Directory.Exists(FMStrings.errorsDirName))
                    Directory.CreateDirectory(FMStrings.errorsDirName);
                Properties.Settings.Default.ErrorsFile = Path.Combine(Directory.GetCurrentDirectory(),
                    FMStrings.errorsDirName, FMStrings.errorsFileName);
            }
        }

        static void InitSize()
        {
            
            int w = Console.LargestWindowWidth;
            int h = Console.LargestWindowHeight;
            FMConstants.fieldWidth =  (int)(w * FMConstants.scale);
            FMConstants.fieldHeight = (int)(h * FMConstants.scale);
            Console.SetBufferSize(FMConstants.fieldWidth, FMConstants.fieldHeight);
            Console.SetWindowSize(FMConstants.fieldWidth, FMConstants.fieldHeight);
            FMConstants.maxFileNameLen = FMConstants.fieldWidth / 3;
            FMConstants.promptPosition = FMConstants.fieldHeight - 2;
            FMConstants.messPosition = FMConstants.promptPosition - 2;

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
            Console.SetCursorPosition(2, FMConstants.messPosition + 1);
            Console.WriteLine(mess);
            Console.SetCursorPosition(2, FMConstants.messPosition + 2);
            Console.WriteLine(FMStrings.pressAnyKey);
            Console.ReadKey();
        }

        static void ShowAndSaveError(string mess, bool save )
        {
            ConsoleColor defColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(2, FMConstants.messPosition + 1);
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
            Console.SetCursorPosition(2, FMConstants.messPosition + 2);
            Console.WriteLine(FMStrings.pressAnyKey);
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
                Console.SetCursorPosition(FMConstants.left, FMConstants.top);
                string[] txt = File.ReadAllLines(Properties.Settings.Default.HelpFile, Encoding.Default);
                foreach (string s in txt)
                {
                    Console.WriteLine(s);
                    Console.SetCursorPosition(FMConstants.left, Console.CursorTop);
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
                    Console.SetCursorPosition(2, FMConstants.messPosition + 1);
                    Console.WriteLine($"Страница {pageNum} из {totalPages}");
                    Console.SetCursorPosition(2, FMConstants.messPosition + 2);
                    Console.WriteLine(FMStrings.pressAnyKeyEsc);
                    Console.SetCursorPosition(2, FMConstants.messPosition + 2);
                    ConsoleKeyInfo cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Escape)
                    {
                        Console.SetCursorPosition(2, FMConstants.messPosition + 2);
                        Console.WriteLine(FMStrings.viewStopped);
                        return;
                    }
                    walls.Draw();
                    Console.SetCursorPosition(FMConstants.left, FMConstants.top);
                    Console.WriteLine(FMStrings.GetTitle());
                    Console.SetCursorPosition(FMConstants.left, Console.CursorTop);

                    total = 1;
                }
                Console.SetCursorPosition(FMConstants.left, Console.CursorTop);
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
            Console.SetCursorPosition(FMConstants.left, FMConstants.top);
            Console.WriteLine(FMStrings.GetTitle());
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
