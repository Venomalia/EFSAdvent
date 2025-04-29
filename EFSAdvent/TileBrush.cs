using EFSAdvent.FourSwords;
using FSALib;
using FSALib.Schema;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EFSAdvent
{
    public class TileBrush
    {
        public readonly Stamp Clipboard = new Stamp();
        private readonly History _history;

        public ushort TileValue => Clipboard.Tiles[0];
        public int Width => Clipboard.Width;
        public int Height => Clipboard.Height;

        public bool AutomaticSetTileActors;

        public TileBrush(History history) => _history = history;

        public void Set(ushort tile, int width, int height)
        {
            Clipboard.SetWidthAndHeight(width, height);
            if ((width | height) > 1)
            {
                Clipboard.Tiles.Fill(tile);
            }
            else
            {
                Clipboard.Tiles[0] = tile;
            }
        }

        public void Copy(Rectangle selection, Level level, int layer)
        {
            if (selection.X < 0)
            {
                selection.Width += selection.X;
                selection.X = 0;
            }
            if (selection.Y < 0)
            {
                selection.Height += selection.Y;
                selection.Y = 0;
            }
            if (selection.Width < 1 || selection.Height < 1)
            {
                return;
            }
            if (selection.X + selection.Width >= Layer.DIMENSION)
            {
                selection.Width = Layer.DIMENSION - selection.X;
            }
            if (selection.Y + selection.Height >= Layer.DIMENSION)
            {
                selection.Height = Layer.DIMENSION - selection.Y;
            }

            ushort tile = level.Room.Layers[layer][selection.X, selection.Y];
            Clipboard.FromLayer(level.Room.Layers[layer], selection.X, selection.Y, selection.Width, selection.Height);
        }

        public bool Draw(Level level, int layer, int posX, int posY)
        {
            if (SavePasteActionToHistory(level, layer, posX, posY))
            {
                level.Room.Layers[layer].SetTiles(Clipboard, posX, posY);
                return true;
            }
            return false;
        }

        private bool SavePasteActionToHistory(Level level, int layer, int x, int y)
        {
            const int DIMENSION = Layer.DIMENSION;

            List<HistoryTile> tileChanges = new List<HistoryTile>();
            bool tileChanged = false;

            ReadOnlySpan<ushort> layerTiles = level.Room.Layers[layer].Tiles;
            ReadOnlySpan<ushort> clipboardTiles = Clipboard.Tiles;

            int width = Math.Min(Clipboard.Width, DIMENSION - x);
            int height = Math.Min(Clipboard.Height, DIMENSION - y);

            for (int cY = 0; cY < height; cY++)
            {
                int startY = (y + cY) * DIMENSION;
                int stampStartY = cY * DIMENSION;
                for (int cX = 0; cX < width; cX++)
                {
                    ushort layerTile = layerTiles[startY + x + cX];
                    ushort clipboardTile = clipboardTiles[stampStartY + cX];

                    if (layerTile != clipboardTile)
                    {
                        tileChanged = true;
                        tileChanges.Add(new HistoryTile()
                        {
                            X = x + cX,
                            Y = y + cY,
                            OldValue = layerTile,
                            NewValue = clipboardTile
                        });

                        if (AutomaticSetTileActors
                            && Assets.TileProperties.TryGetValue(clipboardTile, out TilePropertie propertie)
                            && propertie.RequiredActorID.HasValue)
                        {
                            var tileActor = new Actor(propertie.RequiredActorID.Value, (byte)(layer % 8), (byte)((x + cX) * 2), (byte)((y + cY) * 2), propertie.ActorValue);
                            if (!level.Room.Actors.TrySearch(tileActor.Layer, tileActor.XCoord, tileActor.YCoord, tileActor.ID, out Actor _))
                            {
                                level.Room.Actors.Add(tileActor);
                            }
                        }
                    }
                }
            }

            if (!tileChanged)
            {
                return false;
            }

            _history.StoreTileChange(tileChanges, layer);
            return true;
        }
    }
}
