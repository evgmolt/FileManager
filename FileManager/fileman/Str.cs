using System;
using System.Collections.Generic;
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

        public static readonly string pressAnyKey = "Нажмите любую клавишу для продолжения";
        public static readonly string helpFileName = "readme.txt";
        public static readonly string errorsDirName = "errors";
        public static readonly string errorsFileName = "exception.txt";
    }
}
