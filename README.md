# Universal Tomb Launcher
A modern, cross-platform Tomb Raider launcher for classic and modern engines (TR1, TR2, TR3, TR4, TR5, TRX, and Tomb Engine / TEN), primarily used in the TombIDE project.

![Preview](https://user-images.githubusercontent.com/20436882/142447710-0eff9ad3-de92-4019-bf76-f1b5418bda4f.png)

# Main features
- **Cross-Platform Support**: Runs natively on **Windows, Linux (including Steam Deck / SteamOS), and macOS** powered by .NET 10+ and SDL3;
- **Modern Engine Support**: Full out-of-the-box detection and execution for **TRX** (covering TR1, TR2, TR3), **TEN (Tomb Engine)**, and **Tomb1Main**;
- **Classic Engine Support**: Fully compatible with original TR1–5 executables and dgVoodoo;
- **Hardware-Accelerated Splash Screen**: Borderless window with smooth fade-in animation and customizable top/bottom gradient bars via `splash.xml`;
- **Interactive Setup Dialog**: Press `CTRL` during the splash screen to open the engine's `-setup` dialog, or preview with `-p`;
- **Process Manager**: Detects already running engine instances and prompts to terminate stuck processes cleanly;
- **Log Cleaner**: Automatically tidies TRNG crash and session logs into a separate `logs` folder after the game closes;
- **TR4 Border Fix (Windows 8+)**: Configurable compatibility fix for fullscreen borders in classic TR4;
- **Jumbo Icon Support**: Applies custom 256x256 launcher icon to classic Windows game windows.

# Building and Publishing

### Requirements
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or newer.

### Build (Development)
```bash
dotnet build "Universal Tomb Launcher.sln"
```

### Publish Single-File Binaries

**Windows (x64):**
```cmd
dotnet publish "Universal Tomb Launcher/Universal Tomb Launcher.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

**Linux (x64 / Steam Deck):**
```bash
dotnet publish "Universal Tomb Launcher/Universal Tomb Launcher.csproj" -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained false
```

**macOS (Apple Silicon):**
```bash
dotnet publish "Universal Tomb Launcher/Universal Tomb Launcher.csproj" -c Release -r osx-arm64 -p:PublishSingleFile=true --self-contained false
```

For Intel write `osx-x64`

# Command Line Options
- `-s` or `-setup`: Forces the game to launch directly into its setup configuration dialog.
- `-p` or `-preview`: Opens the splash screen in preview mode (press `ESC` to exit).
- `-d` or `-debug`: Launches the game with console/debug mode enabled (if supported by the engine).

# Screenshots
Dark theme:

![Dark](https://user-images.githubusercontent.com/20436882/142443882-d7c7287d-1216-465f-8657-be050266931c.png)

Light theme:

![Light](https://user-images.githubusercontent.com/20436882/142443909-8a55f031-ff5a-4040-98df-173bbad612a3.png)

Game already running prompt:

![Message](https://user-images.githubusercontent.com/20436882/142454242-a749c79e-4751-49f9-88da-0fc41d31ae3a.png)

Splash screen image from Delca's custom level: https://www.trle.net/sc/levelfeatures.php?lid=3379
