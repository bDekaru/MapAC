using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class ChatEmoteData : IUnpackable
    {
        public string MyEmote; // What the emote string is to the character doing the emote
        public string OtherEmote; // What the emote string is to other characters

        public void Unpack(BinaryReader reader)
        {
            MyEmote = reader.ReadPString(); reader.AlignBoundary();
            OtherEmote = reader.ReadPString(); reader.AlignBoundary();
        }
    }
}
