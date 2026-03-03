using System.Collections.Generic;
using Example009.HWR.Core;

namespace Example009.HWR.Selection
{
    public interface ISelectionService
    {
        IReadOnlyList<EntityId> Current { get; }
        void Replace(IReadOnlyList<EntityId> entities);
        void Clear();
    }

    public sealed class SelectionService : ISelectionService
    {
        private readonly List<EntityId> _selected = new List<EntityId>();
        public IReadOnlyList<EntityId> Current => _selected;

        public void Replace(IReadOnlyList<EntityId> entities)
        {
            _selected.Clear();
            for (int i = 0; i < entities.Count; i++)
            {
                _selected.Add(entities[i]);
            }
        }

        public void Clear()
        {
            _selected.Clear();
        }
    }
}
