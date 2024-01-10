MKUpdate
2021-2024 Haruka
Licensed under GPLv3.

---

A command-line application that copies certain desired files from a directory into another directory (to create an "update package").

Usage: MKUpdate <fromdir> <todir> <filetype1,filetype2,...> [rule1,rule2,...]
fromdir: the source directory to scan
todir: the directory where detected files should be placed
filetype1..n: the filetypes to copy
rule1..n: See rules

Rules:

not_folder:example                      exclude all files in the example sub-directory
folder:example                          exclude all files NOT in the example sub-directory
file_contains:example                   exclude all files NOT containing the word example
file_not_contains:example               exclude all files containing the word example
folder_contains:example         	exclude all files NOT containing the word example in their path name
folder_not_contains:example             exclude all files containing the word example in their path name
updated_after:2020-01-01                only include files edited after 2020-01-01
created_after:2020-01-01                only include files created after 2020-01-01

All textual rules are matched with contains (not equals)

Example script:

MKUpdate.exe data data_update .txt not_folder:DEVICE
MKUpdate.exe data data_update .xtal not_folder:songpack,not_folder:asset,file_contains:haruMod
MKUpdate.exe data data_update .lmz not_folder:songpack,not_folder:asset,updated_after:2021-09-20
MKUpdate.exe data data_update .lmz not_folder:songpack,not_folder:asset,created_after:2021-09-20
MKUpdate.exe data data_update .nut,.lm not_folder:songpack,not_folder:songtexture,not_folder:asset,updated_after:2021-09-20
MKUpdate.exe data data_update .nut,.lm not_folder:songpack,not_folder:songtexture,not_folder:asset,created_after:2021-09-20
MKUpdate.exe data data_update .lm folder:scene
MKUpdate.exe data data_update .nut folder:scene,folder:.lmd