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
The easiest way to start with Nakama is with docker, the Nakama documentation offers a nice [guide](httpshttps://heroiclabs.com/docs/nakama/getting-started/docker-quickstart/) on how to do this, the server code on this repository is on TypeScript so it requires installing the TypeScript support too, [here's](https://heroiclabs.com/docs/nakama/server-framework/typescript-setup/) the official documentation on how guide to do this.

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
`NakamaManager` is the base script of all the Nakama Helpers, on this script you can Login and Logout and send RPC to the server. The calls of the server are asyncronous so there's another singleton to return to the Unity main thread called `UnityMainThread` in the scene `0-Initializer` you can find a prefab called `Managers`that have both the NakamaManager and MainThread.

## Unity Main Thread
This is the singleton that gets all the asyncronous requests from Nakama and return them to the main thread. The code is reallhy simple.
It saves the Actions on a Queue
```csharp
public static void AddJob(Action newJob)
{
   instance.jobs.Enqueue(newJob);
}
```
And execute them all in order on the next update
```csharp
private void Update()
{
    while (jobs.Count > 0)
        jobs.Dequeue().Invoke();
}
```

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
6- The `onMatchJoin` event gets called trough the `UnityMainThread`
```csharp
UnityMainThread.AddJob(() => onMatchJoin?.Invoke());
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
IN PROGRESS: Subscribe and suscribe to messsages.

### Lobby
IN PROGRESS: Wait for other players and time counter

### MultiplayerIdentity
IN PROGRESS: How each player have their own unique id

### Spawning players
IN PROGRESS: How the game spawns the players

### RollbackVar
IN PROGRESS: How it works

## Server game logic
### Time counters
IN PROGRESS: How the time is counted on each scene

### Determining winners
IN PROGRESS: An special logic should be handled in the server to know who really won the battle

### Wins
IN PROGRESS: Save how many rounds each player has won to know who is the final winner.

## Saving/Loading user information
IN PROGRESS: After winning a game the trophies are increased on the server and on the scene home you can see the current count of thropies

## Credits
Programming: [Alan Gaspar](https://www.linkedin.com/in/alangaspar/)

Art: [Monica Murillo](https://www.artstation.com/monicamurilloart)

Music: [FoxSynergy](https://opengameart.org/users/foxsynergy) (8-BitNinja) and [Spring Spring](https://opengameart.org/users/spring-spring) (Ninja Theme) (10 Fanfares)
