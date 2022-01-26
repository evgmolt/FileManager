using System.Collections.Generic;
using System.Linq;

namespace fileman2.Commands
{
    class CommandsRepository
    {
        private readonly IReadOnlyDictionary<string, IFileManagerCommand> _commands;

        public CommandsRepository(IReadOnlyCollection<IFileManagerCommand> commands)
        {
            _commands = commands.ToDictionary(x => x.CommandName, x => x);
        }

        public IFileManagerCommand GetCommand(string key)
        {
            if (_commands.TryGetValue(key.ToUpper(), out IFileManagerCommand command))
            {
                return command;
            }
            return null;
        }
    }
}
