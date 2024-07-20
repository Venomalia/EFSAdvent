using System;
using System.IO;
using System.Text;

namespace EFSAdvent
{
    class RarcPacker
    {
        struct RarcHeader
        {
            public string type; //'RARC'
            public uint size; //size of the .arc file
            public uint unknown1; //0x00000020 Either this or Unknown6 is the offset to the NodeTable, and the other is how much should be added to all the offsets?
            public uint dataStartOffset; //Where the actual data starts. You have to add 0x20 to this value.

            public uint sizeOfDataTable1;
            public uint sizeOfDataTable2;
            public uint unknown4;
            public uint unknown5;

            public uint numNodes;
            public uint unknown6; //0x00000020
            public uint numFiles1; //Total number of file entries
            public uint fileEntriesOffset; //Have to add 0x20

            public uint sizeOfStringTable; //Size of StringTable after it has been padded out so that the data table begins at an address ending in 0
            public uint stringTableOffset; //Where is the string table stored? You have to add 0x20 to this value.
            public ushort numFiles2; //Number of file entries
            public ushort unknown8;
            public uint unknown9;
        }

        struct Node
        {
            public string type; //First four letters of the folder name, in CAPS
            public uint filenameOffset; //directory name, offset into string table
            public ushort foldernameHash; //A hash of the foldername
            public ushort numFileEntries; //how many files belong to this node
            public uint firstFileEntryOffset; //Number of the first file entry that belongs to this node, Zero-based
        }

        struct FileEntry
        {
            public ushort id; //file id. If this is 0xFFFF, then this entry is a subdirectory link
            public ushort filenameHash; //A hash of the file/foldername
            public ushort unknown2; //If folder 0x0200, if file 0x1100.    This should be a byte and filenameOffset 3 bytes? 
            public ushort filenameOffset; //file/subdir name, offset into string table
            public uint dataOffset; //offset to file data (for subdirs: index of Node representing the subdir)
            public uint dataSize; //size of data
            public uint zero; //seems to be always '0'

            //unknown2
            //0x0200 = Folder
            //0x9500 = .szs layer data
        };

        private static string stringTable;
        public static uint numNodesDone;     //How many nodes have been filled out
        public static uint numFilesWithData; //How many files with data have been added
        public static uint lengthOfDataTable;//How much data has been added

        static FileEntry[] fileEntries;
        static Node[] nodes;
        static string[] filesData;

        public static int totalNumFilesAdded;


