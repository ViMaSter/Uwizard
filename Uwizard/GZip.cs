using System;
using System.IO;
using System.IO.Compression;

namespace Uwizard
{
    public struct gzip
    {
        public static string lerror = ""; // Gets the last error that occurred in this struct. Similar to the C perror().

        public static bool decompress(byte[] indata, string outfile)
        {
            try
            {
                MemoryStream ms = new MemoryStream(indata);
                StreamWriter sw = new StreamWriter(outfile);
                GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress);
                int lbyte = gzs.ReadByte();
                while (lbyte != -1)
                {
                    sw.BaseStream.WriteByte((byte)lbyte);
                    lbyte = gzs.ReadByte();
                }
                gzs.Close();
                gzs.Dispose();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                lerror = ex.Message;
                return false;
            }
            return true;
        }

        public static bool compress(string infile, string outfile)
        {
            try
            {
                byte[] ifdata = File.ReadAllBytes(infile);
                StreamWriter sw = new StreamWriter(outfile);
                GZipStream gzs = new GZipStream(sw.BaseStream, CompressionMode.Compress);
                gzs.Write(ifdata, 0, ifdata.Length);
                gzs.Close();
                gzs.Dispose();
            }
            catch (Exception ex)
            {
                lerror = ex.Message;
                return false;
            }
            return true;
        }

        public static bool decompress(string infile, string outfile)
        {
            try
            {
                StreamWriter sw = new StreamWriter(outfile);
                StreamReader sr = new StreamReader(infile);
                GZipStream gzs = new GZipStream(sr.BaseStream, CompressionMode.Decompress);
                int lbyte = gzs.ReadByte();
                while (lbyte != -1)
                {
                    sw.BaseStream.WriteByte((byte)lbyte);
                    lbyte = gzs.ReadByte();
                }
                gzs.Close();
                gzs.Dispose();
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                lerror = ex.Message;
                return false;
            }
            return true;
        }
    }
}
