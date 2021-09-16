const JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
const LogicLoadedLoggerInfo = "Custom logic loaded.";
const MatchModuleName = "match";
const TickRate = 4;
const DurationLobby = 30;
const DurationRoundResults = 5;
const DurationFinalResults = 5;
const NecessaryWins = 3;
const MaxPlayers = 4;
const PlayerNotFound = -1;

const MessagesLogic: { [opCode: number]: (message: nkruntime.MatchMessage, state: GameState, dispatcher: nkruntime.MatchDispatcher) => void } =
{
    4: playerWon
}

const MessagesToSendBack: OperationCode[] = [OperationCode.PlayerInput];
