using System;
using System.Collections.Generic;
using System.Linq;

namespace EFSAdvent
{
    public class History
    {
        private readonly LinkedList<HistoryAction> _actionsToUndo = new LinkedList<HistoryAction>();
        private readonly LinkedList<HistoryAction> _actionsToRedo = new LinkedList<HistoryAction>();
        private int _maxSteps;

        public History(int maxSteps)
        {
            _maxSteps = maxSteps;
        }

        public void Reset()
        {
            _actionsToUndo.Clear();
            _actionsToRedo.Clear();
        }

        public void StoreTileChange((int x, int y)[] coordinates, ushort[] oldValues, ushort[] newValues, int layer)
        {
            int width = (int)Math.Sqrt(oldValues.Length);

            var action = new HistoryAction(layer);
            for (int i = 0; i < oldValues.Length; i++)
            {
                action.Tiles.Add(new HistoryTile
                {
                    x = coordinates[i].x,
                    y = coordinates[i].y,
                    oldValue = oldValues[i],
                    newValue = newValues[i]
                });
            }
            _actionsToUndo.AddLast(action);

            while (_actionsToUndo.Count > _maxSteps)
            {
                _actionsToUndo.RemoveFirst();
            }
            _actionsToRedo.Clear();
        }

        public bool TryGetUndoAction(out HistoryAction action)
        {
            if (_actionsToUndo.Count == 0)
            {
                action = null;
                return false;
            }
            action = _actionsToUndo.Last();
            _actionsToUndo.RemoveLast();

            var reverseAction = new HistoryAction(action, true);
            _actionsToRedo.AddLast(reverseAction);
            return true;
        }

        public bool TryGetRedoAction(out HistoryAction action)
        {
            if (_actionsToRedo.Count == 0)
            {
                action = null;
                return false;
            }
            action = _actionsToRedo.Last();
            _actionsToRedo.RemoveLast();

            var reverseAction = new HistoryAction(action, true);
            _actionsToUndo.AddLast(reverseAction);
            return true;
        }
    }

    public class HistoryAction
    {
        public List<HistoryTile> Tiles;
        public int Layer;

        public HistoryAction(int layer)
        {
            Layer = layer;
            Tiles = new List<HistoryTile>();
        }

        public HistoryAction(HistoryAction action, bool reverse)
        {
            Layer = action.Layer;
            Tiles = action.Tiles.Select(t => new HistoryTile(t, reverse)).ToList();
        }
    }

    public class HistoryTile
    {
        public int x;
        public int y;
        public ushort oldValue;
        public ushort newValue;

        public HistoryTile()
        {
        }
        public HistoryTile(HistoryTile tile, bool reverse)
        {
            x = tile.x;
            y = tile.y;
            oldValue = reverse ? tile.newValue : tile.oldValue;
            newValue = reverse ? tile.oldValue : tile.newValue;
        }

        public void ReverseTileChange()
        {
            var temp = oldValue;
            oldValue = newValue;
            newValue = temp;
        }
    }
}
