using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MindWave
{
    // Neurostate's JSON over TCP output: one JSON object per line. Each one is passed on as the
    // packet ThinkGear Connector would send, so the game handles both sources the same way.
    public class NeuroStateProtocol : IEEGProtocol
    {
        public NeuroStateProtocol(int port)
        {
            Port = port;
        }

        public string Name => "Neurostate";
        public int Port { get; }

        public void Read(Stream stream, Action<string> onPacket, Func<bool> keepReading)
        {
            using (var reader = new StreamReader(stream, Encoding.ASCII))
            {
                string previous = null;
                string line;
                while (keepReading() && (line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    // Neurostate sends 4 updates a second; passing on only changes keeps the
                    // connection controller's poor signal log from repeating
                    string packet = ToThinkGearPacket(line);
                    if (packet != null && packet != previous)
                    {
                        onPacket(packet);
                        previous = packet;
                    }
                }
            }
        }

        // One message from Neurostate's JSON output; fields the game doesn't use (t, calibrated, bands) are skipped
        [Serializable]
        public class NeuroStateData
        {
            public float attention;
            public float relaxation;
            public float quality;
            public string state;
        }

        // Neurostate's values on ThinkGear's scales, as Neurostate's own ThinkGear output computes
        // them, except that calibrating isn't reported as a good signal: Neurostate sends a neutral
        // attention of 50 meanwhile, which is above the default attention threshold.
        private static string ToThinkGearPacket(string json)
        {
            NeuroStateData data;
            try
            {
                data = JsonUtility.FromJson<NeuroStateData>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to parse Neurostate JSON: " + e.Message);
                return null;
            }

            int poorSignal;
            if (data.state == "ok" || data.state == "uncalibrated")
            {
                // Neurostate has no calibration profiles yet, so its usable values come as "uncalibrated"
                poorSignal = 0;
            }
            else if (data.state == "calibrating")
            {
                poorSignal = 50; // the connection icon's middle (yellow) colour: good signal, but not ready yet
            }
            else if (data.state == "poor_signal")
            {
                poorSignal = Mathf.Clamp(Mathf.RoundToInt(200 * (1 - data.quality / 100)), 1, 199);
            }
            else
            {
                poorSignal = 200; // no_signal, or a state added in a later Neurostate version
            }

            // eSense values run from 1 to 100; ThinkGear uses 0 to mean the signal is too poor to tell
            int attention = poorSignal == 0 ? Mathf.Clamp(Mathf.RoundToInt(data.attention), 1, 100) : 0;
            int meditation = poorSignal == 0 ? Mathf.Clamp(Mathf.RoundToInt(data.relaxation), 1, 100) : 0;
            return "{\"eSense\":{\"attention\":" + attention + ",\"meditation\":" + meditation +
                   "},\"poorSignalLevel\":" + poorSignal + "}";
        }
    }
}
