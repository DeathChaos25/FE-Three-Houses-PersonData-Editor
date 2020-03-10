using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeHousesPersonDataEditor
{
    using PersonData.Sections;

    class PersonDataFile
    {
        public uint numOfPointers { get; set; }
        public uint[] SectionPointers { get; set; }
        public uint[] SectionTotalSize { get; set; }

        // Header stuff for Misc Section
        public uint[] SectionMagic { get; set; }
        public uint[] SectionBlockCount { get; set; }
        public uint[] SectionBlockSize { get; set; }
        public CharacterBlocks CharacterSection { get; set; }
        public List<CharacterBlocks> Character { get; set; }
        public AssetIDBlock AssetIDSection { get; set; }
        public List<AssetIDBlock> AssetID { get; set; }

        public void ReadPersonData(string fixed_persondata_path)
        {
            SectionPointers = new uint[20];
            SectionTotalSize = new uint[20];
            SectionMagic = new uint[20];
            SectionBlockCount = new uint[20];
            SectionBlockSize = new uint[20];
            using (EndianBinaryReader fixed_persondata = new EndianBinaryReader(fixed_persondata_path, Endianness.Little))
            {
                numOfPointers = fixed_persondata.ReadUInt32();
                if (numOfPointers == 18)
                {

                    for (int i = 0; i < numOfPointers; i++)
                    {
                        SectionPointers[i] = fixed_persondata.ReadUInt32();
                        SectionTotalSize[i] = fixed_persondata.ReadUInt32();
                    }

                    // For Misc Section, Header of each section
                    for (int i = 0; i < numOfPointers; i++)
                    {
                        fixed_persondata.Seek(SectionPointers[i], SeekOrigin.Begin);
                        SectionMagic[i] = fixed_persondata.ReadUInt32();
                        SectionBlockCount[i] = fixed_persondata.ReadUInt32();
                        SectionBlockSize[i] = fixed_persondata.ReadUInt32();
                    }

                    // Section 0 is Character Block, data such as base stats and growths
                    fixed_persondata.Seek(SectionPointers[0], SeekOrigin.Begin);
                    fixed_persondata.SeekCurrent(0x40);  //skip header cus we already stored info

                    Character = new List<CharacterBlocks>();
                    for (int i = 0; i < SectionBlockCount[0]; i++)
                    {
                        CharacterSection = new CharacterBlocks();
                        CharacterSection.Read(fixed_persondata);
                        Character.Add(CharacterSection);
                    }

                    // Section 1 is Asset ID data
                    fixed_persondata.Seek(SectionPointers[1], SeekOrigin.Begin);
                    fixed_persondata.SeekCurrent(0x40);  //skip header cus we already stored info

                    AssetID = new List<AssetIDBlock>();
                    for (int i = 0; i < SectionBlockCount[1]; i++)
                    {
                        AssetIDSection = new AssetIDBlock();
                        AssetIDSection.Read(fixed_persondata);
                        AssetID.Add(AssetIDSection);
                    }
                }
            }
        }
        
        public void WritePersonData(EndianBinaryWriter fixed_persondata)
        {
            //Write bingz header
            fixed_persondata.WriteUInt32(18);
            for (int i = 0; i < 18; i++)
            {
                fixed_persondata.WriteUInt32(SectionPointers[i]);
                fixed_persondata.WriteUInt32(SectionTotalSize[i]);
            }

            //Under construction, for now we write empty sections for the rest of the file
            //So I can build a "valid" persondata with whatever sections are done
            //So I can test how well the program works as each section is added
            for (int i = 0; i < 18; i++)
            {
                fixed_persondata.Seek(SectionPointers[i], SeekOrigin.Begin);
                fixed_persondata.WriteUInt32(SectionMagic[i]);
                fixed_persondata.WriteUInt32(SectionBlockCount[i]);
                fixed_persondata.WriteUInt32(SectionBlockSize[i]);
                fixed_persondata.WritePadding(0x34);
                if (i == 0)
                {
                    foreach (var character in Character)
                    {
                        character.Write(fixed_persondata);
                    }
                }
                else if (i == 1)
                {
                    foreach (var assetid in AssetID)
                    {
                        assetid.Write(fixed_persondata);
                    }
                }
                else
                {
                    fixed_persondata.WritePadding(SectionBlockCount[i] * SectionBlockSize[i]);
                }
            }
        }
    }
}
