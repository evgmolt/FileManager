using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman
{
    public static class Str
    {
        public static readonly string notCommand = " не является командой";
        public static readonly string syntaxErr = "Синтаксическая ошибка";
        public static readonly string dirNotExist = " - папка не существует";
        public static readonly string dirExist = " - папка уже существует";
        public static readonly string directory = "Папка";
        public static readonly string fileNotExist = " - файл не найден";
        public static readonly string fileExist = " - файл уже существует. Переписать? (Y/N)";

        public static readonly string pressAnyKey = "Нажмите любую клавишу для продолжения, ESC для остановки";
        public static readonly string viewStopped = "Отображение прервано                                    ";
        public static readonly string helpFileName = "readme.txt";
        public static readonly string errorsDirName = "errors";
        public static readonly string errorsFileName = "exception.txt";
        public static readonly string driveNotReady = "Устройство не готово";
        public static readonly string[] driveInfoTitle = { "Диск", "Тип", "Формат", "Метка", "Размер", "Свободно", "Доступно" };

        public static readonly string tName = "ИМЯ";
        public static readonly string tSize = "РАЗМЕР";
        public static readonly string tAttr = "АТРИБУТЫ";
        public static readonly string tCreation = "СОЗДАН";
        public static readonly string tAccess = "ДОСТУП";
        public static readonly string tWrite = "ИЗМЕНЕН";
        public static readonly string dateTimePatt = "MM/dd/yyyy HH:mm:ss";
        public static readonly string appTitle = "File manager FILEMAN v.1.0 2021";
        public static readonly string prompt = "> ";
        public static readonly string tooLongString = "-->";


        public static string GetSizeString(Int64 bytes)
        {
            const Int64 KB = 1024,
                        MB = KB * 1024,
                        GB = MB * 1024,
                        TB = GB * 1024L,
                        PB = TB * 1024L,
                        EB = PB * 1024L;
            if (bytes < KB) return bytes.ToString("N0") + " байт";
            if (bytes < MB) return Decimal.Divide(bytes, KB).ToString("N") + " КБ";
            if (bytes < GB) return Decimal.Divide(bytes, MB).ToString("N") + " МБ";
            if (bytes < TB) return Decimal.Divide(bytes, GB).ToString("N") + " ГБ";
            if (bytes < PB) return Decimal.Divide(bytes, TB).ToString("N") + " ТБ";
            if (bytes < EB) return Decimal.Divide(bytes, PB).ToString("N") + " ПБ";
            return Decimal.Divide(bytes, EB).ToString("N") + " ЕБ";
        }

        public static string GetTitle()
        {
            int mode = Properties.Settings.Default.ViewMode;
            string s = tName;
            string s1;
            s = s.PadRight(Const.GetMaxFileNameLen(mode));
            if (mode > 0)
            {
                s1 = tSize;
                s += s1.PadRight(Const.sizeStringLen);
            }
            if (mode > 1)
            {
                s1 = tAttr;
                s += s1.PadRight(Const.attrStringLen);
            }
            if (mode > 2)
            {
                s1 = tCreation;
                s1 = s1.PadRight(Const.timeStringLen);
                s += s1;
            }
            if (mode > 3)
            {
                s1 = tAccess;
                s += s1.PadRight(Const.timeStringLen);
            }
            if (mode > 4)
            {
                s1 = tWrite;
                s += s1.PadRight(Const.timeStringLen);
            }
            return s;
        }

        public static string GetFileAttributesString(System.IO.FileAttributes fa)
        {
            string s = string.Empty;
            if ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                s += "R";
            }
            else s += "-";
            s += " ";
            if ((fa & FileAttributes.Archive) == FileAttributes.Archive)
            {
                s += "A";
            }
            else s += "-";
            s += " ";
            if ((fa & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                s += "H";
            }
            else s += "-";
            s += " ";
            if ((fa & FileAttributes.System) == FileAttributes.System)
            {
                s += "S";
            }
            else s += "-";
            return s;
        }

        public static int[] GetColumnsWidth(string[,] matrix)
        {
            int[] res = new int[matrix.GetLength(0)];
            int numOfStrings = matrix.GetLength(1);
            for (int k = 0; k < res.Length; k++)
            {
                int len = 0;
                for (int i = 0; i < numOfStrings; i++)
                {
                    len = Math.Max(len, matrix[k, i].Length);
                }
                res[k] = len;
            }
            return res;
        }



    }


}
