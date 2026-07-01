using System.IO;

namespace FSALib
{
    /// <summary>
    /// Defines methods for objects that can self-serialize and deserialize their state using binary streams.
    /// </summary>
    public interface IStreamSerializable
    {
        /// <summary>
        /// Reads binary data from the specified stream and updates the current object's state.
        /// </summary>
        /// <param name="source">The source stream to read the data from.</param>
        void ReadFromStream(Stream source);

        /// <summary>
        /// Writes the current object's state as binary data into the specified stream.
        /// </summary>
        /// <param name="dest">The destination stream where the data will be written.</param>
        void WriteToStream(Stream dest);
    }
}