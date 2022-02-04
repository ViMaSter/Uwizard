using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Uwizard
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var commandLineArguments = Environment.GetCommandLineArgs();

            if (commandLineArguments.Length <= 1)
            {
                return;
            }

            Console.WriteLine($"Uwizard 1.2.0");
            Console.WriteLine();

            var oPath = "";
            for (var c = 2; c < commandLineArguments.Length; c++)
            {
                if (commandLineArguments[c] == "-o" && c + 1 < commandLineArguments.Length)
                {
                    oPath = commandLineArguments[c + 1];
                }
            }

            if (!File.Exists(commandLineArguments[1]))
            {
                if (!Directory.Exists(commandLineArguments[1]))
                {
                    Console.WriteLine("Uwizard can run in command line mode in addition to GUI mode. You may specify a file and Uwizard will take the correct action for that file type. For example, if the first argument is the path to an SZS file, Uwizard will try to decompress it. You may also specify an output path with the \"-o <outputfile>\" parameter. If the input file is a BFSTM sound stream, you may also specify the \"-s\" switch to export all sound channels as separate WAV files. If the input file is a SARC archive, you may add \"-c\" to compress the SARC into a Yaz0 SZS, or \"-e\" to extract it to a directory. You may also specify the SARC padding with the \"-pad <decimalvalue>\".");
                    return;
                }

                if (oPath == "")
                {
                    oPath = $"{commandLineArguments[1]}.sarc";
                }

                Console.WriteLine($"Packing directory into a SARC archive at {oPath}");
                if (SARC.pack(commandLineArguments[1], oPath))
                {
                    Console.WriteLine("Finished!");
                }
                else
                {
                    Console.WriteLine("Error!");
                }

                return;
            }

            Console.WriteLine("Reading \"{0}\".", Path.GetFileName(commandLineArguments[1]));

            var sr = new StreamReader(commandLineArguments[1]);
            var magic = ((char) sr.BaseStream.ReadByte()) + ((char) sr.BaseStream.ReadByte()).ToString() + ((char) sr.BaseStream.ReadByte()) + ((char) sr.BaseStream.ReadByte());

            switch (magic)
            {
                case "WUP-":
                    Console.WriteLine("File is a WUD file.");
                    Console.WriteLine("Extraction start might take a few seconds and will be completed when 'Done!' is printed");
                    Core.runExtract(commandLineArguments[2], commandLineArguments[3], commandLineArguments[1]);
                    Console.WriteLine("Done!");
                    break;
                case "SARC":
                    Console.WriteLine("File is a SARC archive.");
                    if (oPath == "")
                    {
                        oPath = $"{commandLineArguments[1]}_extracted";
                    }

                    Console.WriteLine($"Extracting to \"{oPath}\".");
                    if (SARC.extract(commandLineArguments[1], oPath))
                    {
                        Console.WriteLine("Finished!");
                    }
                    else
                    {
                        Console.WriteLine($"Error!\n{SARC.lerror}");
                    }

                    break;
                case "Yaz0":
                    Console.WriteLine("File is Yaz0 compressed.");
                    if (oPath == "")
                    {
                        oPath = $"{commandLineArguments[1]}.bin";
                    }

                    Console.WriteLine($"Decompressing to \"{oPath}\".");
                    if (Core.extractszs(commandLineArguments[1], oPath))
                    {
                        Console.WriteLine("Finished!");
                        if (!SARC.extract(oPath, $"{oPath}_extracted"))
                        {
                            Console.WriteLine("Error!");
                        }
                        else
                        {
                            Console.WriteLine("Success!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error!");
                    }

                    break;
                default:
                    Console.WriteLine($"Unknown file type! + '{magic}'");
                    break;
            }
        }
    }
}