using System.Threading;

namespace JackettCore.Services
{
    public interface ITrayLockService
    {
        void WaitForSignal();
        void Signal();
    }

    public class TrayLockService : ITrayLockService
    {
        private readonly string EVENT_HANDLE_NAME = "JACKETT.TRAY";

        private EventWaitHandle GetEventHandle()
        {
            return new EventWaitHandle(false, EventResetMode.ManualReset, EVENT_HANDLE_NAME); 
        }

        public void WaitForSignal()
        {
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                GetEventHandle().Reset();
                GetEventHandle().WaitOne();
            }
        }

        public void Signal()
        {
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                GetEventHandle().Set();
            }
        }
    }
}
