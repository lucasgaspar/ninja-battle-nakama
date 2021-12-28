using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaBattle.General
{
    public partial class AudioManager : MonoBehaviour
    {
        #region FIELDS

        private const float CooldownForSounds = 0.05f;
        public const float MusicVolume = 0.65f;
        public const float SoundVolume = 1f;

        private AudioSource musicChannel = null;
        private AudioSource musicChannelCrossFadeHelper = null;
        private AudioSource soundChannel = null;
        private Dictionary<int, AudioSource> persistentSoundsChannels = new Dictionary<int, AudioSource>();
        private List<AudioClip> currentSoundClips = new List<AudioClip>();
        private int persistentCounter = -1;

        #endregion

        #region PROPERTIES

        public static AudioManager Instance { get; private set; } = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
            musicChannel = gameObject.AddComponent<AudioSource>();
            musicChannelCrossFadeHelper = gameObject.AddComponent<AudioSource>();
            soundChannel = gameObject.AddComponent<AudioSource>();
            LoadVolume();
        }

        private void LoadVolume()
        {
            musicChannel.volume = MusicVolume;
            soundChannel.volume = SoundVolume;
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            StopMusic();
            musicChannel.clip = clip;
            musicChannel.loop = loop;
            musicChannel.Play();
        }

        public void StopMusic()
        {
            musicChannel.clip = null;
            musicChannel.Stop();
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip == null || currentSoundClips.Contains(clip))
                return;

            soundChannel.PlayOneShot(clip);
            currentSoundClips.Add(clip);

            StartCoroutine(SoundCooldown(clip, CooldownForSounds));
        }

        public IEnumerator SoundCooldown(AudioClip clip, float cooldown)
        {
            yield return new WaitForSecondsRealtime(cooldown);
            currentSoundClips.Remove(clip);
        }

        public int PlayPersistentSound(AudioClip clip, float duration = 0f)
        {
            AudioSource persistentSoundChannel = gameObject.AddComponent<AudioSource>();
            persistentSoundChannel.volume = soundChannel.volume;
            persistentSoundChannel.mute = soundChannel.mute;
            persistentSoundChannel.loop = true;
            persistentSoundChannel.clip = clip;
            persistentSoundChannel.Play();
            persistentCounter++;
            persistentSoundsChannels.Add(persistentCounter, persistentSoundChannel);

            if (duration > 0)
                StartCoroutine(StopPersistentSoundCoroutine(duration, persistentCounter));

            return persistentCounter;
        }

        public void StopPersistentSound(int persistentSoundId)
        {
            if (!PersistentSoundExists(persistentSoundId))
                return;

            persistentSoundsChannels[persistentSoundId].Stop();
            Destroy(persistentSoundsChannels[persistentSoundId]);
            persistentSoundsChannels.Remove(persistentSoundId);
        }

        public void StopAllPersistentSounds()
        {
            foreach (int id in persistentSoundsChannels.Keys.ToList())
                StopPersistentSound(id);

            persistentSoundsChannels.Clear();
        }

        private IEnumerator StopPersistentSoundCoroutine(float duration, int id)
        {
            yield return new WaitForSeconds(duration);
            StopPersistentSound(id);
        }

        private bool PersistentSoundExists(int persistentSoundId)
        {
            return persistentSoundsChannels.ContainsKey(persistentSoundId);
        }

        #endregion
    }
}
