using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;

public class ClientSocket : MonoBehaviour
{

    bool socketReady = false;
    TcpClient mySocket;
    public NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    public String Host = "10.125.169.65";
    public Int32 Port = 5005;

    void Start()
    {
        setupSocket();
        TextMessage("SocketTest");
    }


    public void setupSocket()
    {                            // Socket setup here
        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);                // catch any exceptions
        }
    }
    public void TextMessage(string message)
    {
        if (socketReady == true)
        {
            theWriter.Write(message);
            theWriter.Flush();
        }
    }
}