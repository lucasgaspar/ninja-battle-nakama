const JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
const LogicLoadedLoggerInfo = "Custom logic loaded.";
const MatchModuleName = "match";
const TickRate = 16;
const DurationLobby = 10;
const DurationRoundResults = 10;
const NecessaryWins = 3;
const MaxPlayers = 4;
const PlayerNotFound = -1;

const MessagesLogic: { [opCode: number]: (message: nkruntime.MatchMessage, state: GameState, dispatcher: nkruntime.MatchDispatcher) => void } =
{
    2: playerWon,
    3: draw
}
