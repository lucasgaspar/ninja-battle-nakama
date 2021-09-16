interface MatchLabel
{
    open: boolean
}

interface GameState
{
    players: Player[]
    scene: Scene
    countdown: number
    endMatch: boolean
}

interface Player
{
    presence: nkruntime.Presence
    wins: number
}

interface PlayerLeaveData
{
    sessionId: string
}

interface TimeRemainingData
{
    time: number
}
