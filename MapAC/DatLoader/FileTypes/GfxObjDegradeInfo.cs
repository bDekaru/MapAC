using System.Collections.Generic;
using System.IO;

using MapAC.DatLoader.Entity;
using MapAC.DatLoader.Enum;

namespace MapAC.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x11. 
    /// Contains info on what objects to display at what distance to help with render performance (e.g. low-poly very far away, but high-poly when close)
    /// </summary>
    [DatFileType(DatFileType.DegradeInfo)]
    public class GfxObjDegradeInfo : FileType
    {
        public List<GfxObjInfo> Degrades { get; } = new List<GfxObjInfo>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Degrades.Unpack(reader);
        }

        public override void Pack(BinaryWriter writer)
        {
            writer.Write(Id);
            if(DatManager.DatVersion == DatVersionType.ACDM)
            {
                writer.Write(Degrades.Count);
                for (int i = 0; i < Degrades.Count; i++)
                {
                    var gfxDegrade = Degrades[i];
                    if (Export.IsAddition(gfxDegrade.Id) && DatManager.CellDat.ExistsInEoR(gfxDegrade.Id))
                        gfxDegrade.Id += (uint)ACDMOffset.DIDDegrade;
                    gfxDegrade.Pack(writer);
                }
            }
            else
                Degrades.Pack(writer);
        }
    }
}
