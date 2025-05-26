# 📘 InputSimulator

**InputSimulator** is a lightweight C# library for simulating low-level keyboard and mouse input using `SendInput`, with support for:
- Key presses/releases (including modifiers)
- Mouse button actions
- Smooth or delta-based human-like mouse movement
- Absolute mouse positioning

---

## 🛠️ Features

- ✅ Full virtual keycode map (letters, digits, modifiers, OEM keys, etc.)
- 🖱️ Mouse click and movement support (LButton, RButton, MButton, XButton1, XButton2)
- 🧠 Smooth delta-based mouse movement with adjustable speed and easing
- 🔧 Utility methods for cursor repositioning and held-modifier tracking

---

## 📦 Installation

Include the two source files in your C# project:
- `Interop.cs` — low-level WinAPI definitions
- `Simulator.cs` — main interaction interface

No external dependencies required. Fully P/Invoke-based.

---

## 🚀 Usage

### Initialize

```csharp
var sim = new InputSimulator.Simulator();
```

---

### 🔡 Send Keyboard or Mouse Input

```csharp
sim.Send("A");             // Press and release 'A'
sim.Send("Ctrl down");     // Hold down Ctrl
sim.Send("C");             // Press and release 'C' while Ctrl is held
sim.Send("Ctrl up");       // Release Ctrl

sim.Send("LButton");       // Left mouse click
sim.Send("RButton down");  // Right mouse down
sim.Send("RButton up");    // Right mouse up
```

---

### 🎯 Move Mouse to Absolute Position

```csharp
sim.MouseSetPos(100, 200);  // Instantly move mouse to (100, 200)
```

---

### 🧭 Smooth Human-Like Delta Mouse Movement

```csharp
sim.MouseDeltaMove(800, 500);          // Move to (800, 500) smoothly
sim.MouseDeltaMove(800, 500, 2.0);     // Move faster (speed multiplier)
sim.MouseDeltaMove(800, 500, 0.5);     // Move slower
```

This creates a natural motion path with easing and sine-wave curvature to simulate human input.

---

## 💡 Internals

- Input is sent using `SendInput` WinAPI.
- Keyboard and mouse events are translated into `INPUT` structs.
- `MouseDeltaMove()` calculates interpolated paths and sends delta updates to mimic physical movement.
- Supports all standard virtual key codes from A-Z, 0-9, F1-F24, symbols, media keys, browser keys, and more.

---

## 📂 KeyMap Reference

The `KeyMap` dictionary includes mappings for:
- **Mouse**: `LButton`, `RButton`, `MButton`, `XButton1`, `XButton2`
- **Control**: `Ctrl`, `Shift`, `Alt`, `Esc`, `Enter`, etc.
- **Alphabet**: `A` to `Z`
- **Digits**: `0` to `9`, `Num0` to `Num9`
- **Function**: `F1` to `F24`
- **OEM keys**: `;`, `=`, `-`, `/`, etc.
- **Media/browser keys**: `MediaPlayPause`, `VolumeUp`, `BrowserBack`, etc.

---

## 🔐 Notes & Limitations

- The library is intended for **local user simulation** — not remote desktop or sandboxed environments.
- It uses **unmanaged code** (P/Invoke), so full trust is required.
- Some games with anti-cheat mechanisms may block or detect `SendInput`.

---

## ✅ Example: Simulate Copy-Paste Shortcut

```csharp
var sim = new InputSimulator.Simulator();

sim.Send("Ctrl down");
sim.Send("C");
sim.Send("Ctrl up");
```

---

## 🧪 Example: Human-Like Drag to Screen Center

```csharp
// Wait 1 second, then move mouse to center of 1920x1080 screen
Thread.Sleep(1000);
sim.MouseDeltaMove(960, 540, speed: 1.2);
```

---

## 📜 License

MIT License