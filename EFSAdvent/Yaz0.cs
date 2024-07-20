using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EFSAdvent
{
    public class Yaz0
    {
        public static byte[] Encode(byte[] source)
        {
            const int SEARCH_RANGE = 0x1000;
            int position = 0;
            // TODO Generalize this instead of using a fixed size of 0x800 here
            var output = new List<byte>() { 0x59, 0x61, 0x7A, 0x30, 0, 0, 0x08, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int maxLength = 0x111;

            while (position < source.Length)
            {
                byte code = 0;
                var buffer = new List<byte>();

                foreach (int codeBit in Enumerable.Range(0, 8))
                {
                    if (position >= source.Length)
                    {
                        break;
                    }

                    int foundLength = 1;
                    int found = -1;
                    (found, foundLength) = FindMatch(source, position, maxLength, SEARCH_RANGE);

                    if (foundLength > 2)
                    {
                        int delta = position - found - 1;
                        if (foundLength < 0x12)
                        {
                            buffer.Add((byte)(delta >> 8 | (foundLength - 2) << 4));
                            buffer.Add((byte)(delta & 0xFF));
                        }
                        else
                        {
                            buffer.Add((byte)(delta >> 8));
                            buffer.Add((byte)(delta & 0xFF));
                            buffer.Add((byte)((foundLength - 0x12) & 0xFF));
                        }
                        position += foundLength;
                    }
                    else
                    {
                        code |= (byte)(1 << (7 - codeBit));
                        buffer.Add(source[position++]);
                    }
                }
                output.Add(code);
                output.AddRange(buffer);
            }

            return output.ToArray();
        }

        public static byte[] Decode(byte[] source)
        {
            if (Encoding.ASCII.GetString(source.Take(4).ToArray()) != "Yaz0")
            {
                return source;
            }

            int sourcePosition = 16; // First 16 bytes are Yaz0 header
            int outputPosition = 0;
            byte[] output = new byte[2048];
            uint remainingCodeByteBits = 0;
            byte codeByte = 0;
            while (outputPosition < 2048)
            {
                //read new codebyte if the current one is used up
                if (remainingCodeByteBits == 0)
                {
                    codeByte = source[sourcePosition++];
                    remainingCodeByteBits = 8;
                }

                if ((codeByte & 0x80) != 0)
                {
                    output[outputPosition++] = source[sourcePosition++];
                }
                else
                {
                    byte byte1 = source[sourcePosition++];
                    byte byte2 = source[sourcePosition++];

                    int distance = ((byte1 & 0xF) << 8) | byte2;
                    int copyPosition = outputPosition - (distance + 1);

                    int numBytes = byte1 >> 4;
                    if (numBytes == 0)
                    {
                        numBytes = source[sourcePosition++] + 0x12;
                    }
                    else
                    {
                        numBytes += 2;
                    }

                    for (int i = 0; i < numBytes; ++i)
                    {
                        output[outputPosition++] = output[copyPosition++];
                    }
                }

                //use next bit from codebyte
                codeByte <<= 1;
                remainingCodeByteBits--;
            }

            return output;
        }

        public static bool IsYaz0(string filePath)
        {
            var fileContent = File.ReadAllBytes(filePath);
            return Encoding.ASCII.GetString(fileContent.Take(4).ToArray()) == "Yaz0";
        }

        private static (int, int) FindMatch(byte[] source, int position, int maxMatchLength, int searchRange)
        {
            int foundLength = 1;
            int found = 0;

            if (position + 2 < source.Length)
            {
                int search = position - searchRange;
                if (search < 0)
                {
                    search = 0;
                }

                int compareEnd = position + maxMatchLength;
                if (compareEnd > source.Length)
                {
                    compareEnd = source.Length;
                }

                byte c1 = source[position];
                while (search < position)
                {
                    search = Find(source, c1, search, position);
                    if (search == -1)
                    {
                        break;
                    }

                    int cmp1 = search + 1;
                    int cmp2 = position + 1;

                    while (cmp2 < compareEnd && source[cmp1] == source[cmp2])
                    {
                        cmp1++;
                        cmp2++;
                    }

                    int len_ = cmp2 - position;
                    if (foundLength < len_)
                    {
                        foundLength = len_;
                        found = search;
                        if (foundLength == maxMatchLength)
                        {
                            break;
                        }
                    }
                    search++;
                }
            }

            return (found, foundLength);
        }

        private static int Find(byte[] source, byte target, int start, int end)
        {
            while (start < end)
            {
                if (source[start] == target)
                {
                    return start;
                }
                start++;
            }
            return -1;
        }
    }
}
