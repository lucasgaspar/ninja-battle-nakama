using System.Collections;
using UnityEngine;

namespace NinjaBattle.Game
{
    public class GameCamera : MonoBehaviour
    {
        #region FIELDS

        private const float WaitTime = 0.5f;
        private const float MoveDuration = 2.5f;
        private const float FinalOrtographicSize = 7.6f;

        #endregion

        #region BEHAVIORS

        public void SetStartingPosition(Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
            StartCoroutine(GoToCenter());
        }

        private IEnumerator GoToCenter()
        {
            yield return new WaitForSeconds(WaitTime);
            Camera camera = GetComponent<Camera>();
            float startingOrtographicSize = camera.orthographicSize;
            Vector3 staringPosition = transform.position;
            Vector3 desiredPosition = Vector3.zero;
            desiredPosition.z = transform.position.z;
            float elapsedTime = default(float);
            while (elapsedTime < MoveDuration)
            {
                elapsedTime += Time.deltaTime;
                float percentage = elapsedTime / MoveDuration;
                transform.position = Vector3.Lerp(staringPosition, desiredPosition, percentage);
                camera.orthographicSize = Mathf.Lerp(startingOrtographicSize, FinalOrtographicSize, percentage);
                yield return null;
            }

            transform.position = desiredPosition;
            camera.orthographicSize = FinalOrtographicSize;
        }

        #endregion
    }
}
