using System.Collections.Generic;
using Example009.HWR.Core;

namespace Example009.HWR.Commands
{
    public sealed class CommandQueue : ICommandQueue
    {
        private readonly Queue<ICommand> _queue = new Queue<ICommand>();

        public void Enqueue(ICommand command)
        {
            _queue.Enqueue(command);
        }

        public IReadOnlyList<ICommand> DrainForTick(Tick tick)
        {
            var list = new List<ICommand>(_queue.Count);
            while (_queue.Count > 0)
            {
                list.Add(_queue.Dequeue());
            }
            return list;
        }
    }
}
