"use strict";
function InitModule(ctx, logger, nk, initializer) {
    initializer.registerRpc(JoinOrCreateMatchRpc, joinOrCreateMatch);
    initializer.registerMatch(MatchModuleName, {
        matchInit: matchInit,
        matchJoinAttempt: matchJoinAttempt,
        matchJoin: matchJoin,
        matchLeave: matchLeave,
        matchLoop: matchLoop,
        matchTerminate: matchTerminate
    });
    logger.info(LogicLoadedLoggerInfo);
}
var joinOrCreateMatch = function (context, logger, nakama, payload) {
    var matches;
    var MatchesLimit = 1;
    var MinimumPlayers = 0;
    var label = { open: true };
    matches = nakama.matchList(MatchesLimit, true, JSON.stringify(label), MinimumPlayers, MaxPlayers - 1);
    if (matches.length > 0)
        return matches[0].matchId;
    return nakama.matchCreate(MatchModuleName);
};
var matchInit = function (context, logger, nakama, params) {
    var label = { open: true };
    var gameState = {
        players: [],
        playersWins: [],
        scene: 0 /* Lobby */,
        countdown: DurationLobby * TickRate,
        endMatch: false
    };
    return {
        state: gameState,
        tickRate: TickRate,
        label: JSON.stringify(label),
    };
};
var matchJoinAttempt = function (context, logger, nakama, dispatcher, tick, state, presence, metadata) {
    var gameState = state;
    return {
        state: gameState,
        accept: gameState.scene == 0 /* Lobby */,
    };
};
var matchJoin = function (context, logger, nakama, dispatcher, tick, state, presences) {
    var gameState = state;
    if (gameState.scene != 0 /* Lobby */)
        return { state: gameState };
    for (var _i = 0, presences_1 = presences; _i < presences_1.length; _i++) {
        var presence = presences_1[_i];
        var nextPlayerNumber = getNextPlayerNumber(gameState.players);
        gameState.players[nextPlayerNumber] = presence;
        gameState.playersWins[nextPlayerNumber] = 0;
    }
    dispatcher.broadcastMessage(0 /* Players */, JSON.stringify(gameState.players), presences);
    gameState.countdown = DurationLobby * TickRate;
    var data = { time: gameState.countdown / TickRate };
    dispatcher.broadcastMessage(3 /* Time */, JSON.stringify(data));
    return { state: gameState };
};
var matchLoop = function (context, logger, nakama, dispatcher, tick, state, messages) {
    var gameState = state;
    processMessages(messages, gameState, dispatcher);
    processMatchLoop(gameState, nakama, dispatcher, logger);
    return gameState.endMatch ? null : { state: gameState };
};
var matchLeave = function (context, logger, nakama, dispatcher, tick, state, presences) {
    var gameState = state;
    for (var _i = 0, presences_2 = presences; _i < presences_2.length; _i++) {
        var presence = presences_2[_i];
        var playerNumber = getPlayerNumber(gameState.players, presence.sessionId);
        delete gameState.players[playerNumber];
    }
    if (getPlayersCount(gameState.players) == 0)
        return null;
    return { state: gameState };
};
var matchTerminate = function (context, logger, nakama, dispatcher, tick, state, graceSeconds) {
    return { state: state };
};
function processMessages(messages, gameState, dispatcher) {
    for (var _i = 0, messages_1 = messages; _i < messages_1.length; _i++) {
        var message = messages_1[_i];
        var opCode = message.opCode;
        if (MessagesLogic.hasOwnProperty(opCode))
            MessagesLogic[opCode](message, gameState, dispatcher);
        else
            messagesDefaultLogic(message, gameState, dispatcher);
    }
}
function messagesDefaultLogic(message, gameState, dispatcher) {
    if (MessagesToSendBack.indexOf(message.opCode) != -1) {
        dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
        return;
    }
    else {
        var otherPlayers_1 = [];
        gameState.players.forEach(function (player) {
            if (player != undefined && player.sessionId != message.sender.sessionId)
                otherPlayers_1.push(player);
        });
        dispatcher.broadcastMessage(message.opCode, message.data, otherPlayers_1, message.sender);
    }
}
function processMatchLoop(gameState, nakama, dispatcher, logger) {
    switch (gameState.scene) {
        case 0 /* Lobby */:
            matchLoopLobby(gameState, nakama, dispatcher);
            break;
        case 2 /* RoundResults */:
            matchLoopRoundResults(gameState, nakama, dispatcher);
            break;
        case 3 /* FinalResults */:
            matchLoopFinalResults(gameState, nakama, dispatcher);
            break;
    }
}
function matchLoopLobby(gameState, nakama, dispatcher) {
    if (gameState.countdown > 0) {
        gameState.countdown--;
        if (gameState.countdown == 0) {
            gameState.scene = 1 /* Game */;
            dispatcher.broadcastMessage(4 /* ChangeScene */, JSON.stringify(gameState.scene));
            dispatcher.matchLabelUpdate(JSON.stringify({ open: false }));
        }
    }
}
function matchLoopRoundResults(gameState, nakama, dispatcher) {
    if (gameState.countdown > 0) {
        gameState.countdown--;
        if (gameState.countdown == 0) {
            if (playerObtainedNecessaryWins(gameState.playersWins)) {
                gameState.countdown = DurationFinalResults * TickRate;
                gameState.scene = 3 /* FinalResults */;
                dispatcher.broadcastMessage(4 /* ChangeScene */, JSON.stringify(gameState.scene));
            }
            else {
                gameState.scene = 1 /* Game */;
                dispatcher.broadcastMessage(4 /* ChangeScene */, JSON.stringify(gameState.scene));
            }
        }
    }
}
function matchLoopFinalResults(gameState, nakama, dispatcher) {
    if (gameState.countdown > 0) {
        gameState.countdown--;
        if (gameState.countdown == 0) {
            gameState.endMatch = true;
        }
    }
}
function playerWon(message, gameState, dispatcher) {
    if (gameState.scene != 1 /* Game */)
        return;
    var playerNumber = getPlayerNumber(gameState.players, message.sender.sessionId);
    if (playerNumber == PlayerNotFound)
        return;
    gameState.playersWins[playerNumber]++;
    gameState.countdown = DurationRoundResults * TickRate;
    dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
}
function getPlayersCount(players) {
    var count = 0;
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined)
            count++;
    return count;
}
function playerObtainedNecessaryWins(playersWins) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (playersWins[playerNumber] == NecessaryWins)
            return true;
    return false;
}
function getPlayerNumber(players, sessionId) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (players[playerNumber] != undefined && players[playerNumber].sessionId == sessionId)
            return playerNumber;
    return PlayerNotFound;
}
function getNextPlayerNumber(players) {
    for (var playerNumber = 0; playerNumber < MaxPlayers; playerNumber++)
        if (!playerNumberIsUsed(players, playerNumber))
            return playerNumber;
    return PlayerNotFound;
}
function playerNumberIsUsed(players, playerNumber) {
    return players[playerNumber] != undefined;
}
var JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
var LogicLoadedLoggerInfo = "Custom logic loaded.";
var MatchModuleName = "match";
var TickRate = 4;
var DurationLobby = 10;
var DurationRoundResults = 5;
var DurationFinalResults = 5;
var NecessaryWins = 3;
var MaxPlayers = 4;
var PlayerNotFound = -1;
var MessagesLogic = {
    4: playerWon
};
var MessagesToSendBack = [];
