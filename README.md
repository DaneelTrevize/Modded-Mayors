# Modded Mayors

This is a modification to a game rules file (ZXRules.dat) for They Are Billions.  
The intent is to change the Mayor choices presented to the player during a game, for improved balance and variety.  

# How To Use This

To play with Modded Mayors, one need simply replace the existing rules file of their TAB install with [a downloaded version](https://github.com/DaneelTrevize/Modded-Mayors/tree/master/Releases/).  
Typically the game files are found at *Drive:\SteamLibrary*\steamapps\common\They Are Billions\  
Then start a new game. It is expected that existing save games may not be compatible with changing your game rules after having chosen any Mayor.

To revert back to the unmodded game, simply use the 'Verify Integrity' option in Steam's Properties menu, found by right-clicking the game in the Library UI.  
![Verify Integrity](https://raw.githubusercontent.com/DaneelTrevize/Modded-Mayors/master/Steam%20verify.png)  
Or have manually backed up your original file before replacing it, keeping in mind it can need to be updated as new game versions are released.

# List of Modded Mayors

A [recently generated spreadsheet can be found here](https://github.com/DaneelTrevize/Modded-Mayors/blob/master/Source/modded%20mayors%20snippet.csv).  
This repo also contains a program to regenerate this spreadsheet from the XML file found adjacent to it.

An invalid older spreadsheet was obtained from `https://docs.google.com/spreadsheets/d/1ZpQz5TmhUMwM26UkHeM6yMczYDfG0VEe0uoJPj4sxcg/edit#gid=0`. Original author(s) unknown.

----

# Origin

The original version of these Mayor changes was made by Alvicate.  
Since their last release of those works, the game has been updated and the modded file was no longer compatible. This project is to access the decrypted version of those changes, merge them with compatible current game files, and re-encrypt them as the game requires.

1.0.18 password for the main Steam branch is:  
`-9495552146701228917317764-9495552146701228917317764334454FADSFASDF45345`

Previous password for the public beta branch is:  
`-167547859615307867291835083137-167547859615307867291835083137334454FADSFASDF45345`  
Previous password for the main Steam branch is:  
`-97961337-1138788515-358272088-97961337-1138788515-358272088334454FADSFASDF45345`

Historically the password was:  
`-2099717824-430703793638994083`  
from https://github.com/ash47/TheyAreBillionsModKit  
Although this also does not decrypt the first public modded instance of ZXRules.dat encounted when starting this project.

For the release 2019/07/03 15:11:00 UTC, Steam manifest ID 4346077441540232602 (TAB 1.0.10?), the password was:  
`1692201870-110592708219527297971692201870-11059270821952729797334454FADSFASDF45345`

----

Compatible 7-Zip settings are:

Archive format:		zip  
Compression level:	Ultra  
Compression method:	Deflate, not Deflate64  
Dictionary size:	32 KB  
Word size:			128  
Encryption method:	ZipCrypto  
