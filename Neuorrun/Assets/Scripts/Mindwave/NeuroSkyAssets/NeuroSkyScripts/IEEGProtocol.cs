using System;
using System.IO;

namespace MindWave
{
    // One way of getting EEG values over TCP. EEGConnectionController opens the connection; the
    // protocol reads from it and passes on what arrives as ThinkGear packets.
    public interface IEEGProtocol
    {
        string Name { get; }
        int Port { get; }

        // Reads until the connection closes or keepReading returns false. Runs on the connection thread.
        void Read(Stream stream, Action<string> onPacket, Func<bool> keepReading);
    }
}
