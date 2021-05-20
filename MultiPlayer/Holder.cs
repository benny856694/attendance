namespace VideoHelper
{
    public delegate void DisconnectEventHandler();
    public delegate void H264ReceivedEventHandler(byte[] nal);
    public delegate void ToggleDoneEventHandler(bool suc);
    public interface Holder
    {
        event DisconnectEventHandler Disconnect;
        event H264ReceivedEventHandler H264Received;
        event ToggleDoneEventHandler ToggleDone;

        object Tag { get; }

        void Start();
        void Stop();
        string Name { get; }

        bool ToggleAble { get; }
        bool IsToggleOn { get; }

        void Toggle();
    }
}
