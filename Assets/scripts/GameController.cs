using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using grid;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    
    static Color newCol;
    private const string RED = "#D32F2F";
    private const string YELLOW = "#F4D03F";
    private const string GREEN = "#4CAF50";
    private const string BLUE = "#3498DB";
    
    // Use Dictionary as a map.
    static readonly Dictionary<int, string> color = new Dictionary<int, string>();
    public static int activePlayer;
    public static State activeState;
    private static List<Player> _players;
    public  StateMachine stateMachine;
    public static Dictionary<State, AbstractState> _states;


    [SerializeField] public Camera cam;
    [SerializeField] public Text state_Text;

    public bool isGameStarted;


    public static void UpdateActivePlayer() {
        Debug.Log(activePlayer);
        if (activePlayer == AirConsole.instance.GetActivePlayerDeviceIds.Count - 1) {
            Debug.Log("activePlayer to inactive ");
            sendMessageToPlayer(updatePlayerMessage(activePlayer, false), activePlayer);
            activePlayer = 0;
            sendMessageToPlayer(updatePlayerMessage(activePlayer, true), activePlayer);
        }
        else {
            activePlayer += 1;
            UpdateActivePlayer(activePlayer);
        }
        if (ColorUtility.TryParseHtmlString(color[activePlayer], out newCol)) ;
    }



    void Awake() {
        stateMachine = new StateMachine();
        _players = new List<Player>();
        _states = new Dictionary<State, AbstractState> {
            {State.STATE_MENU, new MenuState(State.STATE_MENU, stateMachine)},
            {State.STATE_SHIFT, new ShiftingState(State.STATE_SHIFT, stateMachine)},
            {State.STATE_MOVE, new MovingState(State.STATE_MOVE, stateMachine)}
        };
        stateMachine.Initialize(_states[State.STATE_MENU], state_Text);
        

        Debug.Log("Awaking...");
        color.Add(0, RED);
        color.Add(1, YELLOW);
        color.Add(2, GREEN);
        color.Add(3, BLUE);
        ColorUtility.TryParseHtmlString(color[0], out newCol) ;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting...");
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }
    
    private void Update()
    {
        stateMachine.currentState.HandleInput();
        if (activeState != State.STATE_MENU) cam.backgroundColor = newCol;
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
        if (data["action"] != null) {
            var action = (int) data["action"];
            Debug.Log(action);
            switch (action) {
                case (int) InputController.INPUT_LEFT:
                    GridManager.MoveArrowLeft();
                    break;
                case (int) InputController.INPUT_RIGHT:
                    GridManager.MoveArrowRight();
                    break;
                case (int) InputController.INPUT_UP:
                    GridManager.MoveArrowUp();
                    break;
                case (int) InputController.INPUT_DOWN:
                    GridManager.MoveArrowDown();
                    break;
                case (int) InputController.INPUT_ROTATE:
                    GridManager.RotateSpareTile();
                    break;
                case (int) InputController.INPUT_INSERT:
                    GridManager.InsertTile();
                    break;
            }
        }
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
        initPlayers();
        activePlayer = 0;
        Debug.Log("players " + AirConsole.instance.GetActivePlayerDeviceIds.Count);
    }

    private static void initPlayers() {
        for (var index = 0; index < AirConsole.instance.GetActivePlayerDeviceIds.Count; index++) {
            var player = new Player(null, AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(index)), 
                index, AirConsole.instance.ConvertPlayerNumberToDeviceId(0), index == 0 , color[index]);
            _players.Add(player);
            sendMessageToPlayer(updatePlayerMessage(index, index == 0), index);
        }
    }

    public static object updatePlayerMessage(int playerNumber, bool isActive) {
        return new {
            action = "UPDATE_STATE",
            color = color[playerNumber],
            active = isActive,
            state = activeState,
            device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(playerNumber)
        };
    }

    public static void UpdateActivePlayer(int newPlayerNumber) {
        sendMessageToPlayer(updatePlayerMessage(newPlayerNumber - 1, false), newPlayerNumber - 1);
        sendMessageToPlayer(updatePlayerMessage(newPlayerNumber, true), newPlayerNumber);
    }

    public static void sendMessageToPlayer(object message, int playerNumber) {
        Debug.Log(playerNumber);
        Debug.Log(message);
        AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerNumber), message);
    }
}
