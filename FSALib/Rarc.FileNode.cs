using AuroraLib.Compression;
using AuroraLib.Core;
using AuroraLib.Core.Exceptions;
using AuroraLib.Core.IO;
using System.Diagnostics;
using System.IO;

namespace FSALib
{
    public sealed partial class Rarc
    {
        /// <summary>
        /// Represents a file entry within a RARC archive.
        /// </summary>
        [DebuggerDisplay("{Name}")]
        public sealed class FileNode : IArchiveEntry, ICloneable<FileNode>
        {
            /// <inheritdoc/>
            public string Name
            {
                get => name;
                set
                {
                    ThrowIf.Null(value);
                    name = value;
                }
            }
            private string name;

            /// <inheritdoc/>
            public uint Length => (uint)Data.Length;

            /// <summary>
            /// File attributes describing the storage format and compression state.
            /// </summary>
            public FileAttribute Attribute { get; set; }

            /// <summary>
            /// Stream containing the file data.
            /// Compressed data is automatically decompressed when loaded from disk.
            /// </summary>
            public Stream Data
            {
                get
                {
                    data.Position = 0;
                    return data;
                }
                set
                {
                    ThrowIf.Null(value);
                    ThrowIf.Disposed(!value.CanRead, value);
                    data = value;
                }
            }
            private Stream data;

            /// <summary>
            /// Initializes a new file entry with the specified name, data stream, and attributes.
            /// </summary>
            /// <param name="name">The name of the file.</param>
            /// <param name="data">The stream containing the file data.</param>
            /// <param name="attribute">The file attributes.</param>
            public FileNode(string name, Stream data, FileAttribute attribute = FileAttribute.PRELOAD_TO_MRAM)
            {
                Name = name;
                Attribute = attribute;
                Data = data;
            }

            /// <summary>
            /// Initializes a new file entry from a file on disk.
            /// </summary>
            /// <param name="fileInfo">The file information of the source file.</param>
            public FileNode(FileInfo fileInfo) : this(fileInfo.Name, fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (Common.Yaz0.IsMatch(Data))
                {
                    Attribute |= FileAttribute.YAZ0_COMPRESSED;
                    var decode = Common.Yaz0.Decompress(Data);
                    Data.Dispose();
                    Data = decode;
                }
                else if (Common.Yay0.IsMatch(Data))
                {
                    Attribute |= FileAttribute.YAY0_COMPRESSED;
                    var decode = Common.Yay0.Decompress(Data);
                    Data.Dispose();
                    Data = decode;
                }
            }

            /// <summary>
            /// Writes the file data to the specified path.
            /// </summary>
            /// <param name="path">The destination file path.</param>
            public void WriteToFile(string path)
            {
                using FileStream output = File.Create(path);
                EncodeTo(output);
            }

            internal void EncodeTo(Stream dest)
            {
                var source = Data;

                if (Common.Yaz0.IsMatch(source))
                {
                    Attribute |= FileAttribute.YAZ0_COMPRESSED;
                    source.CopyTo(dest);
                }
                else if (Common.Yay0.IsMatch(source))
                {
                    Attribute |= FileAttribute.YAY0_COMPRESSED;
                    source.CopyTo(dest);
                }
                else if (Attribute.HasFlag(FileAttribute.YAZ0_COMPRESSED))
                {
                    Common.Yaz0.Compress(source, dest, CompressionSettings.Maximum);
                }
                else if (Attribute.HasFlag(FileAttribute.YAY0_COMPRESSED))
                {
                    Common.Yay0.Compress(source, dest, CompressionSettings.Maximum);
                }
                else
                {
                    source.CopyTo(dest);
                }
                source.Seek(0, SeekOrigin.Begin);
            }

            /// <inheritdoc/>
            public void Dispose() => Data.Dispose();

            object System.ICloneable.Clone() => Clone();

            /// <inheritdoc/>
            public FileNode Clone() => new FileNode(name, new MemoryPoolStream(data), Attribute);
        }
    }
}
