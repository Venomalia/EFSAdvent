using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using System;
using FSALib.AssetDefinitions;


#if NETSTANDARD || NET20_OR_GREATER
using Newtonsoft.Json;
#else
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

namespace FSALib
{
    /// <summary>
    /// Stores and provides access to metadata for asset and data management, such as tile properties and actor schemas.
    /// </summary>
    public static class Assets
    {
        private const string AssetsDirectory = "assets";

        private static Dictionary<int, string> songs;
        private static Dictionary<ushort, TilePropertyDefinition> tileProperties;
        private static readonly Dictionary<Identifier32, ActorDefinition> actors;
        private static Dictionary<int, WorldDefinition> worlds;
        private static Dictionary<string, StageDefinition> stages;
        private static Dictionary<int, BattleStageDefinition> battleStages;
        private static Dictionary<int, TilesetDefinition> tilesets;
        private static readonly ushort[] mirrorLOT;

        /// <summary>
        /// Gets a read-only dictionary of song IDs and their respective names.
        /// </summary>
        public static IReadOnlyDictionary<int, string> Songs => songs;

        /// <summary>
        /// Gets a read-only dictionary of <see cref="TilePropertyDefinition"/> indexed by their unique ID.
        /// </summary>
        public static IReadOnlyDictionary<ushort, TilePropertyDefinition> TileProperties => tileProperties;

        /// <summary>
        /// Gets a read-only dictionary of world definitions indexed by world ID.
        /// </summary>
        public static IReadOnlyDictionary<int, WorldDefinition> Worlds => worlds;

        /// <summary>
        /// Gets a read-only dictionary of stage definitions indexed by stage name.
        /// </summary>
        public static IReadOnlyDictionary<string, StageDefinition> Stages => stages;

        /// <summary>
        /// Gets a read-only dictionary of battle stage definitions indexed by battle stage ID.
        /// </summary>
        public static IReadOnlyDictionary<int, BattleStageDefinition> BattleStages => battleStages;

        /// <summary>
        /// Gets a read-only dictionary of tileset definitions indexed by tileset ID.
        /// </summary>
        public static IReadOnlyDictionary<int, TilesetDefinition> Tilesets => tilesets;

        /// <summary>
        /// Lookup table that provides the mirrored tile ID for each tile.
        /// </summary>
        public static ReadOnlySpan<ushort> MirrorTileLOT => mirrorLOT;

        /// <summary>
        /// Gets a read-only dictionary of <see cref="ActorDefinition"/> indexed by their unique actor identifier.
        /// </summary>
        public static IReadOnlyDictionary<Identifier32, ActorDefinition> Actors => actors;

        static Assets()
        {
            actors = new Dictionary<Identifier32, ActorDefinition>();
            mirrorLOT = new ushort[0x400];
            Reload();
        }

        /// <summary>
        /// Reloads all assets from their respective JSON files.
        /// </summary>
        public static void Reload()
        {
            // Reload song list
            string songsJson = Path.Combine(AssetsDirectory, "songs.json");
            if (!Deserialize(songsJson, out songs))
                songs = new Dictionary<int, string>();

            // Reload tile properties list
            string tilePropertiesJson = Path.Combine(AssetsDirectory, "tileproperties.json");
            if (!Deserialize(tilePropertiesJson, out tileProperties))
                tileProperties = new Dictionary<ushort, TilePropertyDefinition>();

            // Reload stages list
            string worldsJson = Path.Combine(AssetsDirectory, "worlds.json");
            if (!Deserialize(worldsJson, out worlds))
                worlds = new Dictionary<int, WorldDefinition>();

            // Reload stages list
            string stagesJson = Path.Combine(AssetsDirectory, "stages.json");
            if (!Deserialize(stagesJson, out stages))
                stages = new Dictionary<string, StageDefinition>();

            // Reload battle stages list
            string battleStagesJson = Path.Combine(AssetsDirectory, "battlestages.json");
            if (!Deserialize(battleStagesJson, out battleStages))
                battleStages = new Dictionary<int, BattleStageDefinition>();

            // Reload battle stages list
            string tilesetsJson = Path.Combine(AssetsDirectory, "tilesets.json");
            if (!Deserialize(tilesetsJson, out tilesets))
                tilesets = new Dictionary<int, TilesetDefinition>();

            // Reload actor schemas
            string actorsDirectory = Path.Combine(AssetsDirectory, "actors");
            if (Directory.Exists(actorsDirectory))
            {
                actors.Clear();
                foreach (var filePath in Directory.GetFiles(actorsDirectory, "*.json"))
                {
                    if (Deserialize(filePath, out ActorDefinition schema))
                    {
                        Identifier32 identifier = new Identifier32(PathX.GetFileNameWithoutExtension(filePath.AsSpan()));
                        actors.Add(identifier, schema);
                    }
                }
            }
            else
            {
                Trace.WriteLine($"⚠️ The directory {actorsDirectory} does not exist.");
            }

            for (ushort i = 1; i < mirrorLOT.Length; i++)
            {
                if (TileProperties.TryGetValue(i, out TilePropertyDefinition tileInfo) && tileInfo.MirrorTile != 0)
                {
                    mirrorLOT[i] = tileInfo.MirrorTile;
                }
                else
                {
                    mirrorLOT[i] = i;
                }
            }
        }

        private static bool Deserialize<TValue>(string path, out TValue value) where TValue : class
        {
            if (!File.Exists(path))
            {
                Trace.WriteLine($"⚠️ The file {path} does not exist.");
                value = null;
                return false;
            }
            try
            {
                using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                value = Deserialize<TValue>(stream);
                if (value == null)
                {
                    Trace.WriteLine($"⚠️ JSON parsing failed for {path}.");
                    return false;
                }
                return true;
            }
#if NET6_0_OR_GREATER

            catch (JsonException ex)
            {
                throw new JsonException($"Failed to deserialize JSON file: '{path}'. {ex.Message}", ex);
            }
#else
            catch (JsonReaderException ex)
            {
                throw new JsonReaderException($"Failed to deserialize JSON file: '{path}'. {ex.Message}", ex);
            }
#endif
        }

        internal static TValue? Deserialize<TValue>(Stream stream) where TValue : class
        {
#if NETSTANDARD || NET20_OR_GREATER
    using var reader = new StreamReader(stream);
    string json = reader.ReadToEnd();
    return JsonConvert.DeserializeObject<TValue>(json);
#else
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new Identifier32Converter());
            return JsonSerializer.Deserialize<TValue>(stream, options);
#endif
        }

#if !NETSTANDARD && !NET20_OR_GREATER
        private sealed class Identifier32Converter : JsonConverter<Identifier32>
        {
            public override Identifier32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.String)
                    throw new JsonException();

                return new Identifier32(reader.GetString().AsSpan());
            }

            public override void Write(Utf8JsonWriter writer, Identifier32 value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
#endif

        internal static void Serialize(Stream stream, object? value)
        {
#if NETSTANDARD || NET20_OR_GREATER
            using var writer = new StreamWriter(stream);
            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            writer.Write(json);
#else 
            JsonSerializer.Serialize(stream, value, new JsonSerializerOptions { WriteIndented = true });
#endif
        }
    }
}
