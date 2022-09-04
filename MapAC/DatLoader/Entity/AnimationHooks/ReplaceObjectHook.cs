using MapAC.DatLoader.Enum;
using System;
using System.IO;
using System.Windows.Forms;

namespace MapAC.DatLoader.Entity.AnimationHooks
{
    public class ReplaceObjectHook : AnimationHook
    {
        public AnimationPartChange APChange { get; } = new AnimationPartChange();

        public byte Unknown;

        public override void Unpack(BinaryReader reader)
        {
            base.Unpack(reader);

            // The structure of AnimationPartChange here is slightly different for some reason than the other imeplementations.
            // So we'll read in the 2-byte PartIndex and send that to our other implementation of the Unpack function.
            byte apChangePartIndex = reader.ReadByte();
            Unknown = reader.ReadByte();
            //reader.BaseStream.Position -= 2;
            //ushort apChangePartIndex = reader.ReadUInt16();

            APChange.Unpack(reader, apChangePartIndex);
        }

        public override void Pack(BinaryWriter writer)
        {
            base.Pack(writer);
            writer.Write(APChange.PartIndex);
            writer.Write(Unknown);
            if (Export.IsAddition(APChange.PartID))
                writer.WriteAsDataIDOfKnownType(APChange.PartID + (uint)ACDMOffset.GfxObj, 0x01000000);
            else
                writer.WriteAsDataIDOfKnownType(APChange.PartID, 0x01000000);
        }
    }
}
