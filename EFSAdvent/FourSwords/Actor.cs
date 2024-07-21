using System;

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

        public void SetCoordinates(int x, int y)
        {
            if (x < 0 || x > MAX_COORDINATE || y < 0 || y > MAX_COORDINATE)
            {
                return;
            }
            XCoord = (byte)x;
            YCoord = (byte)y;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
