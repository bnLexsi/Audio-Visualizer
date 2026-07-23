# Audio-Visualizer 🎵
> A lightweight, sleek, and responsive 12-band audio visualizer built with C# and Guna UI 2.

Audio-Visualizer captures system audio in real-time using WASAPI loopback, processes the sound via Fast Fourier Transform (FFT), and visualizes the frequency spectrum across 12 custom logarithmic bands—from sub-bass to air frequencies.

---

## ✨ Features

- **12-Band Logarithmic Spectrum:** Accurately split frequency bands tailored for music visualization (bass, mids, highs, and top-end).
- **Smooth Audio Dynamics:** Custom peak-decay and fall-off animations for organic panel movement.
- **Modern Glassmorphism UI:** Rounded panels, transparency support, and a clean widget-like aesthetic.
- **Smart Alignment Hotkeys:** Easily move the visualizer around your screen using quick keyboard shortcuts.
- **Minimalist & Lightweight:** Runs smoothly in the background without clogging system resources (hidden from taskbar by default).

---

## ⌨️ Hotkeys & Controls

| Shortcut | Action |
| `Ctrl` + `Left` | Snap to screen left edge (12px padding) |
| `Ctrl` + `Right` | Snap to screen right edge (12px padding) |
| `Ctrl` + `Up` | Snap to screen top edge (12px padding) |
| `Ctrl` + `Down` | Snap to screen bottom edge (12px padding) |
| `Ctrl` + `Space` | Reset form to default start position (`120, 120`) |

---

## 🛠️ Built With

- **C# / .NET Framework**
- **NAudio** - WASAPI loopback capture & FFT audio processing
- **Guna UI 2** - Modern UI components & rounded panel designs

---

## 🚀 Getting Started
https://github.com/bnLexsi/Audio-Visualizer/releases/tag/v1.0.0
