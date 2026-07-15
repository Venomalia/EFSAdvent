using AuroraLib.Compression;
using AuroraLib.Core;
using AuroraLib.Core.Exceptions;
using AuroraLib.Core.Format;
using AuroraLib.Core.Format.Identifier;
using AuroraLib.Core.IO;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FSALib
{

    /// <summary>
    /// Nintendo RARC Archive
    /// </summary>
    public sealed partial class Rarc : IStreamSerializable, IDisposable, IFormatRecognition, IFormatInfoProvider
    {
        private readonly static Identifier32 Identifier = new Identifier32("RARC".AsSpan());
        private static readonly IFormatInfo _info = new FormatInfo<Rarc>("Nintendo Rarc Archive", new MediaType(MIMEType.Application, "x-nintendo-rarc"), ".arc", Identifier);
        private const string SelfKey = ".";
        private const string ParentKey = "..";

        /// <summary>
        /// Gets the root directory entry of the RARC archive.
        /// </summary>
        public DirectoryNode Root { get; private set; }

        /// <inheritdoc/>
        public IFormatInfo Info => _info;

        public static class CommonFolderTypes
        {
            /// <summary>
            /// Root node of the RARC archive.
            /// </summary>
            public readonly static Identifier32 Root = new Identifier32(0x544F4F52);

            /// <summary>
            /// image datein
            /// </summary>
            public readonly static Identifier32 Timg = new Identifier32(0x474D4954);

            /// <summary>
            /// Screen and layout resources (e.g. BLO, BRK).
            /// </summary>
            public readonly static Identifier32 SCRN = new Identifier32(0x4E524353);

            /// <summary>
            /// J3D model and animation resources (e.g. BMD, BDL, BTK, BCK, BRK).
            /// </summary>
            public readonly static Identifier32 J3D = new Identifier32(0x2044334A);

            /// <summary>
            /// Audio resources.
            /// </summary>
            public readonly static Identifier32 AUDI = new Identifier32(0x49445541);

            /// <summary>
            /// FSA Level <see cref="MapProperties"/> data.
            /// </summary>
            public readonly static Identifier32 Map = new Identifier32(0x2050414D);

            /// <summary>
            /// FSA <see cref="ActorList"/> data.
            /// </summary>
            public readonly static Identifier32 BIN = new Identifier32(0x204E4942);

            /// <summary>
            /// FSA Tile <see cref="Layer"/> data.
            /// </summary>
            public readonly static Identifier32 SZS = new Identifier32(0x20535A53);

            /// <summary>
            /// FSA 4bpp sprite and tileset graphics.
            /// Used by <see cref="Renderer.SpriteRenderer{TColor}"/> and <see cref="Renderer.TilesetRenderer{TColor}"/>.
            /// </summary>
            public readonly static Identifier32 SCH = new Identifier32(0x20484353);

            /// <summary>
            /// FSA Palette data stored in the Game Boy Advance BGR555 format.
            /// </summary>
            public readonly static Identifier32 SCL = new Identifier32(0x204C4353);

            /// <summary>
            /// FSA <see cref="Structs.SpriteAttribute"/> data.
            /// </summary>
            public readonly static Identifier32 SOB = new Identifier32(0x20424F53);

            /// <summary>
            /// FSA Tileset layout as <see cref="Structs.Tile"/> data. 
            /// </summary>
            public readonly static Identifier32 SPL = new Identifier32(0x204C5053);

            /// <summary>
            /// FSA Tile <see cref="Stamp"/> preset data. 
            /// </summary>
            public readonly static Identifier32 SMP = new Identifier32(0x20504D53);

            /// <summary>
            /// FSA Enemy parameter table (enemyparam.csv).
            /// </summary>
            public readonly static Identifier32 ENM = new Identifier32(0x204D4E45);
        }

        public Rarc()
            => Root = new DirectoryNode("Root");

        public Rarc(DirectoryNode rootnode)
            => Root = rootnode.Clone();
        public Rarc(DirectoryInfo directory)
            => Root = new DirectoryNode(directory);

        public Rarc(Stream source)
            => ReadFromStream(source);

        /// <inheritdoc/>
        public bool IsMatch(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default) => IsMatchStatic(stream, fileNameAndExtension);

        /// <inheritdoc/>
        public static bool IsMatchStatic(Stream stream, ReadOnlySpan<char> fileNameAndExtension = default)
            => stream.Length > 0x30 && stream.Peek(s => s.Match(Identifier));

        /// <inheritdoc/>
        public void ReadFromStream(Stream source)
        {
            Root?.Dispose();

            if (Common.Yaz0.IsMatch(source))
            {
                using MemoryPoolStream temp = Common.Yaz0.Decompress(source);
                ReadFromStream(temp);
                return;
            }

            // Header
            HeaderT header = source.Read<HeaderT>().ReverseEndianness();

            if (header.Identifier != Identifier)
                throw new InvalidIdentifierException(header.Identifier, Identifier);

            // Data Header
            source.Seek(header.DataHeaderOffset, SeekOrigin.Begin);
            DataHeaderT dataHeader = source.Read<DataHeaderT>().ReverseEndianness();

            // allocate Table data
            int directoryTabelSize = Unsafe.SizeOf<DirectoryT>() * (int)dataHeader.DirectoryCount;
            int entrieTabelSize = Unsafe.SizeOf<EntrieT>() * (int)dataHeader.EntryCount;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(directoryTabelSize + entrieTabelSize);
            try
            {
                // Directory Table
                source.Seek(dataHeader.DirectoryTableOffset + header.DataHeaderOffset, SeekOrigin.Begin);
                source.Read(buffer, 0, directoryTabelSize);
                Span<DirectoryT> directories = MemoryMarshal.Cast<byte, DirectoryT>(buffer.AsSpan(0, directoryTabelSize));

                // Entrie Table
                source.Seek(dataHeader.EntryTableOffset + header.DataHeaderOffset, SeekOrigin.Begin);
                source.Read(buffer, directoryTabelSize, entrieTabelSize);
                Span<EntrieT> files = MemoryMarshal.Cast<byte, EntrieT>(buffer.AsSpan(directoryTabelSize, entrieTabelSize));

                //Processing
                Dictionary<uint, DirectoryNode> reference = new Dictionary<uint, DirectoryNode>();
                for (int i = 0; i < directories.Length; i++)
                {
                    var dirNode = directories[i].ReverseEndianness();
                    source.Seek(dataHeader.StringTableOffset + header.DataHeaderOffset + dirNode.NameOffset, SeekOrigin.Begin);
                    string name = source.ReadCString();

                    DirectoryNode currentNode = new DirectoryNode(name);
                    if (dirNode.Type == CommonFolderTypes.Root) // is ROOT
                        Root = currentNode;

                    for (int n = (int)dirNode.FirstFileOffset; n < dirNode.FileCount + dirNode.FirstFileOffset; n++)
                    {
                        var fileNode = files[n].ReverseEndianness();
                        source.Seek(dataHeader.StringTableOffset + header.DataHeaderOffset + fileNode.NameOffset, SeekOrigin.Begin);
                        name = source.ReadCString();

                        if (fileNode.Attribute == FileAttribute.DIRECTORY) //IsDirectory
                        {
                            if (name == SelfKey) // current node index
                                reference.Add(fileNode.Offset, currentNode);
                            else if (name == ParentKey && (int)fileNode.Offset != -1) // current node parent index
                                reference[fileNode.Offset].Directorys.Add(dirNode.Type, currentNode);
                        }
                        else //IsFile
                        {
                            source.Seek(header.DataOffset + header.DataHeaderOffset + fileNode.Offset, SeekOrigin.Begin);
                            MemoryPoolStream data;
                            if (Common.Yaz0.IsMatch(source))
                            {
                                data = Common.Yaz0.Decompress(source);
                            }
                            else if (Common.Yay0.IsMatch(source))
                            {
                                data = Common.Yay0.Decompress(source);
                            }
                            else
                            {
                                data = new MemoryPoolStream(source, (int)fileNode.Size);
                            }

                            currentNode.Files.Add(new FileNode(name, data, fileNode.Attribute));
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public void WriteToStream(Stream dest, CompressionSettings settings)
        {
            using MemoryPoolStream temp = new MemoryPoolStream();
            WriteToStream(temp);
            Common.Yaz0.Compress(temp.UnsafeAsSpan(), dest, settings);
        }

        /// <inheritdoc/>
        public void WriteToStream(Stream dest)
        {
            static int GetEntrieCount(DirectoryNode directory)
            {
                // Count required entry table size (directorys + files + "." + "..")
                int entries = directory.Count + 2;
                foreach (var sub in directory.Directorys)
                {
                    entries += GetEntrieCount(sub.Value);
                }
                return entries;
            }

            // Build file data section and collect file offsets.
            // Data must be ordered by load type (MRAM, ARAM, DVD).
            Dictionary<FileNode, (uint Size, uint Offset)> dataOffsets = new Dictionary<FileNode, (uint Size, uint Offset)>();
            using MemoryPoolStream dataTabel = new MemoryPoolStream();
            BuildDataTabel(dataTabel, dataOffsets, out uint MRAMSize, out uint ARAMSize, out uint DVDSize);
            dataTabel.Seek(0, SeekOrigin.Begin);

            // Build string table.
            // RARC stores names separately and references them by offset.
            using MemoryPoolStream nameTabel = new MemoryPoolStream();
            ProcessName(nameTabel, SelfKey.AsSpan(), out _, out _);
            ProcessName(nameTabel, ParentKey.AsSpan(), out _, out _);
            ProcessName(nameTabel, Root.Name.AsSpan(), out ushort hash, out ushort offset);

            // Build directory and entry tables.
            // The entry table describes the hierarchy and references file data.
            List<DirectoryT> directorieTable = new List<DirectoryT>
            {
                new DirectoryT(CommonFolderTypes.Root, offset, hash, (ushort)(Root.Count + 2), 0).ReverseEndianness()
            };
            EntrieT[] entrieTable = new EntrieT[GetEntrieCount(Root)];

            uint totalEntries = 0;
            BuildDirAndEntrieTabel(directorieTable, entrieTable, nameTabel, dataOffsets, Root, ref totalEntries);

            // Align string table before writing.
            nameTabel.WriteAlign(0x20);
            nameTabel.Seek(0, SeekOrigin.Begin);

            // Calculate offsets of each RARC section.
            uint entryTableOffset = (uint)StreamEx.AlignPosition(directorieTable.Count * Unsafe.SizeOf<DirectoryT>(), 0x20) + 0x20;
            uint stringTableOffset = (uint)StreamEx.AlignPosition(entrieTable.Length * Unsafe.SizeOf<EntrieT>(), 0x20) + entryTableOffset;
            uint DataTableOffset = (uint)nameTabel.Length + stringTableOffset;

            // Write Header
            dest.Write(new HeaderT(Identifier, 0x20 + DataTableOffset + (uint)dataTabel.Length, 0x20, DataTableOffset, (uint)dataTabel.Length, MRAMSize, ARAMSize, DVDSize).ReverseEndianness());
            // Write Data Header
            dest.Write(new DataHeaderT((uint)directorieTable.Count, 0x20, (uint)entrieTable.Length, entryTableOffset, (uint)nameTabel.Length, stringTableOffset, (ushort)entrieTable.Length).ReverseEndianness());
            dest.WriteAlign(0x20);
            // Write Directorie Tables
            dest.WriteCollection(directorieTable);
            dest.WriteAlign(0x20);
            // Write Entrie Tables
            dest.Write<EntrieT>(entrieTable.AsSpan());
            dest.WriteAlign(0x20);
            // Write Data + Name Tables
            nameTabel.WriteTo(dest);
            dataTabel.WriteTo(dest);
        }

        private static void BuildDirAndEntrieTabel(List<DirectoryT> directorieTable, Span<EntrieT> entrieTable, Stream nameTabel, Dictionary<FileNode, (uint Size, uint Offset)> dataOffsets, DirectoryNode currentNode, ref uint firstEntrieIndex, uint parentIndex = uint.MaxValue)
        {
            // Precomputed hashes for "." and ".."
            const ushort SelfKeyHash = 0x002e, ParentKeyHash = 0x00b8;

            uint currentIndex = (uint)(directorieTable.Count - 1);
            int entrieIndex = (int)firstEntrieIndex;
            firstEntrieIndex += (ushort)(currentNode.Count + 2);

            // Add child directories first.
            foreach (var item in currentNode.Directorys)
            {
                ushort entries = (ushort)(item.Value.Count + 2);
                ProcessName(nameTabel, item.Value.Name.AsSpan(), out ushort hash, out ushort offset);
                entrieTable[entrieIndex++] = new EntrieT(hash, offset, (uint)directorieTable.Count).ReverseEndianness();
                directorieTable.Add(new DirectoryT(item.Key, offset, hash, entries, firstEntrieIndex).ReverseEndianness());

                BuildDirAndEntrieTabel(directorieTable, entrieTable, nameTabel, dataOffsets, item.Value, ref firstEntrieIndex, currentIndex);
            }

            // Add files contained in this directory.
            foreach (var node in currentNode.Files)
            {
                ProcessName(nameTabel, node.Name.AsSpan(), out ushort hash, out ushort nameOffset);
                (uint size, uint dataOffset) = dataOffsets[node];
                entrieTable[entrieIndex++] = new EntrieT((short)(entrieIndex - 1), hash, nameOffset, dataOffset, size, node.Attribute | FileAttribute.FILE).ReverseEndianness();
            }

            // Add mandatory RARC directory references.
            entrieTable[entrieIndex++] = new EntrieT(SelfKeyHash, 0, currentIndex).ReverseEndianness();
            entrieTable[entrieIndex++] = new EntrieT(ParentKeyHash, 2, parentIndex).ReverseEndianness();
        }


        private static void ProcessName(Stream nameTabel, ReadOnlySpan<char> name, out ushort hash, out ushort offset)
        {
            hash = CalcNameHash(name);
            offset = (ushort)nameTabel.Position;
            nameTabel.WriteCString(name);
        }

        private static ushort CalcNameHash(ReadOnlySpan<char> name)
        {
            uint hash = 0;
            foreach (char c in name)
            {
                hash = hash * 3 + (byte)c;
            }
            return (ushort)hash;
        }

        private void BuildDataTabel(Stream dataTabel, Dictionary<FileNode, (uint Size, uint Offset)> dataOffsets, out uint MRAMSize, out uint ARAMSize, out uint DVDSize)
        {
            //Files in DataTabel must be sorted by load type, first MRAM, then ARAM and finally DVD.
            List<FileNode> fileARAM = new List<FileNode>();
            List<FileNode> fileDVD = new List<FileNode>();

            // MRAM + sorting
            foreach (FileNode fileNode in GetAllFileNodes(Root))
            {
                if (fileNode.Attribute.HasFlag(FileAttribute.PRELOAD_TO_ARAM))
                {
                    fileARAM.Add(fileNode);
                }
                else if (fileNode.Attribute.HasFlag(FileAttribute.LOAD_FROM_DVD))
                {
                    fileDVD.Add(fileNode);
                }
                else
                {
                    fileNode.Attribute |= FileAttribute.PRELOAD_TO_MRAM;
                    WriteToDataTabel(fileNode);
                }
            }
            MRAMSize = (uint)dataTabel.Position;

            // ARAM
            foreach (FileNode fileNode in fileARAM)
                WriteToDataTabel(fileNode);
            ARAMSize = (uint)dataTabel.Position - MRAMSize;

            // DVD
            foreach (FileNode fileNode in fileDVD)
                WriteToDataTabel(fileNode);
            DVDSize = (uint)dataTabel.Position - MRAMSize - ARAMSize;

            IEnumerable<FileNode> GetAllFileNodes(DirectoryNode d) => d.Files.Concat(d.Directorys.Values.SelectMany(d => GetAllFileNodes(d)));

            void WriteToDataTabel(FileNode file)
            {
                uint offset = (uint)dataTabel.Position;
                file.EncodeTo(dataTabel);
                dataOffsets.Add(file, ((uint)(dataTabel.Position - offset), offset));
                dataTabel.WriteAlign(32);
            }
        }

        public void Dispose() => Root.Dispose();


        private readonly struct HeaderT : IReversibleEndianness<HeaderT>
        {
            public readonly Identifier32 Identifier;
            public readonly uint FileSize;
            public readonly uint DataHeaderOffset;
            public readonly uint DataOffset;
            public readonly uint DataLength;
            public readonly uint MRAMSize;
            public readonly uint ARAMSize;
            public readonly uint DVDSize;

            public HeaderT(Identifier32 identifier, uint fileSize, uint dataHeaderOffset, uint dataOffset, uint dataLength, uint mRAMSize, uint aRAMSize, uint dVDSize)
            {
                Identifier = identifier;
                FileSize = fileSize;
                DataHeaderOffset = dataHeaderOffset;
                DataOffset = dataOffset;
                DataLength = dataLength;
                MRAMSize = mRAMSize;
                ARAMSize = aRAMSize;
                DVDSize = dVDSize;
            }

            public HeaderT ReverseEndianness() => new HeaderT(Identifier, BinaryPrimitives.ReverseEndianness(FileSize), BinaryPrimitives.ReverseEndianness(DataHeaderOffset), BinaryPrimitives.ReverseEndianness(DataOffset), BinaryPrimitives.ReverseEndianness(DataLength), BinaryPrimitives.ReverseEndianness(MRAMSize), BinaryPrimitives.ReverseEndianness(ARAMSize), BinaryPrimitives.ReverseEndianness(DVDSize));
        }

        private readonly struct DataHeaderT : IReversibleEndianness<DataHeaderT>
        {
            public readonly uint DirectoryCount;
            public readonly uint DirectoryTableOffset;
            public readonly uint EntryCount;
            public readonly uint EntryTableOffset;
            public readonly uint StringTableSize;
            public readonly uint StringTableOffset;
            public readonly ushort NextFreeFileID;
            public readonly byte KeepFileIDsSynced;
            public readonly byte Padding;
            public readonly uint Padding2;

            public DataHeaderT(uint directoryCount, uint directoryTableOffset, uint entryCount, uint entryTableOffset, uint stringTableSize, uint stringTableOffset, ushort nextFreeFileID)
            {
                DirectoryCount = directoryCount;
                DirectoryTableOffset = directoryTableOffset;
                EntryCount = entryCount;
                EntryTableOffset = entryTableOffset;
                StringTableSize = stringTableSize;
                StringTableOffset = stringTableOffset;
                NextFreeFileID = nextFreeFileID;
                KeepFileIDsSynced = 1;
                Padding = 0;
                Padding2 = 0;
            }

            public DataHeaderT ReverseEndianness() => new DataHeaderT(BinaryPrimitives.ReverseEndianness(DirectoryCount), BinaryPrimitives.ReverseEndianness(DirectoryTableOffset), BinaryPrimitives.ReverseEndianness(EntryCount), BinaryPrimitives.ReverseEndianness(EntryTableOffset), BinaryPrimitives.ReverseEndianness(StringTableSize), BinaryPrimitives.ReverseEndianness(StringTableOffset), BinaryPrimitives.ReverseEndianness(NextFreeFileID));
        }

        private readonly struct DirectoryT : IReversibleEndianness<DirectoryT>
        {
            public readonly Identifier32 Type;
            public readonly uint NameOffset;
            public readonly ushort NameHash;
            public readonly ushort FileCount;
            public readonly uint FirstFileOffset;

            public DirectoryT(Identifier32 type, uint nameOffset, ushort nameHash, ushort fileCount, uint firstFileOffset)
            {
                Type = type;
                NameOffset = nameOffset;
                NameHash = nameHash;
                FileCount = fileCount;
                FirstFileOffset = firstFileOffset;
            }

            public DirectoryT ReverseEndianness()
                => new DirectoryT(Type, BinaryPrimitives.ReverseEndianness(NameOffset), BinaryPrimitives.ReverseEndianness(NameHash), BinaryPrimitives.ReverseEndianness(FileCount), BinaryPrimitives.ReverseEndianness(FirstFileOffset));
        }

        private readonly struct EntrieT
        {
            public readonly short Index;
            public readonly ushort NameHash;
            public readonly FileAttribute Attribute;
            public readonly byte Padding;
            public readonly ushort NameOffset;
            public readonly uint Offset;
            public readonly uint Size;
            public readonly uint Padding2;

            public EntrieT(ushort nameHash, ushort nameOffset, uint offset) : this(-1, nameHash, nameOffset, offset, 0x10, FileAttribute.DIRECTORY)
            { }

            public EntrieT(short iD, ushort nameHash, ushort nameOffset, uint offset, uint size, FileAttribute attribute)
            {
                Index = iD;
                NameHash = nameHash;
                NameOffset = nameOffset;
                Attribute = attribute;
                Padding = 0;
                Offset = offset;
                Size = size;
                Padding2 = 0;
            }

            public EntrieT ReverseEndianness() => new EntrieT(BinaryPrimitives.ReverseEndianness(Index), BinaryPrimitives.ReverseEndianness(NameHash), BinaryPrimitives.ReverseEndianness(NameOffset), BinaryPrimitives.ReverseEndianness(Offset), BinaryPrimitives.ReverseEndianness(Size), Attribute);
        }
    }
}
