// <copyright file="DirectXSendInput.cs" company="FT Software">
// Copyright (c) 2016 Florian Thurnwald. All rights reserved.
// </copyright>
// <author>Florian Thurnwald</author>

namespace ProjectWerner.MouseCursor.Utils
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Utils;

    public class DirectXSendInput
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, Input[] inputs, int sizeOfInputStructure);

        public static void SendKeyDown(DirectXKeys key)
        {
            var input = new Input();
            input.Type = (uint) InputType.Keyboard;
            input.Data.Keyboard = new KeyboardInput();
            input.Data.Keyboard.Vk = 0;
            input.Data.Keyboard.Scan = (ushort) key;
            input.Data.Keyboard.Flags = (uint) KeyboardFlag.Scancode;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            var inputList = new Input[1];
            inputList[0] = input;
            var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof (Input)));
            if (numberOfSuccessfulSimulatedInputs == 0)
            {
                throw new Exception($"The key down simulation for {key} was not successful.");
            }
        }

        public static void SendKeyUp(DirectXKeys key)
        {
            var input = new Input();
            input.Type = (uint) InputType.Keyboard;
            input.Data.Keyboard = new KeyboardInput();
            input.Data.Keyboard.Vk = 0;
            input.Data.Keyboard.Scan = (ushort) key;
            input.Data.Keyboard.Flags = (uint) KeyboardFlag.Scancode | (uint) KeyboardFlag.KeyUp;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            var inputList = new Input[1];
            inputList[0] = input;
            var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof (Input)));
            if (numberOfSuccessfulSimulatedInputs == 0)
            {
                throw new Exception($"The key up simulation for {key} was not successful.");
            }
        }

        public static async Task SendKeyPressAsync(DirectXKeys key)
        {
            SendKeyDown(key);
            await Task.Delay(100);
            SendKeyUp(key);

            //var down = new Input();
            //down.Type = (uint)InputType.Keyboard;
            //down.Data.Keyboard = new KeyboardInput();
            //down.Data.Keyboard.Vk = 0;
            //down.Data.Keyboard.Scan = (ushort)key;
            //down.Data.Keyboard.Flags = (uint)KeyboardFlag.Scancode;
            //down.Data.Keyboard.Time = 0;
            //down.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            //var up = new Input();
            //up.Type = (uint)InputType.Keyboard;
            //up.Data.Keyboard = new KeyboardInput();
            //up.Data.Keyboard.Vk = 0;
            //up.Data.Keyboard.Scan = (ushort)key;
            //up.Data.Keyboard.Flags = (uint)KeyboardFlag.Scancode | (uint)KeyboardFlag.KeyUp;
            //up.Data.Keyboard.Time = 0;
            //up.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            //var inputList = new Input[2];
            //inputList[0] = down;
            //inputList[1] = up;
            //var numberOfSuccessfulSimulatedInputs = SendInput(2, inputList, Marshal.SizeOf(typeof(Input)));
            //if (numberOfSuccessfulSimulatedInputs == 0)
            //{
            //    throw new Exception($"The key press simulation for {key} was not successful.");
            //}
        }
    }
}