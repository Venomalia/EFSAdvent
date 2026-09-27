using AuroraLib.Core.Format.Identifier;
using AuroraLib.Pixel;
using AuroraLib.Pixel.Formats.Dolphin;
using AuroraLib.Pixel.Image;
using AuroraLib.Pixel.PixelFormats;
using AuroraLib.Pixel.PixelProcessor;
using AuroraLib.Pixel.Processing;
using AuroraLib.Pixel.Processing.Processor;
using AuroraLib.Pixel.Processing.Resampler;
using AuroraLib.Pixel.Texture;
using FSALib.AssetDefinitions;
using FSALib.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace FSALib.Rendering
{
    public sealed class StageRenderer<TColor> : IDisposable where TColor : unmanaged, IColor<TColor>, IRGBA<byte>
    {
        const int PartSize = 8;
        const int TileSize = TilesetRenderer<TColor>.TileSize;
        const int SCREEN_PIXEL_DIMENSION = Layer.DIMENSION * TileSize;
        private static BTI BtiDecoder = new BTI();

        public readonly Rarc data;
        public readonly Dictionary<string, IImage> Images;

        public readonly TilesetRenderer<TColor> tilesetRendererTV;
        public readonly TilesetRenderer<TColor> tilesetRendererGBA;
        private int _CurrentTileSheetIndex = -1;

        public readonly SpriteRenderer<TColor> spriteRendererTV;
        public readonly SpriteRenderer<TColor> spriteRendererGBA;
        private int _CurrentNPSheetIndex = -1;

        public uint LocalVariable { get; set; }
        public Stage CurrentStage { get; set; }

        private TColor[] KawazokoPalette;

        public StageRenderer(Rarc dataRarc)
        {
            LocalVariable = uint.MaxValue;
            this.data = dataRarc;
            tilesetRendererTV = new TilesetRenderer<TColor>(dataRarc);
            tilesetRendererGBA = new TilesetRenderer<TColor>(dataRarc);
            spriteRendererTV = new SpriteRenderer<TColor>(dataRarc);
            spriteRendererGBA = new SpriteRenderer<TColor>(dataRarc);
            CurrentStage = new Stage();
            Images = new Dictionary<string, IImage>();


            var imageDir = dataRarc.Root.Directorys[Rarc.CommonFolderTypes.Timg];
            foreach (var file in imageDir.Files)
            {
                if (BtiDecoder.IsMatch(file.Data))
                {
                    Images.Add(file.Name, BtiDecoder.ReadImage(file.Data));
                }
            }
            imageDir.Dispose();

            KawazokoPalette = new TColor[16];
            var a = new Vector4(0x00, 0x50, 0x78, 255) / 255f;
            var b = new Vector4(0x38, 0x60, 0xA8, 255) / 255f;
            for (int i = 0; i < 16; i++)
            {
                float t = i / 15f;
                KawazokoPalette[i].FromScaledVector4(Vector4.Lerp(a, b, t));
            }
        }


        public void Draw(IImage<TColor> target, int roomIndex, byte layer, Point offset = default, RenderOptions display = RenderOptions.Layers | RenderOptions.Actors | RenderOptions.Overlay, InteractionFlags interactionFlags = default)
        {
            var properties = CurrentStage.Map.GetRoomProperties(roomIndex);
            if (_CurrentTileSheetIndex != properties.TileSheetId)
            {
                _CurrentTileSheetIndex = properties.TileSheetId;
                tilesetRendererTV.LoadTileset(data, _CurrentTileSheetIndex);
                tilesetRendererGBA.LoadTileset(data, _CurrentTileSheetIndex, true);
            }
            if (_CurrentNPSheetIndex != properties.NPCSheetID)
            {
                _CurrentNPSheetIndex = properties.NPCSheetID;
                spriteRendererTV.LoadTilesheet(data, _CurrentNPSheetIndex);
                spriteRendererGBA.LoadTilesheet(data, _CurrentNPSheetIndex, true);
            }
            var room = CurrentStage.Rooms[roomIndex];
            if (room == null)
                return;

            bool isTvLayer = layer == 0;

            if (isTvLayer && display.HasFlag(RenderOptions.Environment))
            {
                // Sets the transparency for the 8th, 9th and 10th color in the 13th palette
                Span<TColor> palette13 = tilesetRendererTV.Palettes.AsSpan(13 * 16, 16);
                palette13[8].A = 80;
                palette13[9].A = 120;
                palette13[10].A = 160;
            }
            else
            {
                Span<TColor> palette13 = tilesetRendererTV.Palettes.AsSpan(13 * 16, 16);
                palette13[10].A = palette13[9].A = palette13[8].A = byte.MaxValue;
            }

            var renderer = isTvLayer ? tilesetRendererTV : tilesetRendererGBA;
            EnvironmentFlags environment = default;

            if (display.HasFlag(RenderOptions.BaseLayer))
            {
                if (interactionFlags == InteractionFlags.None)
                {
                    renderer.Draw(target, room.BaseLayers[layer], offset);
                }
                else
                {
                    var tiles = room.BaseLayers[layer].Tiles;
                    for (int y = 0; y < Layer.DIMENSION; y++)
                    {
                        for (int x = 0; x < Layer.DIMENSION; x++)
                        {
                            ushort tile = tiles[y * Layer.DIMENSION + x];

                            while (Assets.TileProperties.TryGetValue(tile, out var tileDefinition) && ((int)tileDefinition.Interaction & (int)interactionFlags) != 0)
                            {
                                tile = tileDefinition.InteractionTile;
                            }

                            renderer.DrawTile(target, offset.X + x * TileSize, offset.Y + y * TileSize, tile);
                        }
                    }
                }
            }

            bool displayActors = display.HasFlag(RenderOptions.Actors);
            bool displayTileChanges = display.HasFlag(RenderOptions.TileChanges);
            if (displayTileChanges) interactionFlags |= InteractionFlags.GBARewriter;

            if (displayActors || displayTileChanges || display.HasFlag(RenderOptions.Environment))
            {
                var (Start, End) = room.Actors.GetLayerRange(layer);
                for (int i = Start; i < End; i++)
                {
                    var actor = room.Actors[i];
                    int triggerVariable = actor.VariableByte4 >> 3;
                    if (triggerVariable != 0 && ((uint)(1 << triggerVariable) & LocalVariable) == 0)
                        continue;

                    if (actor.ID == 0x56455753) // SWEV)
                    {
                        environment = (EnvironmentFlags)(actor.Variable >> 4 & 0x7FFFFFF);
                        continue;
                    }

                    if (displayTileChanges)
                        DrawTileChange(target, actor, room, offset);

                    if (displayActors)
                        Draw(target, actor, offset);
                }
            }

            if (display.HasFlag(RenderOptions.TopLayer))
                renderer.Draw(target, room.TopLayers[layer], offset);

            if (isTvLayer && display.HasFlag(RenderOptions.Overlay))
                DrawTVOverlay(target, properties.OverlayTextureId, offset);

            if (isTvLayer && display.HasFlag(RenderOptions.Environment))
            {
                DrawEnvironmentEffekt(target, environment, offset);
                // Reset the transparency for the 8th, 9th and 10th color in the 13th palette
                Span<TColor> palette13 = tilesetRendererTV.Palettes.AsSpan(13 * 16, 16);
                palette13[10].A = palette13[9].A = palette13[8].A = byte.MaxValue;
            }

            if (display.HasFlag(RenderOptions.Collision))
                DrawCollision(target, room.BaseLayers[layer], offset, interactionFlags);
        }

        public void DrawEnvironmentEffekt(IImage<TColor> target, EnvironmentFlags environment, Point offset = default)
        {
            var targetRegion = new Rectangle(offset.X, offset.Y, SCREEN_PIXEL_DIMENSION, SCREEN_PIXEL_DIMENSION - 128);
            if (environment.HasFlag(EnvironmentFlags.WaterEffect))
            {
                if (Images.TryGetValue("kawazoko.bti", out var image) && image is IImage<I4> kawazoko)
                {
                    var kawazokoColor = new PaletteImage<I4, TColor>(kawazoko, KawazokoPalette.AsMemory());
                    CopyFromReapet(target, kawazokoColor, targetRegion, BlendModes.Background);
                }
            }
            else if (environment.HasFlag(EnvironmentFlags.LavaEffect))
            {
                FillProcessor lava = new FillProcessor(Color.DarkRed, BlendModes.Background);
                target.Apply(lava, targetRegion);
            }
            else if (environment.HasFlag(EnvironmentFlags.MoriBG))
            {
                if (CurrentStage.Resources.Directorys.TryGetValue(Rarc.CommonFolderTypes.Timg, out var mapFolder) && mapFolder.TryGetFile("bg_mori.bti", out var btiFile))
                {
                    using IImage btiImage = BtiDecoder.ReadImage(btiFile.Data);
                    target.ResizeFrom(btiImage, new Rectangle(32, 32, btiImage.Width - 64, btiImage.Height - 64), targetRegion, Resamplers.NearestNeighbor, BlendModes.Background);
                }
            }
            else if (environment.HasFlag(EnvironmentFlags.PiramidBG))
            {
                if (CurrentStage.Resources.Directorys.TryGetValue(Rarc.CommonFolderTypes.Timg, out var mapFolder) && mapFolder.TryGetFile("bg_piramid.bti", out var btiFile))
                {
                    using IImage btiImage = BtiDecoder.ReadImage(btiFile.Data);
                    var region = new Rectangle(0, 0, SCREEN_PIXEL_DIMENSION, SCREEN_PIXEL_DIMENSION - 128);
                    target.CopyFrom(btiImage, region, offset, BlendModes.Background);
                }
            }
            else if (environment.HasFlag(EnvironmentFlags.MapBG))
            {
                if (CurrentStage.Resources.Directorys.TryGetValue(Rarc.CommonFolderTypes.Timg, out var mapFolder) && mapFolder.TryGetFile("enkei.bti", out var btiFile))
                {
                    using IImage btiImage = BtiDecoder.ReadImage(btiFile.Data);
                    target.ResizeFrom(btiImage, new Rectangle(32, 32, btiImage.Width - 64, btiImage.Height - 64), targetRegion, Resamplers.NearestNeighbor, BlendModes.Background);
                }
            }
            else if (environment.HasFlag(EnvironmentFlags.TowerBG))
            {
                if (CurrentStage.Resources.Directorys.TryGetValue(Rarc.CommonFolderTypes.Timg, out var mapFolder) && mapFolder.TryGetFile("wall.bti", out var btiFile))
                {
                    using IImage btiImage = BtiDecoder.ReadImage(btiFile.Data);
                    CopyFromReapet(target, btiImage, targetRegion, BlendModes.Background);
                }
                if (mapFolder != null && mapFolder.TryGetFile("sky.bti", out btiFile))
                {
                    using IImage btiImage = BtiDecoder.ReadImage(btiFile.Data);
                    CopyFromReapet(target, btiImage, targetRegion, BlendModes.Background);
                }
            }
            else if (environment.HasFlag(EnvironmentFlags.DarkBG))
            {
                FillProcessor bg = new FillProcessor(Color.DarkSlateBlue, BlendModes.Background);
                target.Apply(bg, targetRegion);
            }
            else
            {
                FillProcessor bg = new FillProcessor(Color.Black, BlendModes.Background);
                target.Apply(bg, targetRegion);
            }

            if (environment.HasFlag(EnvironmentFlags.Clouds))
            {
                if (Images.TryGetValue("kumo_env.bti", out var image))
                {
                    var imageRegion = new Rectangle(0, 0, image.Width, image.Height - 16);
                    target.ResizeFrom(image, imageRegion, targetRegion, Resamplers.NearestNeighbor, BlendModes.Subtract, 0.2f);
                }
            }

            if (environment.HasFlag(EnvironmentFlags.Fog))
            {
                FillProcessor Rain = new FillProcessor(Color.FromArgb(32, Color.LightGray), BlendModes.Normal);
                target.Apply(Rain, targetRegion);
            }

            if (environment.HasFlag(EnvironmentFlags.RainB))
            {
                FillProcessor Rain = new FillProcessor(Color.FromArgb(48, Color.DarkBlue), BlendModes.Normal);
                target.Apply(Rain, targetRegion);
            }


        }

        public void CopyFromReapet(IImage image, IReadOnlyImage source, Rectangle region, BlendModes.BlendFunction blend = null, float intensity = 1f)
        {
            Rectangle srcRegion = source.GetBounds();

            for (int y = 0; y < region.Height; y += source.Height)
            {
                for (int x = 0; x < region.Width; x += source.Width)
                {
                    Rectangle targetRegion = Rectangle.Intersect(new Rectangle(region.X + x, region.Y + y, source.Width, source.Height), region);
                    Rectangle sourceRegion = new Rectangle(srcRegion.X, srcRegion.Y, targetRegion.Width, targetRegion.Height);

                    image.CopyFrom(source, sourceRegion, targetRegion.Location, blend);
                }
            }
        }

        public void DrawCollision(IImage<TColor> target, Layer layer, Point offset = default, InteractionFlags flags = default)
        {
            const int TileSize = TilesetRenderer<TColor>.TileSize;
            var fillMain = new FillProcessor(Color.Black, BlendModes.Normal);
            var fillSolid = new FillProcessor(Color.FromArgb(160, Color.Black), BlendModes.Normal);

            ReadOnlySpan<ushort> tiles = layer.Tiles;

            for (int y = 0; y < Layer.DIMENSION; y++)
            {
                for (int x = 0; x < Layer.DIMENSION; x++)
                {
                    ushort tile = tiles[y * Layer.DIMENSION + x];

                    if (Assets.TileProperties.TryGetValue(tile, out var tileDefinition))
                    {
                        Point pos = new Point(offset.X + x * TileSize, offset.Y + y * TileSize);

                        if (((int)tileDefinition.Interaction & (int)flags) != 0)
                        {
                            tile = tileDefinition.InteractionTile;
                            if (!Assets.TileProperties.TryGetValue(tile, out tileDefinition))
                                continue;
                        }

                        Color main = tileDefinition.Surface switch
                        {
                            SurfaceType.Abyss => Color.White,
                            SurfaceType.ShallowWater => Color.Aqua,
                            SurfaceType.DeepWater => Color.Blue,
                            SurfaceType.Slippery => Color.Lavender,
                            SurfaceType.Quicksand => Color.Yellow,
                            SurfaceType.Ladder => Color.Bisque,
                            _ => Color.Transparent,
                        };

                        fillMain.Color = main == Color.Transparent ? default : ((RGBA<byte>)Color.FromArgb(160, main)).ToScaledVector4();
                        if (tileDefinition.Collision == TileCollision.Walkable)
                        {
                            if (main != Color.Transparent)
                                target.Apply(fillMain, new Rectangle(pos.X, pos.Y, TileSize, TileSize));
                        }
                        else if (tileDefinition.Collision == TileCollision.Solid)
                        {
                            target.Apply(fillSolid, new Rectangle(pos.X, pos.Y, TileSize, TileSize));
                        }
                        else
                        {
                            FillProcessor processor = tileDefinition.Collision.HasFlag(TileCollision.TopLeft) ? fillSolid : fillMain;
                            if (processor.Color.W != 0) target.Apply(processor, new Rectangle(pos.X, pos.Y, PartSize, PartSize));
                            processor = tileDefinition.Collision.HasFlag(TileCollision.TopRight) ? fillSolid : fillMain;
                            if (processor.Color.W != 0) target.Apply(processor, new Rectangle(pos.X + PartSize, pos.Y, PartSize, PartSize));
                            processor = tileDefinition.Collision.HasFlag(TileCollision.BottomLeft) ? fillSolid : fillMain;
                            if (processor.Color.W != 0) target.Apply(processor, new Rectangle(pos.X, pos.Y + PartSize, PartSize, PartSize));
                            processor = tileDefinition.Collision.HasFlag(TileCollision.BottomRight) ? fillSolid : fillMain;
                            if (processor.Color.W != 0) target.Apply(processor, new Rectangle(pos.X + PartSize, pos.Y + PartSize, PartSize, PartSize));
                        }

                        Color secondary = tileDefinition.Properties switch
                        {
                            TileProperties.Hazard => Color.Red,
                            TileProperties.EnemyCollision => Color.Pink,
                            TileProperties.ThrowOver => Color.Green,
                            TileProperties.DropOff => Color.Yellow,
                            _ => Color.Transparent,
                        };
                        if (secondary != Color.Transparent)
                        {
                            fillMain.Color = ((RGBA<byte>)Color.FromArgb(160, secondary)).ToScaledVector4();
                            target.Apply(fillMain, new Rectangle(pos.X + 6, pos.Y + 6, 4, 4));
                        }
                    }
                }
            }
        }

        private void DrawTileChange(IImage<TColor> target, Actor actor, Room room, Point offset = default)
        {
            bool isTvLayer = actor.Layer == 0;
            var renderer = isTvLayer ? tilesetRendererTV : tilesetRendererGBA;
            if (actor.ID == 0x43504E50) // PNPC
            {
                ushort tile = 0;
                if (isTvLayer)
                {
                    tile = (ushort)((actor.VariableByte2 & 0x3) << 8 | actor.VariableByte1);
                }
                else
                {
                    ushort targetTile = room.Layers[actor.Layer][actor.XCoord / 2, actor.YCoord / 2];
                    if (Assets.TileProperties.TryGetValue(targetTile, out var tileProperty) || !tileProperty.Interaction.HasFlag(InteractionFlags.GBARewriter))
                        tile = tileProperty.InteractionTile;
                }
                renderer.DrawTile(target, offset.X + actor.XCoord / 2 * TileSize, offset.Y + actor.YCoord / 2 * TileSize, tile);
            }
            else if (actor.ID == 0x32504E50) //PNP2
            {
                ushort tileTarget = (ushort)(actor.Variable & 0xFFF);
                ushort tile = (ushort)(actor.Variable >> 12 & 0xFFF);

                var layerTiles = room.Layers[actor.Layer].Tiles;
                for (int y = 0; y < Layer.DIMENSION; y++)
                {
                    for (int x = 0; x < Layer.DIMENSION; x++)
                    {
                        if (layerTiles[x + (y * Layer.DIMENSION)] == tileTarget)
                        {
                            renderer.DrawTile(target, offset.X + x * TileSize, offset.Y + y * TileSize, tile);
                        }
                    }
                }
            }
        }
        public void Draw(IImage<TColor> target, Actor actor, Point offset = default)
        {
            if (actor.ID == 0x3246494C) //LIF2
            {
                if (Images.TryGetValue("lift_00.bti", out var image))
                {
                    uint width = actor.Variable >> 7 & 0x1F;
                    uint height = actor.Variable >> 17 & 0x1F;
                    uint direction = (actor.Variable >> 12 & 0x3) switch { 0 => 270, 1 => 90, 2 => 0, 3 => 180 };
                    var transform = Matrix3x2.CreateScale(new Vector2(1f / (image.Width / 16f) * width, 1f / (image.Height / 16f) * height));
                    transform *= Matrix3x2.CreateRotation(direction * ((float)Math.PI / 180f), new Vector2(8, 8));
                    transform *= Matrix3x2.CreateTranslation(new Vector2(offset.X + actor.XCoord * 8, offset.Y + actor.YCoord * 8));
                    target.Transform(image, transform, Resamplers.NearestNeighbor);
                }

            }

            if (Assets.Actors.TryGetValue(actor.ID, out ActorDefinition actorDefinition) && actorDefinition.Rendering != null)
            {
                var renderer = actor.Layer == 0 ? spriteRendererTV : spriteRendererGBA;
                var rendering = actorDefinition.Rendering;
                int variantKey = (int)(actor.Variable & rendering.BitMask);
                Point actorOffset = new Point(offset.X + actor.XCoord * 8, offset.Y + actor.YCoord * 8);

                if (rendering.Variants.TryGetValue(variantKey, out var renderList))
                {
                    foreach (var renderInfo in renderList)
                    {
                        Identifier32 tag = actorDefinition.Resources != null ? Rarc.DirectoryNode.ToFourCC(actorDefinition.Resources![0]) : Rarc.CommonFolderTypes.Timg;
                        if (renderInfo.SpriteIndex != -1) // render sprite
                        {
                            if (renderInfo.XScale != 1 || renderInfo.YScale != 1 || renderInfo.Rotation != 0)
                            {
                                using var buffer = new MemoryImage<TColor>(64, 64, 0, true);
                                renderer.DrawSprite(buffer, buffer.Width / 2, buffer.Height / 2, (ushort)renderInfo.SpriteIndex, renderInfo.SpriteListIndex, renderInfo.ReplacementPaletteIndex, renderInfo.TargetPaletteIndex);
                                Draw(target, buffer, actorOffset, renderInfo);
                            }
                            else
                            {
                                renderer.DrawSprite(target, actorOffset.X + renderInfo.XOffset, actorOffset.Y + renderInfo.YOffset, (ushort)renderInfo.SpriteIndex, renderInfo.SpriteListIndex, renderInfo.ReplacementPaletteIndex, renderInfo.TargetPaletteIndex);
                            }
                        }
                        else if (actor.Layer == 0) // render bti, only TV
                        {
                            if (Images.TryGetValue(renderInfo.BtiFile, out var image))
                            {
                                Draw(target, image, actorOffset, renderInfo);
                            }
                            else if (CurrentStage.Resources.Directorys.TryGetValue(tag, out var mapFolder) && mapFolder.TryGetFile(renderInfo.BtiFile, out var btiFile))
                            {
                                using IImage btiImage = BtiDecoder.ReadImage(btiFile.Data);
                                Draw(target, btiImage, actorOffset, renderInfo);
                            }
                            else
                            {
                                target.Apply(new FillProcessor(((RGBA<byte>)Color.Magenta).ToScaledVector4()), new Rectangle(actorOffset, new Size(8, 8)));
                            }
                        }
                    }
                }
            }

            void Draw(IImage<TColor> target, IImage image, Point offset, ActorRenderVariant variant)
            {
                if (variant.XScale != 1 || variant.YScale != 1 || variant.Rotation != 0)
                {
                    var transform = Matrix3x2.CreateScale(new Vector2(variant.XScale, variant.YScale));
                    transform *= Matrix3x2.CreateRotation(variant.Rotation * ((float)Math.PI / 180f), new Vector2(image.Width / 2, image.Height / 2));
                    transform *= Matrix3x2.CreateTranslation(new Vector2(offset.X + variant.XOffset - (image.Width * variant.XScale / 2), offset.Y + variant.YOffset - (image.Height * variant.YScale / 2)));
                    target.Transform(image, transform, Resamplers.NearestNeighbor, BlendModes.Normal);
                    return;
                }

                var targetOffset = new Point(offset.X + variant.XOffset - image.Width / 2, offset.Y + variant.YOffset - image.Height / 2);
                bool ifLight = image is FlatTexture<I4>;
                BlendModes.BlendFunction blend = ifLight ? (BlendModes.BlendFunction)BlendModes.Add : BlendModes.Normal;
                target.CopyFrom(image, targetOffset, blend, ifLight ? 0.6f : 1f);
            }
        }

        public void DrawTVOverlay(IImage image, int overlayIndex, Point offset = default)
        {
            if (Images.TryGetValue($"filter{overlayIndex}.bti", out IImage overlayImage))
            {
                var region = overlayImage.GetBounds();
                for (int y = 0; y < SCREEN_PIXEL_DIMENSION - 128; y += overlayImage.Height)
                {
                    for (int x = 0; x < SCREEN_PIXEL_DIMENSION; x += overlayImage.Width)
                    {
                        var pose = new Point(offset.X + x, offset.Y + y);
                        image.CopyFrom(overlayImage, image, region, pose, pose, Multiply);
                    }
                }
            }

            static Vector4 Multiply(Vector4 baseColor, Vector4 blendColor, float intensity) => intensity == 1f ? BlendModes.Multiply(baseColor, blendColor, 1f) : baseColor;

        }

        public void Dispose()
        {
            data.Dispose();
            foreach (var item in Images.Values)
                item.Dispose();
        }
    }
}
