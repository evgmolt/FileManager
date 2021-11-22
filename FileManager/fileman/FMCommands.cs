using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    public static class FMCommands
    {
        public static byte numOfViewMode = 5;
        public static string[] Cmd =
        {
            "?",
            "QUIT",
            "MD",  //создание папки
            "CD",  //смена текущей папки
            "RM",  //удаление файла
            "RMD",  //удаление папки
            "COPY",//копирование файла(-ов)
            "MOVE",//перемещение файла(-ов)
            "COPYD",//копирование папки
            "MOVED",//перемещение папки
            "VIEW", //режим отображения (1, 2, 3), без параметров - режим 0;
            "SOP", //количество строк на странице, без параметров 10. Сделано, поскольку было в задании. 
            //Установленное значение не запоминается. При старте устанавливается исходя из размера окна.
            "LINES",//включает - выключает отображение горизонтальных линий.
            "DIRSIZE",// размер папки
            "DI",// список дисков
            "LS",//отображение содержимого папки без смены текущей
            "LEV",//включает - выключает отображение второго уровня дерева
            "COL",//Переключает цвет фона (черный - синий)
            "REP"//Вывод информации о дисках в файл
        };

        public static string keyRecurs = "-R";
        public static string keyOverwrite = "-W";

        public static int GetCommandNum(string s)
        {
            for (int i = 0; i < Cmd.Length; i++)
            {
                string ss = s.ToUpper();
                if (ss == Cmd[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<string> ParseCommand(string s)
        {
            s = s.Trim();
            char sep = ' ';
            char sep1 = '"';
            List<string> ls = new List<string>();
            int p = s.IndexOf(sep1);
            if (p <= 0)
            {
                p = s.IndexOf(sep);
                if (p > 0)
                    s = s.Substring(0, p);
                ls.Add(s); //команда без параметров
                return ls;
            }
            List<int> positions = new List<int>();
            string command = s.Substring(0, p - 1);
            ls.Add(command.Trim());
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == sep1)
                    positions.Add(i);
            }
            if (positions.Count % 2 != 0)
                return ls;
            for (int i = 0; i < positions.Count / 2; i++)
            {
                string ss = s.Substring(positions[2 * i], positions[2 * i + 1] - positions[2 * i]);
                ls.Add(ss.Trim(sep1));
            }
            return ls;
        }


    }
}
