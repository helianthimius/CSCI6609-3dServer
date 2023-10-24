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

    // There is only one message type here with ID equals to zero to change the text
    [MessageHandler(0)]
    private static void SetText(ushort clientId, Message message)
    {
        // https://gamedev.stackexchange.com/questions/132569/how-do-i-find-an-object-by-type-and-name-in-unity-using-c
        // Find the text object and change the text to received message
        GameObject.Find("Text1").GetComponent<TMP_Text>().text = message.GetString();
    }
}
