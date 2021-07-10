using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DG.Tweening;
using grid;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
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
    public static int activePlayer = -1;
    public static State activeState;
    public static List<Player> _players;
    public static List<Card> _deck;
    private const int cardsNumber = 24;
    public StateMachine stateMachine = new StateMachine();
    public static Dictionary<State, AbstractState> _states;


    [SerializeField] public Camera cam;
    [SerializeField] public Text active_player_Text;
    [SerializeField] public GameObject playerGO;
    
    
    [SerializeField] public Text playerNameLabel;
    [SerializeField] public Text playerScoreLabel;
    [SerializeField] public GameObject imagePlayer;
    [SerializeField] public GameObject canvasGO;
    [SerializeField] public GameObject currentPlayerSelector;
    

    public GameObject gridGameObject;
    public static GridManager gridManager;

    public static Player getActivePlayer() {
        return _players[activePlayer];
    }

    private static void CreateDeck() {
        for (var i = 1; i <= cardsNumber; i++) {
            var card = new Card(i);
            _deck.Add(card);
        }
        _deck.ForEach(i => Console.WriteLine(i.id));
        _deck = _deck.OrderBy(a => Guid.NewGuid()).ToList();
    }
    
    void Awake() {
        gridManager = gridGameObject.GetComponent<GridManager>();
        _players = new List<Player>();
        Debug.Log("Awaking...");
        color.Add(0, RED);
        color.Add(1, YELLOW);
        color.Add(2, GREEN);
        color.Add(3, BLUE);
        ColorUtility.TryParseHtmlString(color[0], out newCol);
        _deck = new List<Card>(cardsNumber);
        CreateDeck();
        Debug.Log("CANVAS pos " + canvasGO.transform.position);
        imagePlayer.SetActive(false);
        playerNameLabel.enabled = false;
        playerScoreLabel.enabled = false;
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Starting...");
        _states = new Dictionary<State, AbstractState> {
            {State.STATE_MENU, new MenuState(State.STATE_MENU, stateMachine)},
            {State.STATE_SHIFT, new ShiftingState(State.STATE_SHIFT, stateMachine)},
            {State.STATE_MOVE, new MovingState(State.STATE_MOVE, stateMachine, this)}
        };
        stateMachine.Initialize(_states[State.STATE_MENU]);
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    private void Update() {
        if (stateMachine != null && stateMachine.currentState != null) {
            if (stateMachine.currentState.Name == State.STATE_MENU &&
                Input.GetKeyDown(InputController.INPUT_START)) {
                stateMachine.ChangeState(_states[State.STATE_SHIFT]);
                StartGame();
            }
            stateMachine.currentState.HandleInput();
        }
    }


    void OnDisconnect(int deviceID) {
        Debug.Log("Device " + deviceID + " disconnected");
    }

    private int connectedDevices = 0;

    void OnConnect(int deviceID) {
        if (_players.Count <= 2) {
            imagePlayer.SetActive(true);
            playerNameLabel.enabled = true;
            playerScoreLabel.enabled = true;
            Debug.Log("Device " + deviceID + " connected");
            Debug.Log(AirConsole.instance.GetProfilePicture(deviceID));
            var img = Instantiate(imagePlayer, canvasGO.transform);
            var tempTextBox = Instantiate(playerNameLabel, canvasGO.transform);
            var tempScoreBox = Instantiate(playerScoreLabel, canvasGO.transform);
            var playerName = AirConsole.instance.GetNickname(deviceID);
            tempTextBox.text = playerName;
            tempTextBox.transform.DOMove(tempTextBox.transform.position + Vector3.down * connectedDevices, 0.3f);
            tempScoreBox.transform.DOMove(tempScoreBox.transform.position + Vector3.down * connectedDevices, 0.3f);
            StartCoroutine(DownloadImage(img, AirConsole.instance.GetProfilePicture(deviceID)));
            img.gameObject.name = playerName + "Img";
            img.gameObject.transform.DOMove(img.transform.position + Vector3.down * connectedDevices, 0.3f);
            connectedDevices++;
            imagePlayer.SetActive(false);
            playerNameLabel.enabled = false;
            playerScoreLabel.enabled = false;
            var c = color[_players.Count];
            var playerGOGameObject = initPlayerGameObject(_players.Count, c);
            Debug.Log("PLAYERRRRR " + _players.Count + 1);
            var player = new Player(playerGOGameObject,
                AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(_players.Count)),
                _players.Count, AirConsole.instance.ConvertPlayerNumberToDeviceId(0), _players.Count == 0, c);
            player.playerImage = img;
            player.playerLabel = tempTextBox;
            player.playerScoreLabel = tempScoreBox;
            _players.Add(player);
        }
    }


    IEnumerator DownloadImage(GameObject image, string url) {
        WWW www = new WWW(url);
        yield return www;
        if (www.texture != null)
            image.GetComponent<Image>().sprite =
                Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    } 

    void OnMessage(int from, JToken data) {
        Debug.Log("message from " + from + " data: " + data);
        if (data["action"] != null) {
            var action = (int) data["action"];
            Debug.Log(action);
            switch (stateMachine.currentState.Name) {
                case State.STATE_MENU:
                    switch (action) {
                        case (int) InputController.INPUT_INSERT:
                            stateMachine.ChangeState(_states[State.STATE_SHIFT]);
                            break;
                    }

                    break;
                case State.STATE_SHIFT:
                    switch (action) {
                        case (int) InputController.INPUT_LEFT:
                            gridManager.MoveArrowLeft();
                            break;
                        case (int) InputController.INPUT_RIGHT:
                            gridManager.MoveArrowRight();
                            break;
                        case (int) InputController.INPUT_UP:
                            gridManager.MoveArrowUp();
                            break;
                        case (int) InputController.INPUT_DOWN:
                            gridManager.MoveArrowDown();
                            break;
                        case (int) InputController.INPUT_ROTATE:
                            gridManager.RotateSpareTile();
                            break;
                        case (int) InputController.INPUT_INSERT:
                            gridManager.InsertTile();
                            stateMachine.ChangeState(_states[State.STATE_MOVE]);
                            break;
                    }

                    break;
                case State.STATE_MOVE:
                    var movingState = (MovingState) _states[State.STATE_MOVE];
                    switch (action) {
                        case (int) InputController.INPUT_INSERT:
                            var onTile = gridManager._tiles.Find(t =>
                                (Vector2) t.gameObject.transform.position == movingState.selectedTilePos);
                            if (gridManager._allowedTilePath.Contains(onTile)) {
                                gridManager.MovePlayer(getActivePlayer(), onTile.gameObject.transform.position);
                                stateMachine.ChangeState(_states[State.STATE_SHIFT]);
                            }

                            break;
                        case (int) InputController.INPUT_LEFT:
                            movingState.resetTileColor();
                            if (movingState.selectedTilePos.x == 0) {
                                movingState.selectedTilePos = new Vector2(GridManager.N - 1, movingState.selectedTilePos.y);
                            }
                            else {
                                movingState.selectedTilePos += Vector2.left;
                            }

                            gridManager._tiles.
                                Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos).SetColor(Color.gray);
                            break;
                        case (int) InputController.INPUT_RIGHT:
                            movingState.resetTileColor();
                            if ((int) movingState.selectedTilePos.x == GridManager.N - 1) {
                                movingState.selectedTilePos = new Vector2(0, movingState.selectedTilePos.y);
                            }
                            else {
                                movingState.selectedTilePos += Vector2.right;
                            }

                            gridManager._tiles
                                .Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos).SetColor(Color.grey);
                            break;

                        case (int) InputController.INPUT_UP:
                            movingState.resetTileColor();
                            if ((int) movingState.selectedTilePos.y == GridManager.N - 1) {
                                movingState.selectedTilePos = new Vector2(movingState.selectedTilePos.x, 0);
                            }
                            else {
                                movingState.selectedTilePos += Vector2.up;
                            }
                            gridManager._tiles
                                .Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos).SetColor(Color.grey);
                            break;

                        case (int) InputController.INPUT_DOWN:
                            movingState.resetTileColor();
                            if ((int) movingState.selectedTilePos.y == 0) {
                                movingState.selectedTilePos = new Vector2(movingState.selectedTilePos.x, GridManager.N - 1);
                            }
                            else {
                                movingState.selectedTilePos += Vector2.down;
                            }
                            gridManager._tiles
                                .Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos).SetColor(Color.gray);
                            break;
                    }

                    break;
            }
        }
    }

    private void OnDestroy() {
        if (AirConsole.instance != null) AirConsole.instance.onMessage -= OnMessage;
    }

    public void StartGame() {
        Debug.Log("Starting game!!");
        try {
            AirConsole.instance.SetActivePlayers(4);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
        activePlayer = 0;
        initPlayers();
        ColorUtility.TryParseHtmlString(color[activePlayer], out newCol);
        currentPlayerSelector.GetComponent<Image>().DOColor(newCol, 0.3f);
    }

    private void initPlayers() {
        var playersCount = AirConsole.instance.GetActivePlayerDeviceIds.Count;
        Debug.Log("cardsPerPlayer" + _deck.Count);
        Debug.Log("cardsPerPlayer" + _deck.Count / playersCount);
        var cardsPerPlayer = _deck.Count / playersCount;
        Debug.Log("Cards per player" + cardsPerPlayer);
        Debug.Log("PlayerArrayCount, " + _players.Count);
        Debug.Log("AirConsolePlayerCOunt " + playersCount);
        for (var index = 0; index < playersCount; index++) {
            var c = color[index];
            _players[index].cards = new Stack<Card>(_deck.GetRange(index  * cardsPerPlayer, cardsPerPlayer));
            Debug.Log("PLAYERRRRR " + index);
            _players[index].cards.ToList().ForEach(i => Debug.Log(i.id));
            _players[index].playerScoreLabel.text = ""+_players[index].cards.Count;
            sendMessageToPlayer(updatePlayerMessage(index, index == 0), index);
        }
        //_players[activePlayer].playerGameObject.transform.localScale = 2 * Vector3.one;
    }
    
    

    private GameObject initPlayerGameObject(int id, string c) {
        ColorUtility.TryParseHtmlString(c, out newCol);
        var go = Instantiate(playerGO, transform);
        go.GetComponent<SpriteRenderer>().color = newCol;
        go.name = "Player_" + id;
        go.transform.position = id switch {
            0 => new Vector3(0, 0, -1),
            1 => new Vector3(0, GridManager.N - 1, -1),
            2 => new Vector3(GridManager.N - 1, 0, -1),
            3 => new Vector3(GridManager.N - 1, GridManager.N - 1, -1),
            _ => go.transform.position
        };
        return go;
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

    public void UpdateActivePlayer() {
        Debug.Log(activePlayer);
        _players[activePlayer].playerGameObject.transform.localScale = Vector3.one;
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

        //_players[activePlayer].playerGameObject.transform.localScale = 2 * Vector3.one;
        ColorUtility.TryParseHtmlString(color[activePlayer], out newCol);
        currentPlayerSelector.transform.DOMove(getActivePlayer().playerImage.transform.position, 0.3f);
        currentPlayerSelector.GetComponent<Image>().DOColor(newCol, 0.4f);
    }

    public static void sendMessageToPlayer(object message, int playerNumber) {
        AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerNumber), message);
    }
}