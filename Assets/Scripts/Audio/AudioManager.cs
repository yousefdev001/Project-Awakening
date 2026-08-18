using System.Collections.Generic;
using UnityEngine;

namespace Awakening.Audio
{
    /// <summary>
    /// Master Audio Manager maintaining pooled SFX AudioSources and Master/SFX volume channels.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Volume Controls")]
        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 0.85f;
        [Range(0f, 1f)] [SerializeField] private float _bgmVolume = 0.6f;

        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = Mathf.Clamp01(value);
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Mathf.Clamp01(value);
        }

        private const int PoolSize = 10;
        private List<AudioSource> _sfxPool = new List<AudioSource>();
        private AudioSource _bgmSource;
        private int _poolIndex = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeSources();
        }

        private void InitializeSources()
        {
            // Create SFX Pool
            for (int i = 0; i < PoolSize; i++)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f; // 2D clean stereo
                _sfxPool.Add(src);
            }

            // Create BGM Channel
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.spatialBlend = 0f;
        }

        public void PlaySound(SoundType sound, float volumeScale = 1.0f, float pitchRandomness = 0.05f)
        {
            AudioClip clip = ProceduralAudioSynthesizer.GetOrCreateClip(sound);
            if (clip == null) return;

            AudioSource source = GetNextAvailableSource();
            if (source != null)
            {
                source.pitch = 1.0f + Random.Range(-pitchRandomness, pitchRandomness);
                source.volume = _masterVolume * _sfxVolume * volumeScale;
                source.PlayOneShot(clip);
            }
        }

        public void PlaySoundAtPosition(SoundType sound, Vector3 worldPosition, float volumeScale = 1.0f)
        {
            AudioClip clip = ProceduralAudioSynthesizer.GetOrCreateClip(sound);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, worldPosition, _masterVolume * _sfxVolume * volumeScale);
            }
        }

        private AudioSource GetNextAvailableSource()
        {
            if (_sfxPool == null || _sfxPool.Count == 0) return null;

            AudioSource src = _sfxPool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % _sfxPool.Count;
            return src;
        }
    }
}
