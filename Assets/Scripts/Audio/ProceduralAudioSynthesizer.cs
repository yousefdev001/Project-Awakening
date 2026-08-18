using System;
using System.Collections.Generic;
using UnityEngine;

namespace Awakening.Audio
{
    /// <summary>
    /// Programmatically synthesizes rich procedural PCM AudioClips in memory.
    /// Provides zero-dependency, instantaneous, high-fidelity sound effects for all game actions.
    /// </summary>
    public static class ProceduralAudioSynthesizer
    {
        private const int SampleRate = 44100;
        private static readonly Dictionary<SoundType, AudioClip> _clipCache = new Dictionary<SoundType, AudioClip>();

        public static AudioClip GetOrCreateClip(SoundType sound)
        {
            if (_clipCache.TryGetValue(sound, out AudioClip cached) && cached != null)
            {
                return cached;
            }

            AudioClip clip = GenerateClipForSound(sound);
            _clipCache[sound] = clip;
            return clip;
        }

        private static AudioClip GenerateClipForSound(SoundType sound)
        {
            switch (sound)
            {
                case SoundType.AttackSlash:
                    return GenerateSlashClip();

                case SoundType.HeavyCleave:
                    return GenerateHeavyCleaveClip();

                case SoundType.DodgeWhoosh:
                    return GenerateDodgeClip();

                case SoundType.SkillCast:
                    return GenerateSkillClip();

                case SoundType.PlayerHurt:
                    return GenerateHurtClip();

                case SoundType.GroundSlam:
                    return GenerateGroundSlamClip();

                case SoundType.MonsterHurt:
                    return GenerateMonsterHurtClip();

                case SoundType.MonsterDeath:
                    return GenerateMonsterDeathClip();

                case SoundType.BossRoar:
                    return GenerateBossRoarClip();

                case SoundType.BossEnrage:
                    return GenerateBossEnrageClip();

                case SoundType.BossDeath:
                    return GenerateBossDeathClip();

                case SoundType.GoldChink:
                    return GenerateGoldChinkClip();

                case SoundType.PotionDrink:
                    return GeneratePotionClip();

                case SoundType.ChestOpen:
                    return GenerateChestClip();

                case SoundType.ItemEquip:
                    return GenerateEquipClip();

                case SoundType.LevelUpFanfare:
                    return GenerateLevelUpClip();

                case SoundType.QuestComplete:
                    return GenerateQuestCompleteClip();

                case SoundType.VictoryFanfare:
                    return GenerateVictoryFanfareClip();

                default:
                    return GenerateGenericBeep(440f, 0.1f);
            }
        }

        #region Procedural Waveform Generators

