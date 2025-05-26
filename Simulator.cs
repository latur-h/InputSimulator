using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using InputSimulator.Core;
using static InputSimulator.Core.Interop;

namespace InputSimulator
{
    public class Simulator
    {
        private readonly HashSet<string> HeldModifiers = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, ushort> KeyMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // Mouse
            ["LButton"] = 0x01,
            ["RButton"] = 0x02,
            ["Cancel"] = 0x03,
            ["MButton"] = 0x04,
            ["XButton1"] = 0x05,
            ["XButton2"] = 0x06,

            // Control Keys
            ["Backspace"] = 0x08,
            ["Tab"] = 0x09,
            ["Clear"] = 0x0C,
            ["Enter"] = 0x0D,
            ["Shift"] = 0x10,
            ["Ctrl"] = 0x11,
            ["Alt"] = 0x12,
            ["Pause"] = 0x13,
            ["CapsLock"] = 0x14,
            ["Esc"] = 0x1B,
            ["Space"] = 0x20,
            ["PageUp"] = 0x21,
            ["PageDown"] = 0x22,
            ["End"] = 0x23,
            ["Home"] = 0x24,
            ["Left"] = 0x25,
            ["Up"] = 0x26,
            ["Right"] = 0x27,
            ["Down"] = 0x28,
            ["Select"] = 0x29,
            ["Print"] = 0x2A,
            ["Execute"] = 0x2B,
            ["PrintScreen"] = 0x2C,
            ["Insert"] = 0x2D,
            ["Delete"] = 0x2E,
            ["Help"] = 0x2F,

            // Numbers
            ["0"] = 0x30,
            ["1"] = 0x31,
            ["2"] = 0x32,
            ["3"] = 0x33,
            ["4"] = 0x34,
            ["5"] = 0x35,
            ["6"] = 0x36,
            ["7"] = 0x37,
            ["8"] = 0x38,
            ["9"] = 0x39,

            // Letters
            ["A"] = 0x41,
            ["B"] = 0x42,
            ["C"] = 0x43,
            ["D"] = 0x44,
            ["E"] = 0x45,
            ["F"] = 0x46,
            ["G"] = 0x47,
            ["H"] = 0x48,
            ["I"] = 0x49,
            ["J"] = 0x4A,
            ["K"] = 0x4B,
            ["L"] = 0x4C,
            ["M"] = 0x4D,
            ["N"] = 0x4E,
            ["O"] = 0x4F,
            ["P"] = 0x50,
            ["Q"] = 0x51,
            ["R"] = 0x52,
            ["S"] = 0x53,
            ["T"] = 0x54,
            ["U"] = 0x55,
            ["V"] = 0x56,
            ["W"] = 0x57,
            ["X"] = 0x58,
            ["Y"] = 0x59,
            ["Z"] = 0x5A,

            // Windows Keys
            ["Win"] = 0x5B,
            ["RWin"] = 0x5C,
            ["Apps"] = 0x5D,

            // Numpad
            ["Num0"] = 0x60,
            ["Num1"] = 0x61,
            ["Num2"] = 0x62,
            ["Num3"] = 0x63,
            ["Num4"] = 0x64,
            ["Num5"] = 0x65,
            ["Num6"] = 0x66,
            ["Num7"] = 0x67,
            ["Num8"] = 0x68,
            ["Num9"] = 0x69,
            ["Multiply"] = 0x6A,
            ["Add"] = 0x6B,
            ["Separator"] = 0x6C,
            ["Subtract"] = 0x6D,
            ["Decimal"] = 0x6E,
            ["Divide"] = 0x6F,

            // Function Keys
            ["F1"] = 0x70,
            ["F2"] = 0x71,
            ["F3"] = 0x72,
            ["F4"] = 0x73,
            ["F5"] = 0x74,
            ["F6"] = 0x75,
            ["F7"] = 0x76,
            ["F8"] = 0x77,
            ["F9"] = 0x78,
            ["F10"] = 0x79,
            ["F11"] = 0x7A,
            ["F12"] = 0x7B,
            ["F13"] = 0x7C,
            ["F14"] = 0x7D,
            ["F15"] = 0x7E,
            ["F16"] = 0x7F,
            ["F17"] = 0x80,
            ["F18"] = 0x81,
            ["F19"] = 0x82,
            ["F20"] = 0x83,
            ["F21"] = 0x84,
            ["F22"] = 0x85,
            ["F23"] = 0x86,
            ["F24"] = 0x87,

            // Lock Keys
            ["NumLock"] = 0x90,
            ["ScrollLock"] = 0x91,

            // OEM & Symbol keys (U.S. layout, may vary)
            [";"] = 0xBA,
            ["="] = 0xBB,
            [","] = 0xBC,
            ["-"] = 0xBD,
            ["."] = 0xBE,
            ["/"] = 0xBF,
            ["`"] = 0xC0,
            ["["] = 0xDB,
            ["\\"] = 0xDC,
            ["]"] = 0xDD,
            ["'"] = 0xDE,

