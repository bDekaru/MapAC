using System.IO;
using System.Numerics;

namespace MapAC.DatLoader.Entity
{
    public class Sphere : IUnpackable
    {
        public Vector3 Origin { get; private set; }
        public float Radius { get; private set; }

        public void Unpack(BinaryReader reader)
        {
            Origin = reader.ReadVector3();
            Radius = reader.ReadSingle();
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(Origin);
            writer.Write(Radius);
        }

        public static Sphere CreateDummySphere()
        {
            var sphere = new Sphere();
            sphere.Origin = Vector3.Zero;
            return sphere;
        }
    }
}