        private static AudioClip GenerateSlashClip()
        {
            float duration = 0.18f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 2.5f);
                float noise = (UnityEngine.Random.value * 2f - 1f) * 0.7f;
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(800f, 200f, t) * ((float)i / SampleRate)) * 0.3f;
                samples[i] = (noise + tone) * envelope * 0.8f;
            }

            return CreateAudioClip("SFX_AttackSlash", samples);
        }

        private static AudioClip GenerateHeavyCleaveClip()
        {
            float duration = 0.32f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 1.8f);
                float lowImpact = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(160f, 50f, t) * ((float)i / SampleRate)) * 0.7f;
                float noise = (UnityEngine.Random.value * 2f - 1f) * 0.4f * envelope;
                samples[i] = (lowImpact + noise) * envelope;
            }

            return CreateAudioClip("SFX_HeavyCleave", samples);
        }

        private static AudioClip GenerateDodgeClip()
        {
            float duration = 0.22f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Sin(t * Mathf.PI);
                float noise = (UnityEngine.Random.value * 2f - 1f);
                samples[i] = noise * envelope * 0.45f;
            }

            return CreateAudioClip("SFX_DodgeWhoosh", samples);
        }

        private static AudioClip GenerateSkillClip()
        {
            float duration = 0.38f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Sin(t * Mathf.PI);
                float f1 = Mathf.Sin(2f * Mathf.PI * 587.33f * ((float)i / SampleRate)); // D5
                float f2 = Mathf.Sin(2f * Mathf.PI * 880.00f * ((float)i / SampleRate)); // A5
                float f3 = Mathf.Sin(2f * Mathf.PI * 1174.66f * ((float)i / SampleRate)); // D6
                samples[i] = (f1 * 0.4f + f2 * 0.35f + f3 * 0.25f) * envelope * 0.7f;
            }

            return CreateAudioClip("SFX_SkillCast", samples);
        }

        private static AudioClip GenerateHurtClip()
        {
            float duration = 0.2f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 2f);
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(220f, 90f, t) * ((float)i / SampleRate));
                samples[i] = tone * envelope * 0.6f;
            }

            return CreateAudioClip("SFX_PlayerHurt", samples);
        }

        private static AudioClip GenerateGroundSlamClip()
        {
            float duration = 0.55f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 1.5f);
                float bass = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(90f, 35f, t) * ((float)i / SampleRate));
                float noise = (UnityEngine.Random.value * 2f - 1f) * 0.6f;
                samples[i] = (bass * 0.7f + noise * 0.3f) * envelope;
            }

            return CreateAudioClip("SFX_GroundSlam", samples);
        }

        private static AudioClip GenerateMonsterHurtClip()
        {
            return GenerateGenericBeep(260f, 0.12f);
        }

        private static AudioClip GenerateMonsterDeathClip()
        {
            float duration = 0.4f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 1.6f);
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(300f, 60f, t) * ((float)i / SampleRate));
                samples[i] = tone * envelope * 0.65f;
            }

            return CreateAudioClip("SFX_MonsterDeath", samples);
        }

        private static AudioClip GenerateBossRoarClip()
        {
            float duration = 0.85f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Sin(t * Mathf.PI);
                float growl = Mathf.Sin(2f * Mathf.PI * (110f + Mathf.Sin(t * 30f) * 25f) * ((float)i / SampleRate));
                float noise = (UnityEngine.Random.value * 2f - 1f) * 0.35f;
                samples[i] = (growl + noise) * envelope * 0.9f;
            }

            return CreateAudioClip("SFX_BossRoar", samples);
        }

        private static AudioClip GenerateBossEnrageClip()
        {
            float duration = 0.9f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Sin(t * Mathf.PI);
                float rise = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(140f, 520f, t) * ((float)i / SampleRate));
                samples[i] = rise * envelope * 0.85f;
            }

            return CreateAudioClip("SFX_BossEnrage", samples);
        }

        private static AudioClip GenerateBossDeathClip()
        {
            return GenerateGroundSlamClip();
        }

        private static AudioClip GenerateGoldChinkClip()
        {
            float duration = 0.2f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 3.0f);
                float ping1 = Mathf.Sin(2f * Mathf.PI * 1760.0f * ((float)i / SampleRate)); // A6
                float ping2 = Mathf.Sin(2f * Mathf.PI * 2637.0f * ((float)i / SampleRate)); // E7
                samples[i] = (ping1 * 0.6f + ping2 * 0.4f) * envelope * 0.6f;
            }

            return CreateAudioClip("SFX_GoldChink", samples);
        }

        private static AudioClip GeneratePotionClip()
        {
            float duration = 0.28f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Sin(t * Mathf.PI);
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(440f, 660f, t) * ((float)i / SampleRate));
                samples[i] = tone * envelope * 0.5f;
            }

            return CreateAudioClip("SFX_PotionDrink", samples);
        }

        private static AudioClip GenerateChestClip()
        {
            float duration = 0.45f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 1.8f);
                float creak = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(280f, 440f, t) * ((float)i / SampleRate)) * 0.5f;
                float chime = Mathf.Sin(2f * Mathf.PI * 1320f * ((float)i / SampleRate)) * 0.5f;
                samples[i] = (creak + chime) * envelope * 0.7f;
            }

            return CreateAudioClip("SFX_ChestOpen", samples);
        }

        private static AudioClip GenerateEquipClip()
        {
            return GenerateGenericBeep(520f, 0.12f);
        }

        private static AudioClip GenerateLevelUpClip()
        {
            // Triumphant 4-note Arpeggio: C5 -> E5 -> G5 -> C6
            float duration = 0.85f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            float[] frequencies = new float[] { 523.25f, 659.25f, 783.99f, 1046.50f };
            int noteLength = numSamples / frequencies.Length;

            for (int i = 0; i < numSamples; i++)
            {
                int noteIndex = Mathf.Min(i / noteLength, frequencies.Length - 1);
                float noteT = (float)(i % noteLength) / noteLength;
                float envelope = Mathf.Pow(1f - noteT, 1.8f);
                float tone = Mathf.Sin(2f * Mathf.PI * frequencies[noteIndex] * ((float)i / SampleRate));
                samples[i] = tone * envelope * 0.75f;
            }

            return CreateAudioClip("SFX_LevelUpFanfare", samples);
        }

        private static AudioClip GenerateQuestCompleteClip()
        {
            // Positive 3-note chime: F5 -> A5 -> C6
            float duration = 0.6f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            float[] freqs = new float[] { 698.46f, 880.00f, 1046.50f };
            int noteLength = numSamples / freqs.Length;

            for (int i = 0; i < numSamples; i++)
            {
                int noteIndex = Mathf.Min(i / noteLength, freqs.Length - 1);
                float noteT = (float)(i % noteLength) / noteLength;
                float envelope = Mathf.Pow(1f - noteT, 2.0f);
                float tone = Mathf.Sin(2f * Mathf.PI * freqs[noteIndex] * ((float)i / SampleRate));
                samples[i] = tone * envelope * 0.7f;
            }

            return CreateAudioClip("SFX_QuestComplete", samples);
        }

        private static AudioClip GenerateVictoryFanfareClip()
        {
            // Epic Victory Brass Chord sequence
            float duration = 1.3f;
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 1.3f);
                float c = Mathf.Sin(2f * Mathf.PI * 523.25f * ((float)i / SampleRate));
                float e = Mathf.Sin(2f * Mathf.PI * 659.25f * ((float)i / SampleRate));
                float g = Mathf.Sin(2f * Mathf.PI * 783.99f * ((float)i / SampleRate));
                float c2 = Mathf.Sin(2f * Mathf.PI * 1046.50f * ((float)i / SampleRate));
                samples[i] = (c * 0.3f + e * 0.25f + g * 0.25f + c2 * 0.2f) * envelope * 0.85f;
            }

            return CreateAudioClip("SFX_VictoryFanfare", samples);
        }

        private static AudioClip GenerateGenericBeep(float freq, float duration)
        {
            int numSamples = (int)(SampleRate * duration);
            float[] samples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                float t = (float)i / numSamples;
                float envelope = Mathf.Pow(1f - t, 2.0f);
                samples[i] = Mathf.Sin(2f * Mathf.PI * freq * ((float)i / SampleRate)) * envelope * 0.5f;
            }

            return CreateAudioClip($"SFX_Beep_{freq}", samples);
        }

        private static AudioClip CreateAudioClip(string name, float[] samples)
        {
            AudioClip clip = AudioClip.Create(name, samples.Length, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        #endregion
    }
}
