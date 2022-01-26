using fileman2.Messages;

namespace fileman2.Commands
{
    internal class CmdLines : FileManagerCommand
    {
        public CmdLines(IMessager messager) : base(messager)
        {
            _commandName = "LINES";
        }
        public override void Execute(params string[] args)
        {
            Properties.Settings1.Default.HorizLines = !Properties.Settings1.Default.HorizLines;
        }
    }
}
