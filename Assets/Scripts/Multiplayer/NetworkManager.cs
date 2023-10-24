using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using TMPro;

// Written inspired by Riptide Tutorial (Assignment Reference)
// https://www.youtube.com/watch?v=6kWNZOFcFQw
public class NetworkManager : MonoBehaviour
{
    // Defining the message type transferring from client to server
    public enum MessageType : ushort
    {
        // Changes the screen text to the clicked button text
        ChangeText = 0,

        // Shows accelerator value by moving the phone
        AcceleratorValue = 1,

        // Shows touch value by scrolling the finger on the screen
        TouchValue = 2,

        // Shows gyroscope value by moving the phone
        GyroscopeValue = 3,

        // Shows step counts by walking with the phone
        PedometerValue = 4,
    }

    // https://en.wikipedia.org/wiki/Singleton_pattern
    // Make this class singleton to guarantee there is only one instance which is
    // accessible by other classes
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log("NetworkManager already exists");
                Destroy(value);
            }
        }

    }
    public Server Server { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Awake()
    {
        Singleton = this;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;

        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
    }

    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
    // This function is called by fix intervals, so arriving messages can be handled here
    private void FixedUpdate()
    {
        Server.Tick();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    // Sets the value of the given text object
    private static void SetValue(string textName, string value)
    {
        // https://gamedev.stackexchange.com/questions/132569/how-do-i-find-an-object-by-type-and-name-in-unity-using-c
        // Find the text object and change the text to received message
        GameObject.Find(textName).GetComponent<TMP_Text>().text = value;
    }

    // Handles the messages to set the text on server
    [MessageHandler((ushort)MessageType.ChangeText)]
    private static void SetText(ushort clientId, Message message)
    {
        SetValue("Text1", message.GetString());
    }

    // Handles the messages to show the accelerator value
    [MessageHandler((ushort)MessageType.AcceleratorValue)]
    private static void SetAcceleratorValue(ushort clientId, Message message)
    {
        SetValue("AcceleratorValue", message.GetString());
    }

    // Handles the messages to show the touch value
    [MessageHandler((ushort)MessageType.TouchValue)]
    private static void SetTouchValue(ushort clientId, Message message)
    {
        SetValue("TouchValue", message.GetString());
    }

    // Handles the messages to show the gyroscope value
    [MessageHandler((ushort)MessageType.GyroscopeValue)]
    private static void SetGyroscopeValue(ushort clientId, Message message)
    {
        SetValue("GyroscopeValue", message.GetString());
    }

    // Handles the messages to show the pedometer value
    [MessageHandler((ushort)MessageType.PedometerValue)]
    private static void SetPedometerValue(ushort clientId, Message message)
    {
        SetValue("PedometerValue", message.GetString());
    }
}
