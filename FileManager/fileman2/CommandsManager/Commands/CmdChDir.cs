using fileman2.Messages;
using System.IO;

namespace fileman2.Commands
{
    internal class CmdChDir : FileManagerCommand
    {
        public CmdChDir(IMessager messager) : base(messager)
        {
            _commandName = "CD";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length < 2)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                return;
            };
            if (Directory.Exists(args[1]))
            {
                Directory.SetCurrentDirectory(args[1]);
            }
            else
            {
                _messager.ShowAndSaveError(args[1] + FMStrings.dirNotExist, false);
            }
        }
    }
}
