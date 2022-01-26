using fileman2.Messages;
using System;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdShowDrives : FileManagerCommand
    {
        private readonly Walls _walls;
        public CmdShowDrives(IMessager messager, Walls walls) : base(messager)
        {
            _commandName = "DI";
            _walls = walls;
        }

        string[,] GetDrivesInfo()
        {
            int nName = 0;
            int nType = 1;
            int nFormat = 2;
            int nLabel = 3;
            int nTotal = 4;
            int nFree = 5;
            int nAvailable = 6;
            string[,] drivesInfoArr = null;
            DriveInfo[] drives = DriveInfo.GetDrives();
            try
            {
                drivesInfoArr = new string[FMConstants.numOfDrivesParameters, drives.Length + 1];
                for (int i = 0; i < FMConstants.numOfDrivesParameters; i++) //Заголовки столбцов
                {
                    drivesInfoArr[i, 0] = FMStrings.driveInfoTitle[i];
                }
                for (int i = 0; i < drives.Length; i++)
                {
                    drivesInfoArr[nName, i + 1] = drives[i].Name;
                    drivesInfoArr[nType, i + 1] = drives[i].DriveType.ToString();
                    if (!drives[i].IsReady)
                    {
                        drivesInfoArr[nFormat, i + 1] = FMStrings.driveNotReady;
                        drivesInfoArr[nLabel, i + 1] = String.Empty;
                        drivesInfoArr[nTotal, i + 1] = String.Empty;
                        drivesInfoArr[nFree, i + 1] = String.Empty;
                        drivesInfoArr[nAvailable, i + 1] = String.Empty;
                    }
                    else
                    {
                        drivesInfoArr[nFormat, i + 1] = drives[i].DriveFormat;
                        drivesInfoArr[nLabel, i + 1] = drives[i].VolumeLabel;
                        drivesInfoArr[nTotal, i + 1] = FMStrings.GetSizeString(drives[i].TotalSize);
                        drivesInfoArr[nFree, i + 1] = FMStrings.GetSizeString(drives[i].TotalFreeSpace);
                        drivesInfoArr[nAvailable, i + 1] = FMStrings.GetSizeString(drives[i].AvailableFreeSpace);
                    }
                }
            }
            catch (Exception e)
            {
                _messager.ShowAndSaveError(e.Message, true);
            }
            return drivesInfoArr;

        }

        public override void Execute(params string[] args)
        {
            if (args.Length == 1)
            {
                _walls.Draw();
                string[,] dInfo = GetDrivesInfo();
                if (dInfo == null)
                {
                    return;
                }
                int[] colW = FMStrings.GetColumnsWidth(dInfo);
                Console.SetCursorPosition(FMConstants.left, FMConstants.top);
                for (int i = 0; i < dInfo.GetLength(1); i++)
                {
                    if (i == 1)//Пустая строка после заголовка
                    {
                        Console.WriteLine();
                        Console.SetCursorPosition(FMConstants.left, Console.CursorTop);
                    }
                    for (int k = 0; k < dInfo.GetLength(0); k++)
                    {
                        Console.Write(dInfo[k, i].PadRight(colW[k] + 2));
                    }
                    Console.WriteLine();
                    Console.SetCursorPosition(FMConstants.left, Console.CursorTop);
                }
                _messager.ShowInfo("");
            }
        }
    }
}
