using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace Uwizard
{
    /* libWiiSharp is distributed in the hope that it will be
    * useful, but WITHOUT ANY WARRANTY; without even the implied warranty
    * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    * GNU General Public License for more details.
    *
    * You should have received a copy of the GNU General Public License
    * along with this program.  If not, see <http://www.gnu.org/licenses/>.
    */

    namespace libWiiSharp
    {
        internal struct ContentIndices : IComparable
        {
            public int Index { get; }

            public int ContentIndex { get; }

            public ContentIndices(int index, int contentIndex)
            {
                this.Index = index;
                this.ContentIndex = contentIndex;
            }

            public int CompareTo(object obj)
            {
                if (obj is ContentIndices) return ContentIndex.CompareTo(((ContentIndices) obj).ContentIndex);

                throw new ArgumentException();
            }
        }

        public static class Shared
        {
            /// <summary>
            ///     Merges two string arrays into one without double entries.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static string[] MergeStringArrays(string[] a, string[] b)
            {
                var sList = new List<string>(a);

                foreach (var currentString in b)
                    if (!sList.Contains(currentString))
                        sList.Add(currentString);

                sList.Sort();
                return sList.ToArray();
            }

            /// <summary>
            ///     Compares two byte arrays.
            /// </summary>
            /// <param name="first"></param>
            /// <param name="firstIndex"></param>
            /// <param name="second"></param>
            /// <param name="secondIndex"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static bool CompareByteArrays(byte[] first, int firstIndex, byte[] second, int secondIndex, int length)
            {
                if (first.Length < length || second.Length < length) return false;

                for (var i = 0; i < length; i++)
                    if (first[firstIndex + i] != second[secondIndex + i])
                        return false;

                return true;
            }

            /// <summary>
            ///     Compares two byte arrays.
            /// </summary>
            /// <param name="first"></param>
            /// <param name="second"></param>
            /// <returns></returns>
            public static bool CompareByteArrays(byte[] first, byte[] second)
            {
                if (first.Length != second.Length) return false;

                for (var i = 0; i < first.Length; i++)
                    if (first[i] != second[i])
                        return false;

                return true;
            }

            /// <summary>
            ///     Turns a byte array into a string, default separator is a space.
            /// </summary>
            /// <param name="byteArray"></param>
            /// <param name="separator"></param>
            /// <returns></returns>
            public static string ByteArrayToString(byte[] byteArray, char separator)
            {
                var res = string.Empty;

                foreach (var b in byteArray) res += b.ToString("x2").ToUpper() + separator;

                return res.Remove(res.Length - 1);
            }

            /// <summary>
            ///     Turns a hex string into a byte array.
            /// </summary>
            /// <param name="hexString"></param>
            /// <returns></returns>
            public static byte[] HexStringToByteArray(string hexString)
            {
                var ba = new byte[hexString.Length / 2];

                for (var i = 0; i < hexString.Length / 2; i++) ba[i] = byte.Parse(hexString.Substring(i * 2, 2), NumberStyles.HexNumber);

                return ba;
            }

            /// <summary>
            ///     Counts how often the given char exists in the given string.
            /// </summary>
            /// <param name="theString"></param>
            /// <param name="theChar"></param>
            /// <returns></returns>
            public static int CountCharsInString(string theString, char theChar)
            {
                var count = 0;

                foreach (var thisChar in theString)
                    if (thisChar == theChar)
                        count++;

                return count;
            }

            /// <summary>
            ///     Pads the given value to a multiple of the given padding value, default padding value is 64.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long AddPadding(long value)
            {
                return AddPadding(value, 64);
            }

            /// <summary>
            ///     Pads the given value to a multiple of the given padding value, default padding value is 64.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static long AddPadding(long value, int padding)
            {
                if (value % padding != 0) value = value + (padding - value % padding);

                return value;
            }

            /// <summary>
            ///     Pads the given value to a multiple of the given padding value, default padding value is 64.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static int AddPadding(int value)
            {
                return AddPadding(value, 64);
            }

            /// <summary>
            ///     Pads the given value to a multiple of the given padding value, default padding value is 64.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static int AddPadding(int value, int padding)
            {
                if (value % padding != 0) value = value + (padding - value % padding);

                return value;
            }

            /// <summary>
            ///     Swaps endianness.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static ushort Swap(ushort value)
            {
                return (ushort) IPAddress.HostToNetworkOrder((short) value);
            }

            /// <summary>
            ///     Swaps endianness.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static uint Swap(uint value)
            {
                return (uint) IPAddress.HostToNetworkOrder((int) value);
            }

            /// <summary>
            ///     Swaps endianness
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static ulong Swap(ulong value)
            {
                return (ulong) IPAddress.HostToNetworkOrder((long) value);
            }

            /// <summary>
            ///     Turns a ushort array into a byte array.
            /// </summary>
            /// <param name="array"></param>
            /// <returns></returns>
            public static byte[] UShortArrayToByteArray(ushort[] array)
            {
                var results = new List<byte>();
                foreach (var value in array)
                {
                    var converted = BitConverter.GetBytes(value);
                    results.AddRange(converted);
                }

                return results.ToArray();
            }

            /// <summary>
            ///     Turns a uint array into a byte array.
            /// </summary>
            /// <param name="array"></param>
            /// <returns></returns>
            public static byte[] UIntArrayToByteArray(uint[] array)
            {
                var results = new List<byte>();
                foreach (var value in array)
                {
                    var converted = BitConverter.GetBytes(value);
                    results.AddRange(converted);
                }

                return results.ToArray();
            }

            /// <summary>
            ///     Turns a byte array into a uint array.
            /// </summary>
            /// <param name="array"></param>
            /// <returns></returns>
            public static uint[] ByteArrayToUIntArray(byte[] array)
            {
                var converted = new uint[array.Length / 4];
                var j = 0;

                for (var i = 0; i < array.Length; i += 4) converted[j++] = BitConverter.ToUInt32(array, i);

                return converted;
            }

            /// <summary>
            ///     Turns a byte array into a ushort array.
            /// </summary>
            /// <param name="array"></param>
            /// <returns></returns>
            public static ushort[] ByteArrayToUShortArray(byte[] array)
            {
                var converted = new ushort[array.Length / 2];
                var j = 0;

                for (var i = 0; i < array.Length; i += 2) converted[j++] = BitConverter.ToUInt16(array, i);

                return converted;
            }
        }

        public enum StoreType
        {
            EncryptedContent = 0,
            DecryptedContent = 1,
            WAD = 2,
            All = 3,
            Empty = 4
        }

        public class MessageEventArgs : EventArgs
        {
            public MessageEventArgs(string message)
            {
                this.Message = message;
            }

            public string Message { get; }
        }


        public enum ContentType : ushort
        {
            Normal = 0x0001,
            DLC = 0x4001, //Seen this in a DLC wad...
            Shared = 0x8001
        }

        public enum Region : ushort
        {
            Japan = 0,
            USA = 1,
            Europe = 2,
            Free = 3
        }

        public class TMD : IDisposable
        {
            private uint accessRights;
            private ushort bootIndex;
            private byte caCrlVersion;
            private List<TMD_Content> contents;
            private ushort groupId;
            private byte[] issuer = new byte[64];
            private byte[] padding = new byte[60];
            private ushort padding2;
            private ushort padding3;
            private byte paddingByte;
            private ushort region;
            private byte[] reserved = new byte[58];
            private byte[] signature = new byte[256];

            private uint signatureExponent = 0x00010001;
            private byte signerCrlVersion;
            private uint titleType;
            private byte version;

            /// <summary>
            ///     The region of the title.
            /// </summary>
            public Region Region
            {
                get => (Region) region;
                set => region = (ushort) value;
            }

            /// <summary>
            ///     The IOS the title is launched with.
            /// </summary>
            public ulong StartupIOS { get; set; }

            /// <summary>
            ///     The Title ID.
            /// </summary>
            public ulong TitleID { get; set; }

            /// <summary>
            ///     The Title Version.
            /// </summary>
            public ushort TitleVersion { get; set; }

            /// <summary>
            ///     The Number of Contents.
            /// </summary>
            public ushort NumOfContents { get; private set; }

            /// <summary>
            ///     The boot index. Represents the index of the nand loader.
            /// </summary>
            public ushort BootIndex
            {
                get => bootIndex;
                set
                {
                    if (value <= NumOfContents) bootIndex = value;
                }
            }

            /// <summary>
            ///     The content descriptions in the TMD.
            /// </summary>
            public TMD_Content[] Contents
            {
                get => contents.ToArray();
                set
                {
                    contents = new List<TMD_Content>(value);
                    NumOfContents = (ushort) value.Length;
                }
            }

            /// <summary>
            ///     If true, the TMD will be fakesigned while saving.
            /// </summary>
            public bool FakeSign { get; set; }

            #region IDisposable Members

            private bool isDisposed;

            ~TMD()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing && !isDisposed)
                {
                    signature = null;
                    padding = null;
                    issuer = null;
                    reserved = null;

                    contents.Clear();
                    contents = null;
                }

                isDisposed = true;
            }

            #endregion

            #region Public Functions

            /// <summary>
            ///     Loads a tmd file.
            /// </summary>
            /// <param name="pathToTmd"></param>
            /// <returns></returns>
            public static TMD Load(string pathToTmd)
            {
                return Load(File.ReadAllBytes(pathToTmd));
            }

            /// <summary>
            ///     Loads a tmd file.
            /// </summary>
            /// <param name="tmdFile"></param>
            /// <returns></returns>
            public static TMD Load(byte[] tmdFile)
            {
                var t = new TMD();
                var ms = new MemoryStream(tmdFile);

                try
                {
                    t.parseTmd(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                ms.Dispose();
                return t;
            }

            /// <summary>
            ///     Loads a tmd file.
            /// </summary>
            /// <param name="tmd"></param>
            /// <returns></returns>
            public static TMD Load(Stream tmd)
            {
                var t = new TMD();
                t.parseTmd(tmd);
                return t;
            }


            /// <summary>
            ///     Loads a tmd file.
            /// </summary>
            /// <param name="pathToTmd"></param>
            public void LoadFile(string pathToTmd)
            {
                LoadFile(File.ReadAllBytes(pathToTmd));
            }

            /// <summary>
            ///     Loads a tmd file.
            /// </summary>
            /// <param name="tmdFile"></param>
            public void LoadFile(byte[] tmdFile)
            {
                var ms = new MemoryStream(tmdFile);

                try
                {
                    parseTmd(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                ms.Dispose();
            }

            /// <summary>
            ///     Loads a tmd file.
            /// </summary>
            /// <param name="tmd"></param>
            public void LoadFile(Stream tmd)
            {
                parseTmd(tmd);
            }


            /// <summary>
            ///     Saves the TMD.
            /// </summary>
            /// <param name="savePath"></param>
            public void Save(string savePath)
            {
                Save(savePath, false);
            }

            /// <summary>
            ///     Saves the TMD. If fakeSign is true, the Ticket will be fakesigned.
            /// </summary>
            /// <param name="savePath"></param>
            /// <param name="fakeSign"></param>
            public void Save(string savePath, bool fakeSign)
            {
                if (fakeSign) this.FakeSign = true;

                if (File.Exists(savePath)) File.Delete(savePath);

                using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    writeToStream(fs);
                }
            }

            /// <summary>
            ///     Returns the TMD as a memory stream.
            /// </summary>
            /// <returns></returns>
            public MemoryStream ToMemoryStream()
            {
                return ToMemoryStream(false);
            }

            /// <summary>
            ///     Returns the TMD as a memory stream. If fakeSign is true, the Ticket will be fakesigned.
            /// </summary>
            /// <param name="fakeSign"></param>
            /// <returns></returns>
            public MemoryStream ToMemoryStream(bool fakeSign)
            {
                if (fakeSign) this.FakeSign = true;

                var ms = new MemoryStream();

                try
                {
                    writeToStream(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                return ms;
            }

            /// <summary>
            ///     Returns the TMD as a byte array.
            /// </summary>
            /// <returns></returns>
            public byte[] ToByteArray()
            {
                return ToByteArray(false);
            }

            /// <summary>
            ///     Returns the TMD as a byte array. If fakeSign is true, the Ticket will be fakesigned.
            /// </summary>
            /// <param name="fakeSign"></param>
            /// <returns></returns>
            public byte[] ToByteArray(bool fakeSign)
            {
                if (fakeSign) this.FakeSign = true;

                var ms = new MemoryStream();

                try
                {
                    writeToStream(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                var res = ms.ToArray();
                ms.Dispose();
                return res;
            }

            /// <summary>
            ///     Updates the content entries.
            /// </summary>
            /// <param name="contentDir"></param>
            /// <param name="namedContentId">
            ///     True if you use the content ID as name (e.g. 0000008a.app).
            ///     False if you use the index as name (e.g. 00000000.app)
            /// </param>
            public void UpdateContents(string contentDir)
            {
                var namedContentId = true;
                for (var i = 0; i < contents.Count; i++)
                    if (!File.Exists($"{contentDir}{Path.DirectorySeparatorChar}{contents[i].ContentID:x8}.app"))
                    {
                        namedContentId = false;
                        break;
                    }

                if (!namedContentId)
                    for (var i = 0; i < contents.Count; i++)
                        if (!File.Exists($"{contentDir}{Path.DirectorySeparatorChar}{contents[i].ContentID:x8}.app"))
                            throw new Exception("Couldn't find all content files!");

                var conts = new byte[contents.Count][];

                for (var i = 0; i < contents.Count; i++)
                {
                    var file = $"{contentDir}{Path.DirectorySeparatorChar}{(namedContentId ? contents[i].ContentID.ToString("x8") : contents[i].Index.ToString("x8"))}.app";
                    conts[i] = File.ReadAllBytes(file);
                }

                updateContents(conts);
            }

            /// <summary>
            ///     Updates the content entries.
            /// </summary>
            /// <param name="contentDir"></param>
            /// <param name="namedContentId">
            ///     True if you use the content ID as name (e.g. 0000008a.app).
            ///     False if you use the index as name (e.g. 00000000.app)
            /// </param>
            public void UpdateContents(byte[][] contents)
            {
                updateContents(contents);
            }

            /// <summary>
            ///     Returns the Upper Title ID as a string.
            /// </summary>
            /// <returns></returns>
            public string GetUpperTitleID()
            {
                var titleBytes = BitConverter.GetBytes(Shared.Swap((uint) TitleID));
                return new string(new[] {(char) titleBytes[0], (char) titleBytes[1], (char) titleBytes[2], (char) titleBytes[3]});
            }

            /// <summary>
            ///     The Number of memory blocks the content will take.
            /// </summary>
            /// <returns></returns>
            public string GetNandBlocks()
            {
                return calculateNandBlocks();
            }

            /// <summary>
            ///     Adds a TMD content.
            /// </summary>
            /// <param name="content"></param>
            public void AddContent(TMD_Content content)
            {
                contents.Add(content);

                NumOfContents = (ushort) contents.Count;
            }

            /// <summary>
            ///     Removes the content with the given index.
            /// </summary>
            /// <param name="contentIndex"></param>
            public void RemoveContent(int contentIndex)
            {
                for (var i = 0; i < NumOfContents; i++)
                    if (contents[i].Index == contentIndex)
                    {
                        contents.RemoveAt(i);
                        break;
                    }

                NumOfContents = (ushort) contents.Count;
            }

            /// <summary>
            ///     Removes the content with the given ID.
            /// </summary>
            /// <param name="contentId"></param>
            public void RemoveContentByID(int contentId)
            {
                for (var i = 0; i < NumOfContents; i++)
                    if (contents[i].ContentID == contentId)
                    {
                        contents.RemoveAt(i);
                        break;
                    }

                NumOfContents = (ushort) contents.Count;
            }

            #endregion

            #region Private Functions

            private void writeToStream(Stream writeStream)
            {
                fireDebug("Writing TMD...");

                if (FakeSign)
                {
                    fireDebug("   Clearing Signature...");
                    signature = new byte[256];
                } //Clear Signature if we fake Sign

                var ms = new MemoryStream();
                ms.Seek(0, SeekOrigin.Begin);

                fireDebug("   Writing Signature Exponent... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(signatureExponent)), 0, 4);

                fireDebug("   Writing Signature... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(signature, 0, signature.Length);

                fireDebug("   Writing Padding... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(padding, 0, padding.Length);

                fireDebug("   Writing Issuer... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(issuer, 0, issuer.Length);

                fireDebug("   Writing Version... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(version);

                fireDebug("   Writing CA Crl Version... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(caCrlVersion);

                fireDebug("   Writing Signer Crl Version... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(signerCrlVersion);

                fireDebug("   Writing Padding Byte... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(paddingByte);

                fireDebug("   Writing Startup IOS... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(StartupIOS)), 0, 8);

                fireDebug("   Writing Title ID... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(TitleID)), 0, 8);

                fireDebug("   Writing Title Type... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(titleType)), 0, 4);

                fireDebug("   Writing Group ID... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(groupId)), 0, 2);

                fireDebug("   Writing Padding2... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(padding2)), 0, 2);

                fireDebug("   Writing Region... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(region)), 0, 2);

                fireDebug("   Writing Reserved... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(reserved, 0, reserved.Length);

                fireDebug("   Writing Access Rights... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(accessRights)), 0, 4);

                fireDebug("   Writing Title Version... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(TitleVersion)), 0, 2);

                fireDebug("   Writing NumOfContents... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(NumOfContents)), 0, 2);

                fireDebug("   Writing Boot Index... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(bootIndex)), 0, 2);

                fireDebug("   Writing Padding3... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(padding3)), 0, 2);

                //Write Contents
                var contentList = new List<ContentIndices>();
                for (var i = 0; i < contents.Count; i++) contentList.Add(new ContentIndices(i, contents[i].Index));

                contentList.Sort();

                for (var i = 0; i < contentList.Count; i++)
                {
                    fireDebug("   Writing Content #{1} of {2}... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper().ToUpper(), i + 1, NumOfContents);

                    ms.Write(BitConverter.GetBytes(Shared.Swap(contents[contentList[i].Index].ContentID)), 0, 4);
                    ms.Write(BitConverter.GetBytes(Shared.Swap(contents[contentList[i].Index].Index)), 0, 2);
                    ms.Write(BitConverter.GetBytes(Shared.Swap((ushort) contents[contentList[i].Index].Type)), 0, 2);
                    ms.Write(BitConverter.GetBytes(Shared.Swap(contents[contentList[i].Index].Size)), 0, 8);

                    ms.Write(contents[contentList[i].Index].Hash, 0, contents[contentList[i].Index].Hash.Length);
                }

                //fake Sign
                var tmd = ms.ToArray();
                ms.Dispose();

                if (FakeSign)
                {
                    fireDebug("   Fakesigning TMD...");

                    var hash = new byte[20];
                    var s = SHA1.Create();

                    for (ushort i = 0; i < 0xFFFF; i++)
                    {
                        var bytes = BitConverter.GetBytes(i);
                        tmd[482] = bytes[1];
                        tmd[483] = bytes[0];

                        hash = s.ComputeHash(tmd);
                        if (hash[0] == 0x00)
                        {
                            fireDebug("   -> Signed ({0})", i);
                            break;
                        } //Win! It's signed...

                        if (i == 0xFFFF - 1)
                        {
                            fireDebug("    -> Signing Failed...");
                            throw new Exception("Fakesigning failed...");
                        }
                    }

                    s.Clear();
                }

                writeStream.Seek(0, SeekOrigin.Begin);
                writeStream.Write(tmd, 0, tmd.Length);

                fireDebug("Writing TMD Finished...");
            }

            private void updateContents(byte[][] conts)
            {
                var s = SHA1.Create();

                for (var i = 0; i < contents.Count; i++)
                {
                    contents[i].Size = (ulong) conts[i].Length;
                    contents[i].Hash = s.ComputeHash(conts[i]);
                }

                s.Clear();
            }

            private void parseTmd(Stream tmdFile)
            {
                fireDebug("Pasing TMD...");

                tmdFile.Seek(0, SeekOrigin.Begin);
                var temp = new byte[8];

                fireDebug("   Reading Signature Exponent... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 4);
                signatureExponent = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                fireDebug("   Reading Signature... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(signature, 0, signature.Length);

                fireDebug("   Reading Padding... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(padding, 0, padding.Length);

                fireDebug("   Reading Issuer... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(issuer, 0, issuer.Length);

                fireDebug("   Reading Version... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading CA Crl Version... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading Signer Crl Version... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading Padding Byte... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 4);
                version = temp[0];
                caCrlVersion = temp[1];
                signerCrlVersion = temp[2];
                paddingByte = temp[3];

                fireDebug("   Reading Startup IOS... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 8);
                StartupIOS = Shared.Swap(BitConverter.ToUInt64(temp, 0));

                fireDebug("   Reading Title ID... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 8);
                TitleID = Shared.Swap(BitConverter.ToUInt64(temp, 0));

                fireDebug("   Reading Title Type... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 4);
                titleType = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                fireDebug("   Reading Group ID... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 2);
                groupId = Shared.Swap(BitConverter.ToUInt16(temp, 0));

                fireDebug("   Reading Padding2... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 2);
                padding2 = Shared.Swap(BitConverter.ToUInt16(temp, 0));

                fireDebug("   Reading Region... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 2);
                region = Shared.Swap(BitConverter.ToUInt16(temp, 0));

                fireDebug("   Reading Reserved... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(reserved, 0, reserved.Length);

                fireDebug("   Reading Access Rights... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 4);
                accessRights = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                fireDebug("   Reading Title Version... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading NumOfContents... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading Boot Index... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading Padding3... (Offset: 0x{0})", tmdFile.Position.ToString("x8").ToUpper());
                tmdFile.Read(temp, 0, 8);
                TitleVersion = Shared.Swap(BitConverter.ToUInt16(temp, 0));
                NumOfContents = Shared.Swap(BitConverter.ToUInt16(temp, 2));
                bootIndex = Shared.Swap(BitConverter.ToUInt16(temp, 4));
                padding3 = Shared.Swap(BitConverter.ToUInt16(temp, 6));
                tmdFile.Position = 0xb04;

                contents = new List<TMD_Content>();

                //Read Contents
                for (var i = 0; i < NumOfContents; i++)
                {
                    fireDebug("   Reading Content #{0} of {1}... (Offset: 0x{2})", i + 1, NumOfContents, tmdFile.Position.ToString("x8").ToUpper().ToUpper());

                    var tempContent = new TMD_Content {
                        Hash = new byte[20]
                    };

                    tmdFile.Read(temp, 0, 8);
                    tempContent.ContentID = Shared.Swap(BitConverter.ToUInt32(temp, 0));
                    tempContent.Index = Shared.Swap(BitConverter.ToUInt16(temp, 4));
                    tempContent.Type = (ContentType) Shared.Swap(BitConverter.ToUInt16(temp, 6));

                    tmdFile.Read(temp, 0, 8);
                    tempContent.Size = Shared.Swap(BitConverter.ToUInt64(temp, 0));

                    tmdFile.Read(tempContent.Hash, 0, tempContent.Hash.Length);

                    contents.Add(tempContent);
                    var paddingcontent = new byte[12];
                    tmdFile.Read(paddingcontent, 0, 12);
                }

                fireDebug("Pasing TMD Finished...");
            }

            private string calculateNandBlocks()
            {
                var nandSizeMin = 0;
                var nandSizeMax = 0;

                for (var i = 0; i < NumOfContents; i++)
                {
                    nandSizeMax += (int) contents[i].Size;
                    if (contents[i].Type == ContentType.Normal) nandSizeMin += (int) contents[i].Size;
                }

                var blocksMin = (int) Math.Ceiling((double) nandSizeMin / (128 * 1024));
                var blocksMax = (int) Math.Ceiling((double) nandSizeMax / (128 * 1024));

                if (blocksMin == blocksMax) return blocksMax.ToString();

                return string.Format("{0} - {1}", blocksMin, blocksMax);
            }

            #endregion

            #region Events

            /// <summary>
            ///     Fires debugging messages. You may write them into a log file or log textbox.
            /// </summary>
            public event EventHandler<MessageEventArgs> Debug;

            private void fireDebug(string debugMessage, params object[] args)
            {
                var debug = Debug;
                if (debug != null) debug(new object(), new MessageEventArgs(string.Format(debugMessage, args)));
            }

            #endregion
        }

        public class TMD_Content
        {
            private ushort type;

            public uint ContentID { get; set; }

            public ushort Index { get; set; }

            public ContentType Type
            {
                get => (ContentType) type;
                set => type = (ushort) value;
            }

            public ulong Size { get; set; }

            public byte[] Hash { get; set; } = new byte[20];
        }

        public enum CommonKeyType : byte
        {
            Standard = 0x00,
            Korean = 0x01
        }

        public class Ticket : IDisposable
        {
            private byte commonKeyIndex = (byte) CommonKeyType.Standard;
            private byte[] decryptedTitleKey = new byte[16];

            private bool dsitik;
            private uint enableTimeLimit;
            private byte[] encryptedTitleKey = new byte[16];
            private byte[] issuer = new byte[64];
            private byte[] newEncryptedTitleKey = new byte[0];
            private byte newKeyIndex = (byte) CommonKeyType.Standard;
            private byte[] padding = new byte[60];
            private byte padding2;
            private ushort padding3;
            private byte[] padding4 = new byte[88];
            private bool reDecrypt;
            private byte[] signature = new byte[256];

            private uint signatureExponent = 0x00010001;
            private uint timeLimit;
            private ulong titleId;
            private byte[] unknown = new byte[63];
            private byte unknown2;
            private ushort unknown3 = 0xFFFF;
            private ulong unknown4;
            private byte[] unknown5 = new byte[48];
            private byte[] unknown6 = new byte[32]; //0xFF

            /// <summary>
            ///     The Title Key the WADs content is encrypted with.
            /// </summary>
            public byte[] TitleKey
            {
                get => decryptedTitleKey;
                set
                {
                    decryptedTitleKey = value;
                    TitleKeyChanged = true;
                    reDecrypt = false;
                }
            }

            /// <summary>
            ///     Defines which Common Key is used (Standard / Korean).
            /// </summary>
            public CommonKeyType CommonKeyIndex
            {
                get => (CommonKeyType) newKeyIndex;
                set => newKeyIndex = (byte) value;
            }

            /// <summary>
            ///     The Ticket ID.
            /// </summary>
            public ulong TicketID { get; set; }

            /// <summary>
            ///     The Console ID.
            /// </summary>
            public uint ConsoleID { get; set; }

            /// <summary>
            ///     The Title ID.
            /// </summary>
            public ulong TitleID
            {
                get => titleId;
                set
                {
                    titleId = value;
                    if (reDecrypt) reDecryptTitleKey();
                }
            }

            /// <summary>
            ///     Number of DLC.
            /// </summary>
            public ushort NumOfDLC { get; set; }

            /// <summary>
            ///     If true, the Ticket will be fakesigned while saving.
            /// </summary>
            public bool FakeSign { get; set; }

            /// <summary>
            ///     True if the Title Key was changed.
            /// </summary>
            public bool TitleKeyChanged { get; private set; }

            /// <summary>
            ///     If true, the Ticket will utilize the DSi CommonKey.
            /// </summary>
            public bool DSiTicket
            {
                get => dsitik;
                set
                {
                    dsitik = value;
                    decryptTitleKey();
                }
            }

            #region IDisposable Members

            private bool isDisposed;

            ~Ticket()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing && !isDisposed)
                {
                    decryptedTitleKey = null;
                    newEncryptedTitleKey = null;
                    signature = null;
                    padding = null;
                    issuer = null;
                    unknown = null;
                    encryptedTitleKey = null;
                    unknown5 = null;
                    unknown6 = null;
                    padding4 = null;
                }

                isDisposed = true;
            }

            #endregion

            #region Public Functions

            /// <summary>
            ///     Loads a tik file.
            /// </summary>
            /// <param name="pathToTicket"></param>
            /// <returns></returns>
            public static Ticket Load(string pathToTicket)
            {
                return Load(File.ReadAllBytes(pathToTicket));
            }

            /// <summary>
            ///     Loads a tik file.
            /// </summary>
            /// <param name="ticket"></param>
            /// <returns></returns>
            public static Ticket Load(byte[] ticket)
            {
                var tik = new Ticket();
                var ms = new MemoryStream(ticket);

                try
                {
                    tik.parseTicket(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                ms.Dispose();
                return tik;
            }

            /// <summary>
            ///     Loads a tik file.
            /// </summary>
            /// <param name="ticket"></param>
            /// <returns></returns>
            public static Ticket Load(Stream ticket)
            {
                var tik = new Ticket();
                tik.parseTicket(ticket);
                return tik;
            }


            /// <summary>
            ///     Loads a tik file.
            /// </summary>
            /// <param name="pathToTicket"></param>
            public void LoadFile(string pathToTicket)
            {
                LoadFile(File.ReadAllBytes(pathToTicket));
            }

            /// <summary>
            ///     Loads a tik file.
            /// </summary>
            /// <param name="ticket"></param>
            public void LoadFile(byte[] ticket)
            {
                var ms = new MemoryStream(ticket);

                try
                {
                    parseTicket(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                ms.Dispose();
            }

            /// <summary>
            ///     Loads a tik file.
            /// </summary>
            /// <param name="ticket"></param>
            public void LoadFile(Stream ticket)
            {
                parseTicket(ticket);
            }


            /// <summary>
            ///     Saves the Ticket.
            /// </summary>
            /// <param name="savePath"></param>
            public void Save(string savePath)
            {
                Save(savePath, false);
            }

            /// <summary>
            ///     Saves the Ticket. If fakeSign is true, the Ticket will be fakesigned.
            /// </summary>
            /// <param name="savePath"></param>
            /// <param name="fakeSign"></param>
            public void Save(string savePath, bool fakeSign)
            {
                if (fakeSign) this.FakeSign = true;

                if (File.Exists(savePath)) File.Delete(savePath);

                using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    writeToStream(fs);
                }
            }

            /// <summary>
            ///     Returns the Ticket as a memory stream.
            /// </summary>
            /// <returns></returns>
            public MemoryStream ToMemoryStream()
            {
                return ToMemoryStream(false);
            }

            /// <summary>
            ///     Returns the Ticket as a memory stream. If fakeSign is true, the Ticket will be fakesigned.
            /// </summary>
            /// <param name="fakeSign"></param>
            /// <returns></returns>
            public MemoryStream ToMemoryStream(bool fakeSign)
            {
                if (fakeSign) this.FakeSign = true;

                var ms = new MemoryStream();

                try
                {
                    writeToStream(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                return ms;
            }

            /// <summary>
            ///     Returns the Ticket as a byte array.
            /// </summary>
            /// <returns></returns>
            public byte[] ToByteArray()
            {
                return ToByteArray(false);
            }

            /// <summary>
            ///     Returns the Ticket as a byte array. If fakeSign is true, the Ticket will be fakesigned.
            /// </summary>
            /// <param name="fakeSign"></param>
            /// <returns></returns>
            public byte[] ToByteArray(bool fakeSign)
            {
                if (fakeSign) this.FakeSign = true;

                var ms = new MemoryStream();

                try
                {
                    writeToStream(ms);
                }
                catch
                {
                    ms.Dispose();
                    throw;
                }

                var res = ms.ToArray();
                ms.Dispose();
                return res;
            }

            /// <summary>
            ///     This will set a new encrypted Title Key (i.e. the one that you can "read" in the Ticket).
            /// </summary>
            /// <param name="newTitleKey"></param>
            public void SetTitleKey(string newTitleKey)
            {
                SetTitleKey(newTitleKey.ToCharArray());
            }

            /// <summary>
            ///     This will set a new encrypted Title Key (i.e. the one that you can "read" in the Ticket).
            /// </summary>
            /// <param name="newTitleKey"></param>
            public void SetTitleKey(char[] newTitleKey)
            {
                if (newTitleKey.Length != 16) throw new Exception("The title key must be 16 characters long!");

                for (var i = 0; i < 16; i++) encryptedTitleKey[i] = (byte) newTitleKey[i];

                decryptTitleKey();
                TitleKeyChanged = true;

                reDecrypt = true;
                newEncryptedTitleKey = encryptedTitleKey;
            }

            /// <summary>
            ///     This will set a new encrypted Title Key (i.e. the one that you can "read" in the Ticket).
            /// </summary>
            /// <param name="newTitleKey"></param>
            public void SetTitleKey(byte[] newTitleKey)
            {
                if (newTitleKey.Length != 16) throw new Exception("The title key must be 16 characters long!");

                encryptedTitleKey = newTitleKey;
                decryptTitleKey();
                TitleKeyChanged = true;

                reDecrypt = true;
                newEncryptedTitleKey = newTitleKey;
            }

            /// <summary>
            ///     Returns the Upper Title ID as a string.
            /// </summary>
            /// <returns></returns>
            public string GetUpperTitleID()
            {
                var titleBytes = BitConverter.GetBytes(Shared.Swap((uint) titleId));
                return new string(new[] {(char) titleBytes[0], (char) titleBytes[1], (char) titleBytes[2], (char) titleBytes[3]});
            }

            #endregion

            #region Private Functions

            private void writeToStream(Stream writeStream)
            {
                fireDebug("Writing Ticket...");

                fireDebug("   Encrypting Title Key...");
                encryptTitleKey();
                fireDebug("    -> Decrypted Title Key: {0}", Shared.ByteArrayToString(decryptedTitleKey, ' '));
                fireDebug("    -> Encrypted Title Key: {0}", Shared.ByteArrayToString(encryptedTitleKey, ' '));

                if (FakeSign)
                {
                    fireDebug("   Clearing Signature...");
                    signature = new byte[256];
                } //Clear Signature if we fake Sign

                var ms = new MemoryStream();
                ms.Seek(0, SeekOrigin.Begin);

                fireDebug("   Writing Signature Exponent... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(signatureExponent)), 0, 4);

                fireDebug("   Writing Signature... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(signature, 0, signature.Length);

                fireDebug("   Writing Padding... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(padding, 0, padding.Length);

                fireDebug("   Writing Issuer... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(issuer, 0, issuer.Length);

                fireDebug("   Writing Unknown... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(unknown, 0, unknown.Length);

                fireDebug("   Writing Title Key... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(encryptedTitleKey, 0, encryptedTitleKey.Length);

                fireDebug("   Writing Unknown2... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(unknown2);

                fireDebug("   Writing Ticket ID... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(TicketID)), 0, 8);

                fireDebug("   Writing Console ID... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(ConsoleID)), 0, 4);

                fireDebug("   Writing Title ID... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(titleId)), 0, 8);

                fireDebug("   Writing Unknwon3... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(unknown3)), 0, 2);

                fireDebug("   Writing NumOfDLC... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(NumOfDLC)), 0, 2);

                fireDebug("   Writing Unknwon4... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(unknown4)), 0, 8);

                fireDebug("   Writing Padding2... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(padding2);

                fireDebug("   Writing Common Key Index... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.WriteByte(commonKeyIndex);

                fireDebug("   Writing Unknown5... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(unknown5, 0, unknown5.Length);

                fireDebug("   Writing Unknown6... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(unknown6, 0, unknown6.Length);

                fireDebug("   Writing Padding3... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(padding3)), 0, 2);

                fireDebug("   Writing Enable Time Limit... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(enableTimeLimit)), 0, 4);

                fireDebug("   Writing Time Limit... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(BitConverter.GetBytes(Shared.Swap(timeLimit)), 0, 4);

                fireDebug("   Writing Padding4... (Offset: 0x{0})", ms.Position.ToString("x8").ToUpper());
                ms.Write(padding4, 0, padding4.Length);

                var tik = ms.ToArray();
                ms.Dispose();

                //fake Sign
                if (FakeSign)
                {
                    fireDebug("   Fakesigning Ticket...");

                    var hash = new byte[20];
                    var s = SHA1.Create();

                    for (ushort i = 0; i < 0xFFFF; i++)
                    {
                        var bytes = BitConverter.GetBytes(i);
                        tik[498] = bytes[1];
                        tik[499] = bytes[0];

                        hash = s.ComputeHash(tik);
                        if (hash[0] == 0x00)
                        {
                            fireDebug("   -> Signed ({0})", i);
                            break;
                        } //Win! It's signed...

                        if (i == 0xFFFF - 1)
                        {
                            fireDebug("    -> Signing Failed...");
                            throw new Exception("Fakesigning failed...");
                        }
                    }

                    s.Clear();
                }

                writeStream.Seek(0, SeekOrigin.Begin);
                writeStream.Write(tik, 0, tik.Length);

                fireDebug("Writing Ticket Finished...");
            }

            private void parseTicket(Stream ticketFile)
            {
                fireDebug("Parsing Ticket...");

                ticketFile.Seek(0, SeekOrigin.Begin);
                var temp = new byte[8];

                fireDebug("   Reading Signature Exponent... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 4);
                signatureExponent = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                fireDebug("   Reading Signature... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(signature, 0, signature.Length);

                fireDebug("   Reading Padding... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(padding, 0, padding.Length);

                fireDebug("   Reading Issuer... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(issuer, 0, issuer.Length);

                fireDebug("   Reading Unknown... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(unknown, 0, unknown.Length);

                fireDebug("   Reading Title Key... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(encryptedTitleKey, 0, encryptedTitleKey.Length);

                fireDebug("   Reading Unknown2... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                unknown2 = (byte) ticketFile.ReadByte();

                fireDebug("   Reading Ticket ID.. (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 8);
                TicketID = Shared.Swap(BitConverter.ToUInt64(temp, 0));

                fireDebug("   Reading Console ID... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 4);
                ConsoleID = Shared.Swap(BitConverter.ToUInt32(temp, 0));

                fireDebug("   Reading Title ID... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 8);
                titleId = Shared.Swap(BitConverter.ToUInt64(temp, 0));

                fireDebug("   Reading Unknown3... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading NumOfDLC... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 4);
                unknown3 = Shared.Swap(BitConverter.ToUInt16(temp, 0));
                NumOfDLC = Shared.Swap(BitConverter.ToUInt16(temp, 2));

                fireDebug("   Reading Unknown4... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 8);
                unknown4 = Shared.Swap(BitConverter.ToUInt64(temp, 0));

                fireDebug("   Reading Padding2... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                padding2 = (byte) ticketFile.ReadByte();

                fireDebug("   Reading Common Key Index... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                commonKeyIndex = (byte) ticketFile.ReadByte();

                newKeyIndex = commonKeyIndex;

                fireDebug("   Reading Unknown5... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(unknown5, 0, unknown5.Length);

                fireDebug("   Reading Unknown6... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(unknown6, 0, unknown6.Length);

                fireDebug("   Reading Padding3... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 2);
                padding3 = Shared.Swap(BitConverter.ToUInt16(temp, 0));

                fireDebug("   Reading Enable Time Limit... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                fireDebug("   Reading Time Limit... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(temp, 0, 8);
                enableTimeLimit = Shared.Swap(BitConverter.ToUInt32(temp, 0));
                timeLimit = Shared.Swap(BitConverter.ToUInt32(temp, 4));

                fireDebug("   Reading Padding4... (Offset: 0x{0})", ticketFile.Position.ToString("x8").ToUpper());
                ticketFile.Read(padding4, 0, padding4.Length);

                fireDebug("   Decrypting Title Key...");
                decryptTitleKey();
                fireDebug("    -> Encrypted Title Key: {0}", Shared.ByteArrayToString(encryptedTitleKey, ' '));
                fireDebug("    -> Decrypted Title Key: {0}", Shared.ByteArrayToString(decryptedTitleKey, ' '));

                fireDebug("Parsing Ticket Finished...");
            }

            private void decryptTitleKey()
            {
                byte[] ckey = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                var iv = BitConverter.GetBytes(Shared.Swap(titleId));
                Array.Resize(ref iv, 16);

                var rm = new RijndaelManaged {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.None,
                    KeySize = 128,
                    BlockSize = 128,
                    Key = ckey,
                    IV = iv
                };

                var decryptor = rm.CreateDecryptor();

                var ms = new MemoryStream(encryptedTitleKey);
                var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

                cs.Read(decryptedTitleKey, 0, decryptedTitleKey.Length);

                cs.Dispose();
                ms.Dispose();
                decryptor.Dispose();
                rm.Clear();
            }

            private void encryptTitleKey()
            {
                commonKeyIndex = newKeyIndex;
                byte[] ckey = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                var iv = BitConverter.GetBytes(Shared.Swap(titleId));
                Array.Resize(ref iv, 16);

                var rm = new RijndaelManaged {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.None,
                    KeySize = 128,
                    BlockSize = 128,
                    Key = ckey,
                    IV = iv
                };

                var encryptor = rm.CreateEncryptor();

                var ms = new MemoryStream(decryptedTitleKey);
                var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Read);

                cs.Read(encryptedTitleKey, 0, encryptedTitleKey.Length);

                cs.Dispose();
                ms.Dispose();
                encryptor.Dispose();
                rm.Clear();
            }

            private void reDecryptTitleKey()
            {
                encryptedTitleKey = newEncryptedTitleKey;
                decryptTitleKey();
            }

            #endregion

            #region Events

            /// <summary>
            ///     Fires debugging messages. You may write them into a log file or log textbox.
            /// </summary>
            public event EventHandler<MessageEventArgs> Debug;

            private void fireDebug(string debugMessage, params object[] args)
            {
                var debug = Debug;
                if (debug != null) debug(new object(), new MessageEventArgs(string.Format(debugMessage, args)));
            }

            #endregion
        }
    }
}