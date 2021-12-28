using Nakama.Helpers;
using UnityEngine;

namespace NinjaBattle.Game
{
    public class InputManager : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private int delay = 0;
        [SerializeField] private KeyCode keyUp = KeyCode.None;
        [SerializeField] private KeyCode keyLeft = KeyCode.None;
        [SerializeField] private KeyCode keyRight = KeyCode.None;
        [SerializeField] private KeyCode keyDown = KeyCode.None;

        #endregion

        #region BEHAVIORS

        private void Update()
        {
            BattleManager battleManager = BattleManager.Instance;
            if (Input.GetKeyDown(keyUp))
                SendData(battleManager.CurrentTick - delay, Direction.North);
            else if (Input.GetKeyDown(keyLeft))
                SendData(battleManager.CurrentTick - delay, Direction.West);
            else if (Input.GetKeyDown(keyRight))
                SendData(battleManager.CurrentTick - delay, Direction.East);
            else if (Input.GetKeyDown(keyDown))
                SendData(battleManager.CurrentTick - delay, Direction.South);
            else
                return;
        }

        private void SendData(int tick, Direction direction)
        {
            MultiplayerManager.Instance.Send(MultiplayerManager.Code.PlayerInput, new InputData(tick, (int)direction));
        }

        #endregion
    }
}
