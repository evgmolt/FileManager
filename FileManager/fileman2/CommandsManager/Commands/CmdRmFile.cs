using fileman2.Messages;
using System;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdRmFile : FileManagerCommand
    {
        public CmdRmFile(IMessager messager) : base(messager)
        {
            _commandName = "RM";
        }

        public override void Execute(params string[] args)
        {
            if (!File.Exists(args[1]))
            {
                _messager.ShowAndSaveError(args[1] + FMStrings.fileNotExist, false);
                return;
            }
            try
            {
                File.Delete(args[1]);
            }
            catch (Exception e)
            {
                _messager.ShowAndSaveError(e.Message, true);
            }
        }
    }
}
