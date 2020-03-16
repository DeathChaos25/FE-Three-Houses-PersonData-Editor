# FE Three Houses PersonData Editor
WIP GUI Editor for fixed_persondata.bin (update v1.1.0 or later)  
DISCLAIMER: While very much in a usable state, it is not complete and in a state I would like for it to be publicly used, lots of bugs need to be ironed out, QoL implemented, code optimizations, etc etc.

![Program WIP Screenshot](https://cdn.discordapp.com/attachments/377899265424621569/688953959037141009/unknown.png) ![Program WIP Screenshot](https://cdn.discordapp.com/attachments/377899265424621569/688953825402290214/unknown.png)
![Program WIP Screenshot](https://cdn.discordapp.com/attachments/377899265424621569/688953595021754403/unknown.png)

Makes use of (an earlier version of) Amicitia.IO from TGEnigma.
https://github.com/TGEnigma/Amicitia.IO

Current Progress:
- Loads and parses fixed_persondata and all of its section headers
- Section 0 (Character blocks) has a working editor
- Section 1 (Asset ID blocks) has a working editor
- Section 2 (Voice ID Blocks) has a working editor (although further research is needed)
- Section 3 (Default Weapon Ranks and Combat Assets) has a working editor
- Section 4 (Character Spell List)  has a working editor
- Allows viewing/editing of all values in implemented sections
- Saves a completely valid and usable fixed_persondata file, for now the program reads and writes byte arrays for the sections without an editor, meaning loading a fixed_persondata file and saving it into a new file creates a 1:1 copy.

ToDo:
- Implement the rest of the sections
- Better portrait loading code (currently loads it based on asset ID and only very few are actually implemented)
- Multi lang support maybe? (Partially done)
- Add a reset button for current highlighted character (done, but only resets section 0 for that character)
