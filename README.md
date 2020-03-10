# FE Three Houses PersonData Editor
WIP GUI Editor for fixed_persondata.bin (update v1.1.0 or later)

![Program WIP Screenshot](https://cdn.discordapp.com/attachments/377899265424621569/687053814733209605/unknown.png)

Makes use of (an earlier version of) Amicitia.IO from TGEnigma.
https://github.com/TGEnigma/Amicitia.IO

Current Progress:
- Loads and parses fixed_persondata and all of its section headers
- Section 0 (Character blocks) has been implemented/parsed in its entirety
- Section 1 (Asset ID blocks) has been implemented/parsed in its entirety
- Allows viewing/editing of all values in character block (minus name ID for now)
- Saves a "valid" fixed_persondata file (currently only section 0 has real data, the rest of the sections are filled with padding)

ToDo:
- Implement the rest of the sections
- Better portrait loading code (currently loads it based on asset ID and only very few are actually implemented)
- Multi lang support maybe? (Partially done)
- Add a reset button for current highlighted character
