using System.Runtime.InteropServices;

namespace WorkMonitor_360.Services
{
    public class IdleDetectionService
    {
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public TimeSpan GetIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            GetLastInputInfo(ref lastInputInfo);

            uint idleTime = ((uint)Environment.TickCount - lastInputInfo.dwTime);

            return TimeSpan.FromMilliseconds(idleTime);
        }
    }
}