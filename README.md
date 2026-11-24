# Erenshor Command History

A lightweight **Lunaris** mod that adds **Up/Down arrow history** to Erenshor's in-game chat command input.

**Current version:** 0.1.0

## Features

- **Up Arrow:** browse older commands.
- **Down Arrow:** browse newer commands; moving past the newest restores the unfinished input.
- Keeps the **last 50 slash commands** for the current game session.
- Records built-in commands and commands supplied by other mods.
- Skips consecutive duplicate commands.
- Does **not** record ordinary chat or `/whisper` / `/w` private messages.
- Leaves Erenshor's existing `//` previous-message shortcut unchanged.

History is **in memory only** and resets when the game exits. Arrow-key navigation works while the chat input is open.

## Usage

Run a few slash commands, then reopen the chat input and press **Up** to recall them. Press **Down** to move toward the most recent command or return to your unfinished text. Edit and submit recalled commands normally.

## Requirements

- Erenshor (Windows PC)
- Lunaris mod loader with Harmony patching available
- For building: Visual Studio with .NET Framework 4.8.1 development tools

## Installation

1. Install Lunaris according to its documentation.
2. Build this project in **Release**, or obtain the DLL from a release when available.
3. Place the compiled DLL in the Lunaris plugin directory used by your Erenshor installation.
4. Launch Erenshor and confirm the plugin loads in the Lunaris log.

This is a **Lunaris-only** mod. BepInEx and Thunderstore packaging are not supported.

## Building from source

1. Create or open a **Class Library (.NET Framework)** project targeting **.NET Framework 4.8.1**.
2. Add `Plugin.cs` to the project.
3. Add references to `Assembly-CSharp.dll`, `Lunaris.dll`, `0Harmony.dll`, and the Unity DLLs required by the source, from your local Erenshor/Lunaris installation.
4. Set game/loader DLL references to **Copy Local = False**.
5. Select **Release** and build the solution. The output DLL is typically under `bin/Release/`.

Game and loader assemblies are **not included** in this repository. Build references may need updating after Erenshor or Lunaris updates.

## Compatibility

Developed against the Erenshor build current in November 2026. Future game updates may change the methods this plugin patches. No compatibility guarantee is made for later builds.

## Disclaimer

Unofficial fan-made mod; not affiliated with or endorsed by Burgee Media or the Lunaris developers. Use at your own risk and back up your saves before trying new mods.
