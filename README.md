# Photo Viewer Ultimate

![License](https://img.shields.io/github/license/VishnuKaranth/Image-Viewer)
![Platform](https://img.shields.io/badge/platform-windows-blue)
![Framework](https://img.shields.io/badge/dotnet-9.0-purple)

**Photo Viewer Ultimate** is a high-performance, modern image viewer and editor build with C# (WPF) and .NET 9. It is designed to be a lightweight, "frameless" alternative to the default Windows Photos app, featuring a dark studio UI, advanced format support, and professional editing tools.

---

## ✨ Features

### 🚀 Core Experience
*   **High Performance**: Instant loading and smooth zooming/panning.
*   **Modern UI**: Fully frameless, dark-themed "Studio" interface with custom window controls.
*   **Adaptive Workflow**: 
    *   **Welcome Studio**: Centered start page for quick access.
    *   **Direct Open**: Open single images or entire folders instantly.
    *   **Auto-Fade UI**: Controls vanish when you don't need them.

### 🎨 Professional Editor
*   **Real-time Filters**: Adjust Brightness and Contrast with zero latency.
*   **Crop Tool**: Precise cropping with an intuitive mouse overlay.
*   **Transform**: Rotate Left/Right and **Flip Horizontal/Vertical**.
*   **Format Support**: Powered by `Magick.NET` to support PSD, WEBP, RAW, and more.

### 🛠 Power Tools
*   **Color Picker**: Built-in Eyedropper to grab HEX codes from any pixel.
*   **Metadata HUD**: Toggleable overlay showing resolution, file size, and date.
*   **Transparency Grid**: Professional checkerboard background for PNGs.
*   **Filmstrip**: Context-aware thumbnail bar for easy navigation.
*   **Slideshow**: Auto-play mode for hands-free viewing.

---

## 🎮 Controls

| Action | Shortcut |
| :--- | :--- |
| **Next / Prev Image** | `Right Arrow` / `Left Arrow` |
| **Zoom In / Out** | `Ctrl + Wheel` |
| **Pan Image** | `Mouse Drag` |
| **Toggle Metadata** | `I` |
| **Close App** | `Esc` (from Gallery) or Close Button |
| **Rotate** | UI Buttons |

---

## 📦 Getting Started

### Prerequisites
*   Windows 10 / 11
*   [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Installation
1.  Clone the repository:
    ```bash
    git clone https://github.com/VishnuKaranth/Image-Viewer.git
    ```
2.  Navigate to the project:
    ```bash
    cd Image-Viewer/ImageViewer
    ```
3.  Run the app:
    ```bash
    dotnet run
    ```

---

## 🔮 Roadmap
*   [ ] Hardware Acceleration (GPU)
*   [ ] Batch Processing/Renaming
*   [ ] WebP Animation Support
*   [ ] Plugin System

---

Built with ❤️ by [Vishnu Karanth](https://github.com/VishnuKaranth).
