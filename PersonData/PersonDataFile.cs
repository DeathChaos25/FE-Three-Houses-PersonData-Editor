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
        public int numOfPointers { get; set; }
        public int[] SectionPointers { get; set; }
        public int[] SectionTotalSize { get; set; }

        // Header stuff for Misc Section
        public int[] SectionMagic { get; set; }
        public int[] SectionBlockCount { get; set; }
        public int[] SectionBlockSize { get; set; }
        public CharacterBlocks CharacterSection { get; set; }
        public List<CharacterBlocks> Character { get; private set; }

        public void ReadPersonData(string fixed_persondata_path)
        {
            SectionPointers = new int[20];
            SectionTotalSize = new int[20];
            SectionMagic = new int[20];
            SectionBlockCount = new int[20];
            SectionBlockSize = new int[20];
            using (EndianBinaryReader fixed_persondata = new EndianBinaryReader(fixed_persondata_path, Endianness.Little))
            {
                numOfPointers = fixed_persondata.ReadInt32();
                if (numOfPointers == 18)
                {

                    for (int i = 0; i < numOfPointers; i++)
                    {
                        SectionPointers[i] = fixed_persondata.ReadInt32();
                        SectionTotalSize[i] = fixed_persondata.ReadInt32();
                    }

                    // For Misc Section, Header of each section
                    for (int i = 0; i < numOfPointers; i++)
                    {
                        fixed_persondata.Seek(SectionPointers[i], SeekOrigin.Begin);
                        SectionMagic[i] = fixed_persondata.ReadInt32();
                        SectionBlockCount[i] = fixed_persondata.ReadInt32();
                        SectionBlockSize[i] = fixed_persondata.ReadInt32();
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
                }
            }
        }
    }
}
