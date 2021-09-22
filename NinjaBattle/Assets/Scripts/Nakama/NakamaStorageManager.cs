using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaStorageManager : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private List<NakamaCollectionObject> autoLoadObjects;

        #endregion

        #region EVENTS

        public event Action onLoadedData = null;

        #endregion

        #region PROPERTIES

        public static NakamaStorageManager Instance { get; private set; } = null;
        public bool LoadingFinished { get; private set; } = false;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            NakamaManager.Instance.onLoginSuccess += AutoLoad;
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onLoginSuccess -= AutoLoad;
        }

        private void AutoLoad()
        {
            UpdateCollectionObjectsAsync(autoLoadObjects);
        }

        public void UpdateCollectionObject(NakamaCollectionObject collectionObject)
        {
            UpdateCollectionObjectsAsync(new List<NakamaCollectionObject>() { collectionObject });
        }

        public void UpdateCollectionObjects(IEnumerable<NakamaCollectionObject> collectionObjects)
        {
            UpdateCollectionObjectsAsync(collectionObjects);
        }

        private async void UpdateCollectionObjectsAsync(IEnumerable<NakamaCollectionObject> collectionObjects)
        {
            if (collectionObjects.Count() == default(int))
            {
                onLoadedData?.Invoke();
                return;
            }

            List<IApiReadStorageObjectId> storageObjectIds = new List<IApiReadStorageObjectId>();
            foreach (NakamaCollectionObject collectionObject in collectionObjects)
            {
                collectionObject.ResetData();
                StorageObjectId storageObjectId = new StorageObjectId
                {
                    Collection = collectionObject.Collection,
                    Key = collectionObject.Key,
                    UserId = NakamaManager.Instance.Session.UserId
                };

                storageObjectIds.Add(storageObjectId);
            }

            var result = await NakamaManager.Instance.Client.ReadStorageObjectsAsync(NakamaManager.Instance.Session, storageObjectIds.ToArray<IApiReadStorageObjectId>());
            foreach (IApiStorageObject storageObject in result.Objects)
            {
                foreach (NakamaCollectionObject collectionObject in collectionObjects)
                {
                    if (storageObject.Key != collectionObject.Key)
                        continue;

                    collectionObject.SetDatabaseValue(storageObject.Value, storageObject.Version);
                }
            }

            LoadingFinished = true;
            onLoadedData?.Invoke();
        }

        public async void SendValueToServer(NakamaCollectionObject collectionObject, object newValue)
        {
            WriteStorageObject writeStorageObject = new WriteStorageObject
            {
                Collection = collectionObject.Collection,
                Key = collectionObject.Key,
                Value = newValue.Serialize(),
                Version = collectionObject.Version
            };

            var objectIds = await NakamaManager.Instance.Client.WriteStorageObjectsAsync(NakamaManager.Instance.Session, new[] { writeStorageObject });
            foreach (IApiStorageObjectAck storageObject in objectIds.Acks)
            {
                if (storageObject.Key != collectionObject.Key)
                    continue;

                collectionObject.SetDatabaseValue(newValue.Serialize(), storageObject.Version);
            }
        }

        #endregion
    }
}
