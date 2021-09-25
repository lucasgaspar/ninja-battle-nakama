interface MatchLabel
{
    open: boolean
}

interface GameState
{
    players: nkruntime.Presence[]
    playersWins: number[]
    scene: Scene
    countdown: number
    endMatch: boolean
}

interface TimeRemainingData
{
    time: number
}
