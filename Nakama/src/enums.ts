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
    PlayerInput = 1,
    PlayerWon = 2,
    Time = 3,
    ChangeScene = 4
}
