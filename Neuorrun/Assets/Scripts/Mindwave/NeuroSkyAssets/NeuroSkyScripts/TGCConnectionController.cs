using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MindWave.LitJson;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace MindWave
{
    public class TGCConnectionController : MonoBehaviour
    {
        public enum EEGSource
        {
            Auto,       // Neurostate if it's running, otherwise ThinkGear Connector
            NeuroState, // Neurostate's own JSON output
            ThinkGear   // ThinkGear Connector, or anything else that speaks its protocol
        }

        [Header("EEG source")]
        [SerializeField] private EEGSource source = EEGSource.Auto;
        [SerializeField] private string host = "127.0.0.1";
        [Tooltip("Neurostate's JSON over TCP output.")]
        [SerializeField] private int neuroStatePort = 13855;
        [SerializeField] private int thinkGearPort = 13854;

        private const int RetryDelayMs = 2000;
        private const string NoSignalPacket = "{\"eSense\":{\"attention\":0,\"meditation\":0},\"poorSignalLevel\":200}";

        private TcpClient client;
        private Stream stream;
        private byte[] buffer;
        private Thread thread;

        public delegate void UpdateIntValueDelegate(int value);

        public delegate void UpdateFloatValueDelegate(float value);

        public event UpdateIntValueDelegate UpdatePoorSignalEvent;
        public event UpdateIntValueDelegate UpdateAttentionEvent;
        public event UpdateIntValueDelegate UpdateMeditationEvent;
        public event UpdateIntValueDelegate UpdateRawdataEvent;
        public event UpdateIntValueDelegate UpdateBlinkEvent;

        public event UpdateFloatValueDelegate UpdateDeltaEvent;
        public event UpdateFloatValueDelegate UpdateThetaEvent;
        public event UpdateFloatValueDelegate UpdateLowAlphaEvent;
        public event UpdateFloatValueDelegate UpdateHighAlphaEvent;
        public event UpdateFloatValueDelegate UpdateLowBetaEvent;
        public event UpdateFloatValueDelegate UpdateHighBetaEvent;
        public event UpdateFloatValueDelegate UpdateLowGammaEvent;
        public event UpdateFloatValueDelegate UpdateHighGammaEvent;

        private volatile bool m_waitForExit = true;
        public Controller controller;

        private void Start()
        {
            StartConnection();
            controller = FindObjectOfType<Controller>();
        }

        public void StartConnection()
        {
            if (thread != null && thread.IsAlive)
            {
                return; // already connected, or retrying
            }
            m_waitForExit = true;
            thread = new Thread(Connect) { IsBackground = true };
            thread.Start();
        }

        public void Disconnect()
        {
            try
            {
                stream.Close();
            }
            catch
            {
                Debug.Log("Stream already closed");
            }
            
        }

        // Runs on its own thread. Keeps trying the sources in order, so the EEG software can start
        // after the game, and reconnects whenever the connection drops.
        public void Connect()
        {
            EEGSource[] candidates = source == EEGSource.Auto
                ? new[] { EEGSource.NeuroState, EEGSource.ThinkGear }
                : new[] { source };
            bool warned = false;

            while (m_waitForExit)
            {
                EEGSource? connected = null;
                foreach (EEGSource candidate in candidates)
                {
                    if (TryConnect(candidate))
                    {
                        connected = candidate;
                        break;
                    }
                }

                if (connected != null)
                {
                    warned = false;
                    Debug.Log("Connected to " + Describe(connected.Value) + ".");
                    try
                    {
                        if (connected == EEGSource.NeuroState)
                        {
                            ReadNeuroState();
                        }
                        else
                        {
                            ReadThinkGear();
                        }
                    }
                    catch (Exception e) when (e is IOException || e is ObjectDisposedException || e is SocketException)
                    {
                        // the connection dropped, or the game is quitting
                    }
                    Disconnect();
                    client = null;
                    if (m_waitForExit)
                    {
                        Debug.LogWarning("Lost connection to " + Describe(connected.Value) + ".");
                        HandlePacket(NoSignalPacket); // so the game doesn't keep acting on the last values
                    }
                }
                else if (!warned)
                {
                    Debug.LogWarning("No EEG source found (" + string.Join(", ", Array.ConvertAll(candidates, Describe)) +
                                     "). Retrying in the background.");
                    warned = true;
                }

                for (int waited = 0; m_waitForExit && waited < RetryDelayMs; waited += 100)
                {
                    Thread.Sleep(100);
                }
            }
        }

        // Returns false if the source isn't running.
        private bool TryConnect(EEGSource from)
        {
            var attempt = new TcpClient();
            try
            {
                attempt.Connect(host, from == EEGSource.NeuroState ? neuroStatePort : thinkGearPort);
            }
            catch (Exception)
            {
                attempt.Close();
                return false;
            }
            if (!m_waitForExit)
            {
                attempt.Close();
                return false;
            }
            client = attempt;
            stream = client.GetStream();
            return true;
        }

        private void ReadThinkGear()
        {
            buffer = new byte[1024];
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

            while (m_waitForExit && ParseData())
            {
                Thread.Sleep(100);
            }
        }

        // Neurostate sends one JSON object per line. Each one is passed on as the packet ThinkGear
        // Connector would send, so the rest of the game handles both sources the same way.
        private void ReadNeuroState()
        {
            using (var reader = new StreamReader(stream, Encoding.ASCII))
            {
                string previous = null;
                string line;
                while (m_waitForExit && (line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    // Neurostate sends 4 updates a second; passing on only changes keeps HandlePacket's
                    // poor signal log from repeating
                    string packet = ToThinkGearPacket(line);
                    if (packet != null && packet != previous)
                    {
                        HandlePacket(packet);
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

        private string Describe(EEGSource s)
        {
            return s == EEGSource.NeuroState
                ? "Neurostate on " + host + ":" + neuroStatePort
                : "ThinkGear Connector on " + host + ":" + thinkGearPort;
        }

        public bool IsConnected()
        {
            TcpClient current = client; // the connection thread may replace it meanwhile
            if (current == null || !current.Connected)
            {
                return false;
            }



            return true;
        }



        public class PowerData
        {
            public float delta = 0;
            public float theta = 0;
            public float lowAlpha = 0;
            public float highAlpha = 0;
            public float lowBeta = 0;
            public float highBeta = 0;
            public float lowGamma = 0;
            public float highGamma = 0;
            public PowerData()
            {
            }
        }

        public class SenseData
        {
            public int attention = 0;
            public int meditation = 0;
            public PowerData eegPower = null;
            public SenseData()
            {
            }
        }

        public class PackatData
        {
            public string status = string.Empty;
            public int poorSignalLevel = 0;
            public int rawEeg = 0;
            public int blinkStrength = 0;
            public SenseData eSense = null;
            public PackatData()
            {
            }
        }

        int GetObjectCount(String json)
        {
            int level = 0;
            int count = 0;
            for (int i = 0; i < json.Length; ++i)
            {
                if (json[i].Equals('{'))
                {
                    if (level == 0)
                    {
                        ++count;
                    }
                    ++level;
                }
                if (json[i].Equals('}'))
                {
                    --level;
                }
            }
            return count;
        }

        // Reads what ThinkGear Connector sent since the last call; returns false once it closes the connection.
        private bool ParseData()
        {
            if (!stream.CanRead)
            {
                return false;
            }
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                return false;
            }
            HandlePacket(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            return true;
        }

        private void HandlePacket(String packet)
        {
            try
            {
                List<PackatData> packets = new List<PackatData>();

                if (!string.IsNullOrEmpty(packet))
                {
                    controller.setPacket(packet);
                   /* Debug.Log(packet);*/
                    if (packet.Contains("}"))
                    {
                        int count = GetObjectCount(packet);
                        if (count == 1)
                        {
                            PackatData data = JsonMapper.ToObject<PackatData>(packet);
                            packets.Add(data);
                        }
                        else if (count > 1)
                        {
                            PackatData[] data = JsonMapper.ToObject<PackatData[]>(packet);
                            for (int index = 0; index < data.Length; ++index)
                            {
                                packets.Add(data[index]);
                            }
                        }
                    }
                }

                foreach (PackatData data in packets)
                {
                    if (null == data)
                    {
                        continue;
                    }
                    if (data.poorSignalLevel != 0)
                    {
                        Debug.Log("data.poorSignalLevel: " + data.poorSignalLevel);
                        if (null != UpdatePoorSignalEvent)
                        {
                            UpdatePoorSignalEvent.Invoke(data.poorSignalLevel);
                        }

                        if (null != data.eSense)
                        {
                            if (UpdateAttentionEvent != null)
                            {
                                UpdateAttentionEvent(data.eSense.attention);
                            }
                            if (UpdateMeditationEvent != null)
                            {
                                UpdateMeditationEvent(data.eSense.meditation);
                            }

                            if (null != data.eSense.eegPower)
                            {
                                if (UpdateDeltaEvent != null)
                                {
                                    UpdateDeltaEvent(data.eSense.eegPower.delta);
                                }
                                if (UpdateThetaEvent != null)
                                {
                                    UpdateThetaEvent(data.eSense.eegPower.theta);
                                }
                                if (UpdateLowAlphaEvent != null)
                                {
                                    UpdateLowAlphaEvent(data.eSense.eegPower.lowAlpha);
                                }
                                if (UpdateHighAlphaEvent != null)
                                {
                                    UpdateHighAlphaEvent(data.eSense.eegPower.highAlpha);
                                }
                                if (UpdateLowBetaEvent != null)
                                {
                                    UpdateLowBetaEvent(data.eSense.eegPower.lowBeta);
                                }
                                if (UpdateHighBetaEvent != null)
                                {
                                    UpdateHighBetaEvent(data.eSense.eegPower.highBeta);
                                }
                                if (UpdateLowGammaEvent != null)
                                {
                                    UpdateLowGammaEvent(data.eSense.eegPower.lowGamma);
                                }
                                if (UpdateHighGammaEvent != null)
                                {
                                    UpdateHighGammaEvent(data.eSense.eegPower.highGamma);
                                }
                            }


                        }
                    }
                    else if (data.rawEeg != 0)
                    {
                        if (null != UpdateRawdataEvent)
                        {
                            UpdateRawdataEvent(data.rawEeg);
                        }
                    }
                    else if (data.blinkStrength != 0)
                    {
                        if (null != UpdateRawdataEvent)
                        {
                            UpdateBlinkEvent(data.blinkStrength);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
             /*   Debug.Log("Exception " + e);*/
            }
        } // end HandlePacket

        void OnDisable()
        {
            m_waitForExit = false;
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            m_waitForExit = false;
            Disconnect();
        }


    }
}