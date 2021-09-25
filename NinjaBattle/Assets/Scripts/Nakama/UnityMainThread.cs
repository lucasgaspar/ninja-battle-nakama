using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nakama.Helpers
{
    public class UnityMainThread : MonoBehaviour
    {
        #region FIELDS

        private Queue<Action> jobs = new Queue<Action>();
        private static UnityMainThread instance = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            while (jobs.Count > 0)
                jobs.Dequeue().Invoke();
        }

        public static void AddJob(Action newJob)
        {
            instance.jobs.Enqueue(newJob);
        }

        #endregion
    }
}
