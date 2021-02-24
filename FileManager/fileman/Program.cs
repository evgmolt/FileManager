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
            if (Properties.Settings.Default.DefaultPath == String.Empty)
            {
                Properties.Settings.Default.DefaultPath = Directory.GetCurrentDirectory();
                Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine(Properties.Settings.Default.DefaultPath);
            }
            Console.ReadKey();
        }
    }
}
