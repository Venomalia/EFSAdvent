using EFSAdvent.FourSwords;
using FSALib;
using FSALib.Schema;
using System.Collections.Generic;
using System.Drawing;

namespace EFSAdvent
{
    public class TileBrush
    {
        private readonly List<ClipboardTile> _clipboardTiles = new List<ClipboardTile>();
        private readonly History _history;

        public ushort TileValue { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TileBrush(History history)
        {
            Width = Height = 1;
            _history = history;
        }

        public void Set(ushort tile, int width, int height)
        {
            _clipboardTiles.Clear();
            TileValue = tile;
            Width = width;
            Height = height;
        }

        public void Copy(Rectangle selection, Level level, int layer)
        {
            _clipboardTiles.Clear();
            if (selection.Width < 1 || selection.Height < 1)
            {
                return;
            }

            Width = selection.Width;
            Height = selection.Height;

            ushort tile = level.Room.Layers[layer][selection.X, selection.Y];
            TileValue = tile;

            if ((selection.Width | selection.Height) != 1)
            {
                for (int y = selection.Y; y < selection.Y + selection.Height; y++)
                {
                    for (int x = selection.X; x < selection.X + selection.Width; x++)
                    {
                        tile = level.Room.Layers[layer][x, y];
                        _clipboardTiles.Add(new ClipboardTile(x - selection.X, y - selection.Y, tile));
                    }
                }
            }
        }

        public bool Draw(Level level, int layer, int posX, int posY)
        {
            if (_clipboardTiles.Count == 0)
            {
                return DrawTiles(level, layer, posX, posY);
            }
            else
            {
                return Paste(level, layer, posX, posY);
            }

        }

        private bool DrawTiles(Level level, int layer, int posX, int posY)
        {
            bool hasPropertie = Assets.TileProperties.TryGetValue(TileValue, out TilePropertie propertie);
            if (hasPropertie &&  propertie.PlaceAlwaysOnTopLayer && layer < 8)
            {
                layer += 8;
            }

            if (SaveActionToHistory(level, layer, posX, posY))
            {
                for (int y = posY; y < posY + Height; y++)
                {
                    for (int x = posX; x < posX + Width; x++)
                    {
                        level.Room.Layers[layer][x, y] = TileValue;

                        if (hasPropertie && propertie.RequiredActorID.HasValue)
                        {
                            var tileActor = new Actor(propertie.RequiredActorID.Value, (byte)(layer % 8), (byte)(x * 2), (byte)(y * 2), propertie.ActorValue);
                            if (!level.Room.Actors.TrySearch(tileActor.Layer, tileActor.XCoord, tileActor.YCoord, tileActor.ID, out Actor _))
                            {
                                level.Room.Actors.Add(tileActor);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool Paste(Level level, int layer, int posX, int posY)
        {
            if (SavePasteActionToHistory(level, layer, posX, posY))
            {
                foreach (ClipboardTile tile in _clipboardTiles)
                {
                    level.Room.Layers[layer][tile.xOffset + posX, tile.yOffset + posY] = tile.value;
                }
                return true;
            }
            return false;
        }

        private bool SavePasteActionToHistory(Level level, int layer, int x, int y)
        {
            var oldValues = new List<ushort>();
            var newValues = new List<ushort>();
            var coordinates = new List<(int x, int y)>();
            bool tileChanged = false;

            foreach (var tile in _clipboardTiles)
            {
                var currentTile = level.Room.Layers[layer][tile.xOffset + x, tile.yOffset + y];
                tileChanged |= currentTile != tile.value;
                coordinates.Add((tile.xOffset + x, tile.yOffset + y));
                oldValues.Add(currentTile);
                newValues.Add(tile.value);
            }

            if (!tileChanged)
            {
                return false;
            }

            _history.StoreTileChange(coordinates.ToArray(), oldValues.ToArray(), newValues.ToArray(), layer);
            return true;
        }

        private bool SaveActionToHistory(Level level, int layer, int x, int y)
        {
            var oldValues = new List<ushort>();
            var newValues = new List<ushort>();
            var coordinates = new List<(int x, int y)>();
            bool tileChanged = false;

            for (int testY = y; testY < y + Height; testY++)
            {
                for (int testX = x; testX < x + Width; testX++)
                {
                    var currentTile = level.Room.Layers[layer][testX, testY];
                    tileChanged |= currentTile != TileValue;
                    coordinates.Add((testX, testY));
                    oldValues.Add(currentTile);
                    newValues.Add(TileValue);
                }
            }

            if (!tileChanged)
            {
                return false;
            }

            _history.StoreTileChange(coordinates.ToArray(), oldValues.ToArray(), newValues.ToArray(), layer);
            return true;
        }
    }

    public readonly struct ClipboardTile
    {
        public readonly int xOffset;
        public readonly int yOffset;
        public readonly ushort value;

        public ClipboardTile(int xOffset, int yOffset, ushort value)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.value = value;
        }
    }
}
