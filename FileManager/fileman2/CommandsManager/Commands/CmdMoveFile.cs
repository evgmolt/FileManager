using fileman2.Messages;

namespace fileman2.Commands
{
    internal class CmdMoveFile : FileManagerCommand
    {
        public CmdMoveFile(IMessager messager) : base(messager)
        {
            _commandName = "MOVE";
        }

        public override void Execute(params string[] args)
        {
            if (args.Length < 3)
            {
                _messager.ShowAndSaveError(FMStrings.syntaxErr, false);
                return;
            }
            Utils.FileCopyOrMove(true, args[1], args[2], _messager);
        }
    }
}
