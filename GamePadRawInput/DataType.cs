using System;
using System.Runtime.InteropServices;

namespace GamePadRawInput
{
    internal struct RawInputDevice
    {
        public RawInputDeviceUsagePage usUsagePage;
        public RawInputDeviceUsageId usUsage;
        public RawInputDeviceFlags dwFlags;
        public IntPtr hWnd;
    }

    internal enum RawInputDeviceUsagePage : ushort
    {
        GenericDesktopControls = 0x01,
        GameControls = 0x05,
        LEDs = 0x08,
        Button = 0x09,
    }

    internal enum RawInputDeviceUsageId : ushort
    {
        Pointer = 0x01,
        Mouse = 0x02,
        Joystick = 0x04,
        GamePad = 0x05,
        Keyboard = 0x06,
        Keypad = 0x07,
        MultiAxisController = 0x08,
    }


    [Flags]
    internal enum RawInputDeviceFlags
    {
        None,
        /// <summary>
        /// RIDEV_REMOVE. Removes the top level collection from the inclusion list.
        /// </summary>
        Remove = 0x1,
        /// <summary>
        /// RIDEV_EXCLUDE. Specifies the top level collections to exclude when reading a complete usage page.
        /// </summary>
        Exclude = 0x10,
        /// <summary>
        /// RIDEV_PAGEONLY. Specifies all devices whose top level collection is from the specified <see cref="RawInputDeviceRegistration.UsagePage"/>.
        /// </summary>
        PageOnly = 0x20,
        /// <summary>
        /// RIDEV_NOLEGACY. Prevents any devices specified by <see cref="RawInputDeviceRegistration.UsagePage"/> or <see cref="RawInputDeviceRegistration.Usage"/> from generating legacy messages.
        /// </summary>
        NoLegacy = 0x30,
        /// <summary>
        /// RIDEV_INPUTSINK. Enables the caller to receive the input even when the caller is not in the foreground. Note that <see cref="RawInputDeviceRegistration.HwndTarget"/> must be specified.
        /// </summary>
        InputSink = 0x100,
        /// <summary>
        /// RIDEV_CAPTUREMOUSE. The mouse button click does not activate the other window.
        /// </summary>
        CaptureMouse = 0x200,
        /// <summary>
        /// RIDEV_NOHOTKEYS. The application-defined keyboard device hotkeys are not handled. This can be specified even if <see cref="NoLegacy"/> is not specified and <see cref="RawInputDeviceRegistration.HwndTarget"/> is <see cref="IntPtr.Zero"/>.
        /// </summary>
        NoHotKeys = 0x200,
        /// <summary>
        /// RIDEV_APPKEYS. The application command keys are handled. This can be specified only if <see cref="NoLegacy"/> is specified for a keyboard device.
        /// </summary>
        AppKeys = 0x400,
        /// <summary>
        /// RIDEV_EXINPUTSINK. Enables the caller to receive input in the background only if the foreground application does not process it.
        /// </summary>
        ExInputSink = 0x1000,
        /// <summary>
        /// RIDEV_DEVNOTIFY. Enables the caller to receive WM_INPUT_DEVICE_CHANGE notifications for device arrival and device removal.
        /// </summary>
        DevNotify = 0x2000,
    }

    internal struct RawInputDeviceListItem
    {
        public IntPtr handle;
        public RawInputDeviceType type;
    }

    internal enum RawInputDeviceType
    {
        Mouse,
        Keyboard,
        Hid,
    }

    internal enum RawInputDeviceInfoBehavior : uint
    {
        /// <summary>
        /// RIDI_PREPARSEDDATA
        /// </summary>
        PreparsedData = 0x20000005,
        /// <summary>
        /// RIDI_DEVICENAME
        /// </summary>
        DeviceName = 0x20000007,
        /// <summary>
        /// RIDI_DEVICEINFO
        /// </summary>
        DeviceInfo = 0x2000000b,
    }

    internal enum RawInputGetBehavior : uint
    {
        Input = 0x10000003,
        Header = 0x10000005,
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct RawInputDeviceInfo
    {
        [FieldOffset(0)]
        readonly int sbSize;
        [FieldOffset(4)]
        readonly RawInputDeviceType dwType;
        [FieldOffset(8)]
        readonly RawInputMouseInfo mouse;
        [FieldOffset(8)]
        readonly RawInputKeyboardInfo keyboard;
        [FieldOffset(8)]
        readonly RawInputHidInfo hid;

        /// <summary>
        /// dwType
        /// </summary>
        public RawInputDeviceType Type => dwType;

        /// <summary>
        /// mouse
        /// </summary>
        public RawInputMouseInfo Mouse => mouse;

        /// <summary>
        /// keyboard
        /// </summary>
        public RawInputKeyboardInfo Keyboard => keyboard;

        /// <summary>
        /// hid
        /// </summary>
        public RawInputHidInfo Hid => hid;
    }


