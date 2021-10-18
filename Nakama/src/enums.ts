const enum Scene
{
    Initializer = 0,
    Splash = 1,
    Home = 2,
    Lobby = 3,
    Battle = 4,
    RoundResults = 5,
    FinalResults = 6
}

const enum OperationCode
{
    Players = 0,
    PlayerJoined = 1,
    PlayerInput = 2,
    PlayerWon = 3,
    Draw = 4,
    ChangeScene = 5
}
