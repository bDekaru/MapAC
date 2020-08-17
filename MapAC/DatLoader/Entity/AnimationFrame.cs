using System;
using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class AnimationFrame : IUnpackable
    {
        public List<Frame> Frames { get; } = new List<Frame>();
        public List<AnimationHook> Hooks { get; } = new List<AnimationHook>();

        /// <summary>
        /// You must use the Unpack(BinaryReader reader, int numParts) method.
        /// </summary>
        /// <exception cref="NotSupportedException">You must use the Unpack(BinaryReader reader, int numParts) method.</exception>
        public void Unpack(BinaryReader reader)
        {
            throw new NotSupportedException();
        }

        public void Unpack(BinaryReader reader, uint numParts)
        {
            Frames.Unpack(reader, numParts);

            uint numHooks = reader.ReadUInt32();
            for (uint i = 0; i < numHooks; i++)
            {
                var hook = AnimationHook.ReadHook(reader);
                Hooks.Add(hook);
            }
        }

        public void Pack(BinaryWriter writer)
        {
            for(var i = 0; i < Frames.Count; i++)
                Frames[i].Pack(writer);

            writer.Write(Hooks.Count);
            for (int i = 0; i < Hooks.Count; i++)
            {
                Hooks[i].Pack(writer);
            }
        }

    }
}
