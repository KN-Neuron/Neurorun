using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine.PlayerLoop;

public class ThinkGearConnector : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private void Start()
    {
        ConnectToThinkGear();
    }

    private void ConnectToThinkGear()
    {
        try
        {
            // Connect to the ThinkGear Connector
            client = new TcpClient();
            client.Connect("127.0.0.1", 13854);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream, Encoding.ASCII);

            // Send the configuration packet
            string configPacket = "{\"enableRawOutput\": false, \"format\": \"Json\"}";
            SendPacket(configPacket);

            // Read the response
            string response = ReadPacket();
            Debug.Log("ThinkGear Connector response: " + response);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to ThinkGear Connector: " + e.Message);
        }
    }

    private void SendPacket(string packet)
    {
        writer.Write(packet);
        writer.Flush();
    }

    private string ReadPacket()
    {
        return reader.ReadLine();
    }

    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }
    }
}





