using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class PhysicsScriptTableData : IUnpackable
    {
        public List<ScriptAndModData> Scripts { get; } = new List<ScriptAndModData>();

        public void Unpack(BinaryReader reader)
        {
            Scripts.Unpack(reader);
        }
    }
}
