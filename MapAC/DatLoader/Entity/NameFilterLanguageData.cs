using System.Collections.Generic;
using System.IO;

namespace MapAC.DatLoader.Entity
{
    public class NameFilterLanguageData : IUnpackable
    {
        public uint MaximumVowelsInARow; 
        public uint FirstNCharactersMustHaveAVowel;
        public uint VowelContainingSubstringLength;
        public uint ExtraAllowedCharacters;
        public byte Unknown;

        public List<string> CompoundLetterGroups = new List<string>();

        public void Unpack(BinaryReader reader)
        {
            MaximumVowelsInARow = reader.ReadUInt32();
            FirstNCharactersMustHaveAVowel = reader.ReadUInt32();
            VowelContainingSubstringLength = reader.ReadUInt32();
            ExtraAllowedCharacters = reader.ReadUInt32();

            Unknown = reader.ReadByte(); // Not sure what this is...

            uint numLetterGroup = reader.ReadUInt32();
            for (uint i = 0; i < numLetterGroup; i++)
                CompoundLetterGroups.Add(reader.ReadUnicodeString());
        }

        public void Pack(BinaryWriter writer)
        {
            writer.Write(MaximumVowelsInARow);
            writer.Write(FirstNCharactersMustHaveAVowel);
            writer.Write(VowelContainingSubstringLength);
            writer.Write(ExtraAllowedCharacters);

            writer.Write(Unknown);

            writer.Write(CompoundLetterGroups.Count);
            for (int i = 0; i < CompoundLetterGroups.Count; i++)
                writer.WriteObfuscatedString(CompoundLetterGroups[i]);
        }

    }
}
