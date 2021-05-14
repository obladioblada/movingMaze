using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    private const string RED = "#D32F2F";
    private const string YELLOW = "#F4D03F";
    private const string GREEN = "#4CAF50";
    private const string BLUE = "#3498DB";
    
    // Use Dictionary as a map.
    static Dictionary<int, string> color = new Dictionary<int, string>();
    private int activePlayer;



    void Awake()
    {
        Debug.Log("Awaking...");
        // ... Add some keys and values.
        color.Add(0, RED);
        color.Add(1, YELLOW);
        color.Add(2, GREEN);
        color.Add(3, BLUE);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting...");
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void OnDisconnect(int deviceID) {
        Debug.Log("Device " + deviceID + " disconnected");
    }

    void OnConnect(int deviceID) {
        Debug.Log("Device " + deviceID + " connected");
    }

    void OnMessage(int from, JToken data)
    {   
        Debug.Log("message from " + from + " data: " + data);
        AirConsole.instance.Message(from,new {});
        
    }
    private void OnDestroy()
    {
        if (AirConsole.instance != null) AirConsole.instance.onMessage -= OnMessage;
    }
    
    public static void StartGame() {
        Debug.Log("Starting game!!");
        try {
            AirConsole.instance.SetActivePlayers(4);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
      
        Debug.Log("first player " + AirConsole.instance.ConvertPlayerNumberToDeviceId(0));
        AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(0),new {
            action = "UPDATE_STATE",
            color = color[0] ,
            active = true,
            device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(0)
        });
        Debug.Log("active players " + AirConsole.instance.GetActivePlayerDeviceIds.Count);
        for (var index = 1; index < AirConsole.instance.GetActivePlayerDeviceIds.Count; index++) {
            AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(index),new {
                action = "UPDATE_STATE",
                color = color[index] ,
                active = false,
                device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(index)
            });
        }
    }
}
