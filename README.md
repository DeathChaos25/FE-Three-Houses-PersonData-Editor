# FE Three Houses PersonData Editor
WIP GUI Editor for fixed_persondata.bin (update v1.1.0 or later)    

Dependencies:
- .net Framework v4.7.2
https://dotnet.microsoft.com/download/dotnet-framework/net472
  
    
  
DISCLAIMER: While very much in a usable state, there are a lots of bugs that need to be ironed out, QoL implemented, code optimizations, etc etc.

![Program WIP Screenshot](https://cdn.discordapp.com/attachments/516416628150239245/758438613854978058/unknown.png) 
![Program WIP Screenshot](https://cdn.discordapp.com/attachments/377899265424621569/694378127849422949/unknown.png)

Makes use of (an earlier version of) Amicitia.IO from TGEnigma.
https://github.com/TGEnigma/Amicitia.IO

Current Progress:
- Loads and parses fixed_persondata and all of its section headers
- Section 0 (Character blocks) has a working editor
- Section 1 (Asset ID blocks) has a working editor
- Section 2 (Voice ID Blocks) has a working editor (incomplete functionality, further research is needed)
- Section 3 (Default Weapon Ranks and Combat Assets) has a working editor
- Section 4 (Character Spell List)  has a working editor
- Section 5 (Character Skill List)  has a working editor
- Section 6 (Starting Inventory)  has a working editor
- Section 7 (Character Combat Arts List)  has a working editor
- Allows viewing/editing of all values in implemented sections
- Saves a completely valid and usable fixed_persondata file, for now the program reads and writes byte arrays for the sections without an editor, meaning loading a fixed_persondata file and saving it into a new file creates a 1:1 copy.

ToDo:
- Implement the rest of the sections
- Better portrait loading code (currently loads it based on asset ID and only very few are actually implemented)
- Full Multi lang support maybe? (Partially support implemented for now)
- Add a reset button for current highlighted character (done kinda)

Planned Features:  
- Add a button to create all the necessary blocks tied to save ID (needs more investigation, is there a hardcoded max value?)
- Some sort of "Byleth customizer" (stuff like boon/bane)
- RESERVE
