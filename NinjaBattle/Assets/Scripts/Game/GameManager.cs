using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float TickRate = 4f;

    [SerializeField] private List<MapData> maps = null;
    [SerializeField] private List<string> players = new List<string>();
    [SerializeField] private Map map = null;

    private Dictionary<int, Queue<Direction>> nextPlayerMovement = new Dictionary<int, Queue<Direction>>();
    private MapData currentMap = null;

    public float TickDuration { get => 1 / TickRate; }
    public int CurrentTick { get; private set; }
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize(players.Count);
        StartGame();
    }

    private void Initialize(int playersAmount)
    {
        List<MapData> possibleMaps = maps.FindAll(map => playersAmount >= map.MinimumPlayers && playersAmount <= map.MaximumPlayers);
        currentMap = possibleMaps[UnityEngine.Random.Range(0, possibleMaps.Count)];
        map.Initialize(currentMap, players);
    }

    private void StartGame()
    {
        InvokeRepeating(nameof(ProcessTick), TickDuration, TickDuration);
    }

    private void ProcessTick()
    {
        map.Ninjas.ForEach(ninja => ninja.ProcessTick());
        CurrentTick++;
    }

    public void SetNextPlayerInput(int playerNumber, int tick, Direction direction)
    {
        if (!nextPlayerMovement.ContainsKey(playerNumber))
            nextPlayerMovement.Add(playerNumber, new Queue<Direction>());

        nextPlayerMovement[playerNumber].Enqueue(direction);
        map.GetNinja(playerNumber).SetNextDirection(direction);
    }
}
