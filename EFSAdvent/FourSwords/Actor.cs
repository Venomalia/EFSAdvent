using System;
using System.Text.RegularExpressions;

namespace EFSAdvent.FourSwords
{
    public class Actor
    {
        public const int MAX_COORDINATE = 63;

        public string Name { get; private set; }
        public byte Layer { get; private set; }
        public byte XCoord { get; private set; }
        public byte YCoord { get; private set; }
        public byte Variable1 { get; private set; }
        public byte Variable2 { get; private set; }
        public byte Variable3 { get; private set; }
        public byte Variable4 { get; private set; }

        private static readonly Regex regex = new Regex(@"^(.{4}), L:(\d+), X:(\d+), Y:(\d+), V:(\d+),(\d+),(\d+),(\d+)");

        public Guid Id = Guid.NewGuid();

        public Actor()
        {
        }

        public Actor(Actor donor)
        {
            Name = donor.Name;
            Layer = donor.Layer;
            XCoord = donor.XCoord;
            YCoord = donor.YCoord;
            Variable1 = donor.Variable1;
            Variable2 = donor.Variable2;
            Variable3 = donor.Variable3;
            Variable4 = donor.Variable4;
        }

        public Actor(string name, int layer)
        {
            Name = name;
            Layer = (byte)layer;
        }

        public Actor(string name, byte layer, byte xCoord, byte yCoord, byte variable1 = 0, byte variable2 = 0, byte variable3 = 0, byte variable4 = 0)
        {
            Name = name;
            Layer = layer;
            XCoord = xCoord;
            YCoord = yCoord;
            Variable1 = variable1;
            Variable2 = variable2;
            Variable3 = variable3;
            Variable4 = variable4;
        }

        public void Update(string name, byte layer, byte xCoord, byte yCoord, byte variable1, byte variable2, byte variable3, byte variable4)
        {
            Name = name;
            Layer = layer;
            XCoord = xCoord;
            YCoord = yCoord;
            Variable1 = variable1;
            Variable2 = variable2;
            Variable3 = variable3;
            Variable4 = variable4;
        }

        public void Update(byte layer, byte x, byte y)
        {
            Layer = layer;
            XCoord = x;
            YCoord = y;
        }

        public void SetCoordinates(int x, int y)
        {
            if (x < 0 || x > MAX_COORDINATE || y < 0 || y > MAX_COORDINATE)
            {
                return;
            }
            XCoord = (byte)x;
            YCoord = (byte)y;
        }

        public string CopyToString()
        {
            return $"{Name}, L:{Layer}, X:{XCoord}, Y:{YCoord}, V:{Variable1},{Variable2},{Variable3},{Variable4}.";
        }

        public static bool IsStringActor(string actor)
        {
            return regex.Match(actor).Success;
        }

        public bool PasteFromString(string actor)
        {
            var match = regex.Match(actor);

            if (!match.Success)
            {
                return false;
            }

            Name = match.Groups[1].Value;
            Layer = byte.Parse(match.Groups[2].Value);
            XCoord = byte.Parse(match.Groups[3].Value);
            YCoord = byte.Parse(match.Groups[4].Value);
            Variable1 = byte.Parse(match.Groups[5].Value);
            Variable2 = byte.Parse(match.Groups[6].Value);
            Variable3 = byte.Parse(match.Groups[7].Value);
            Variable4 = byte.Parse(match.Groups[8].Value);

            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
