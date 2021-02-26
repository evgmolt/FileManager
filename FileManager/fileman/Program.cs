using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    class Program
    {
        static void Main(string[] args)
        {
            InitSize();
            InitSettings();
            Walls walls = new Walls(Const.fieldWidth - 1, Const.fieldHeight - 1);

            do
            {
                ShowDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()), walls);
//                walls.Draw();
                Console.SetCursorPosition(Const.left, Const.promptPosition);
                Console.Write(Directory.GetCurrentDirectory().ToString() + "> ");
                string enterStr = Console.ReadLine();
                Console.Clear();
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
                        ShowHelp();
                        break;
                    case 1: //exit
                        Properties.Settings.Default.DefaultPath = Directory.GetCurrentDirectory();
                        Properties.Settings.Default.Save();
                        return;
                    case 2: //вывод содержимого папки
                        System.IO.DirectoryInfo startPathInfo;
                        if (enters.Count == 1)
                        {
                            startPathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                        }
                        else
                        {
                            startPathInfo = new DirectoryInfo(enters[1]);
                        }
                        ShowDirectory(startPathInfo, walls);
                        break;
                    case 3: //смена текущей папки
                        if (enters.Count < 2)
                        {
                            Console.WriteLine(Str.syntaxErr);
                            break;
                        };
                        if (Directory.Exists(enters[1]))
                            Directory.SetCurrentDirectory(enters[1]);
                        else
                            Console.WriteLine(enters[1] +Str.dirNotExist);
                        break;
                    case 4: //удаление файла 
                        if (!File.Exists(enters[1]))
                        {
                            Console.WriteLine(enters[1] +Str.fileNotExist);
                            break;
                        }
                        try
                        {
                            File.Delete(enters[1]);
                        }
                        catch (Exception e)
                        {
                            ShowAndSaveError(e);
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
                            ShowAndSaveError(e);
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
                            Console.Clear();
                            break;
                        default:
                        Console.WriteLine(enters[0] + Str.notCommand);
                        ShowHelp();
                        break;
                }
                Console.WriteLine();
            } 
            while (true);
        }

        static void InitSettings()
        {
            Properties.Settings.Default.StringsOnPage = Const.fieldHeight - 5;
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
            Const.promptPosition = Const.fieldHeight - 2;
        }
        static void ShowAndSaveError(Exception e)
        {
            DateTime dt = DateTime.Now;
            try
            {
                File.AppendAllText(Properties.Settings.Default.ErrorsFile, dt.ToString() + " " + e.Message + "\n");
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.Message);
            }
            Console.WriteLine(e.Message);
        }

        static void ShowHelp()
        {
            try
            {
                string[] txt = File.ReadAllLines(Properties.Settings.Default.HelpFile, Encoding.Default);
                foreach (string s in txt)
                    Console.WriteLine(s);
            }
            catch (Exception e)
            {
                ShowAndSaveError(e);
            }
        }

        static void ShowList(List<string> ls, int max, ref int total, Walls walls)
        {
            for (int i = 0; i < ls.Count; i++)
            {
                if (total == max)
                {
                    Console.WriteLine("--- Для продолжения нажмите любую клавишу ---");
                    Console.ReadKey();
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
                ShowAndSaveError(e);
            }

        }
        static void ShowDir(System.IO.DirectoryInfo[] dirs, System.IO.FileInfo[] files, Walls walls)
        {
            List<string> listForShowD = new List<string>();
            List<string> listForShowF = new List<string>();
            int mode = Properties.Settings.Default.ViewMode;
            int totalLines = 0;
            int linesOnPage = Properties.Settings.Default.StringsOnPage;
            Console.ForegroundColor = ConsoleColor.White;
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
            for (int i = 0; i < dirs.Length; i++)
            {
                string s = dirs[i].Name;
                s = s.PadRight(len[0] + 1);//колонка 1
                if (mode > 0)
                {
                    string s1 = dirs[i].LastWriteTime.ToString();
                    s1 = s1.PadRight(len[1] + 1); //колонка 2
                    s += s1;
                }
                if (mode > 1)
                {
                    string s1 = " ";
                    s1 = s1.PadRight(len[2] + 1); //колонка 3
                    s += s1;
                }
                if (mode > 2)
                {
                    s += " " + dirs[i].Attributes.ToString();//колонка 4
                }
                listForShowD.Add(s);
            }
            ShowList(listForShowD, linesOnPage, ref totalLines, walls);
            Console.ResetColor();
            for (int i = 0; i < files.Length; i++)
            {
                string s = files[i].Name;
                s = s.PadRight(len[0] + 1); //колонка 1
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
                    s += " " + files[i].Attributes.ToString();
                listForShowF.Add(s);
            }
            ShowList(listForShowF, linesOnPage, ref totalLines, walls);
        }

    }
}
