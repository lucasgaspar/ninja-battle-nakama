# Ninja Battle
Ninja Battle is a deterministic multiplayer online game made in Unity that implements basic rollback, intended for use as a example project of different technologies that [Nakama](https://heroiclabs.com/) offers.

Genre: Action PvP

Players: 2 to 4 players

Game engine: Unity 2021.1.18f1

Language: C# for Unity, Typescript for Nakama

Platform: PC, Mac and Web

Multiplayer: Nakama server authoritative

Server logic: Manage score and rounds

Client logic: Send inputs on a deterministic simulation and implement a simple rollback 

Gameplay: You play as a ninja that has to battle against other player-controlled ninjas. When a ninja moves, they leave behind a caltrops path that makes the terrain deadly even for themselves. The ninjas cannot stop moving. You can only control the direction that the ninja is moving in. A single touch to the caltrops eliminates you. The ninja can prevent the damage by jumping over the caltrops automatically. If the ninja runs against a wall or falls into caltrops, he is eliminated instantly. If two ninjas run into each other, both are eliminated.

## Screenshots
![image](https://user-images.githubusercontent.com/7684147/152168433-d5f475b5-fb7d-4129-94bd-7c2da0c25bbb.png)
![image](https://user-images.githubusercontent.com/7684147/152169866-cee459fe-f251-4662-a15a-744ea63fa600.png)
![image](https://user-images.githubusercontent.com/7684147/152170143-c1d0e68d-5469-4c6f-a928-d9214189da31.png)
![image](https://user-images.githubusercontent.com/7684147/152170370-8311835d-fc17-4b3d-a772-781e9c5d86e8.png)

## Feel free to contribute
This is an open source project, all contributions are welcome, if you want to contribute you can create and issue first and we can discuss the changes or the features you want to add in there.

## Nakama Helpers
The Nakama Helpers are a small library of functions I made intended to be used on any project that uses Nakama, it helps to develop easier or as an example of how to do certain things. Eventually I want to create their own repository and link it with this one, they are not official Nakama utilities.

## Set Up Nakama
The easiest way to start with Nakama is with docker, the Nakama documentation offers a nice [guide](https://heroiclabs.com/docs/nakama/getting-started/docker-quickstart/) on how to do this, the server code on this repository is on TypeScript so it requires installing the TypeScript support too, [here's](https://heroiclabs.com/docs/nakama/server-framework/typescript-setup/) the official documentation on how guide to do this.

## Understanding rollback netcode
The rollback netcode is a type of netcode that uses deterministic simulations on each of the clients and works sending their inputs to the others with the information of the exact time it ocurred, the simulation does not wait for the inputs of other users, but if in a determined moment a message is received with an previous time than the actual time of the simulation the simulation should be able to go back in time, handle the inputs and fastforward to the current time.

There's an awesome article that explains how the rollback netcode works [here](https://ki.infil.net/w02-netcode.html).

## Unity structure
The game consist of 7 scenes, numbered from 0 to 6, each scene handle different moments of the application.
| Scene | Logic |
| ------ | ------ |
| 0-Initializer | Initialize all singletons, this scene is run only once per session. |
| 1-Splash | Handles the Nakama login. |
| 2-Home | Main menu of the app in here you can see your trophies, change your name and start a match |
| 3-Lobby | The lobby where you wait for another players, in this scene you are already on a match. |
| 4-Battle | This is where the gameplay occcurs. |
| 5-RoundResults | Shows how many rounds each player has won. |
| 6-FinalResults | Show the winner of the game. |

## Nakama Manager
`NakamaManager` is the base script of all the Nakama Helpers, on this script you can Login and Logout and send RPC to the server.

## Login
The current state of `NakamaManager` handles 3 types of login:
- LoginWithUdid - It generates a random UDID and saves it to the players prefs. Recommended for WebGL since it is impossible to obtain a device ID.
- LoginWithDevice - Uses the device unique identifier to handle the login. Recommended for mobile devices.
- LoginWithCustomId - Uses a string to handle the authentication, recommended for use for testing on the editor.

To call them you just have to reference the instance of `NakamaManager` and call the function you want, for example on the scene `1-Splash` on the script `NakamaÀutoLogin` it's called like this:
```csharp
NakamaManager.Instance.LoginWithUdid();
```
You can hear the response suscribing to the events of NakamaManager 
After the login is executed you can subscribe to It has various events you can subscribe to handle your game logic. You can use the component "NakamaEvents" that exposes all the `NakamaManager` events to the unity inspector, you can see an example on the scene `1-Splash` on the prefab `ChangeSceneOnLogin` that after receiving an login success it executes the `ChangeScene` action of the `SceneChanger`.

## Set up a name
The functionality of setting up a name can be found on the script `SetDisplayName` and on `NakamaUserManager`. On this game we wanted for the players to join a match as soon as possible so if a player don't want to set up a customized name a name will be automatically generated using a combination of two predefined words, this will be done just as soon as the user information is loaded, if there's no name assosiated to that account one will be generated or if its already one it will display it on the area. Now to update the player's name we are suscribing to the `ValueChanged` event of the `TMP_InputField` that each time that it triggers a countown starts so on the moment the player stops typing the name will be saved on the Nakama server, the actual saving of the name is done on the `NakamaUserManager` on the function `UpdateDisplayName``
```csharp
public async void UpdateDisplayName(string displayName)
{
    await NakamaManager.Instance.Client.UpdateAccountAsync(NakamaManager.Instance.Session, null, displayName);
}
```

## Joining a lobby
The lobby feature consists on two parts one is handled via the unity client and other from a custom rpc call on the nakama server, lets follow the logic:
1- The player presses the `Battle` button that triggers the `MultiplayerJoinMatchButton` component function `FindMatch`:
```csharp
private void FindMatch()
{
    button.interactable = false;
    MultiplayerManager.Instance.JoinMatchAsync();
}
```
2- The `MultiplayerManager`registers for the upcoming match states:
```csharp
NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
```
3- The RPC "JoinOrCreateMatchRpc" is called using the `NakamaManager`:
```csharp
private const string JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
...csharp
IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(JoinOrCreateMatchRpc);
```
4- On the server the `joinOrCreateMatch` RPC is executed, this RPC finds one match that has still room left for another player and its still open, open reffers that the game has not started yet, and in the case that no match is found a new match will be created.
```csharp
let joinOrCreateMatch: nkruntime.RpcFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, payload: string): string
{
    let matches: nkruntime.Match[];
    const MatchesLimit = 1;
    const MinimumPlayers = 0;
    var label: MatchLabel = { open: true }
    matches = nakama.matchList(MatchesLimit, true, JSON.stringify(label), MinimumPlayers, MaxPlayers - 1);
    if (matches.length > 0)
        return matches[0].matchId;

    return nakama.matchCreate(MatchModuleName);
}
```
5- Now back on the client we obtain the matchId returned by the server and send a request to join the match
```csharp
string matchId = rpcResult.Payload;
match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId);
```
6- The `onMatchJoin` event gets called
```csharp
onMatchJoin?.Invoke();
```
7- The `GameManager`is suscribed to `onMatchJoin` and switches to the `3-Lobby` scene
```csharp
MultiplayerManager.Instance.onMatchJoin += JoinedMatch;
...
private void JoinedMatch()
{
    ResetPlayerWins();
    GoToLobby();
}
...
private void GoToLobby()
{
    SceneManager.LoadScene((int)Scenes.Lobby);
}
```

## Client game logic
### MultiplayerManager
The `MultiplayerManager` is the responsable for joining a match and sending and receiving messages from and to the match on the server. To find a specific match a RPC is sent to the server that helps creating or joining a match. The specific steps can be found on the `Joining a Lobby` section.
To receive and send messages we want to sepearate the different behaviours by using a code, you can find the codes I used for this game in `MultiplayerManager.OperationCode` let's take a look into each one:
| Code | Name | Logic |
| ------ | ------ | ------ |
| 0 | Players | Is a list with of the players including their display name, is sent by the server to the new player when a he joins the match |
| 1 | PlayerJoined | Is the display name of the new player, is sent to all other players by the server when a new player joins the match |
| 2 | PlayerInput | This is the direction that the player wants to move, is sent by the client to all other clients |
| 3 | PlayerWon | This is when a player reported that someone won the match, every client must report the same result to the server to trust the client |
| 4 | Draw | This is when a player reported that the game ended on a draw, every client must report the same result to the server to trust the client |
| 5 | ChangeScene | Sent by the server to all clients to report to which scene the client should change |

The `MultiplayerManager` is subscribed to a match state change just before joining the match like this:
```csharp
socket.ReceivedMatchState += Receive;
```
The idea is that the `MultiplayerManager` will receive all the messages and distribute them to the interested behaviours.
For a script to receive a message he must subscribe to the `MultiplayerManager` like this:
```csharp
MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.PlayerInput, ReceivedPlayerInput);
```
All the messages received with the code you subscribed will execute the funcion you passed.
To send a message you should send a object that can be serialized and the `MultiplayerManager` handles the serialization
```csharp
private void SendData(int tick, Direction direction)
{
   MultiplayerManager.Instance.Send(MultiplayerManager.Code.PlayerInput, new InputData(tick, (int)direction));
}
```
To `receive` a message you should deserialize to the class you want, the `MultiplayerMessage` can do it like this
```csharp
private void ReceivedPlayerInput(MultiplayerMessage message)
{
   InputData inputData = message.GetData<InputData>();
   SetPlayerInput(GetPlayerNumber(message.SessionId), inputData.Tick, (Direction)inputData.Direction);
}
```

### Multiplayer Identity
Is a script that holds the unique id of a object or player, in this case is the `SessionId` of each player, if we want to have objects in the scene it could be an auto-incemented number.

### Spawning players
The spawn of the players is made by the `Map` class on initialization, it takes each player and put each one on a spawn point
```csharp
public void InstantiateNinja(int playerNumber, SpawnPoint spawnPoint, string sessionId)
{
    Ninja ninja = Instantiate(ninjaPrefab, transform);
    ninja.Initialize(spawnPoint, playerNumber, this, sessionId);
    Ninjas.Add(ninja);
    if (MultiplayerManager.Instance.Self.SessionId == sessionId)
        gameCamera.SetStartingPosition(spawnPoint.Coordinates);
}
```

### Rollback Var
Have in mind that this is the most basic implementation of a rollback, the class `RollbackVar<T>` handles the save of information on a timeline, how it works is that each position and input is saved on a dictionary of an int and a T, being T any type you want, the int part represents each tick of the gameloop.
```csharp
private Dictionary<int, T> history = new Dictionary<int, T>();
```

## Server game logic
### RPC
The RPC (Remote procedure call) helps send information from a client to the server when its outside a match, the registering of the RPC must be done inside of the `InitModule`, int his exaple is on the `main` TypeScript file and the RPC is located under `rpcs` file.
```typescript
function InitModule(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer)
{
    initializer.registerRpc("JoinOrCreateMatchRpc", joinOrCreateMatch);
}

let joinOrCreateMatch: nkruntime.RpcFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, payload: string): string
{
    return "response";
}
```

On unity you can call the RPC using the client you used to create a socket:
```csharp
return await client.RpcAsync(session, rpc, payload);
```

### Match Handler
The match handler manages an match, from the lobby to the end results.
Like the RPCs the registering of the match must be done inside of the `InitModule`, you should assign a name in this case is just called "match", the functions of each state can be found on `match_handler`.
```typescript
function InitModule(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer)
{
    initializer.registerMatch("match", {
        matchInit,
        matchJoinAttempt,
        matchJoin,
        matchLeave,
        matchLoop,
        matchTerminate
    });
}
```
    
The match is created on the RPC `joinOrCreateMatch`, when creating the match a MatchId is returnes and it should be sent to the clients
```typescript
nakama.matchCreate("match");
```

The `match_handler` holds all the logic of a match, all the data specific of a match will be being passed to all the match functions on a match state object, this is a dictionary and you can cast it to a interface, in this case we use a interface called "GameState"
```typescript
interface GameState
{
    players: Player[]
    playersWins: number[]
    roundDeclaredWins: number[][]
    roundDeclaredDraw: number[]
    scene: Scene
    countdown: number
    endMatch: boolean
}
```
You can find the initialization of this state on the matchInit function
```typescript
let matchInit: nkruntime.MatchInitFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, params: { [key: string]: string })
{
    var label: MatchLabel = { open: true }
    var gameState: GameState =
    {
        players: [],
        playersWins: [],
        roundDeclaredWins: [[]],
        roundDeclaredDraw: [],
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
```
### Match Loop
The matchLoop is the function that gets called each tick, on each of these ticks all the client's messages sent on that period of time are received as a list, with this list you can decide what to do with the messages, to send them back to the clients or execute a custom logic.

### Scenes
On this example the server behavior are separated on different logical Scenes, each scene have a different behaviour, so each tick only runs the logic of that scene.
The flow of a match goes like this:
Lobby -> Battle -> RoundResult -> if no player has obtained 3 victories go back to Battle, if a player has already obtained 3 victories end the match.

### Custom and default messages logic
Each tick all the messages received by the clients are received by the `matchLoop` then they are processed on the `processMessages` function, if a message code has a custom logic a special fuction is called or if there is no custom logic a default logic will be eecuted, in this example the custom logics are registered on the `consts` file and the default logic just send the message to all the clients.
```typescript
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
    dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
}
```

### Time counters
On the scenes `Lobby` and `RoundResults` the server runs countdown to wait for the next scene, you can see how it works on the `Lobby` scene behaviour
```typescript
function matchLoopLobby(gameState: GameState, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher): void
{
    if (gameState.countdown > 0 && getPlayersCount(gameState.players) > 1)
    {
        gameState.countdown--;
        if (gameState.countdown == 0)
        {
            gameState.scene = Scene.Battle;
            dispatcher.broadcastMessage(OperationCode.ChangeScene, JSON.stringify(gameState.scene));
            dispatcher.matchLabelUpdate(JSON.stringify({ open: false }));
        }
    }
}
```
The countdown is set on matchJoin when a player joins, the duration is on seconds and its multiplied to the tick rate of the server
```typescript
gameState.countdown = DurationLobby * TickRate;
```

### Determining battle winners
Since the game uses rollback there can be a erroneous message sent by a client telling who was the winner of the match, for example if there are two players remaining and a client sees a one of them hitting a wall he can determine wrongly that the player lose the battle, but is possible that the player turned just before hitting the wall, when a client determines a victory a message is sent to the server, if the server got messages of every player determining that on the same tick the same outcome then the server can trust the clients and determine who was the winner. This is done on the `playerWon` function.
```typescript
function playerWon(message: nkruntime.MatchMessage, gameState: GameState, dispatcher: nkruntime.MatchDispatcher): void 
{
    if (gameState.scene != Scene.Battle || gameState.countdown > 0)
        return;

    let data: PlayerWonData = JSON.parse(message.data);
    let tick: number = data.tick;
    let playerNumber: number = data.playerNumber;
    if (gameState.roundDeclaredWins[tick] == undefined)
        gameState.roundDeclaredWins[tick] = [];

    if (gameState.roundDeclaredWins[tick][playerNumber] == undefined)
        gameState.roundDeclaredWins[tick][playerNumber] = 0;

    gameState.roundDeclaredWins[tick][playerNumber]++;
    if (gameState.roundDeclaredWins[tick][playerNumber] < getPlayersCount(gameState.players))
        return;

    gameState.playersWins[playerNumber]++;
    gameState.countdown = DurationBattleEnding * TickRate;
    dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
}
```

### Ending a match
When a player has reached 3 victories or all the players has been disconected the match ends, this can be done by returning null on any of the match fuctions, this behaviour can be seen on the matchLoop.
```typescript
return gameState.endMatch ? null : { state: gameState };
```

## Incrementing user trophies
The winner of the game gets a trophy when the match ends, to do this a storage read of a user is done and the variable is incremented by the desired amount and then a storage write is done after that.
```typescript
let storageReadRequests: nkruntime.StorageReadRequest[] = [{
    collection: CollectionUser,
    key: KeyTrophies,
    userId: winner.presence.userId
}];

let result: nkruntime.StorageObject[] = nakama.storageRead(storageReadRequests);
var trophiesData: TrophiesData = { amount: 0 };
for (let storageObject of result)
{
    trophiesData = <TrophiesData>storageObject.value;
    break;
}

trophiesData.amount++;
let storageWriteRequests: nkruntime.StorageWriteRequest[] = [{
    collection: CollectionUser,
    key: KeyTrophies,
    userId: winner.presence.userId,
    value: trophiesData
}];

nakama.storageWrite(storageWriteRequests);
```

## Credits
Programming: [Alan Gaspar](https://www.linkedin.com/in/alangaspar/)

Art: [Monica Murillo](https://www.artstation.com/monicamurilloart)

Music: [FoxSynergy](https://opengameart.org/users/foxsynergy) (8-BitNinja) and [Spring Spring](https://opengameart.org/users/spring-spring) (Ninja Theme) (10 Fanfares)
