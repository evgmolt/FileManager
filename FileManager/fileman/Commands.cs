﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    public static class Commands
    {
        public static byte numOfViewMode = 4;
        public static string[] Cmd =
        {
            "?",
            "QUIT",
            "LS",  //вывод содержимого текущей папки
            "CD",  //смена текущей папки
            "RM",  //удаление файла
            "RMD",  //удаление папки
            "COPY",//копирование
            "MOVE",//перемещение
            "VIEW", //режим отображения (1, 2, 3), без параметров - режим 0;
            "SOP", //количество строк на странице, без параметров 10
            "CLEAR" 
        };

        public static int GetCommandNum(string s)
        {
            for (int i = 0; i < Cmd.Length; i++)
                if (s.ToUpper() == Cmd[i])
                {
                    return i;
                }
            return -1;
        }

        public static List<string> ParseCommand(string s)
        {
            s = s.Trim();
            char sep1 = '"';
            List<string> ls = new List<string>();
            int p = s.IndexOf(sep1);
            if (p < 0)
            {
               ls.Add(s); //команда без параметров
                return ls;
            }
            List<int> positions = new List<int>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == sep1)
                    positions.Add(i);
            }
            if (positions.Count % 2 != 0)
                return null;
            for (int i = 0; i < positions.Count / 2; i++)
            {
                string ss = s.Substring(positions[2 * i], positions[2 * i + 1] - positions[2 * i] + 1);
                ls.Add(ss);
            }
            return ls;
        }

        public static List<string> ParseCommandOld(string s)
        {
            s = s.Trim();
            char sep = ' ';
            List<string> ls = new List<string>();
            int p = s.IndexOf(sep);
            while (p != -1) 
            {
                string ss = s.Substring(0, p);
                ls.Add(ss);
                s = s.Substring(p + 1);
                s = s.Trim();
                p = s.IndexOf(sep);
            };
            ls.Add(s);
            return ls;
        }
    }
}
