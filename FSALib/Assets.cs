using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using System;
using FSALib.AssetEntries;

#if NETSTANDARD || NET20_OR_GREATER
using Newtonsoft.Json;
#else
using System.Text.Json;
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
        private static Dictionary<ushort, TilePropertyEntry> tileProperties;
        private static readonly Dictionary<Identifier32, ActorEntry> actors;
        private static Dictionary<int, WorldEntry> worlds;
        private static Dictionary<string, StageEntry> stages;
        private static Dictionary<int, BattleStageEntry> battleStages;
        private static ushort[]? mirrorLOT;

        /// <summary>
        /// Gets a read-only dictionary of song IDs and their respective names.
        /// </summary>
        public static IReadOnlyDictionary<int, string> Songs => songs;

        /// <summary>
        /// Gets a read-only dictionary of <see cref="TilePropertyEntry"/> indexed by their unique ID.
        /// </summary>
        public static IReadOnlyDictionary<ushort, TilePropertyEntry> TileProperties => tileProperties;

        /// <summary>
        /// Gets a read-only dictionary of world definitions indexed by world ID.
        /// </summary>
        public static IReadOnlyDictionary<int, WorldEntry> Worlds => worlds;

        /// <summary>
        /// Gets a read-only dictionary of stage definitions indexed by stage name.
        /// </summary>
        public static IReadOnlyDictionary<string, StageEntry> Stages => stages;

        /// <summary>
        /// Gets a read-only dictionary of battle stage definitions indexed by battle stage ID.
        /// </summary>
        public static IReadOnlyDictionary<int, BattleStageEntry> BattleStages => battleStages;

        /// <summary>
        /// Lookup table that provides the mirrored tile ID for each tile.
        /// </summary>
        public static ReadOnlySpan<ushort> MirrorTileLOT
        {
            get
            {
                if (mirrorLOT == null)
                {
                    mirrorLOT = new ushort[0x400];
                    for (ushort i = 1; i < mirrorLOT.Length; i++)
                    {
                        if (TileProperties.TryGetValue(i, out TilePropertyEntry tileInfo) && tileInfo.MirrorTile != 0)
                        {
                            mirrorLOT[i] = tileInfo.MirrorTile;
                        }
                        else
                        {
                            mirrorLOT[i] = i;
                        }
                    }
                }
                return mirrorLOT;
            }
        }

        /// <summary>
        /// Gets a read-only dictionary of <see cref="ActorEntry"/> indexed by their unique actor identifier.
        /// </summary>
        public static IReadOnlyDictionary<Identifier32, ActorEntry> Actors => actors;

        static Assets()
        {
            actors = new Dictionary<Identifier32, ActorEntry>();
            Reload();
        }

        /// <summary>
        /// Reloads all assets from their respective JSON files.
        /// </summary>
        public static void Reload()
        {
            mirrorLOT = null;

            // Reload song list
            const string songsJson = AssetsDirectory + "\\songs.json";
            if (!Deserialize(songsJson, out songs))
            {
                songs = new Dictionary<int, string>();
            }

            // Reload tile properties list
            const string tilePropertiesJson = AssetsDirectory + "\\tileproperties.json";
            if (!Deserialize(tilePropertiesJson, out tileProperties))
            {
                tileProperties = new Dictionary<ushort, TilePropertyEntry>();
            }

            // Reload stages list
            const string worldsJson = AssetsDirectory + "\\worlds.json";
            if (!Deserialize(worldsJson, out worlds))
            {
                worlds = new Dictionary<int, WorldEntry>();
            }

            // Reload stages list
            const string stagesJson = AssetsDirectory + "\\stages.json";
            if (!Deserialize(stagesJson, out stages))
            {
                stages = new Dictionary<string, StageEntry>();
            }

            // Reload battle stages list
            const string battleStagesJson = AssetsDirectory + "\\battlestages.json";
            if (!Deserialize(battleStagesJson, out battleStages))
            {
                battleStages = new Dictionary<int, BattleStageEntry>();
            }

            // Reload actor schemas
            const string actorsDirectory = AssetsDirectory + "\\actors";
            if (Directory.Exists(actorsDirectory))
            {
                actors.Clear();
                foreach (var filePath in Directory.GetFiles(actorsDirectory, "*.json"))
                {
                    if (Deserialize(filePath, out ActorEntry schema))
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
            catch (Exception ex)
            {
                Trace.WriteLine($"❌ Exception while loading {path}: {ex.Message}");
                value = null;
                return false;
            }
        }

        internal static TValue? Deserialize<TValue>(Stream stream) where TValue : class
        {
#if NETSTANDARD || NET20_OR_GREATER
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<TValue>(json);
#else
            return JsonSerializer.Deserialize<TValue>(stream);
#endif
        }

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
