using fileman2.Common;
using fileman2.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileman2
{
    internal static class Show
    {
        internal static void ShowDirectory(DirectoryInfo directoryInfo, Walls walls, IMessager messager)
        {
            Dir dir = new Dir(directoryInfo, messager);
            List<Item> dirList = dir.GetList();
            walls.Draw();
            Console.SetCursorPosition(FMConstants.left, FMConstants.top);
            Console.WriteLine(FMStrings.GetTitle());
            int linesOnPage = Properties.Settings1.Default.StringsOnPage;
            int totalPages = dirList.Count / linesOnPage;
            if ((dirList.Count + dirList.Count) % linesOnPage != 0)
            {
                totalPages++;
            }
            Console.ForegroundColor = ConsoleColor.White;
            int pNum = 0;
            ShowList(pNum, totalPages, dirList, linesOnPage, walls);
            Console.SetCursorPosition(FMConstants.left, FMConstants.promptPosition);
            Console.Write(Directory.GetCurrentDirectory().ToString() + FMStrings.prompt);
        }

        private static void ShowList(int pageNum, int totalPages, List<Item> ls, int max, Walls walls)
        {
            int total = 0; for (int i = 0; i < ls.Count; i++)
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
                if (ls[i].IsDirectory)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                };
                Console.WriteLine(ls[i].Content);
                Console.ForegroundColor = ConsoleColor.Gray;
                total++;
            }
        }
    }
}