        public void CreateRarc(string sourcePath, string saveDestination)
        {
            stringTable = CreateStringTable();//Setup the string table

            //Get all directories and sub-ones in an array and create an appropriately sized Node array
            string[] allDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            nodes = new Node[allDirectories.Length + 1]; //Add 1 for the ROOT node

            numNodesDone = 0;
            numFilesWithData = 0;
            lengthOfDataTable = 0;

            //Fill out the ROOT node
            nodes[0].type = "ROOT";

            nodes[0].filenameOffset = (uint)stringTable.Length;
            String rootDirName = new FileInfo(sourcePath).Name;
            stringTable = stringTable + rootDirName + (char)0x00;

            nodes[0].foldernameHash = Hash(rootDirName);

            string[] files = Directory.GetFileSystemEntries(sourcePath);
            nodes[0].numFileEntries = (ushort)(files.Length + 2);

            nodes[0].firstFileEntryOffset = 0;

            numNodesDone++; //One node is complete


            //Get the total number of subdirectories and files
            string[] allFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            int numOfFilesAndDirs = allFiles.Length + allDirectories.Length;
            //Now set up an array of FileEntrys(Taking into account the "." and ".." file entries for each folder
            fileEntries = new FileEntry[numOfFilesAndDirs + (allDirectories.Length * 2) + 2];

            filesData = new string[allFiles.Length]; //Setup an array to store all the file data paths in
            totalNumFilesAdded = 0; //How many file entries have been done

            CreateNodesForFolders(sourcePath);

            //Fill out the filename & data offsets for the folder entries with the offset from the appropriate Node
            for (int n = 0; n < totalNumFilesAdded; n++)
            {
                if (fileEntries[n].filenameOffset == 0xFFFE)
                {
                    uint nodeNum = 0;
                    foreach (Node node in nodes)
                    {
                        if (node.foldernameHash == fileEntries[n].filenameHash)
                        {
                            fileEntries[n].filenameOffset = (ushort)node.filenameOffset;
                            fileEntries[n].dataOffset = nodeNum;
                        }
                        nodeNum++;
                    }
                }
            }

            //Make the data table a mutiple of 16
            int numOfPaddingBytes = 0;
            while ((lengthOfDataTable % 16) != 0)
            {
                lengthOfDataTable++;
                numOfPaddingBytes++;
            }

            //Fill out Header information
            RarcHeader header = new RarcHeader
            {
                type = "RARC",
                numFiles1 = (uint)totalNumFilesAdded,
                numFiles2 = (ushort)totalNumFilesAdded,
                sizeOfDataTable1 = lengthOfDataTable,
                sizeOfDataTable2 = lengthOfDataTable,
                unknown1 = 0x20,
                unknown6 = 0x20,
                unknown8 = 0x100,
                fileEntriesOffset = (numNodesDone * 16) + 0x20
            };
            if ((header.fileEntriesOffset % 32) != 0)//Check if it's a multiple of 32 and make it one if it's not
            {
                header.fileEntriesOffset += 16;
            }

            header.numNodes = numNodesDone;

            int x = 0;
            while (0 != ((totalNumFilesAdded * 20) + x) % 16)
            {
                x++;
            }
            header.stringTableOffset = header.fileEntriesOffset + (uint)((totalNumFilesAdded * 20) + x);
            if ((header.stringTableOffset % 32) != 0)//Check if it's a multiple of 32 and make it one if it's not
            {
                header.stringTableOffset += 16;
            }

            while (0 != (stringTable.Length) % 16)//Pad out the string table so the data table starts at a 0based address
            {
                stringTable = stringTable + (char)0x00;
            }
            header.dataStartOffset = (uint)(header.stringTableOffset + stringTable.Length);
            if ((header.dataStartOffset % 32) != 0)//Check if it's a multiple of 32 and make it one if it's not
            {
                header.dataStartOffset += 16;
            }

            header.sizeOfStringTable = (uint)stringTable.Length;
            header.size = lengthOfDataTable + header.dataStartOffset + 0x20;

            //Let's write it out
            FileStream filestreamWriter = new FileStream(saveDestination, FileMode.Create);
            BinaryWriter binWriter = new BinaryWriter(filestreamWriter);
            //First the Header is written
            binWriter.Write(header.type[0]);
            binWriter.Write(header.type[1]);
            binWriter.Write(header.type[2]);
            binWriter.Write(header.type[3]);
            byte[] buffer = BitConverter.GetBytes(header.size);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.unknown1);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.dataStartOffset);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(header.sizeOfDataTable1);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.sizeOfDataTable2);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.unknown4);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.unknown5);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(header.numNodes);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.unknown6);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.numFiles1);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.fileEntriesOffset);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(header.sizeOfStringTable);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.stringTableOffset);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.numFiles2);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.unknown8);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);
            buffer = BitConverter.GetBytes(header.unknown9);
            Array.Reverse(buffer);
            filestreamWriter.Write(buffer, 0, buffer.Length);

            foreach (Node currentNode in nodes)
            {
                binWriter.Write(currentNode.type[0]);
                if (currentNode.type.Length > 1)       //Incase the dirname is only 1 char long
                    binWriter.Write(currentNode.type[1]);
                else
                    filestreamWriter.WriteByte(0x20);
                if (currentNode.type.Length > 2)       //Incase the dirname is only 2 char long
                    binWriter.Write(currentNode.type[2]);
                else
                    filestreamWriter.WriteByte(0x20);
                if (currentNode.type.Length == 4)       //Incase the dirname is only 3 char long
                    binWriter.Write(currentNode.type[3]);
                else
                    filestreamWriter.WriteByte(0x20);

                buffer = BitConverter.GetBytes(currentNode.filenameOffset);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(currentNode.foldernameHash);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(currentNode.numFileEntries);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(currentNode.firstFileEntryOffset);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
            }

            //Pad out the file to get the file entries at their correct offset
            while (filestreamWriter.Position != (header.fileEntriesOffset + 32))
            {
                filestreamWriter.WriteByte(0x00);
            }

            //Write all the file entries
            foreach (FileEntry entry in fileEntries)
            {
                buffer = BitConverter.GetBytes(entry.id);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(entry.filenameHash);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(entry.unknown2);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(entry.filenameOffset);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(entry.dataOffset);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(entry.dataSize);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                buffer = BitConverter.GetBytes(entry.zero);
                Array.Reverse(buffer);
                filestreamWriter.Write(buffer, 0, buffer.Length);
            }

            //Pad out the file to get the string table at its correct offset
            while (filestreamWriter.Position != (header.stringTableOffset + 32))
            {
                filestreamWriter.WriteByte(0x00);
            }

            //Write string table
            Encoding enc = Encoding.UTF8;
            binWriter.Write(enc.GetBytes(stringTable));

            //Pad out the file to get the data table at its correct offset
            while (filestreamWriter.Position != (header.dataStartOffset + 32))
            {
                filestreamWriter.WriteByte(0x00);
            }

            //Write files data
            foreach (string file in filesData)
            {
                buffer = File.ReadAllBytes(file);
                filestreamWriter.Write(buffer, 0, buffer.Length);
                //Pad out the data so the next file begins on a 0-based multiple of 32
                while ((filestreamWriter.Position % 32) != 0)
                {
                    filestreamWriter.WriteByte(0x00);
                }
            }
            //Write the bytes neccessary to make the entire file divisble by 32
            for (int pad = 0; pad < numOfPaddingBytes; pad++)
            {
                filestreamWriter.WriteByte(0x00);
            }

            binWriter.Close();
            filestreamWriter.Close();

            Console.WriteLine("Packed and good to go!");
        }

        static string CreateStringTable()
        {
            char spacer = (char)0x00;
            char fullStop = '.';
            char[] stringTableHeader = new char[5];
            stringTableHeader[0] = fullStop;
            stringTableHeader[1] = spacer;
            stringTableHeader[2] = fullStop;
            stringTableHeader[3] = fullStop;
            stringTableHeader[4] = spacer;
            //Load the string table with the "Header"
            String stringTable = new string(stringTableHeader);

            return stringTable;
        }

        static void CreateNodesForFolders(string path)
        {
            string[] subFolders = ProcessFilesAndFolders(path);
            foreach (string foldr in subFolders)
            {
                CreateNode(foldr);
                CreateNodesForFolders(foldr);
            }
        }

        static string[] ProcessFilesAndFolders(string sourcePath)
        {
            string[] files = Directory.GetFiles(sourcePath, "*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                CreateFileEntries(files);
            }

            string[] folders = Directory.GetDirectories(sourcePath, "*", SearchOption.TopDirectoryOnly);
            if (folders.Length > 0)
            {
                CreateFolderEntries(folders);
            }

            CreateDummyFiles();
            return folders;
        }

        static void CreateFileEntries(string[] entries)
        {
            foreach (string entry in entries)
            {
                fileEntries[totalNumFilesAdded].id = (ushort)totalNumFilesAdded;

                if (Path.GetExtension(entry) == ".szs")//Check if szs file and use right.. marker?
                    fileEntries[totalNumFilesAdded].unknown2 = 0x9500;
                else
                    fileEntries[totalNumFilesAdded].unknown2 = 0x1100;

                fileEntries[totalNumFilesAdded].filenameOffset = (ushort)stringTable.Length;
                string fileName = new FileInfo(entry).Name;
                stringTable = stringTable + fileName + (char)0x00;
                fileEntries[totalNumFilesAdded].filenameHash = Hash(fileName);

                fileEntries[totalNumFilesAdded].dataOffset = lengthOfDataTable;
                lengthOfDataTable += (uint)new FileInfo(entry).Length;
                //Pad the data table out so new files start at a 0-based address
                while ((lengthOfDataTable % 16) != 0)
                    lengthOfDataTable++;
                if ((lengthOfDataTable % 32) != 0)//Check the new address is a multiple of 32 and add 16 if not
                    lengthOfDataTable += 16;
                filesData[numFilesWithData] = entry;

                fileEntries[totalNumFilesAdded].dataSize = (uint)new FileInfo(entry).Length;

                numFilesWithData++;
                totalNumFilesAdded++;
            }
        }

        static void CreateFolderEntries(string[] entries)
        {
            foreach (string entry in entries)
            {
                fileEntries[totalNumFilesAdded].id = 0xFFFF;

                fileEntries[totalNumFilesAdded].unknown2 = 0x0200;

                fileEntries[totalNumFilesAdded].filenameOffset = (ushort)0xFFFE;
                string fileName = new FileInfo(entry).Name;
                fileEntries[totalNumFilesAdded].filenameHash = Hash(fileName);

                fileEntries[totalNumFilesAdded].dataOffset = (uint)0xFFFE;

                fileEntries[totalNumFilesAdded].dataSize = 0x10;
                totalNumFilesAdded++;
            }
        }

        static void CreateDummyFiles()
        {
            //Add in the "Dummy" folder entries
            for (int i = 0; i < 2; i++)
            {
                fileEntries[totalNumFilesAdded].id = 0xFFFF;
                fileEntries[totalNumFilesAdded].unknown2 = 0x0200;
                if (i == 0)
                {
                    fileEntries[totalNumFilesAdded].filenameOffset = 0;
                    fileEntries[totalNumFilesAdded].filenameHash = Hash(Convert.ToString(stringTable[0]));
                    fileEntries[totalNumFilesAdded].dataOffset = (uint)(numNodesDone - 1);
                }
                else
                {
                    fileEntries[totalNumFilesAdded].filenameOffset = 0x0002;
                    string name = "..";
                    fileEntries[totalNumFilesAdded].filenameHash = Hash(name);
                    if (numNodesDone == 1)
                        fileEntries[totalNumFilesAdded].dataOffset = 0xFFFFFFFF;
                    else
                        fileEntries[totalNumFilesAdded].dataOffset = 0;
                }
                fileEntries[totalNumFilesAdded].dataSize = 0x10;

                totalNumFilesAdded++;
            }
        }

        static void CreateNode(string path)
        {
            string dirName = new FileInfo(path).Name;
            dirName = dirName.ToUpper();
            for (int c = 0; c < dirName.Length; c++)
            {
                if (c == 4)
                {
                    break;
                }
                nodes[numNodesDone].type = nodes[numNodesDone].type + dirName[c];

            }
            nodes[numNodesDone].filenameOffset = (uint)stringTable.Length;
            stringTable = stringTable + dirName.ToLower() + (char)0x00;
            string[] numFiles = Directory.GetFileSystemEntries(path);
            nodes[numNodesDone].numFileEntries = (ushort)(numFiles.Length + 2);
            nodes[numNodesDone].firstFileEntryOffset = (uint)totalNumFilesAdded;

            dirName = new FileInfo(path).Name;
            nodes[numNodesDone].foldernameHash = Hash(dirName);

            numNodesDone++;
        }

        static ushort Hash(string filename)
        {
            ushort hash = 0;
            int multiplier = 1;
            byte currentChar;

            for (int i = (filename.Length - 1); i >= 0; i--)
            {
                currentChar = Convert.ToByte(filename[i]);
                hash += (ushort)(currentChar * multiplier);
                multiplier = (multiplier * 3);
            }
            return hash;
            //The hash of filename "xyz" = (z*M)+(y*M)+(x*M) etc...
            //M = 1 to start with, after each multipication M = (M*3)
        }
    }
}
