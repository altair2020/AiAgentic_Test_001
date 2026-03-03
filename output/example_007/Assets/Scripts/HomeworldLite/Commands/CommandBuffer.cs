using System.Collections.Generic;

namespace Example007.HomeworldLite.Commands
{
    public sealed class CommandBuffer
    {
        private readonly Queue<ICommand> _queue = new Queue<ICommand>();

        public int Count => _queue.Count;

        public void Enqueue(ICommand command)
        {
            if (command == null)
            {
                return;
            }

            _queue.Enqueue(command);
        }

        public bool TryDequeue(out ICommand command)
        {
            if (_queue.Count == 0)
            {
                command = null;
                return false;
            }

            command = _queue.Dequeue();
            return true;
        }
    }
}
