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
        private List<AudioClip> currentSoundClips = new List<AudioClip>();
        private List<AudioClip> playingSoundClips = new List<AudioClip>();

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
            StartCoroutine(Dispose(clip, clip.length));
        }

        public IEnumerator SoundCooldown(AudioClip clip, float cooldown)
        {
            yield return new WaitForSecondsRealtime(cooldown);
            currentSoundClips.Remove(clip);
        }

        public IEnumerator Dispose(AudioClip clip, float cooldown)
        {
            yield return new WaitForSecondsRealtime(cooldown);
            playingSoundClips.Remove(clip);
        }

        #endregion
    }
}
