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
    // Connects to an EEG source through one of the IEEGProtocols, and passes on its ThinkGear packets
    // to Controller.setPacket and as events.
    public class EEGConnectionController : MonoBehaviour
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
            IEEGProtocol[] candidates = Candidates();
            bool warned = false;

            while (m_waitForExit)
            {
                IEEGProtocol connected = null;
                foreach (IEEGProtocol candidate in candidates)
                {
                    if (TryConnect(candidate.Port))
                    {
                        connected = candidate;
                        break;
                    }
                }

                if (connected != null)
                {
                    warned = false;
                    Debug.Log("Connected to " + Describe(connected) + ".");
                    try
                    {
                        connected.Read(stream, HandlePacket, () => m_waitForExit);
                    }
                    catch (Exception e) when (e is IOException || e is ObjectDisposedException || e is SocketException)
                    {
                        // the connection dropped, or the game is quitting
                    }
                    Disconnect();
                    client = null;
                    if (m_waitForExit)
                    {
                        Debug.LogWarning("Lost connection to " + Describe(connected) + ".");
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

        private IEEGProtocol[] Candidates()
        {
            var neuroState = new NeuroStateProtocol(neuroStatePort);
            var thinkGear = new ThinkGearProtocol(thinkGearPort);
            switch (source)
            {
                case EEGSource.NeuroState:
                    return new IEEGProtocol[] { neuroState };
                case EEGSource.ThinkGear:
                    return new IEEGProtocol[] { thinkGear };
                default:
                    return new IEEGProtocol[] { neuroState, thinkGear };
            }
        }

        // Returns false if the source isn't running.
        private bool TryConnect(int port)
        {
            var attempt = new TcpClient();
            try
            {
                attempt.Connect(host, port);
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

        private string Describe(IEEGProtocol protocol)
        {
            return protocol.Name + " on " + host + ":" + protocol.Port;
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