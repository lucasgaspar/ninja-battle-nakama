let matchInit: nkruntime.MatchInitFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, params: { [key: string]: string })
{
    var label: MatchLabel = { open: true }
    var gameState: GameState =
    {
        players: [],
        scene: Scene.Lobby,
        countdown: DurationLobby * TickRate,
        endMatch: false
    }

    return {
        state: gameState,
        tickRate: TickRate,
        label: JSON.stringify(label),
    }
}

let matchJoinAttempt: nkruntime.MatchJoinAttemptFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presence: nkruntime.Presence, metadata: { [key: string]: any })
{
    let gameState = state as GameState;
    return {
        state: gameState,
        accept: gameState.scene == Scene.Lobby,
    }
}

let matchJoin: nkruntime.MatchJoinFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[])
{
    let gameState = state as GameState;
    if (gameState.scene != Scene.Lobby)
        return { state: gameState };

    let playersOnMatch: nkruntime.Presence[] = [];
    gameState.players.forEach(player => { if (player != undefined) playersOnMatch.push(player.presence); });
    for (let presence of presences)
    {
        let player: Player =
        {
            presence: presence,
            wins: 0
        }

        gameState.countdown = DurationLobby * TickRate;
        let nextPlayerNumber: number = getNextPlayerNumber(gameState.players);
        gameState.players[nextPlayerNumber] = player;
        dispatcher.broadcastMessage(OperationCode.PlayerJoined, JSON.stringify(player), playersOnMatch);
        playersOnMatch.push(presence);
    }

    dispatcher.broadcastMessage(OperationCode.Players, JSON.stringify(gameState.players), presences);
    let data: TimeRemainingData = { time: gameState.countdown / TickRate };
    dispatcher.broadcastMessage(OperationCode.Time, JSON.stringify(data));
    return { state: gameState };
}

let matchLoop: nkruntime.MatchLoopFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, messages: nkruntime.MatchMessage[])
{
    let gameState = state as GameState;
    processMessages(messages, gameState, dispatcher);
    processMatchLoop(gameState, nakama, dispatcher, logger);
    return gameState.endMatch ? null : { state: gameState };
}

let matchLeave: nkruntime.MatchLeaveFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[])
{
    let gameState = state as GameState;
    for (let presence of presences)
    {
        let playerNumber: number = getPlayerNumber(gameState.players, presence.sessionId);
        delete gameState.players[playerNumber];
        dispatcher.broadcastMessage(OperationCode.PlayerLeft, JSON.stringify(<PlayerLeaveData>{ sessionId: presence.sessionId }), null);
    }

    if (getPlayersCount(gameState.players) == 0)
        return null;

    return { state: gameState };
}

let matchTerminate: nkruntime.MatchTerminateFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, graceSeconds: number)
{
    return { state };
}

function processMessages(messages: nkruntime.MatchMessage[], gameState: GameState, dispatcher: nkruntime.MatchDispatcher): void
{
    for (let message of messages)
    {
        let opCode: number = message.opCode;
        if (MessagesLogic.hasOwnProperty(opCode))
            MessagesLogic[opCode](message, gameState, dispatcher);
        else
            messagesDefaultLogic(message, gameState, dispatcher);
    }
}

function messagesDefaultLogic(message: nkruntime.MatchMessage, gameState: GameState, dispatcher: nkruntime.MatchDispatcher): void
{
    if (MessagesToSendBack.indexOf(<OperationCode>message.opCode) != -1)
    {
        dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
        return;
    }
    else
    {
        let otherPlayers: nkruntime.Presence[] = [];
        gameState.players.forEach(player =>
        {
            if (player != undefined && player.presence.sessionId != message.sender.sessionId)
                otherPlayers.push(player.presence);
        });
        dispatcher.broadcastMessage(message.opCode, message.data, otherPlayers, message.sender);
    }
}

function processMatchLoop(gameState: GameState, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, logger: nkruntime.Logger): void
{
    switch (gameState.scene)
    {
        case Scene.Lobby: matchLoopLobby(gameState, nakama, dispatcher); break;
        case Scene.RoundResults: matchLoopRoundResults(gameState, nakama, dispatcher); break;
        case Scene.FinalResults: matchLoopFinalResults(gameState, nakama, dispatcher); break;
    }
}

function matchLoopLobby(gameState: GameState, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher): void
{
    if (gameState.countdown > 0)
    {
        gameState.countdown--;
        if (gameState.countdown == 0)
        {
            gameState.scene = Scene.Game;
            dispatcher.matchLabelUpdate(JSON.stringify({ open: false }));
        }
    }
}

function matchLoopRoundResults(gameState: GameState, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher): void
{
    if (gameState.countdown > 0)
    {
        gameState.countdown--;
        if (gameState.countdown == 0)
        {
            if (playerObtainedNecessaryWins(gameState.players))
            {
                gameState.countdown = DurationFinalResults * TickRate;
                gameState.scene = Scene.FinalResults;
            }
            else
            {
                gameState.scene = Scene.Game;
            }
        }
    }
}

function matchLoopFinalResults(gameState: GameState, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher): void
{
    if (gameState.countdown > 0)
    {
        gameState.countdown--;
        if (gameState.countdown == 0)
        {
            gameState.endMatch = true;
        }
    }
}

function playerWon(message: nkruntime.MatchMessage, gameState: GameState, dispatcher: nkruntime.MatchDispatcher): void 
{
    if (gameState.scene != Scene.Game)
        return;

    for (let playerNumber = 0; playerNumber < gameState.players.length; playerNumber++)
    {
        let player: Player = gameState.players[playerNumber];
        if (player.presence.sessionId != message.sender.sessionId)
            continue;

        player.wins++;
        gameState.countdown = DurationRoundResults * TickRate;
        dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
        break;
    }
}

function getPlayersCount(players: Player[]): number
{
    var count: number = 0;
    for (let playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined)
            count++;

    return count;
}

function playerObtainedNecessaryWins(players: Player[]): boolean
{
    for (let playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined && players[playerNumber].wins == NecessaryWins)
            return true;

    return false;
}

function getPlayerNumber(players: Player[], sessionId: string): number
{
    for (let playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined && players[playerNumber].presence.sessionId == sessionId)
            return playerNumber;

    return PlayerNotFound;
}


function getNextPlayerNumber(players: Player[]): number
{
    for (let playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (!playerNumberIsUsed(players, playerNumber))
            return playerNumber;

    return PlayerNotFound;
}

function playerNumberIsUsed(players: Player[], playerNumber: number): boolean
{
    return players[playerNumber] != undefined;
}
