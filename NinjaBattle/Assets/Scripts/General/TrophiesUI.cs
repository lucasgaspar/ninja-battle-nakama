using Nakama.Helpers;
using TMPro;
using UnityEngine;

namespace NinjaBattle.General
{
    public class TrophiesUI : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private NakamaCollectionObject nakamaCollectionObject = null;
        [SerializeField] private TMP_Text text = null;

        #endregion

        #region BEHAVIORS

        public void Start()
        {
            TrophiesData trophiesData = nakamaCollectionObject.GetValue<TrophiesData>();
            if (trophiesData == null)
                trophiesData = new TrophiesData();

            text.text = trophiesData.Amount.ToString();
        }

        #endregion
    }
}
