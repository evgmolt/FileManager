using System;
using System.IO;
using System.Text;

namespace fileman2.Messages
{
    class ConsoleMessager : IMessager
    {
        public void ShowAndSaveError(string mess, bool save)
        {
            ConsoleColor defColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(2, FMConstants.messPosition + 1);
            DateTime dt = DateTime.Now;
            if (save)
                try
                {
                    File.AppendAllText(Properties.Settings1.Default.ErrorsFile, dt.ToString() + " " + mess + "\n");
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

        public void ShowHelp()
        {
            try
            {
                Console.SetCursorPosition(FMConstants.left, FMConstants.top);
                string[] txt = File.ReadAllLines(Properties.Settings1.Default.HelpFile, Encoding.Default);
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

        public void ShowInfo(string mess)
        {
            Console.SetCursorPosition(2, FMConstants.messPosition + 1);
            Console.WriteLine(mess);
            Console.SetCursorPosition(2, FMConstants.messPosition + 2);
            Console.WriteLine(FMStrings.pressAnyKey);
            Console.ReadKey();
        }
    }
}
