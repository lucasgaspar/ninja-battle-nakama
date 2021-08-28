using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private int playerNumber = 0;
    [SerializeField] private int delay = 0;
    [SerializeField] private KeyCode keyUp = KeyCode.None;
    [SerializeField] private KeyCode keyLeft = KeyCode.None;
    [SerializeField] private KeyCode keyRight = KeyCode.None;
    [SerializeField] private KeyCode keyDown = KeyCode.None;

    private void Update()
    {
        GameManager gameManager = GameManager.Instance;
        if (Input.GetKeyDown(keyUp))
            gameManager.SetNextPlayerInput(playerNumber, gameManager.CurrentTick - delay, Direction.North);
        else if (Input.GetKeyDown(keyLeft))
            gameManager.SetNextPlayerInput(playerNumber, gameManager.CurrentTick - delay, Direction.West);
        else if (Input.GetKeyDown(keyRight))
            gameManager.SetNextPlayerInput(playerNumber, gameManager.CurrentTick - delay, Direction.East);
        else if (Input.GetKeyDown(keyDown))
            gameManager.SetNextPlayerInput(playerNumber, gameManager.CurrentTick - delay, Direction.South);
    }
}