    /// <summary>
    /// RID_DEVICE_INFO_KEYBOARD
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputKeyboardInfo
    {
        readonly int dwType;
        readonly int dwSubType;
        readonly int dwKeyboardMode;
        readonly int dwNumberOfFunctionKeys;
        readonly int dwNumberOfIndicators;
        readonly int dwNumberOfKeysTotal;

        /// <summary>
        /// dwType
        /// </summary>
        public int KeyboardType => dwType;

        /// <summary>
        /// dwSubType
        /// </summary>
        public int KeyboardSubType => dwSubType;

        /// <summary>
        /// dwKeyboardMode
        /// </summary>
        public int KeyboardMode => dwKeyboardMode;

        /// <summary>
        /// dwNumberOfFunctionKeys
        /// </summary>
        public int FunctionKeyCount => dwNumberOfFunctionKeys;

        /// <summary>
        /// dwNumberOfIndicators
        /// </summary>
        public int IndicatorCount => dwNumberOfIndicators;

        /// <summary>
        /// dwNumberOfKeysTotal
        /// </summary>
        public int TotalKeyCount => dwNumberOfKeysTotal;
    }

    /// <summary>
    /// RID_DEVICE_INFO_MOUSE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputMouseInfo
    {
        readonly int dwId;
        readonly int dwNumberOfButtons;
        readonly int dwSampleRate;
        [MarshalAs(UnmanagedType.Bool)]
        readonly bool fHasHorizontalWheel;

        /// <summary>
        /// dwId
        /// </summary>
        public int Id => dwId;

        /// <summary>
        /// dwNumberOfButtons
        /// </summary>
        public int ButtonCount => dwNumberOfButtons;

        /// <summary>
        /// dwSampleRate
        /// </summary>
        public int SampleRate => dwSampleRate;

        /// <summary>
        /// fHasHorizontalWheel
        /// </summary>
        public bool HasHorizontalWheel => fHasHorizontalWheel;
    }


    /// <summary>
    /// RID_DEVICE_INFO_HID
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputHidInfo
    {
        readonly int dwVendorId;
        readonly int dwProductId;
        readonly int dwVersionNumber;
        readonly ushort usUsagePage;
        readonly ushort usUsage;

        /// <summary>
        /// dwVendorId
        /// </summary>
        public int VendorId => dwVendorId;

        /// <summary>
        /// dwProductId
        /// </summary>
        public int ProductId => dwProductId;

        /// <summary>
        /// dwVersionNumber
        /// </summary>
        public int VersionNumber => dwVersionNumber;

        /// <summary>
        /// usUsagePage, usUsage
        /// </summary>
        public HidUsageAndPage UsageAndPage => new HidUsageAndPage(usUsagePage, usUsage);
    }


    internal struct HidUsageAndPage : IEquatable<HidUsageAndPage>
    {
        public static readonly HidUsageAndPage Mouse = new HidUsageAndPage(0x01, 0x02);
        public static readonly HidUsageAndPage Joystick = new HidUsageAndPage(0x01, 0x04);
        public static readonly HidUsageAndPage GamePad = new HidUsageAndPage(0x01, 0x05);
        public static readonly HidUsageAndPage Keyboard = new HidUsageAndPage(0x01, 0x06);
        public static readonly HidUsageAndPage Pen = new HidUsageAndPage(0x0D, 0x02);
        public static readonly HidUsageAndPage TouchScreen = new HidUsageAndPage(0x0D, 0x04);
        public static readonly HidUsageAndPage TouchPad = new HidUsageAndPage(0x0D, 0x05);

        public HidUsageAndPage(ushort usagePage, ushort usage)
        {
            UsagePage = usagePage;
            Usage = usage;
        }

        public ushort Usage
        {
            get;
        }

        public ushort UsagePage
        {
            get;
        }

        public static bool operator ==(HidUsageAndPage a, HidUsageAndPage b) =>
            a.UsagePage == b.UsagePage &&
            a.Usage == b.Usage;

        public static bool operator !=(HidUsageAndPage a, HidUsageAndPage b) =>
            a.UsagePage != b.UsagePage ||
            a.Usage != b.Usage;

        public bool Equals(HidUsageAndPage other) =>
            GetHashCode() == other.GetHashCode();

        public override bool Equals(object obj) =>
            obj is HidUsageAndPage huap ? Equals(huap) : base.Equals(obj);

        public override int GetHashCode() =>
            typeof(HidUsageAndPage).GetHashCode() ^
            UsagePage.GetHashCode() ^
            Usage.GetHashCode();

        public override string ToString() =>
            $"{UsagePage:X2}:{Usage:X2}";
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputHeader
    {
        readonly RawInputDeviceType dwType;
        readonly int dwSize;
        readonly IntPtr hDevice;
        readonly IntPtr wParam;

        public RawInputDeviceType Type => dwType;
        public int Size => dwSize;
        public IntPtr DeviceHandle => hDevice;
        public IntPtr WParam => wParam;

        public override string ToString() => $"{{{Type}: {DeviceHandle}, WParam: {WParam}}}";
    }

    internal struct RawHid
    {
        public int dwSizeHid;
        public int dwCount;
        public byte[] rawData;
    }
}
