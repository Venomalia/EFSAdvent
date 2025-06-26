using FSALib;
using FSALib.Schema;
using FSALib.Structs;
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

        public void StoreTileChange(List<HistoryTile> tileChanges, int layer)
        {
            var action = new HistoryAction(tileChanges, layer);
            _actionsToUndo.AddLast(action);

            while (_actionsToUndo.Count > _maxSteps)
            {
                _actionsToUndo.RemoveFirst();
            }
            _actionsToRedo.Clear();
        }

        public bool TryUndoAction(Room room, out int layer)
        {
            if (_actionsToUndo.Count == 0)
            {
                layer = 0;
                return false;
            }
            HistoryAction action = _actionsToUndo.Last();
            layer = action.Layer;
            action.Undo(room);
            _actionsToUndo.RemoveLast();

            var reverseAction = new HistoryAction(action, true);
            _actionsToRedo.AddLast(reverseAction);
            return true;
        }

        public bool TryRedoAction(Room room, out int layer)
        {
            if (_actionsToRedo.Count == 0)
            {
                layer = 0;
                return false;
            }
            HistoryAction action = _actionsToRedo.Last();
            layer = action.Layer;
            action.Undo(room);
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

        public HistoryAction(List<HistoryTile> tiles, int layer)
        {
            Layer = layer;
            Tiles = tiles;
        }

        public HistoryAction(HistoryAction action, bool reverse)
        {
            Layer = action.Layer;
            Tiles = action.Tiles.Select(t => new HistoryTile(t, reverse)).ToList();
        }

        public void Undo(Room room)
        {
            foreach (var tile in Tiles)
            {
                ushort currentTile = room.Layers[Layer][tile.X, tile.Y];
                if (Assets.TileProperties.TryGetValue(currentTile, out TilePropertie propertie) && propertie.RequiredActorID.HasValue)
                {
                    var tileActor = new Actor(propertie.RequiredActorID.Value, (byte)(Layer % 8), (byte)((tile.X) * 2), (byte)((tile.Y) * 2), propertie.ActorValue);
                    if (room.Actors.TrySearch(tileActor.Layer, tileActor.XCoord, tileActor.YCoord, tileActor.ID, out Actor actor))
                    {
                        room.Actors.Remove(actor);
                    }
                }

                room.Layers[Layer][tile.X, tile.Y] = tile.OldValue;
            }
        }
    }

    public class HistoryTile
    {
        public int X;
        public int Y;
        public ushort OldValue;
        public ushort NewValue;

        public HistoryTile()
        {
        }
        public HistoryTile(HistoryTile tile, bool reverse)
        {
            X = tile.X;
            Y = tile.Y;
            OldValue = reverse ? tile.NewValue : tile.OldValue;
            NewValue = reverse ? tile.OldValue : tile.NewValue;
        }

        public void ReverseTileChange()
            => (NewValue, OldValue) = (OldValue, NewValue);
    }
}
