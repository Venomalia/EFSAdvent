
# EFSAdvent
[![Wiki](https://img.shields.io/badge/Wiki-Documentation-white)](https://github.com/Venomalia/EFSAdvent/wiki)
[![Discord](https://img.shields.io/badge/Discord-Four_Swords_Plus-blue?logo=Discord&logoColor=fff)](https://discord.gg/bKTMyrFXhr)

A level editor for the Nintendo GameCube game *Four Swords Adventures*.  
This project is an unofficial continuation of EFSAdvent, based on the original [source code](https://bitbucket.org/jaytheham/efsadvent/src/main/) by Jay (theHam) Harland.

## Features & Improvements

- Improved stability, error handling, and overall editor reliability.
- Enhanced level editing with easier room management, importing, exporting, and tile editing tools.
- Added advanced actor support, including variable editing, templates, documentation, rendering, and clipboard support.
- Added visual tools for actor paths, layer usage, Overlay effects, and tile modifications.
- Added native sprite and tile rendering based on the original `data.arc` resources.
- Added full RARC archive support for loading and saving FSA resources.
- Added support for Shadow Battle Maps and improved GBA level rendering.
- Added Tiled TMX import/export and PNG level export.
- Added a Sprite Converter tool.
- Improved tile information, metadata, and property visualization.

## Goals
- Fully document all actor variables and their behavior.
- Continue improving support for advanced FSA level editing features.

## Getting Started
- Use Dolphin to extract the entire **The Legend of Zelda: Four Swords Adventures** game disc into a folder.
Launch the editor.
- On first launch, the editor will ask you to locate **data.arc**. Select: `files/GC4Sword/data.arc`
The editor will copy this file to its local data folder, so this step only needs to be completed once.
- After the editor has started, open a stage archive from the extracted game files.
For example, the first stage is located at: `files/GC4Sword/Boss/boss010.arc`

Changes can be saved directly to the stage archive.  
Since Dolphin can boot extracted game folders, you can test your changes immediately without rebuilding the disc image!

## Preview
<img width="960" height="606" alt="grafik" src="https://github.com/user-attachments/assets/a39e8801-a415-40e5-9c8a-2de2dd4dd52d" />

