using AuroraLib.Core;
using AuroraLib.Core.Exceptions;
using AuroraLib.Core.Format.Identifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FSALib
{
    public sealed partial class Rarc
    {
        /// <summary>
        /// Represents a directory entry within a RARC archive.
        /// Contains child directories and file entries.
        /// </summary>
        [DebuggerDisplay("{Name}[{Count}]")]
        public sealed class DirectoryNode : IArchiveEntry, ICloneable<DirectoryNode>
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

            /// <summary>
            /// Gets the total size of all files contained in this directory and its subdirectories.
            /// </summary>
            public uint Length => (uint)(Directorys.Sum(d => d.Value.Length) + Files.Sum(f => f.Length));

            /// <summary>
            /// Gets the number of direct child entries contained in this directory.
            /// </summary>
            public int Count => Directorys.Count + Files.Count;

            /// <summary>
            /// Gets the child directories indexed by their RARC directory identifier.
            /// </summary>
            public Dictionary<Identifier32, DirectoryNode> Directorys { get; }

            /// <summary>
            /// Gets the files directly contained in this directory.
            /// </summary>
            public List<FileNode> Files { get; }

            /// <summary>
            /// Initializes a new empty directory entry with the specified name.
            /// </summary>
            /// <param name="name">The name of the directory.</param>
            public DirectoryNode(string name)
            {
                Name = name;
                Directorys = new Dictionary<Identifier32, DirectoryNode>();
                Files = new List<FileNode>();
            }

            /// <summary>
            /// Initializes a directory entry by loading its contents from a directory on disk.
            /// Automatically detects and loads all child directories and files recursively.
            /// </summary>
            /// <param name="directory">The source directory information.</param>
            public DirectoryNode(DirectoryInfo directory) : this(directory.Name)
            {
                foreach (DirectoryInfo sub in directory.EnumerateDirectories())
                {
                    Directorys.Add(ToFourCC(sub.Name), new DirectoryNode(sub));
                }
                foreach (FileInfo file in directory.EnumerateFiles())
                {
                    Files.Add(new FileNode(file));
                }
            }

            private static Identifier32 ToFourCC(string value)
            {
                Span<char> buffer = stackalloc char[4];
                buffer.Fill(' ');

                ReadOnlySpan<char> source = value.AsSpan(0, Math.Min(value.Length, 4));
                MemoryExtensions.ToUpperInvariant(source, buffer);

                return new Identifier32(buffer);
            }

            /// <summary>
            /// Attempts to retrieve a file with the specified name.
            /// </summary>
            /// <param name="name">The name of the file to locate.</param>
            /// <param name="file">When this method returns, contains the matching <see cref="FileNode"/> if found; otherwise, <see langword="null"/>.
            /// </param>
            /// <returns>
            /// <see langword="true"/> if a matching file was found; otherwise, <see langword="false"/>.
            /// </returns>
            public bool TryGetFile(string name, out FileNode file)
            {
                file = Files.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
                return file != null;
            }

            /// <summary>
            /// Gets a file with the specified name using the given file mode.
            /// Creates the file if required by the specified mode.
            /// </summary>
            /// <param name="name">The name of the file.</param>
            /// <param name="mode">The file creation and access mode.</param>
            public FileNode GetFile(string name, FileMode mode = FileMode.Open)
            {
                bool exists = TryGetFile(name, out FileNode file);
                switch (mode)
                {
                    case FileMode.CreateNew:
                        if (exists)
                            throw new InvalidOperationException($"The file '{name}' already exists.");

                        goto case FileMode.Create;
                    case FileMode.Create:
                    case FileMode.Truncate:
                        if (!exists)
                        {
                            file = new FileNode(name, new MemoryStream());
                            Files.Add(file);
                        }
                        else
                        {
                            file.Data.SetLength(0);
                        }
                        break;
                    case FileMode.Open:
                        if (!exists)
                            throw new InvalidOperationException($"The file '{name}' does not exist.");
                        break;
                    case FileMode.OpenOrCreate:
                    case FileMode.Append: // Does not work in the current setup
                        if (!exists)
                            goto case FileMode.Create;
                        break;
                }
                return file!;
            }

            /// <summary>
            /// Removes the specified child directory from this directory and disposes all of its contents.
            /// </summary>
            /// <param name="key">The identifier of the directory to remove.</param>
            public void DeleteAndDisposeDirectory(Identifier32 key)
            {
                if (Directorys.TryGetValue(key, out DirectoryNode directory))
                {
                    directory.Dispose();
                    Directorys.Remove(key);
                }
            }

            /// <summary>
            /// Removes the specified file from this directory and disposes its associated data stream.
            /// </summary>
            /// <param name="name">The name of the file to remove.</param>
            public void DeleteAndDisposeFile(string name)
            {
                if (TryGetFile(name, out FileNode file))
                {
                    file.Dispose();
                    Files.Remove(file);
                }
            }

            /// <summary>
            /// Removes empty files and recursively removes empty directories from this directory.
            /// </summary>
            public void RemoveEmptyEntries()
            {
                for (int i = Files.Count - 1; i >= 0; i--)
                {
                    if (Files[i].Length == 0)
                    {
                        Files[i].Dispose();
                        Files.RemoveAt(i);
                    }
                }

                foreach (var item in Directorys.ToList())
                {
                    item.Value.RemoveEmptyEntries();
                    if (item.Value.Count == 0)
                        DeleteAndDisposeDirectory(item.Key);
                }
            }

            /// <summary>
            /// Gets an existing child directory by identifier or creates a new one.
            /// </summary>
            /// <param name="key">The four-character directory identifier.</param>
            /// <returns>The existing or newly created directory.</returns>
            public DirectoryNode CreateDirectory(Identifier32 key)
            {
                if (!Directorys.TryGetValue(key, out DirectoryNode? directory))
                {
                    directory = new DirectoryNode(((string)key).ToLower());
                    Directorys.Add(key, directory);
                }

                return directory;
            }

            /// <summary>
            /// Writes the directory and all contained files and subdirectories to disk.
            /// </summary>
            /// <param name="path">The destination directory path.</param>
            public void WriteToDirectory(string path)
            {
                DirectoryInfo directory = Directory.CreateDirectory(path);

                foreach (FileNode file in Files)
                {
                    string filePath = Path.Combine(directory.FullName, file.Name);
                    file.WriteToFile(filePath);
                }

                foreach (DirectoryNode subDirectory in Directorys.Values)
                {
                    string directoryPath = Path.Combine(directory.FullName, subDirectory.Name);
                    subDirectory.WriteToDirectory(directoryPath);
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                foreach (var sub in Directorys)
                    sub.Value.Dispose();
                foreach (var file in Files)
                    file.Dispose();
            }


            /// <inheritdoc/>
            public DirectoryNode Clone()
            {
                var clone = new DirectoryNode(Name);
                foreach (var sub in Directorys)
                    clone.Directorys.Add(sub.Key, sub.Value.Clone());

                foreach (var file in Files)
                    clone.Files.Add(file.Clone());

                return clone;
            }

            object ICloneable.Clone() => Clone();
        }
    }
}
