using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsDecode
{
    public class Program
    {
        public const int MAGIC_NO_COMPRESS_START = 0x03;
        public const int MAGIC_COMPRESS_START = 0x04;
        public const int MAGIC_COMPRESS_START1 = 0x05;
        public const int MAGIC_END = 0x00;

        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (Directory.Exists(args[0]))
                {
                    var filelist = Directory.GetFiles(args[0], "*.xlog");
                    foreach(var filepath in filelist)
                    {
                        ParseFile(filepath, filepath + ".log");
                    }
                }
                else
                {
                    ParseFile(args[0], args[0] + ".log");
                }
            }
            else if(args.Length == 2)
            {
                ParseFile(args[0], args[0] + ".log");
            }
        }

        static void ParseFile(string filePath, string outfilePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                //1046
                byte[] fileBytes = new byte[fs.Length];
                fs.Read(fileBytes, 0, (int)fs.Length);
                var startPos = GetLogStartPos(fileBytes, 2);
                if (startPos == -1)
                    return;

                Stream outStream = new MemoryStream();
                while (true)
                {
                    startPos = DecodeBuffer(fileBytes, startPos, outStream);
                    if (startPos == -1)
                        break;
                }
                if (outStream.Length == 0)
                    return;
                outStream.Position = 0;
                StreamReader sr = new StreamReader(outStream, Encoding.ASCII);
                string s = sr.ReadToEnd();
            }
        }

        static int GetLogStartPos(byte[] buffer, int count)
        {
            int offset = 0;
            while(true)
            {
                if (offset >= buffer.Length)
                    break;
                //3
                if (MAGIC_NO_COMPRESS_START == buffer[offset] || MAGIC_COMPRESS_START == buffer[offset] || MAGIC_COMPRESS_START1 == buffer[offset])
                {
                    if (IsGoodLogBuffer(buffer, offset, count))
                        return offset;
                }
                offset++;
            }
            return -1;
        }

        static bool IsGoodLogBuffer(byte[] buffer, int offset, int count)
        {
            //13
            int headerLen = 0;
            if (offset == buffer.Length)
                return true;

            if(MAGIC_NO_COMPRESS_START == buffer[offset] || MAGIC_COMPRESS_START == buffer[offset] || MAGIC_COMPRESS_START1 == buffer[offset])
            {
                headerLen = 1 + 2 + 1 + 1 + 4 + 4;
            }
            else
            {
                return false;
            }

            if (offset + headerLen + 1 + 1 > buffer.Length)
                return false;
            //26
            uint length = BitConverter.ToUInt32(buffer, offset + headerLen - 4 - 4);
            if (offset + headerLen + length + 1 > buffer.Length)
                return false;
            if (MAGIC_END != buffer[offset + headerLen + length])
                return false;

            if (1 >= count)
                return true;
            else
                return IsGoodLogBuffer(buffer, (int)(offset + headerLen + length + 1), count - 1);
        }

        static int DecodeBuffer(byte[] buffer, int offset,Stream outbuffer)
        {
            int headerLen = 0;
            if (offset >= buffer.Length)
                return -1;

            bool ret = IsGoodLogBuffer(buffer, offset, 1);
            if(!ret)
            {
                var logStartBuffer = new byte[buffer.Length - offset];
                Array.Copy(buffer, offset, logStartBuffer, 0, logStartBuffer.Length);
                int fixpos = GetLogStartPos(logStartBuffer, 1);
                if (fixpos == -1)
                    return -1;
                else
                    offset += fixpos;
            }

            if(MAGIC_NO_COMPRESS_START == buffer[offset] || MAGIC_COMPRESS_START == buffer[offset] || MAGIC_COMPRESS_START1 == buffer[offset])
            {
                headerLen = 1 + 2 + 1 + 1 + 4 + 4;
            }
            else
            {
                return -1;
            }

            uint length = BitConverter.ToUInt32(buffer, offset + headerLen - 4 - 4);
            var tmpbuffer = new byte[length];

            ushort seq = BitConverter.ToUInt16(buffer, offset + headerLen - 4 - 4 - 2 - 2);
            char begin_hour = BitConverter.ToChar(buffer, offset + headerLen - 4 - 4 - 1 - 1);
            char end_hour = BitConverter.ToChar(buffer, offset + headerLen - 4 - 4 - 1);

            int lastseq = 0;
            if (seq != 0 && seq != 1 && lastseq != 0 && seq != (lastseq + 1))
                Console.WriteLine($"[F]decode_log_file_py log seq:{lastseq + 1}-{seq - 1} is missing");

            if (seq != 0)
                lastseq = seq;

            Array.Copy(buffer, offset + headerLen, tmpbuffer, 0, (int)length);

            if (MAGIC_COMPRESS_START == buffer[offset])
            {
                using (var ms = new MemoryStream(tmpbuffer))
                {
                    ms.Position = 0;
                    using (var zipStream = new ZInputStream(ms, true))
                    {
                        tmpbuffer = Streams.ReadAll(zipStream);
                    }
                }
            }
            else if(MAGIC_COMPRESS_START1 == buffer[offset])
            {
                using (MemoryStream ms = new MemoryStream())
                {

                    while (tmpbuffer.Length > 0)
                    {
                        ushort single_log_len = BitConverter.ToUInt16(tmpbuffer, 0);
                        var writeLength = (single_log_len + 2 >= tmpbuffer.Length) ? tmpbuffer.Length - 2 : single_log_len;
                        ms.Write(tmpbuffer, 2, writeLength);
                        var copybuffer = new byte[tmpbuffer.Length - 2 - single_log_len];
                        Array.Copy(tmpbuffer, single_log_len + 2, copybuffer, 0, tmpbuffer.Length - 2 - single_log_len);
                        tmpbuffer = copybuffer;
                    }


                    ms.Position = 0;
                    using (var zipStream = new ZInputStream(ms, true))
                    {
                        tmpbuffer = Streams.ReadAll(zipStream);
                        var str = Encoding.ASCII.GetString(tmpbuffer);
                    }
                }
            }
            outbuffer.Write(tmpbuffer, 0, tmpbuffer.Length);

            return (int)(offset + headerLen + length + 1);
        }
    }
}
