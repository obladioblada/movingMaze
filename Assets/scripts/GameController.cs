using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DG.Tweening;
using grid;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    // COLOR
    static Color newCol;
    private const string RED = "#D32F2F";
    private const string YELLOW = "#F4D03F";
    private const string GREEN = "#4CAF50";
    private const string BLUE = "#3498DB";
    static readonly Dictionary<int, string> color = new Dictionary<int, string>();

    // PLAYER
    public static int activePlayer = -1;
    public static List<Player> _players;
    [SerializeField] public GameObject playerGO;

    // CARD
    public static List<Card> _deck;
    private const int cardsNumber = 24;
    [SerializeField] public GameObject cardGO;

    // STATE
    public StateMachine stateMachine = new StateMachine();
    public static State activeState;
    public static Dictionary<State, AbstractState> _states;

    // UI
    [SerializeField] public GameObject playerNameLabel;
    [SerializeField] public GameObject playerScoreLabel;
    [SerializeField] public GameObject imagePlayer;
    [SerializeField] public GameObject canvasGO;
    [SerializeField] public GameObject currentPlayerSelector;
    
    //AUDIO
    private AudioSource _audioSource;

    public GameObject gridGameObject;
    public static GridManager gridManager;
    
    private int connectedDevices;

    public static Player getActivePlayer() {
        return _players[activePlayer];
    }

    private void CreateDeck() {
        _deck = new List<Card>(cardsNumber);
        for (var i = 1; i <= cardsNumber; i++) {
            var card = new Card(i);
            card.cardGO = Instantiate(cardGO, transform);
            Debug.Log("SPRIIIIIIIITE");
            card.cardGO.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Cards/card_" + (i - 1));
            card.cardGO.name = "card_" + i;
            card.snippet = "id:" + card.id + ", GoName:" + card.cardGO.name;
            _deck.Add(card);
            // todo get associateimage
        }

        _deck = _deck.OrderBy(a => Guid.NewGuid()).ToList();
        Destroy(cardGO);
    }

    void Awake() {
        CreateDeck();
        gridManager = gridGameObject.GetComponent<GridManager>();
        _players = new List<Player>();
        Debug.Log("Awaking...");
        color.Add(0, RED);
        color.Add(1, YELLOW);
        color.Add(2, GREEN);
        color.Add(3, BLUE);
        ColorUtility.TryParseHtmlString(color[0], out newCol);
        Debug.Log("CANVAS pos " + canvasGO.transform.position);
        imagePlayer.SetActive(false);
        playerNameLabel.SetActive(false);
        playerScoreLabel.SetActive(false);
        currentPlayerSelector.SetActive(false);
        SceneManager.activeSceneChanged += ChangedActiveScene;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canvasGO);
        _audioSource = GetComponent<AudioSource>();
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
            stateMachine.currentState.HandleInput();
        }
    }

    private static Boolean gameStarted;
    
    private void ChangedActiveScene(Scene current, Scene next)
    {
        Debug.Log("SCENEEEEE current" + current);
        Debug.Log("SCENEEEEE next" + next.name);
        if ( next.name == "game" && !gameStarted)
        {
           StartGame();
           stateMachine.ChangeState(_states[State.STATE_SHIFT]);
           currentPlayerSelector.SetActive(true);
           gameStarted = true;
           _audioSource.Play();
        }
        else {
            ResumeGame();
        }
    }
    
    private void ResumeGame() {
    }



    void OnDisconnect(int deviceID) {
        Debug.Log("Device " + deviceID + " disconnected");
        foreach (var player in _players) {
            if (player.deviceIDs.Contains(deviceID)) {
                Debug.Log("oh you should come back to the game dude!");
            }
            else {
                Debug.Log("another player! don't need you!");
            }
        }
    }

    void OnConnect(int deviceID) {
        // todo, if previous connected player, send message with color and state of player
        if (_players.Count < 4) {
            var img = initPlayerUI(deviceID, out var tempTextBox, out var tempScoreBox);
            var c = color[_players.Count];
            var playerGOGameObject = initPlayerGameObject(_players.Count, c);
            Debug.Log("PLAYERRRRR " + _players.Count);
            var player = new Player(playerGOGameObject,
                AirConsole.instance.GetNickname(deviceID),
                _players.Count, deviceID, _players.Count == 0, c);
            Debug.Log("SCORE POS OUT");
            Debug.Log(tempScoreBox.transform.position);
            player.playerImage = img;
            player.playerLabel = tempTextBox;
            player.playerScoreLabel = tempScoreBox;
            player.playerScoreLabel.SetActive(false);
            player.initialPosition = player.playerGameObject.transform.position;
            _players.Add(player);
        }
        else {
            //todo: send message to screen saying number of max player reached
            Debug.Log("player with device ID" + deviceID + "can't playe");
        }
    }

    [SerializeField] private float spanPLayerUI = 1;

    private GameObject initPlayerUI(int deviceID, out GameObject tempTextBox, out GameObject tempScoreBox) {
        Debug.Log("INIT------UI");
        imagePlayer.SetActive(true);
        playerNameLabel.SetActive(true);
        playerScoreLabel.SetActive(true);
        var img = Instantiate(imagePlayer, canvasGO.transform);
        tempTextBox = Instantiate(playerNameLabel, canvasGO.transform);
        tempScoreBox = Instantiate(playerScoreLabel, canvasGO.transform);
        var playerName = AirConsole.instance.GetNickname(deviceID);
        tempTextBox.GetComponent<TextMeshProUGUI>().text = playerName;
        tempTextBox.name = playerName + "Name";
        tempScoreBox.gameObject.name = playerName + "Score";;
        tempTextBox.transform.DOMove(playerNameLabel.transform.position + Vector3.down * (connectedDevices * spanPLayerUI), 0.3f);
        tempScoreBox.transform.DOMove(playerScoreLabel.transform.position + Vector3.down * (connectedDevices * spanPLayerUI), 0.3f);
        StartCoroutine(DownloadImage(img, AirConsole.instance.GetProfilePicture(deviceID)));
        img.gameObject.name = playerName + "Img";
        img.transform.DOMove(imagePlayer.transform.position + Vector3.down * (connectedDevices * spanPLayerUI) , 0.3f);
        connectedDevices++;
        imagePlayer.SetActive(false);
        playerNameLabel.SetActive(false);
        playerScoreLabel.SetActive(false);
        return img;
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
                            var targetTile = gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos);
                            if (gridManager._allowedTilePath.Contains(targetTile) &&
                                !DOTween.IsTweening(getActivePlayer().playerGameObject.transform)) {
                                var playerTile = gridManager._tiles
                                    .Find(t => (Vector2) t.gameObject.transform.position ==
                                               (Vector2) getActivePlayer().playerGameObject.transform.position);
                                if (targetTile != playerTile) {
                                    Debug.Log("player tile with weight = " + playerTile.weight);
                                    Debug.Log("target tile with weight = " + targetTile.weight);
                                    Debug.Log("connected tile with weight = " + targetTile.connectedTile.weight);
                                    var path = new List<Tile>();
                                    movingState.getPath(path, targetTile);
                                    if (path.Count > 0) {
                                        var vectorPath = path.OrderBy(tile => tile.weight).Select(tile => {
                                            var tilePos = tile.gameObject.transform.position;
                                            Debug.Log(new Vector3(tilePos.x, tilePos.y,
                                                getActivePlayer().playerGameObject.transform.position.z));
                                            return new Vector3(tilePos.x, tilePos.y,
                                                getActivePlayer().playerGameObject.transform.position.z);
                                        });
                                        var enumerable = vectorPath as Vector3[] ?? vectorPath.ToArray();
                                        Debug.Log("vectorPath.count " + enumerable.Count());
                                        getActivePlayer().playerGameObject.transform.DOPath(enumerable.ToArray(),
                                            1f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() => {
                                            if (getActivePlayer().cards.Count > 0) {
                                                Debug.Log("TARGET TILE");
                                                if (targetTile.card != null) {
                                                    Debug.Log(targetTile.card.id);
                                                    Debug.Log(targetTile.card.cardGO.name);
                                                    Debug.Log(targetTile.card.id + ":" + getActivePlayer().cards.Peek().id);
                                                }

                                                if (targetTile.card == null || getActivePlayer().cards.Peek().id !=
                                                    targetTile.card.id) {
                                                    Debug.Log("CHANGING STATE!");
                                                    stateMachine.ChangeState(_states[State.STATE_SHIFT]);
                                                    return;
                                                }

                                                getActivePlayer().cards.Pop();
                                                getActivePlayer().playerScoreLabel.GetComponent<TextMeshProUGUI>().text = "" + getActivePlayer().cards.Count;
                                                    
                                                Debug.Log(getActivePlayer().ToString());
                                                var bytes = getActivePlayer().cards.Peek().cardGO.GetComponent<SpriteRenderer>().sprite
                                                    .texture.EncodeToPNG();
                                                UpdateActivePlayerCard(getActivePlayer().number,
                                                    Convert.ToBase64String(bytes));
                                                Debug.Log("GAME FINISHED");
                                                SceneManager.LoadScene(0);
                                            }
                                            else if (getActivePlayer().initialPosition ==
                                                     (Vector2) targetTile.gameObject.transform.position) {
                                                Debug.Log("GAME FINISHED");
                                                stateMachine.ChangeState(_states[State.STATE_MENU]);
                                            }
                                        });
                                    }
                                }
                                else {
                                    Debug.Log("CHANGING STATE!");
                                    stateMachine.ChangeState(_states[State.STATE_SHIFT]);
                                    ;
                                }
                            }


                            break;
                        case (int) InputController.INPUT_LEFT:
                            if (!movingState.checkIfSelectedTileInPath(Vector2.left)) return;
                            movingState.resetTileColor();
                            if (movingState.selectedTilePos.x == 0)
                                movingState.selectedTilePos = new Vector2(GridManager.N - 1, movingState.selectedTilePos.y);
                            else movingState.selectedTilePos += Vector2.left;
                            gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos)
                                .SetColor(Color.gray);
                            break;
                        case (int) InputController.INPUT_RIGHT:
                            if (!movingState.checkIfSelectedTileInPath(Vector2.right)) return;
                            movingState.resetTileColor();
                            if ((int) movingState.selectedTilePos.x == GridManager.N - 1)
                                movingState.selectedTilePos = new Vector2(0, movingState.selectedTilePos.y);
                            else movingState.selectedTilePos += Vector2.right;
                            gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos)
                                .SetColor(Color.grey);
                            break;

                        case (int) InputController.INPUT_UP:
                            if (!movingState.checkIfSelectedTileInPath(Vector2.up)) return;
                            movingState.resetTileColor();
                            if ((int) movingState.selectedTilePos.y == GridManager.N - 1)
                                movingState.selectedTilePos = new Vector2(movingState.selectedTilePos.x, 0);
                            else movingState.selectedTilePos += Vector2.up;
                            gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos)
                                .SetColor(Color.grey);
                            break;

                        case (int) InputController.INPUT_DOWN:
                            if (!movingState.checkIfSelectedTileInPath(Vector2.down)) return;
                            movingState.resetTileColor();
                            if ((int) movingState.selectedTilePos.y == 0)
                                movingState.selectedTilePos = new Vector2(movingState.selectedTilePos.x, GridManager.N - 1);
                            else movingState.selectedTilePos += Vector2.down;
                            gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == movingState.selectedTilePos)
                                .SetColor(Color.gray);
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
        AirConsole.instance.SetActivePlayers(4);
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
        Debug.Log("AirConsolePlayerCount " + playersCount);
        for (var index = 0; index < playersCount; index++) {
            var c = color[index];
            _players[index].cards = new Stack<Card>(_deck.GetRange(index * cardsPerPlayer, cardsPerPlayer));
            Debug.Log("PLAYERRRRR " + index);
            _players[index].cards.ToList().ForEach(i => Debug.Log(i.id));
            _players[index].playerScoreLabel.SetActive(true);
            _players[index].playerScoreLabel.GetComponent<TextMeshProUGUI>().text = ""+_players[index].cards.Count;
            Debug.Log(_players[index].ToString());
            sendMessageToPlayer(updatePlayerMessage(index, index == 0), index);
            if (_players[index].cards.Peek().cardGO.GetComponent<SpriteRenderer>() != null) {
                var bytes = _players[index].cards.Peek().cardGO.GetComponent<SpriteRenderer>().sprite.texture.EncodeToPNG();
                UpdateActivePlayerCard(index, Convert.ToBase64String(bytes));
            }
        }
    }

    private GameObject initPlayerGameObject(int id, string c) {
        ColorUtility.TryParseHtmlString(c, out newCol);
        var go = Instantiate(playerGO, transform);
        go.GetComponent<SpriteRenderer>().color = newCol;
        go.name = "Player_" + id;
        var z = playerGO.transform.position.z;
        go.transform.position = id switch {
            0 => new Vector3(0, 0, z),
            1 => new Vector3(0, GridManager.N - 1, z),
            2 => new Vector3(GridManager.N - 1, 0, z),
            3 => new Vector3(GridManager.N - 1, GridManager.N - 1, z),
            _ => go.transform.position
        };
        return go;
    }

    public static object updatePlayerCardMessage(string imageBase64, string snippet) {
        return new {
            action = "UPDATE_STATE_CARD",
            card_url = imageBase64,
            card_snippet = snippet
        };
    }

    public static object updatePlayerMessage(int playerNumber, bool isActive) {
        return new {
            action = "UPDATE_STATE",
            color = color[playerNumber],
            active = isActive,
            state = activeState,
            device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(playerNumber),
        };
    }

    public static void UpdateActivePlayer(int newPlayerNumber) {
        sendMessageToPlayer(updatePlayerMessage(newPlayerNumber - 1, false), newPlayerNumber - 1);
        sendMessageToPlayer(updatePlayerMessage(newPlayerNumber, true), newPlayerNumber);
    }

    public static void UpdateActivePlayerCard(int playerNumber, string imageBase64) {
        var snippet = _players[playerNumber].cards.Peek().snippet;
        sendMessageToPlayer(updatePlayerCardMessage(imageBase64, snippet), playerNumber);
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