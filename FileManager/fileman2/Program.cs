using fileman2.Commands;
using fileman2.Common;
using fileman2.Messages;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace fileman2
{
    class Program
    {      
        static void Main(string[] args)
        {
            Console.Title = FMStrings.appTitle;
            InitSize();
            InitSettings();

            IKernel kernel = new StandardKernel();
            kernel.Bind<IMessager>().To<ConsoleMessager>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdChDir>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdCopyDir>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdCopyFile>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdDirSize>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdLines>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdMkDir>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdMoveDir>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdMoveFile>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdRmDir>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdRmFile>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdViewMode>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdShowDrives>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdLevel>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdColor>().InSingletonScope();
            kernel.Bind<IFileManagerCommand>().To<CmdSave>().InSingletonScope();

            Walls walls = new Walls();
            IMessager messager = kernel.Get<ConsoleMessager>();
            CommandsRepository commands = new CommandsRepository(kernel.Get<List<IFileManagerCommand>>());
            do
            {
                Show.ShowDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()), walls, messager);
                string enterStr = Console.ReadLine();
                var enters = enterStr.Split('"').Select(s => s.Trim()).Where(s => s != "").ToArray();
                if (enters == null || enters.Length == 0)
                {
                    Console.WriteLine(FMStrings.syntaxErr);
                    continue;
                }
                IFileManagerCommand cmd = commands.GetCommand(enters[0]);
                if (cmd != null)
                {
                    cmd.Execute(enters);
                }
                else
                {
                    walls.Draw();
                    messager.ShowHelp();
                    messager.ShowAndSaveError(enters[0] + FMStrings.notCommand, false);
                }
            }
            while (true);
        }


        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void InitSize()
        {
            int w = Console.LargestWindowWidth;
            int h = Console.LargestWindowHeight;
            FMConstants.fieldWidth = (int)(w * FMConstants.scale);
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
                DeleteMenu(sysMenu, FMConstants.SC_MINIMIZE, FMConstants.MF_BYCOMMAND);
                DeleteMenu(sysMenu, FMConstants.SC_MAXIMIZE, FMConstants.MF_BYCOMMAND);
                DeleteMenu(sysMenu, FMConstants.SC_SIZE, FMConstants.MF_BYCOMMAND);
            }
        }
        static void InitSettings()
        {
            Properties.Settings1.Default.StringsOnPage = FMConstants.messPosition - 5;
            if (Properties.Settings1.Default.DefaultPath != String.Empty)
            {
                Directory.SetCurrentDirectory(Properties.Settings1.Default.DefaultPath);
                FMConstants.backColor = Properties.Settings1.Default.BColor ? ConsoleColor.Black : ConsoleColor.Blue;
            }
            else
            {
                Properties.Settings1.Default.DefaultPath = Directory.GetCurrentDirectory();
                Properties.Settings1.Default.HelpFile = Path.Combine(Directory.GetCurrentDirectory(), FMStrings.helpFileName);
                if (!Directory.Exists(FMStrings.errorsDirName))
                    Directory.CreateDirectory(FMStrings.errorsDirName);
                Properties.Settings1.Default.ErrorsFile = Path.Combine(Directory.GetCurrentDirectory(),
                    FMStrings.errorsDirName, FMStrings.errorsFileName);
            }
        }
    }
}
