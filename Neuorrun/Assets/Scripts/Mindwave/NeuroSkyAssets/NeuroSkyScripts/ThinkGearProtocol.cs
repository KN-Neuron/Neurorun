using System;
using System.IO;
using System.Text;
using System.Threading;

namespace MindWave
{
    // ThinkGear Connector, or anything else that speaks its protocol. Its packets are passed on as they are.
    public class ThinkGearProtocol : IEEGProtocol
    {
        public ThinkGearProtocol(int port)
        {
            Port = port;
        }

        public string Name => "ThinkGear Connector";
        public int Port { get; }

        public void Read(Stream stream, Action<string> onPacket, Func<bool> keepReading)
        {
            byte[] config = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
            stream.Write(config, 0, config.Length);

            var buffer = new byte[1024];
            while (keepReading())
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    return; // ThinkGear Connector closed the connection
                }
                onPacket(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                Thread.Sleep(100);
            }
        }
    }
}
