using System;
using System.Windows.Forms;

namespace GamePadRawInput
{
    public class EmptyWindow : NativeWindow
    {
        IntPtr[] deviceHandles;

        GamePadRaw joystickRaw;

        byte[] buffer;

        public EmptyWindow(GamePadRaw joystickRaw, ref byte [] buffer)
        {
            CreateHandle(new CreateParams { X = 0, Y = 0, Width = 0, Height = 0, Style = 0x800000, });

            this.joystickRaw = joystickRaw;
            this.buffer = buffer;
        }

        const int WM_INPUT = 0x00ff;
        const int WM_INPUT_DEVICE_CHANGE = 0xfe;

        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_INPUT:
                    if (deviceHandles.Length == 0) break;

                    var n = GamePadRaw.ParseWindowMessage(m.LParam, deviceHandles, ref buffer);
                    if (n) joystickRaw.InputParsedData(ref buffer);
                    break;
                case WM_INPUT_DEVICE_CHANGE:
                    Console.WriteLine("change");
                    deviceHandles = joystickRaw.FindTargetDevices();
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
