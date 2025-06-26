using AuroraLib.Core.Format.Identifier;
using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FSALib.Structs
{
    /// <summary>
    /// Represents an actor in the FSA game world.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 11)]
    public struct Actor : IComparable<Actor>
    {
        public static Actor Null => new Actor() { ID = new Identifier32(0x20, 0x20, 0x20, 0x20) };

        private static readonly Regex regex = new Regex(@"^(.{4}), L:(\d+), X:(\d+), Y:(\d+), V:(\d+),(\d+),(\d+),(\d+)");

        /// <summary>
        /// The identifier for the actor type.
        /// </summary>
        [FieldOffset(0)]
        public Identifier32 ID;

        /// <summary>
        /// The layer on which the actor is placed.
        /// </summary>
        [FieldOffset(4)]
        public byte Layer;

        /// <summary>
        /// The X-coordinate of the actor in the game world, measured in half-tiles.
        /// </summary>
        [FieldOffset(5)]
        public byte XCoord;

        /// <summary>
        /// The Y-coordinate of the actor in the game world, measured in half-tiles.
        /// </summary>
        [FieldOffset(6)]
        public byte YCoord;

        [FieldOffset(7)]
        private uint variable;

        /// <summary>
        /// Additional variable data associated with the actor, used for customization or behavior.
        /// </summary>
        public uint Variable
        {
            readonly get => BinaryPrimitives.ReverseEndianness(variable);
            set => variable = BinaryPrimitives.ReverseEndianness(value);
        }

        public byte VariableByte1
        {
            readonly get => (byte)(variable >> 24);
            set => variable = variable & 0x00FFFFFF | (uint)value << 24;
        }

        public byte VariableByte2
        {
            readonly get => (byte)(variable >> 16);
            set => variable = variable & 0xFF00FFFF | (uint)value << 16;
        }

        public byte VariableByte3
        {
            readonly get => (byte)(variable >> 8);
            set => variable = variable & 0xFFFF00FF | (uint)value << 8;
        }

        public byte VariableByte4
        {
            readonly get => (byte)variable;
            set => variable = variable & 0xFFFFFF00 | value;
        }

        public string Name { get => ID.ToString(); set => ID = new Identifier32(value.AsSpan()); }

        /// <inheritdoc/>
        public int CompareTo(Actor other)
        {
            int result;
            if ((result = Layer.CompareTo(other.Layer)) != 0) return result;
            if ((result = XCoord.CompareTo(other.XCoord)) != 0) return result;
            if ((result = YCoord.CompareTo(other.YCoord)) != 0) return result;
            if ((result = ID.CompareTo(other.ID)) != 0) return result;
            return variable.CompareTo(other.variable);
        }

        public Actor(Identifier32 iD, byte layer, byte xCoord, byte yCoord, uint variable)
        {
            ID = iD;
            Layer = layer;
            XCoord = xCoord;
            YCoord = yCoord;
            this.variable = BinaryPrimitives.ReverseEndianness(variable);
        }

        /// <summary>
        /// Determines if the given <see cref="string"/> represents a valid <see cref="Actor"/>.
        /// </summary>
        /// <param name="actor">The string to check against the regex pattern for a valid actor format.</param>
        /// <returns>True if the string matches the actor format, otherwise false.</returns>
        public static bool IsStringActor(string actor)
            => regex.Match(actor).Success;

        /// <summary>
        /// Parses a <see cref="string"/> into an <see cref="Actor"/> if the string matches the expected format.
        /// </summary>
        /// <param name="actor">The string representing the actor, expected to contain ID, Layer, Coordinates, and Variables.</param>
        /// <param name="newActor">The Actor that will be populated with the parsed data.</param>
        /// <returns>True if the string successfully parses into an Actor, otherwise false.</returns>
        public static bool PasteFromString(string actor, out Actor newActor)
        {
            newActor = default;
            var match = regex.Match(actor);
            if (!match.Success)
            {
                return false;
            }
            newActor.ID = new Identifier32(match.Groups[1].Value.AsSpan());
            newActor.Layer = byte.Parse(match.Groups[2].Value);
            newActor.XCoord = byte.Parse(match.Groups[3].Value);
            newActor.YCoord = byte.Parse(match.Groups[4].Value);
            newActor.VariableByte4 = byte.Parse(match.Groups[5].Value);
            newActor.VariableByte3 = byte.Parse(match.Groups[6].Value);
            newActor.VariableByte2 = byte.Parse(match.Groups[7].Value);
            newActor.VariableByte1 = byte.Parse(match.Groups[8].Value);
            return true;
        }

        /// <inheritdoc/>
        public readonly string ToStringCode()
            => $"{ID}, L:{Layer}, X:{XCoord}, Y:{YCoord}, V:{VariableByte4},{VariableByte3},{VariableByte2},{VariableByte1}.";

        /// <inheritdoc/>
        public readonly override string ToString()
            => ID.ToString();

        /// <inheritdoc/>
        public override readonly bool Equals(object? obj)
            => obj is Actor actor && ID.Equals(actor.ID) && Layer == actor.Layer && XCoord == actor.XCoord && YCoord == actor.YCoord && variable == actor.variable;

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            HashCode hashCode = new HashCode();
            hashCode.Add(ID);
            hashCode.Add(Layer);
            hashCode.Add(XCoord);
            hashCode.Add(YCoord);
            hashCode.Add(Variable);
            return hashCode.ToHashCode();
#else
            int hashCode = 414193188;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + Layer.GetHashCode();
            hashCode = hashCode * -1521134295 + XCoord.GetHashCode();
            hashCode = hashCode * -1521134295 + YCoord.GetHashCode();
            hashCode = hashCode * -1521134295 + variable.GetHashCode();
            return hashCode;
#endif
        }
    }
}
