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
            InitSize();
            InitSettings();
            Walls walls = new Walls(Const.fieldWidth - 1, Const.fieldHeight - 1);
            Const.messPosition = walls.messPos;
            do
            {
                ShowDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()), walls);
                Console.SetCursorPosition(Const.left, Const.promptPosition);
                Console.Write(Directory.GetCurrentDirectory().ToString() + "> ");
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
                        Console.Clear();
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
                        try
                        {
                            string[] fList = Directory.GetFiles(enters[1], "*.*");
                            foreach (string s in fList)
                                File.Delete(s);
                            Directory.Delete(enters[1]);
                        }
                        catch (Exception e)
                        {
                            ShowAndSaveError(e.Message, true);
                        }
                        break;
                    case 6:
                        break;
                    case 7:
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
                            if (res > 0 && res < Commands.numOfViewMode)
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
                        case 10:
                            Properties.Settings.Default.HorizLines = !Properties.Settings.Default.HorizLines;
                            break;
                        default:
                            Console.Clear();
                            walls.Draw();
                            ShowHelp();
                            ShowAndSaveError(enters[0] + Str.notCommand, false);
                            break;
                }
                Console.Clear();

                Console.WriteLine();
            } 
            while (true);
        }

        static void InitSettings()
        {
            Properties.Settings.Default.StringsOnPage = Const.messPosition - 5;
            if (Properties.Settings.Default.DefaultPath != String.Empty)
            {
                Directory.SetCurrentDirectory(Properties.Settings.Default.DefaultPath);
            }
            else
            {
                Properties.Settings.Default.DefaultPath = Directory.GetCurrentDirectory();
                Properties.Settings.Default.HelpFile = Directory.GetCurrentDirectory() + "\\" + Str.helpFileName;
                Properties.Settings.Default.ViewMode = 0;
                if (!Directory.Exists(Str.errorsDirName))
                    Directory.CreateDirectory(Str.errorsDirName);
                Properties.Settings.Default.ErrorsFile = Directory.GetCurrentDirectory() + "\\" +
                    Str.errorsDirName + "\\" + Str.errorsFileName;
            }
        }

        static void InitSize()
        {
            
            int w = Console.LargestWindowWidth;
            int h = Console.LargestWindowHeight;
            Const.fieldWidth = (int)(w * Const.scale);
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

        static void ShowList(ref int pageNum, int totalPages, List<string> ls, int max, ref int total, Walls walls)
        {
            for (int i = 0; i < ls.Count; i++)
            {
                if (total == max)
                {
                    pageNum++;
                    ConsoleColor color = Console.ForegroundColor;
                    Console.ResetColor();
                    Console.SetCursorPosition(2, Const.messPosition + 1);
                    Console.WriteLine($"Страница {pageNum} из {totalPages}");
                    Console.SetCursorPosition(2, Const.messPosition + 2);
                    Console.WriteLine("--- Для продолжения нажмите любую клавишу ---");
                    Console.ReadKey();
                    Console.ForegroundColor  = color;
                    Console.Clear();
                    walls.Draw();
                    Console.SetCursorPosition(Const.left, Const.top);

                    total = 0;
                }
                Console.SetCursorPosition(Const.left, Console.CursorTop);
                Console.WriteLine(ls[i]);
                total++;
            }
        }

        static void ShowDirectory(DirectoryInfo startPathInfo, Walls walls)
        {
            Console.Clear();
            walls.Draw();
            Console.SetCursorPosition(Const.left, Const.top);
            try
            {
                System.IO.DirectoryInfo[] subDirs = startPathInfo.GetDirectories();
                System.IO.FileInfo[] files = startPathInfo.GetFiles("*.*");
                ShowDir(subDirs, files, walls);
            }
            catch (Exception e)
            {
                ShowAndSaveError(e.Message, true);
            }

        }

        static string GetFileAttributesString(System.IO.FileAttributes fa)
        {
            string s = string.Empty;
            if ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                s += "R";
            }
            else s += "-";
            if ((fa & FileAttributes.Archive) == FileAttributes.Archive)
            {
                s += "A";
            }
            else s += "-";
            if ((fa & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                s += "H";
            }
            else s += "-";
            if ((fa & FileAttributes.System) == FileAttributes.System)
            {
                s += "S";
            }
            else s += "-";
            return s;
        }
        static void ShowDir(System.IO.DirectoryInfo[] dirs, System.IO.FileInfo[] files, Walls walls)
        {
            char horizLine;
            if (Properties.Settings.Default.HorizLines)
                horizLine = '-';
            else
                horizLine = ' ';
            List<string> listForShowD = new List<string>();
            List<string> listForShowF = new List<string>();
            int mode = Properties.Settings.Default.ViewMode;
            int totalLines = 0;
            int linesOnPage = Properties.Settings.Default.StringsOnPage;
            int[] len = new int[4]; //ширина столбцов
            for (int i = 0; i < dirs.Length; i++)
            {
                len[0] = Math.Max(len[0], dirs[i].Name.Length);
                len[1] = Math.Max(len[1], dirs[i].LastWriteTime.ToString().Length);
            }
            for (int i = 0; i < files.Length; i++)
            {
                len[0] = Math.Max(len[0], files[i].Name.Length);
                len[1] = Math.Max(len[1], files[i].LastWriteTime.ToString().Length);
                len[2] = Math.Max(len[2], files[i].Length.ToString().Length);
            }
            int maxFileNameLen = Const.fieldWidth;
            for (int i = 1; i < mode; i++)
                maxFileNameLen -= len[i];
            len[0] = maxFileNameLen - 12;
//            len[0] = Math.Min(len[0], maxFileNameLen);
            for (int i = 0; i < dirs.Length; i++)
            {
                string s = dirs[i].Name;
                if (s.Length > len[0])
                    s = s.Substring(0, len[0] - 3) + "-->";
                s = s.PadRight(len[0] + 1, horizLine);//колонка 1
                if (mode > 0)
                {
                    string s1 = dirs[i].LastWriteTime.ToString();
                    s1 = s1.PadRight(len[1] + 1); //колонка 2
                    s += s1;
                }
                if (mode > 1)
                {
                    string s1 = Str.directory;
                    s1 = s1.PadRight(len[2] + 1); //колонка 3
                    s += s1;
                }
                if (mode > 2)
                {
                    s += " " + GetFileAttributesString(dirs[i].Attributes);//колонка 4
                }
                listForShowD.Add(s);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string s = files[i].Name;
                if (s.Length > len[0])
                    s = s.Substring(0, len[0] - 3) + "-->";
                s = s.PadRight(len[0] + 1, horizLine); //колонка 1
                if (mode > 0)
                {
                    string s1 = files[i].LastWriteTime.ToString();
                    s1 = s1.PadRight(len[1] + 1); //колонка 2
                    s += s1;
                }
                if (mode > 1)
                {
                    string s1 = files[i].Length.ToString(); ;
                    s1 = s1.PadRight(len[2] + 1); //колонка 3
                    s += s1;
                }
                if (mode > 2)
                    s += " " + GetFileAttributesString(files[i].Attributes);
                listForShowF.Add(s);
            }
            int totalPages = (listForShowD.Count + listForShowF.Count) / linesOnPage;
            if ((listForShowD.Count + listForShowF.Count) % linesOnPage != 0)
                totalPages++;
            Console.ForegroundColor = ConsoleColor.White;
            int pNum = 0;
            ShowList(ref pNum, totalPages, listForShowD, linesOnPage, ref totalLines, walls);
            Console.ResetColor();
            ShowList(ref pNum, totalPages, listForShowF, linesOnPage, ref totalLines, walls);
        }

    }
}
