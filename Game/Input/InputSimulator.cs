﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using WinAPI;

namespace Game
{
    public class InputSimulator : IInput
    {
        private readonly int MIN_DELAY;
        private readonly int MAX_DELAY;

        private readonly Random random = new Random();
        private readonly GregsStack.InputSimulatorStandard.InputSimulator simulator;
        private readonly Process process;

        public InputSimulator(Process process, int minDelay, int maxDelay)
        {
            this.process = process;

            MIN_DELAY = minDelay;
            MAX_DELAY = maxDelay;

            simulator = new GregsStack.InputSimulatorStandard.InputSimulator();
        }

        private async ValueTask<int> Delay(int milliseconds)
        {
            int delay = milliseconds + random.Next(1, MAX_DELAY);
            await Task.Delay(delay);
            return delay;
        }

        public void KeyDown(int key)
        {
            if(NativeMethods.GetForegroundWindow() != process.MainWindowHandle)
                NativeMethods.SetForegroundWindow(process.MainWindowHandle);

            simulator.Keyboard.KeyDown((VirtualKeyCode)key);
        }

        public void KeyUp(int key)
        {
            if (NativeMethods.GetForegroundWindow() != process.MainWindowHandle)
                NativeMethods.SetForegroundWindow(process.MainWindowHandle);

            simulator.Keyboard.KeyUp((VirtualKeyCode)key);
        }

        public async ValueTask<int> KeyPress(int key, int milliseconds)
        {
            simulator.Keyboard.KeyDown((VirtualKeyCode)key);
            int delay = await Delay(milliseconds);
            simulator.Keyboard.KeyUp((VirtualKeyCode)key);
            return delay;
        }

        public async ValueTask KeyPressNoDelay(int key, int milliseconds)
        {
            simulator.Keyboard.KeyDown((VirtualKeyCode)key);
            await Task.Delay(milliseconds);
            simulator.Keyboard.KeyUp((VirtualKeyCode)key);
        }

        public void KeyPressSleep(int key, int milliseconds)
        {
            simulator.Keyboard.KeyDown((VirtualKeyCode)key);
            Thread.Sleep(milliseconds);
            simulator.Keyboard.KeyUp((VirtualKeyCode)key);
        }

        public async ValueTask LeftClickMouse(Point p)
        {
            SetCursorPosition(p);
            simulator.Mouse.LeftButtonDown();
            await Delay(MIN_DELAY);
            simulator.Mouse.LeftButtonUp();
        }

        public async ValueTask RightClickMouse(Point p)
        {
            SetCursorPosition(p);
            simulator.Mouse.RightButtonDown();
            await Delay(MIN_DELAY);
            simulator.Mouse.RightButtonUp();
        }

        public void SetCursorPosition(Point p)
        {
            NativeMethods.GetWindowRect(process.MainWindowHandle, out var rect);
            p.X = p.X * 65535 / rect.Width;
            p.Y = p.Y * 65535 / rect.Height;
            simulator.Mouse.MoveMouseTo(Convert.ToDouble(p.X), Convert.ToDouble(p.Y));
        }

        public async ValueTask SendText(string text)
        {
            if (NativeMethods.GetForegroundWindow() != process.MainWindowHandle)
                NativeMethods.SetForegroundWindow(process.MainWindowHandle);

            simulator.Keyboard.TextEntry(text);
            await Task.Delay(25);
        }

        public void PasteFromClipboard()
        {
            simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LCONTROL, VirtualKeyCode.VK_V);
        }
    }
}
