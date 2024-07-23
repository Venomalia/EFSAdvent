using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EFSAdvent.FourSwords
{
    public class Room
    {
        private readonly Layer[,] _layers;
        private List<Actor> _actors;
        private readonly byte _roomNumber;
        private readonly Logger _logger;

        public bool ActorsAreDirty { get; private set; }
        public bool LayersAreDirty { get; private set; }

        public static string GetLayerFolder(string path, string levelNumber) => Path.Combine(path, "szs", $"m{levelNumber}");

        public static string GetLayerFileName(string levelNumber, int roomNumber, int level = 1, int layer = 0) => $"d_map{levelNumber}_{roomNumber:D2}_mmm_{level}_{layer}.szs";

        public static string GetLayerFilePath(string path, string levelNumber, int roomNumber, int level = 1, int layer = 0) => Path.Combine(GetLayerFolder(path, levelNumber), GetLayerFileName(levelNumber, roomNumber, level, layer));

        public static string GetActorFolder(string path, string levelNumber) => Path.Combine(path, "bin", $"b{levelNumber}");

        public static string GetActorFileName(string levelNumber, int roomNumber) => $"d_enemy_map{levelNumber}_{roomNumber:D2}.bin";

        public static string GetActorFilePath(string path, string levelNumber, int roomNumber) => Path.Combine(GetActorFolder(path, levelNumber), GetActorFileName(levelNumber, roomNumber));

        public Room(string path, string levelNumber, byte roomNumber, Logger logger, bool newRoom = false)
        {
            _layers = new Layer[8, 2];

            string layerFolder = GetLayerFolder(path, levelNumber);

            _roomNumber = roomNumber;
            _logger = logger;

            for (int layer = 0; layer < 8; layer++)
            {
                for (int level = 1; level < 3; level++)
                {
                    if (newRoom)
                    {
                        _layers[layer, level - 1] = new Layer();
                    }
                    else
                    {
                        string layerFileName = GetLayerFileName(levelNumber, roomNumber, level, layer);
                        string szsPath = Path.Combine(layerFolder, layerFileName);

                        _layers[layer, level - 1] = new Layer(szsPath, _logger);
                    }
                }
            }

            LayersAreDirty = false;

            if (newRoom)
            {
                _actors = new List<Actor>();
            }
            else
            {
                ReloadActors(path, levelNumber);
            }
        }

        public bool SaveLayers(string path, string levelNumber)
        {
            string layerFolder = GetLayerFolder(path, levelNumber);
            for (int layer = 0; layer < 8; layer++)
            {
                for (int level = 1; level < 3; level++)
                {
                    var curLayer = _layers[layer, level - 1];
                    var szsFormatLayer = curLayer.ConvertToSzsFormat();

                    var encodedSzs = Yaz0.Encode(szsFormatLayer);

                    string layerFileName = GetLayerFileName(levelNumber, _roomNumber, level, layer);
                    string szsPath = Path.Combine(layerFolder, layerFileName);
                    FileStream fileStream = File.Create(szsPath, encodedSzs.Length);
                    fileStream.Write(encodedSzs, 0, encodedSzs.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            LayersAreDirty = false;
            return true;
        }

        public ushort? GetLayerTile(int layer, int x, int y)
        {
            return _layers[layer % 8, layer < 8 ? 0 : 1].GetTile(x, y);
        }

        public bool SetLayerTile(int layer, int x, int y, ushort newValue)
        {
            bool success = _layers[layer % 8, layer < 8 ? 0 : 1].SetTile(x, y, newValue);
            if (success)
            {
                LayersAreDirty = true;
            }
            return success;
        }

        private void SortActors()
        {
            _actors = _actors.OrderBy(a => a.Layer).ThenBy(a => a.Name).ThenBy(a => a.XCoord).ThenBy(a => a.YCoord).ToList();
        }

        public void ReloadActors(string path, string levelNumber)
        {
            string actorsPath = GetActorFilePath(path, levelNumber, _roomNumber);
            _actors = new List<Actor>();
            if (File.Exists(actorsPath))
            {
                byte[] readBuffer = File.ReadAllBytes(actorsPath);

                //Skip the last null entry
                for (int i = 0; i < readBuffer.Length - 11; i += 11)
                {
                    _actors.Add(new Actor
                    (
                        Encoding.ASCII.GetString(readBuffer.Skip(i).Take(4).ToArray()),
                        readBuffer[i + 4],
                        readBuffer[i + 5],
                        readBuffer[i + 6],
                        readBuffer[i + 7],
                        readBuffer[i + 8],
                        readBuffer[i + 9],
                        readBuffer[i + 10]
                    ));
                }

                SortActors();

            }
            else
            {
                _logger.AppendLine("Can't find actors *.bin file. A new actors file has been created.");
            }
            ActorsAreDirty = false;
        }

        public void SaveActors(string path, string levelNumber)
        {
            string actorsPath = Path.Combine(GetActorFolder(path, levelNumber), GetActorFileName(levelNumber, _roomNumber));
            FileStream actorsStream = File.Create(actorsPath);
            byte[] actorsBinary = GetActorsAsBinary();

            actorsStream.Write(actorsBinary, 0, actorsBinary.Length);
            actorsStream.Flush();
            actorsStream.Close();
        }

        public Actor GetActor(int index)
        {
            if (index < 0 || index > _actors.Count)
            {
                return null;
            }

            return _actors[index];
        }

        public int IndexOf(Actor actor)
        {
            return _actors.IndexOf(actor);
        }

        public List<Actor> GetActors()
        {
            return _actors;
        }

        public Actor AddActor(string name, byte layer, byte xCoord = 0, byte yCoord = 0)
        {
            Actor actor = new Actor(name, layer, xCoord, yCoord);
            AddActor(actor);
            return actor;
        }

        public void AddActor(Actor actor)
        {
            _actors.Add(actor);
            SortActors();
            ActorsAreDirty = true;
        }

        public bool RemoveActorAt(int index)
        {
            if (_actors.Count <= index || index < 0)
            {
                return false;
            }
            _actors.RemoveAt(index);
            SortActors();
            ActorsAreDirty = true;
            return true;
        }

        public void SetActorLocation(int index, int x, int y)
        {
            if (index < 0 || index >= _actors.Count)
            {
                return;
            }
            _actors[index].SetCoordinates(x, y);
            ActorsAreDirty = true;
        }

        public void UpdateActor(int index, string name, byte layer, byte xCoord, byte yCoord, byte variable1, byte variable2, byte variable3, byte variable4)
        {
            if (index < 0 || index >= _actors.Count)
            {
                return;
            }
            _actors[index].Update(name, layer, xCoord, yCoord, variable1, variable2, variable3, variable4);
            SortActors();
            ActorsAreDirty = true;
        }

        public bool CloneActor(int index)
        {
            if (_actors.Count <= index || index < 0)
            {
                return false;
            }
            _actors.Add(new Actor(_actors[index]));
            SortActors();
            return true;
        }

        public int GetActorCount()
        {
            return _actors.Count;
        }

        public byte[] GetActorsAsBinary()
        {
            const int ACTOR_BINARY_SIZE = 11;
            Actor actor;
            var binary = new byte[(_actors.Count + 1) * ACTOR_BINARY_SIZE];
            int i;
            for (i = 0; i < binary.Length - ACTOR_BINARY_SIZE; i += ACTOR_BINARY_SIZE)
            {
                actor = _actors[i / ACTOR_BINARY_SIZE];
                var nameBytes = Encoding.ASCII.GetBytes(actor.Name);
                binary[i + 0] = nameBytes[0];
                binary[i + 1] = nameBytes[1];
                binary[i + 2] = nameBytes[2];
                binary[i + 3] = nameBytes[3];
                binary[i + 4] = actor.Layer;
                binary[i + 5] = actor.XCoord;
                binary[i + 6] = actor.YCoord;
                binary[i + 7] = actor.Variable1;
                binary[i + 8] = actor.Variable2;
                binary[i + 9] = actor.Variable3;
                binary[i + 10] = actor.Variable4;
            }
            //Final null entry
            binary[i + 0] = 0x20;
            binary[i + 1] = 0x20;
            binary[i + 2] = 0x20;
            binary[i + 3] = 0x20;
            binary[i + 4] = 0x0;
            binary[i + 5] = 0x0;
            binary[i + 6] = 0x0;
            binary[i + 7] = 0x0;
            binary[i + 8] = 0x0;
            binary[i + 9] = 0x0;
            binary[i + 10] = 0x0;

            ActorsAreDirty = false;

            return binary;
        }
    }
}
