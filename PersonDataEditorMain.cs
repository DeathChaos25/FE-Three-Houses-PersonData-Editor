using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeHousesPersonDataEditor
{
    using PersonData.Sections;
    using System.Reflection;

    public partial class PersonDataEditorMain : Form
    {
        private PersonDataFile currentPersonData;
        public PersonDataEditorMain()
        {
            InitializeComponent();
        }
        public ushort numOfmsgDataPointers { get; set; }
        public uint msgDataPointer { get; set; }
        public List<String> msgDataNames { get; private set; }

        string[] SectionNames = new string[] { "Character Data", "Asset IDs", "Voice IDs", "Skill Levels", "Spell Learnset", "Skill Learnset", "Starting Inventory", "Combat Arts Learnset", "Support Bonuses", "Support Bonuses", "Support List", "Budding Talents", "Generic Learnset", "Faculty Teachings", "Seminar Teachings", "Character Goals", "Portrait IDs", "Enemy Personal Skills" };
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            var nameOfFile = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "fixed_persondata.bin (*.bin)|*.bin|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Look for fixed_persondata.bin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    nameOfFile = Path.GetFileName(filePath);

                    //Reset Stuff
                    PointersListBox.Items.Clear();

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    currentPersonData = new PersonDataFile();
                    currentPersonData.ReadPersonData(filePath);

                    //Write Info in Misc Section
                    if (currentPersonData.numOfPointers == 18)
                    {
                        tabControl1.TabPages.Add(CharacterBlocksTab);
                        tabControl1.TabPages.Add(MiscInfoTab);
                        for (int i = 0; i < currentPersonData.numOfPointers; i++)
                        {
                            FillMiscSection(i);
                        }

                        //Load msgData en_US section with names
                        Assembly myAssembly = Assembly.GetExecutingAssembly();
                        Stream msgDataStream = myAssembly.GetManifestResourceStream("ThreeHousesPersonDataEditor.msgData.en_US.bin");
                        using (EndianBinaryReader msgData = new EndianBinaryReader(msgDataStream, Endianness.Little))
                        {
                            msgData.SeekCurrent(0x8); // skip header, we don't care
                            numOfmsgDataPointers = msgData.ReadUInt16();
                            msgDataNames = new List<String>();

                            //store all strings in msgData on List
                            for (int i = 0; i < numOfmsgDataPointers; i++)
                            {
                                msgData.Seek(0x14 + (4 * i), SeekOrigin.Begin);
                                msgDataPointer = msgData.ReadUInt32();
                                msgData.Seek(msgDataPointer + 0x14, SeekOrigin.Begin);
                                msgDataNames.Add(msgData.ReadString(StringBinaryFormat.NullTerminated));
                            }
                            //read List for character names
                            for (int i = 0; i < currentPersonData.SectionBlockCount[0]; i++)
                            {
                                characterListBox.Items.Add(i.ToString("D" + 4) + " : " + msgDataNames[(currentPersonData.Character[i].nameID) + 1157]);
                            }
                            //read List for class names
                            for (int i = 0; i <= 100; i++)
                            {
                                classComboBox.Items.Add(msgDataNames[i + 3453]);
                            }

                            //read List for allegiances
                            for (int i = 0; i <= 30; i++)
                            {
                                allegianceComboBox.Items.Add(msgDataNames[i + 9498]);
                            }

                            //read List for Crest Names
                            for (int i = 0; i <= 85; i++)
                            {
                                crest1ComboBox.Items.Add(msgDataNames[i + 9590]);
                                crest2ComboBox.Items.Add(msgDataNames[i + 9590]);
                            }
                            crest1ComboBox.Items.Add("No Crest");
                            crest2ComboBox.Items.Add("No Crest");

                            //read List for battalion names
                            for (int i = 0; i <= 200; i++)
                            {
                                battalionComboBox.Items.Add(msgDataNames[i + 9096]);
                            }
                        }
                    }
                    else
                    {
                        ResetLabels();
                        return;
                    }
                }
                else return;
            }
        }

        private void PointersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pointerLabel.Text = "Pointer to Section: " + currentPersonData.SectionPointers[PointersListBox.SelectedIndex].ToString();
            totalSizeLabel.Text = "Total Size of Section: " + currentPersonData.SectionTotalSize[PointersListBox.SelectedIndex].ToString();

            // Nonsensical stuff pls ignore
            charsectFileMagicLabel.Text = "File Magic: " + currentPersonData.SectionMagic[PointersListBox.SelectedIndex].ToString("X8");
            charsectNumOfBlocksLabel.Text = "Number of Blocks: " + currentPersonData.SectionBlockCount[PointersListBox.SelectedIndex].ToString();
            charsectBlockSizeLabel.Text = "Size of Each Block: " + currentPersonData.SectionBlockSize[PointersListBox.SelectedIndex].ToString();
        }
        // Not Necessary, Misc Section info
        private void FillMiscSection(int j)
        {
            numOfPointersLabel.Text = "Number of Pointers: " + currentPersonData.numOfPointers.ToString();
            PointersListBox.Items.Add(SectionNames[j]);
        }

        public void ResetLabels()
        {
            numOfPointersLabel.Text = "Number of Pointers: 0";
            pointerLabel.Text = "Pointer to Section: 0";
            totalSizeLabel.Text = "Total Size of Section: 0";
            charsectFileMagicLabel.Text = "File Magic: 00000000";
            charsectNumOfBlocksLabel.Text = "Number of Blocks: 0";
            charsectBlockSizeLabel.Text = "Size of Each Block: 0";
            MessageBox.Show("This file is not a valid PersonData file", "Invalid file");
            //Hide Tabs to prevent data being set
            tabControl1.TabPages.Remove(CharacterBlocksTab);
            tabControl1.TabPages.Remove(MiscInfoTab);
            characterListBox.Items.Clear();
        }
        private void PersonDataEditorMain_Load(object sender, EventArgs e)
        {
            //Hide Tabs to prevent data being set
            tabControl1.TabPages.Remove(CharacterBlocksTab);
            tabControl1.TabPages.Remove(MiscInfoTab);

            //Hide Tabs under construction
            tabControl1.TabPages.Remove(AssetIDTab);
            tabControl1.TabPages.Remove(PortraitIDTab);
            tabControl1.TabPages.Remove(VoiceIDTab);
            tabControl1.TabPages.Remove(SupportBonuses1Tab);
            tabControl1.TabPages.Remove(SupportBonuses2Tab);
            tabControl1.TabPages.Remove(SupportListTab);
            tabControl1.TabPages.Remove(StartingInventoryTab);
            tabControl1.TabPages.Remove(SeminarTeachingTab);
            tabControl1.TabPages.Remove(FacultyTeachingTab);
            tabControl1.TabPages.Remove(SpellLearnsetTab);
            tabControl1.TabPages.Remove(SkillLearnsetTab);
            tabControl1.TabPages.Remove(SkillLevelsTab);
            tabControl1.TabPages.Remove(CharacterGoalsTab);
            tabControl1.TabPages.Remove(BuddingTalentsTab);
            tabControl1.TabPages.Remove(CombatArtsTab);
            tabControl1.TabPages.Remove(GenericLearnsetTab);
            tabControl1.TabPages.Remove(EnemyPersonalSkillTab);
        }

        private void characterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            height1NumberBox.Text = currentPersonData.Character[characterListBox.SelectedIndex].height1.ToString();
            height2NumberBox.Text = currentPersonData.Character[characterListBox.SelectedIndex].height2.ToString();
            ageNumberBox.Text = currentPersonData.Character[characterListBox.SelectedIndex].age.ToString();
            birthMonthNumberBox.Text = currentPersonData.Character[characterListBox.SelectedIndex].month.ToString();
            birthDayNumberBox.Text = currentPersonData.Character[characterListBox.SelectedIndex].birthDay.ToString();

            //Load Character Portrait
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("ThreeHousesPersonDataEditor.Face.unk.bmp");
            if (currentPersonData.Character[characterListBox.SelectedIndex].assetID <= 56 || currentPersonData.Character[characterListBox.SelectedIndex].assetID == 85 || (currentPersonData.Character[characterListBox.SelectedIndex].assetID >= 500 && currentPersonData.Character[characterListBox.SelectedIndex].assetID <= 507))
            {
                myStream = myAssembly.GetManifestResourceStream("ThreeHousesPersonDataEditor.Face.Face_" + currentPersonData.Character[characterListBox.SelectedIndex].assetID.ToString() + ".bmp");
            }
            Bitmap face = new Bitmap(myStream);
            facePicBox.Image = face;

            //Combo Boxes
            classComboBox.SelectedIndex = currentPersonData.Character[characterListBox.SelectedIndex].classID;
            allegianceComboBox.SelectedIndex = currentPersonData.Character[characterListBox.SelectedIndex].allegiance;
            genderComboBox.SelectedIndex = currentPersonData.Character[characterListBox.SelectedIndex].gender;
            crest1ComboBox.SelectedIndex = currentPersonData.Character[characterListBox.SelectedIndex].crest1;
            crest2ComboBox.SelectedIndex = currentPersonData.Character[characterListBox.SelectedIndex].crest2;
            battalionComboBox.SelectedIndex = currentPersonData.Character[characterListBox.SelectedIndex].baseBattalion;

            //Build the full character name
            //Checking for empty middle names, because the way KT constructs the full name is very dumb
            if (msgDataNames[currentPersonData.Character[characterListBox.SelectedIndex].nameID + 1731] == "")
            {
                nameTextBox.Text = msgDataNames[currentPersonData.Character[characterListBox.SelectedIndex].nameID + 1157] + " " + msgDataNames[currentPersonData.Character[characterListBox.SelectedIndex].nameID + 2305];
            }
            else nameTextBox.Text = msgDataNames[currentPersonData.Character[characterListBox.SelectedIndex].nameID + 1157] + " " + msgDataNames[currentPersonData.Character[characterListBox.SelectedIndex].nameID + 1731] + " " + msgDataNames[currentPersonData.Character[characterListBox.SelectedIndex].nameID + 2305];

            //Stats stuff
            //Base Stats
            baseHPNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseHP;
            baseStrNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseStr;
            baseMagNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseMag;
            baseDexNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseDex;
            baseSpdNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseSpd;
            baseLckNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseLck;
            baseDefNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseDef;
            baseResNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseRes;
            baseMovNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseMov;
            baseChaNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].baseCha;

            //Growths
            HPGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].hpGrowth;
            StrGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].strGrowth;
            MagGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].magGrowth;
            DexGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].dexGrowth;
            SpdGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].spdGrowth;
            LckGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].lckGrowth;
            DefGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].defGrowth;
            ResGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].resGrowth;
            MovGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].movGrowth;
            ChaGrowthNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].chaGrowth;

            //Max Stats
            MaxHPNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxHP;
            MaxStrNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxStr;
            MaxMagNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxMag;
            MaxDexNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxDex;
            MaxSpdNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxSpd;
            MaxLckNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxLck;
            MaxDefNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxDef;
            MaxResNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxRes;
            MaxMovNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxMov;
            MaxChaNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].maxCha;

            //Scale values
            chestSize1NumBox.Value = Convert.ToDecimal(currentPersonData.Character[characterListBox.SelectedIndex].chestSize1);
            chestSize2NumBox.Value = Convert.ToDecimal(currentPersonData.Character[characterListBox.SelectedIndex].chestSize2);
            chestWidthScale.Value = Convert.ToDecimal(currentPersonData.Character[characterListBox.SelectedIndex].chestBandMod);
            modelScaleNumBox.Value = Convert.ToDecimal(currentPersonData.Character[characterListBox.SelectedIndex].modelScale);

            //Other Section
            unk0x10NumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x10;
            unk0x14NumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x14;
            unk0x1FNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x1F;
            unk0x21NumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x21;
            unk0x23NumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x23;
            unk0x25NumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x25;
            unk0x2ENumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x2E;
            unk0x31NumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].unk_0x31;

            NameIDNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].nameID;
            voiceIDNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].voiceID;
            assetIDNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].assetID;
            birthdayFlagNumbox.Value = currentPersonData.Character[characterListBox.SelectedIndex].birthDayFlag;
            saveDataIDNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].saveDataID;
            bodyTypeNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].bodyType;
            nonCombatAnimNumBox.Value = currentPersonData.Character[characterListBox.SelectedIndex].nonCombatAnimSet;
        }

        private void height1NumberBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].height1 = Decimal.ToByte(height1NumberBox.Value);
        }

        private void height2NumberBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].height2 = Decimal.ToByte(height2NumberBox.Value);
        }

        private void birthMonthNumberBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].month = Decimal.ToByte(birthMonthNumberBox.Value);
        }

        private void birthDayNumberBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].birthDay = Decimal.ToByte(birthDayNumberBox.Value);
        }

        private void ageNumberBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].age = Decimal.ToByte(ageNumberBox.Value);
        }

        private void baseHPNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseHP = Decimal.ToByte(baseHPNumBox.Value);
        }

        private void baseStrNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseStr = Decimal.ToByte(baseStrNumBox.Value);
        }

        private void baseMagNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseMag = Decimal.ToByte(baseMagNumBox.Value);
        }

        private void baseDexNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseDex = Decimal.ToByte(baseDexNumBox.Value);
        }

        private void baseSpdNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseSpd = Decimal.ToByte(baseSpdNumBox.Value);
        }

        private void baseLckNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseLck = Decimal.ToByte(baseLckNumBox.Value);
        }

        private void baseDefNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseDef = Decimal.ToByte(baseDefNumBox.Value);
        }

        private void baseResNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseRes = Decimal.ToByte(baseResNumBox.Value);
        }

        private void baseMovNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseMov = Decimal.ToByte(baseMovNumBox.Value);
        }

        private void baseChaNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseCha = Decimal.ToByte(baseChaNumBox.Value);
        }

        private void HPGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].hpGrowth = Decimal.ToByte(HPGrowthNumBox.Value);
        }

        private void StrGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].strGrowth = Decimal.ToByte(StrGrowthNumBox.Value);
        }

        private void MagGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].magGrowth = Decimal.ToByte(MagGrowthNumBox.Value);
        }

        private void DexGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].dexGrowth = Decimal.ToByte(DexGrowthNumBox.Value);
        }

        private void SpdGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].spdGrowth = Decimal.ToByte(SpdGrowthNumBox.Value);
        }

        private void LckGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].lckGrowth = Decimal.ToByte(LckGrowthNumBox.Value);
        }

        private void DefGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].defGrowth = Decimal.ToByte(DefGrowthNumBox.Value);
        }

        private void ResGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].resGrowth = Decimal.ToByte(ResGrowthNumBox.Value);
        }

        private void MovGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].movGrowth = Decimal.ToByte(MovGrowthNumBox.Value);
        }

        private void ChaGrowthNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].chaGrowth = Decimal.ToByte(ChaGrowthNumBox.Value);
        }

        private void MaxHPNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxHP = Decimal.ToByte(MaxHPNumBox.Value);
        }

        private void MaxStrNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxStr = Decimal.ToByte(MaxStrNumBox.Value);
        }

        private void MaxMagNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxMag = Decimal.ToByte(MaxMagNumBox.Value);
        }

        private void MaxDexNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxDex = Decimal.ToByte(MaxDexNumBox.Value);
        }

        private void MaxSpdNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxSpd = Decimal.ToByte(MaxSpdNumBox.Value);
        }

        private void MaxLckNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxLck = Decimal.ToByte(MaxLckNumBox.Value);
        }

        private void MaxDefNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxDef = Decimal.ToByte(MaxDefNumBox.Value);
        }

        private void MaxResNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxRes = Decimal.ToByte(MaxResNumBox.Value);
        }

        private void MaxMovNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxMov = Decimal.ToByte(MaxMovNumBox.Value);
        }

        private void MaxChaNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].maxCha = Decimal.ToByte(MaxChaNumBox.Value);
        }

        private void classComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].classID = Convert.ToByte(classComboBox.SelectedIndex);
        }

        private void allegianceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].allegiance = Convert.ToByte(allegianceComboBox.SelectedIndex);
        }

        private void chestSize1NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].chestSize1 = Decimal.ToSingle(chestSize1NumBox.Value);
        }

        private void chestSize2NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].chestSize2 = Decimal.ToSingle(chestSize2NumBox.Value);
        }

        private void chestWidthScale_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].chestBandMod = Decimal.ToSingle(chestWidthScale.Value);
        }

        private void modelScaleNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].modelScale = Decimal.ToSingle(modelScaleNumBox.Value);
        }

        private void genderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].gender = Convert.ToByte(genderComboBox.SelectedIndex);
        }

        private void battalionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].baseBattalion = Convert.ToByte(battalionComboBox.SelectedIndex);
        }

        private void unk0x10NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x10 = Decimal.ToInt16(unk0x10NumBox.Value);
        }

        private void NameIDNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].nameID = Decimal.ToUInt16(NameIDNumBox.Value);
        }

        private void unk0x14NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x14 = Decimal.ToUInt16(unk0x14NumBox.Value);
        }

        private void voiceIDNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].voiceID = Decimal.ToUInt16(voiceIDNumBox.Value);
        }

        private void assetIDNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].assetID = Decimal.ToUInt16(assetIDNumBox.Value);
        }

        private void birthdayFlagNumbox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].birthDayFlag = Decimal.ToByte(birthdayFlagNumbox.Value);
        }

        private void unk0x1FNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x1F = Decimal.ToByte(unk0x1FNumBox.Value);
        }

        private void saveDataIDNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].saveDataID = Decimal.ToSByte(saveDataIDNumBox.Value);
        }

        private void unk0x21NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x21 = Decimal.ToByte(unk0x21NumBox.Value);
        }

        private void unk0x23NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x23 = Decimal.ToByte(unk0x23NumBox.Value);
        }

        private void unk0x25NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x25 = Decimal.ToSByte(unk0x25NumBox.Value);
        }

        private void bodyTypeNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].bodyType = Decimal.ToByte(bodyTypeNumBox.Value);
        }

        private void nonCombatAnimNumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].nonCombatAnimSet = Decimal.ToByte(nonCombatAnimNumBox.Value);
        }

        private void unk0x2ENumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x2E = Decimal.ToByte(unk0x2ENumBox.Value);
        }

        private void unk0x31NumBox_ValueChanged(object sender, EventArgs e)
        {
            currentPersonData.Character[characterListBox.SelectedIndex].unk_0x31 = Decimal.ToUInt16(unk0x31NumBox.Value);
        }
    }
}
