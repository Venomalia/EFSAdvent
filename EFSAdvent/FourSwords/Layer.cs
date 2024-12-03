using System.IO;

namespace EFSAdvent.FourSwords
{
    public class Layer
    {
        public const int DIMENSION = 32;

        private readonly ushort[] _data;

        public Layer()
        {
            _data = new ushort[0x400];
        }

        public Layer(string szsFilePath, Logger logger) : this()
        {
            byte[] source;

            if (File.Exists(szsFilePath))
            {
                source = File.ReadAllBytes(szsFilePath);
            }
            else
            {
                logger.AppendLine($"File not found: {szsFilePath}");
                return;
            }

            byte[] decodedSource = Yaz0.Decode(source);
            var gameFormatData = new ushort[_data.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                gameFormatData[i] = (ushort)(decodedSource[i * 2] + (decodedSource[(i * 2) + 1] << 8));
            }

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    _data[(y * DIMENSION) + x] = gameFormatData[(y * 16) + x];
                    _data[(y * DIMENSION) + x + 16] = gameFormatData[0x100 + (y * 16) + x];
                    _data[((y + 16) * DIMENSION) + x] = gameFormatData[0x200 + (y * 16) + x];
                    _data[((y + 16) * DIMENSION) + x + 16] = gameFormatData[0x300 + (y * 16) + x];
                }
            }
        }

        public ushort? GetTile(int x, int y)
        {
            if (x < 0 || x >= DIMENSION || y < 0 || y >= DIMENSION)
            {
                return null;
            }
            return _data[(y * DIMENSION) + x];
        }

        public bool IsEmpty()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SetTile(int x, int y, ushort newValue)
        {
            if (x < 0 || x >= DIMENSION || y < 0 || y >= DIMENSION)
            {
                return false;
            }
            _data[(y * DIMENSION) + x] = newValue;
            return true;
        }

        public byte[] ConvertToSzsFormat()
        {
            var chunkedLayer = new ushort[_data.Length]; for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    chunkedLayer[(y * 16) + x] = _data[(y * DIMENSION) + x];
                    chunkedLayer[0x100 + (y * 16) + x] = _data[(y * DIMENSION) + x + 16];
                    chunkedLayer[0x200 + (y * 16) + x] = _data[((y + 16) * DIMENSION) + x];
                    chunkedLayer[0x300 + (y * 16) + x] = _data[((y + 16) * DIMENSION) + x + 16];
                }
            }

            byte[] szsFormatLayer = new byte[_data.Length * 2];
            for (int i = 0; i < _data.Length; i++)
            {
                //Put the ushorts back into reverse order bytes
                szsFormatLayer[i * 2] = (byte)(chunkedLayer[i] & 0xFF);
                szsFormatLayer[i * 2 + 1] = (byte)(chunkedLayer[i] >> 8);
            }

            return szsFormatLayer;
        }
    }
}
