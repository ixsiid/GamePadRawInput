using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GamePadRawInput
{

    public class GamePadRaw
    {

        [DllImport("Kernel32.Dll")]
        static extern ulong GetLastError();

        [DllImport("User32.Dll")]
        static extern bool RegisterRawInputDevices(RawInputDevice[] device, uint uiNumDevices, uint cbSize);


        [DllImport("User32.Dll")]
        static extern uint GetRawInputDeviceList([Out] RawInputDeviceListItem[] pRawInputDeviceList, ref uint puiNumDevices, uint cbSize);


        [DllImport("User32.Dll")]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, IntPtr pData, out uint pcbSize);

        [DllImport("User32.Dll", CharSet = CharSet.Unicode)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, StringBuilder pData, in uint pcbSize);

        [DllImport("User32.Dll")]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, out RawInputDeviceInfo pData, in uint pcbSize);


        [DllImport("User32.Dll")]
        static extern uint GetRawInputData(IntPtr hRawInput, RawInputGetBehavior uiBehavior, out RawInputHeader pData, ref uint pcbSize, uint cbSizeHeader);

        [DllImport("User32.Dll")]
        static extern uint GetRawInputData(IntPtr hRawInput, RawInputGetBehavior uiBehavior, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);


        [DllImport("User32.Dll")]
        static extern uint GetRawInputBuffer(IntPtr pData, ref uint pcbSize, uint cbSizeHeader);


        bool registered;
        EmptyWindow window;

        public delegate void RawInputEventArgs(object sender, ref byte [] values);
        public event RawInputEventArgs OnRawInput;

        public GamePadRaw(ref byte[] buffer)
        {
            window = new EmptyWindow(this, ref buffer);

            Initialize(window.Handle);
        }

        public void Initialize(IntPtr windowHandle)
        {
            if (registered) UnregistJoystickDevice();

            registered = false;
            if (!RegistJoystickDevice(windowHandle)) return;
            registered = true;

            var deviceHandle = SelectTargetDevicesHandle(GetAllInputDevices());
            if (deviceHandle.Length == 0)
            {
                UnregistJoystickDevice();
                registered = false;
                return;
            }
        }

        ~GamePadRaw()
        {
            if (registered) UnregistJoystickDevice();
        }

        private RawInputDeviceListItem[] GetAllInputDevices()
        {
            var size = (uint)Marshal.SizeOf<RawInputDeviceListItem>();

            // Get device count by passing null for pRawInputDeviceList.
            uint deviceCount = 0;
            GetRawInputDeviceList(null, ref deviceCount, size);

            // Now, fill the buffer using the device count.
            var devices = new RawInputDeviceListItem[deviceCount];
            uint res = GetRawInputDeviceList(devices, ref deviceCount, size);

            return devices;
        }

        public IntPtr[] FindTargetDevices()
        {
            return SelectTargetDevicesHandle(GetAllInputDevices());
        }

        private bool RegistJoystickDevice(IntPtr handle)
        {
            RawInputDevice[] d = new RawInputDevice[1];
            d[0].usUsage = RawInputDeviceUsageId.GamePad;
            d[0].usUsagePage = RawInputDeviceUsagePage.GenericDesktopControls;
            d[0].dwFlags = RawInputDeviceFlags.InputSink | RawInputDeviceFlags.DevNotify;
            d[0].hWnd = handle;

            return RegisterRawInputDevices(d, 1, (uint)Marshal.SizeOf<RawInputDevice>());
        }


        private bool UnregistJoystickDevice()
        {
            RawInputDevice[] d = new RawInputDevice[1];
            d[0].usUsage = RawInputDeviceUsageId.GamePad;
            d[0].usUsagePage = RawInputDeviceUsagePage.GenericDesktopControls;
            d[0].dwFlags = RawInputDeviceFlags.Remove;
            d[0].hWnd = IntPtr.Zero;

            return RegisterRawInputDevices(d, 1, (uint)Marshal.SizeOf<RawInputDevice>());
        }

        private IntPtr[] SelectTargetDevicesHandle(RawInputDeviceListItem[] devices)
        {
            return devices.Select(device => device.handle)
                .Where(handle =>
                {
                    uint infoSize = (uint)Marshal.SizeOf<RawInputDeviceInfo>();
                    GetRawInputDeviceInfo(handle, RawInputDeviceInfoBehavior.DeviceInfo, out var deviceInfo, in infoSize);

                    return deviceInfo.Type == RawInputDeviceType.Hid;
                })
                .Where(handle =>
                {
                    GetRawInputDeviceInfo(handle, RawInputDeviceInfoBehavior.DeviceName, IntPtr.Zero, out var size);
                    var sb = new StringBuilder((int)size);
                    GetRawInputDeviceInfo(handle, RawInputDeviceInfoBehavior.DeviceName, sb, in size);

                    return sb.ToString().IndexOf("{00001812-0000-1000-8000-00805f9b34fb}") >= 0;
                }).ToArray();
        }

        public void InputParsedData(ref byte[] buffer)
        {
            OnRawInput?.Invoke(this, ref buffer);
        }

        static public unsafe bool ParseWindowMessage(IntPtr wmLParam, IntPtr[] targetHandles, ref byte [] buffer)
        {
            if (targetHandles.Length == 0) return false;
            var header = GetHeaderFromWMInputLParam(wmLParam);

            if (!targetHandles.Contains(header.DeviceHandle)) return false;

            uint headerSize = (uint)Marshal.SizeOf<RawInputHeader>();
            uint size = 0;
            GetRawInputData(wmLParam, RawInputGetBehavior.Input, IntPtr.Zero, ref size, headerSize);

            var bytes = new byte[size];

            var raw = new RawHid();
            fixed (byte* bytesPtr = bytes)
            {
                GetRawInputData(wmLParam, RawInputGetBehavior.Input, (IntPtr)bytesPtr, ref size, headerSize);

                header = *(RawInputHeader*)bytesPtr;

                int* ptr = (int*)(bytesPtr + headerSize);
                raw.dwSizeHid = ptr[0];
                raw.dwCount = ptr[1];
                // raw.rawData = new byte[raw.dwSizeHid * raw.dwCount];
                // Marshal.Copy(new IntPtr(&ptr[2]), raw.rawData, 0, raw.rawData.Length);
                Marshal.Copy(new IntPtr(&ptr[2]), buffer, 0, Math.Min(raw.dwSizeHid * raw.dwCount, buffer.Length));
            }
            
            return true;
        }

        static private RawInputHeader GetHeaderFromWMInputLParam(IntPtr lparam)
        {
            uint headerSize = (uint)Marshal.SizeOf<RawInputHeader>();
            uint size = headerSize;
            GetRawInputData(lparam, RawInputGetBehavior.Header, out var header, ref size, headerSize);

            return header;
        }
    }
}
