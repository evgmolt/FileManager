using fileman2.Messages;

namespace fileman2.Commands
{
    internal class CmdLevel : FileManagerCommand
    {
        public CmdLevel(IMessager messager) : base(messager)
        {
            _commandName = "LEV";
        }
        public override void Execute(params string[] args)
        {
            Properties.Settings1.Default.Level = !Properties.Settings1.Default.Level;
        }
    }
}