            // Others
            ["BrowserBack"] = 0xA6,
            ["BrowserForward"] = 0xA7,
            ["BrowserRefresh"] = 0xA8,
            ["BrowserStop"] = 0xA9,
            ["BrowserSearch"] = 0xAA,
            ["BrowserFavorites"] = 0xAB,
            ["BrowserHome"] = 0xAC,
            ["VolumeMute"] = 0xAD,
            ["VolumeDown"] = 0xAE,
            ["VolumeUp"] = 0xAF,
            ["MediaNext"] = 0xB0,
            ["MediaPrev"] = 0xB1,
            ["MediaStop"] = 0xB2,
            ["MediaPlayPause"] = 0xB3,
            ["LaunchMail"] = 0xB4,
            ["LaunchMediaSelect"] = 0xB5,
            ["LaunchApp1"] = 0xB6,
            ["LaunchApp2"] = 0xB7,
        };

        public Simulator(){ }

        public void Send(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string key = parts[0];
            string action = parts.Length > 1 ? parts[1].ToLowerInvariant() : "click";

            if (!KeyMap.TryGetValue(key, out ushort vkCode))
            {
                if (key.Length == 1)
                    vkCode = (ushort)char.ToUpperInvariant(key[0]);
                else
                    throw new ArgumentException($"Unknown key: {key}");
            }

            if (IsMouseKey(key))
            {
                switch (action)
                {
                    case "down": SendMouse(key, down: true); break;
                    case "up": SendMouse(key, down: false); break;
                    default:
                        SendMouse(key, down: true);
                        SendMouse(key, down: false);
                        break;
                }
            }
            else
            {
                switch (action)
                {
                    case "down":
                        if (!HeldModifiers.Contains(key))
                        {
                            SendKey(vkCode, true);
                            if (IsModifier(key))
                                HeldModifiers.Add(key);
                        }
                        break;
                    case "up":
                        SendKey(vkCode, false);
                        if (IsModifier(key))
                            HeldModifiers.Remove(key);
                        break;
                    default:
                        SendKey(vkCode, true);
                        SendKey(vkCode, false);
                        break;
                }
            }
        }
        public void MouseSetPos(int x, int y) => SetCursorPos(x, y);
        public void MouseDeltaMove(int targetX, int targetY, double speed = 1.0)
        {
            if (speed <= 0) speed = 1.0;

            GetCursorPos(out Point start);
            int totalDx = targetX - start.X;
            int totalDy = targetY - start.Y;

            double distance = Math.Sqrt(totalDx * totalDx + totalDy * totalDy);
            int durationMs = (int)(distance * 0.8 / speed);

            int steps = Math.Max(10, durationMs / 10);
            Random rand = new Random();
            double curveStrength = rand.NextDouble() * 0.5 + 0.5;

            double prevX = 0, prevY = 0;

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double easeT = t * t * (3 - 2 * t);
                double curve = Math.Sin(easeT * Math.PI) * curveStrength;

                double x = (totalDx * easeT) + curve * 10;
                double y = (totalDy * easeT) + curve * 5;

                int dx = (int)Math.Round(x - prevX);
                int dy = (int)Math.Round(y - prevY);

                prevX += dx;
                prevY += dy;

                if (dx != 0 || dy != 0)
                    SendDelta(dx, dy);

                Thread.Sleep((int)(10 / speed) + rand.Next(3));
            }
        }

        private void SendKey(ushort vkCode, bool isKeyDown)
        {
            var input = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vkCode,
                        dwFlags = isKeyDown ? 0 : KEYEVENTF_KEYUP,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
        private void SendMouse(string button, bool down)
        {
            uint flags = button.ToLowerInvariant() switch
            {
                "lbutton" => down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP,
                "rbutton" => down ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP,
                "mbutton" => down ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP,
                "xbutton1" => down ? MOUSEEVENTF_XDOWN : MOUSEEVENTF_XUP,
                "xbutton2" => down ? MOUSEEVENTF_XDOWN : MOUSEEVENTF_XUP,
                _ => throw new ArgumentException($"Unknown mouse key: {button}")
            };

            uint data = button.ToLowerInvariant() switch
            {
                "xbutton1" => XBUTTON1,
                "xbutton2" => XBUTTON2,
                _ => 0
            };

            var input = new INPUT
            {
                type = INPUT_MOUSE,
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dwFlags = flags,
                        mouseData = data,
                        dx = 0,
                        dy = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
        private static void SendDelta(int dx, int dy)
        {
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_MOUSE;
            input[0].U.mi.dx = dx;
            input[0].U.mi.dy = dy;
            input[0].U.mi.mouseData = 0;
            input[0].U.mi.dwFlags = MOUSEEVENTF_MOVE;
            input[0].U.mi.time = 0;
            input[0].U.mi.dwExtraInfo = IntPtr.Zero;

            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }

        private bool IsMouseKey(string key) =>
            key.StartsWith("LButton", StringComparison.OrdinalIgnoreCase) ||
            key.StartsWith("RButton", StringComparison.OrdinalIgnoreCase) ||
            key.StartsWith("MButton", StringComparison.OrdinalIgnoreCase) ||
            key.StartsWith("XButton1", StringComparison.OrdinalIgnoreCase) ||
            key.StartsWith("XButton2", StringComparison.OrdinalIgnoreCase);
        private bool IsModifier(string key) =>
            key.Equals("Shift", StringComparison.OrdinalIgnoreCase) ||
            key.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) ||
            key.Equals("Alt", StringComparison.OrdinalIgnoreCase) ||
            key.Equals("Win", StringComparison.OrdinalIgnoreCase);
    }
}
