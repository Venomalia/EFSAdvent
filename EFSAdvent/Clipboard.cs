using EFSAdvent.FourSwords;
using System.Collections.Generic;
using System.Drawing;

namespace EFSAdvent
{
    public class Clipboard
    {
        private List<ClipboardTile> _tiles = new List<ClipboardTile>();
        private readonly History _history;

        public Clipboard(History history)
        {
            _history = history;
        }

        public void Clear()
        {
            _tiles.Clear();
        }

        public void Copy(Rectangle selection, Level level, int layer)
        {
            _tiles.Clear();
            if (selection.Width < 1 || selection.Height < 1)
            {
                return;
            }

            for (int y = selection.Y; y < selection.Y + selection.Height; y++)
            {
                for (int x = selection.X; x < selection.X + selection.Width; x++)
                {
                    ushort? tile = level.Room.GetLayerTile(layer, x, y);
                    if (tile.HasValue)
                    {
                        _tiles.Add(new ClipboardTile
                        {
                            xOffset = x - selection.X,
                            yOffset = y - selection.Y,
                            value = tile.Value
                        });
                    }
                }
            }
        }

        public void Paste(Rectangle selection, Level level, int layer)
        {
            SaveActionToHistory(selection, level, layer);
            _tiles.ForEach(tile =>
            {
                level.Room.SetLayerTile(layer, tile.xOffset + selection.X, tile.yOffset + selection.Y, tile.value);
            });
        }

        private void SaveActionToHistory(Rectangle selection, Level level, int layer)
        {
            var oldValues = new List<ushort>();
            var newValues = new List<ushort>();
            var coordinates = new List<(int x, int y)>();
            bool tileChanged = false;

            foreach (var tile in _tiles)
            {
                var currentTile = level.Room.GetLayerTile(layer, tile.xOffset + selection.X, tile.yOffset + selection.Y);
                if (currentTile.HasValue)
                {
                    tileChanged |= currentTile != tile.value;
                    coordinates.Add((tile.xOffset + selection.X, tile.yOffset + selection.Y));
                    oldValues.Add(currentTile.Value);
                    newValues.Add(tile.value);
                }
            }

            if (!tileChanged)
            {
                return;
            }

            _history.StoreTileChange(coordinates.ToArray(), oldValues.ToArray(), newValues.ToArray(), layer);
        }
    }

    public class ClipboardTile
    {
        public int xOffset;
        public int yOffset;
        public ushort value;
    }
}
