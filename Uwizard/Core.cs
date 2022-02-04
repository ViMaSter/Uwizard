using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Uwizard.Properties;

namespace Uwizard
{
    public static class Core
    {
        public static bool IsHex(char ch)
        {
            ch = char.ToUpper(ch);
            switch (ch)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return true;
            }

            return false;
        }

        public static byte[] hex2byte(string str)
        {
            var ret = new byte[str.Length >> 1];
            for (var c = 0; c < ret.Length; c++)
            {
                if (!(IsHex(str[c * 2]) && IsHex(str[c * 2 + 1])))
                {
                    return null;
                }

                ret[c] = Convert.ToByte(str[c * 2] + str[c * 2 + 1].ToString(), 16);
            }

            return ret;
        }

        public static void runExtract(string titleKey, string commonKey, string wudPath)
        {
            var titleKeyByte = hex2byte(titleKey);
            var commonKeyByte = hex2byte(commonKey);

            var hasDiscU = File.Exists("DiscU.exe");

            File.WriteAllBytes("ckey.bin", commonKeyByte);
            File.WriteAllBytes("tkey.bin", titleKeyByte);

            if (!hasDiscU)
            {
                gzip.decompress(Resources.DiscU, "DiscU.exe");
                gzip.decompress(Resources.libeay32, "libeay32.dll");
            }

            var discU = new Process {
                StartInfo = {
                    FileName = $"{Environment.CurrentDirectory}\\DiscU.exe",
                    Arguments = $"\"{Environment.CurrentDirectory}\\tkey.bin\" \"{wudPath}\" \"{Environment.CurrentDirectory}\\ckey.bin\"",
                    WorkingDirectory = Environment.CurrentDirectory,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };


            discU.Start();
            //string line;
            while (!discU.StandardOutput.EndOfStream)
            {
                Console.WriteLine(discU.StandardOutput.ReadLine());
                Application.DoEvents();
            }

            discU.WaitForExit();
            discU.Dispose();

            if (File.Exists("tkey.bin"))
            {
                File.Delete("tkey.bin");
            }

            if (File.Exists("ckey.bin"))
            {
                File.Delete("ckey.bin");
            }
            if (!hasDiscU)
            {
                if (File.Exists("DiscU.exe"))
                {
                    File.Delete("DiscU.exe");
                }

                if (File.Exists("libeay32.dll"))
                {
                    File.Delete("libeay32.dll");
                }
            }
        }

        public static int lastnumchannels = -1;

        public static bool convertbfstm(string infile, string outfile, bool exchannels)
        {
            if (exchannels)
            {
                gzip.decompress(Resources.bfstm_decoder, "bfstm_decoder.exe");
                gzip.decompress(Resources.libg7221_decode, "libg7221_decode.dll");
                gzip.decompress(Resources.libmpg123_0, "libmpg123-0.dll");
                gzip.decompress(Resources.libvorbis, "libvorbis.dll");

                var bfstm_decoder_p = new Process {
                    StartInfo = {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        FileName = Path.GetFileName("bfstm_decoder.exe"),
                        Arguments = $"-m \"{infile}\""
                    }
                };


                bfstm_decoder_p.Start();

                lastnumchannels = 1;

                while (!bfstm_decoder_p.StandardOutput.EndOfStream)
                {
                    var tmp = bfstm_decoder_p.StandardOutput.ReadLine();
                    if (Strings.Left(tmp, "channels: ".Length) != "channels: ")
                    {
                        continue;
                    }
                    lastnumchannels = int.Parse(Strings.Right(tmp, tmp.Length - "channels: ".Length)) / 2;
                    break;
                }

                bfstm_decoder_p.WaitForExit();
                bfstm_decoder_p.StartInfo.RedirectStandardOutput = false;

                var ofwo = $"{Path.GetDirectoryName(outfile)}/{Path.GetFileNameWithoutExtension(outfile)}";
                var ofex = Path.GetExtension(outfile);

                for (var c = 0; c < lastnumchannels; c++)
                {
                    var tfn = $"{ofwo}_channel_{(c + 1)}{ofex}";
                    if (File.Exists(tfn))
                    {
                        File.Delete(tfn);
                    }

                    bfstm_decoder_p.StartInfo.Arguments = $"-2 {c} -o \"{tfn}\" \"{infile}\"";
                    bfstm_decoder_p.Start();
                    bfstm_decoder_p.WaitForExit();
                }

                bfstm_decoder_p.Dispose();

                File.Delete("bfstm_decoder.exe");
                File.Delete("libg7221_decode.dll");
                File.Delete("libmpg123-0.dll");
                File.Delete("libvorbis.dll");

                return true;
            }

            return convertbfstm(infile, outfile);
        }

        public static bool convertbfstm(string infile, string outfile)
        {
            gzip.decompress(Resources.bfstm_decoder, "bfstm_decoder.exe");
            gzip.decompress(Resources.libg7221_decode, "libg7221_decode.dll");
            gzip.decompress(Resources.libmpg123_0, "libmpg123-0.dll");
            gzip.decompress(Resources.libvorbis, "libvorbis.dll");

            if (File.Exists(outfile))
            {
                File.Delete(outfile);
            }

            var bfstm_decoder_p = new Process {StartInfo = {CreateNoWindow = true, UseShellExecute = false, FileName = Path.GetFileName("bfstm_decoder.exe"), Arguments = $"-o \"{outfile}\" \"{infile}\""}};
            bfstm_decoder_p.Start();
            bfstm_decoder_p.WaitForExit();
            bfstm_decoder_p.Dispose();

            File.Delete("bfstm_decoder.exe");
            File.Delete("libg7221_decode.dll");
            File.Delete("libmpg123-0.dll");
            File.Delete("libvorbis.dll");

            return File.Exists(outfile);
        }

        public static bool convertwav2mp3(string infile, string outfile)
        {
            gzip.decompress(Resources.mp3enc, "mp3enc.exe");

            var tof = Path.GetTempFileName();
            File.Delete(tof);

            var bfstm_decoder_p = new Process {StartInfo = {CreateNoWindow = true, UseShellExecute = false, FileName = Path.GetFileName("mp3enc.exe"), Arguments = $"-V 5 \"{infile}\" \"{tof}"}};
            bfstm_decoder_p.Start();
            bfstm_decoder_p.WaitForExit();
            bfstm_decoder_p.Dispose();

            File.Delete("mp3enc.exe");

            if (File.Exists(tof))
            {
                var newmp3 = File.ReadAllBytes(tof);

                for (var c = 3; c < newmp3.Length; c++)
                {
                    if (newmp3[c - 3] == 'L' && newmp3[c - 2] == 'A' && newmp3[c - 1] == 'M' && newmp3[c] == 'E')
                    {
                        newmp3[c - 3] = (byte)'U';
                        newmp3[c - 2] = (byte)'W';
                        newmp3[c - 1] = (byte)'I';
                        newmp3[c] = (byte)'Z';
                        newmp3[c + 1] = (byte)115.ToString()[0];
                        newmp3[c + 2] = (byte)'.';
                        newmp3[c + 3] = (byte)115.ToString()[1];
                        newmp3[c + 4] = (byte)'.';
                        newmp3[c + 5] = (byte)115.ToString()[2];
                    }
                }

                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }

                File.WriteAllBytes(outfile, newmp3);
                File.Delete(tof);

                return true;
            }

            return false;
        }

        public static bool extractszs(string infile, string outfile)
        {
            gzip.decompress(Resources.yaz0dec, "yaz0dec.exe");
            var yaz0dec = new Process {StartInfo = {Arguments = $"\"{infile}\"", FileName = Path.GetFullPath("yaz0dec.exe"), CreateNoWindow = true, UseShellExecute = false}};
            yaz0dec.Start();
            yaz0dec.WaitForExit();
            File.Delete("yaz0dec.exe");
            if (!File.Exists($"{infile} 0.rarc"))
            {
                return false;
            }

            File.Move($"{infile} 0.rarc", outfile);
            return true;
        }

        public static bool packszs(string infile, string outfile)
        {
            gzip.decompress(Resources.yaz0enc, "yaz0enc.exe");
            var yaz0enc = new Process {StartInfo = {Arguments = $"\"{infile}\"", FileName = Path.GetFullPath("yaz0enc.exe"), CreateNoWindow = true, UseShellExecute = false}};
            yaz0enc.Start();
            yaz0enc.WaitForExit();
            File.Delete("yaz0enc.exe");
            if (!File.Exists($"{infile}.yaz0"))
            {
                return false;
            }

            File.Move($"{infile}.yaz0", outfile);
            return true;
        }
    }
}