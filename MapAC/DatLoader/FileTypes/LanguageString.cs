using MapAC.DatLoader.Enum;
using System.IO;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_local_English.dat files starting with 0x31.
    /// This is called a "String" in the client; It has been renamed to avoid conflicts with the generic "String" class.
    /// </summary>
    [DatFileType(DatFileType.String)]
    public class LanguageString : FileType
    {
        public string CharBuffer;

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            uint strLen = reader.ReadCompressedUInt32();
            if (strLen > 0)
            {
                byte[] thestring = reader.ReadBytes((int)strLen);
                CharBuffer = System.Text.Encoding.Default.GetString(thestring);
            }
            else
                CharBuffer = "";
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.WriteOffset(Id, ACDMOffset.String);
            writer.WriteCompressedUInt32((uint)CharBuffer.Length);
            if (CharBuffer.Length > 0)
            {
                byte[] stringBytes = System.Text.Encoding.Default.GetBytes(CharBuffer);
                writer.Write(stringBytes);
            }
        }
    }
}
