using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{

    void Awake()
    {
        Debug.Log("Awaking...");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting...");
        AirConsole.instance.onMessage += OnMessage;
    
    }


    void OnMessage(int from, JToken data)
    {
        Debug.Log("message from " + from + " data: " + data);

    }

    private void OnDestroy()
    {
        if (AirConsole.instance != null) AirConsole.instance.onMessage -= OnMessage;
    }
}
