const enum Scene
{
    Lobby,
    Game,
    RoundResults,
    FinalResults
}

const enum OperationCode
{
    Players = 0,
    PlayerJoined = 1,
    PlayerLeft = 2,
    PlayerInput = 3,
    PlayerWon = 4,
    Time = 5
}
