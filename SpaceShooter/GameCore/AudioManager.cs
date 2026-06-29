using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.GameCore
{
    public enum SoundEffect
    {
        Shot,
        Explosion,
        Coin,
        PowerUp,
        PlayerHit
    }

    public static class AudioManager
    {
        private static readonly Dictionary<SoundEffect, SoundPlayer> Players = new();
        private static readonly List<MemoryStream> Streams = new();
        private static SoundPlayer? _musicPlayer;
        private static bool _initialized;

        public static bool MusicEnabled { get; private set; } = true;
        public static bool SfxEnabled { get; private set; } = true;

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            _musicPlayer = CreatePlayer("SpaceShooter.Resources.music_loop.wav");
            Players[SoundEffect.Shot] = CreatePlayer("SpaceShooter.Resources.sfx_shot.wav");
            Players[SoundEffect.Explosion] = CreatePlayer("SpaceShooter.Resources.sfx_explosion.wav");
            Players[SoundEffect.Coin] = CreatePlayer("SpaceShooter.Resources.sfx_coin.wav");
            Players[SoundEffect.PowerUp] = CreatePlayer("SpaceShooter.Resources.sfx_powerup.wav");
            Players[SoundEffect.PlayerHit] = CreatePlayer("SpaceShooter.Resources.sfx_hit.wav");
        }

        public static void SetMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;

            if (enabled)
                StartBackgroundMusic();
            else
                StopBackgroundMusic();
        }

        public static void SetSfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
        }

        public static void StartBackgroundMusic()
        {
            Initialize();

            if (!MusicEnabled || _musicPlayer is null)
                return;

            try
            {
                _musicPlayer.PlayLooping();
            }
            catch
            {
            }
        }

        public static void StopBackgroundMusic()
        {
            try
            {
                _musicPlayer?.Stop();
            }
            catch
            {
            }
        }

        public static void Play(SoundEffect effect)
        {
            Initialize();

            if (!SfxEnabled || !Players.TryGetValue(effect, out SoundPlayer? player))
                return;

            try
            {
                player.Play();
            }
            catch
            {
            }
        }

        public static void Shutdown()
        {
            StopBackgroundMusic();

            foreach (MemoryStream stream in Streams)
                stream.Dispose();

            Streams.Clear();
            Players.Clear();
            _musicPlayer = null;
            _initialized = false;
        }

        private static SoundPlayer CreatePlayer(string resourceName)
        {
            Assembly assembly = typeof(AudioManager).Assembly;
            using Stream? source = assembly.GetManifestResourceStream(resourceName);

            if (source is null)
                return new SoundPlayer();

            var buffer = new MemoryStream();
            source.CopyTo(buffer);
            buffer.Position = 0;
            Streams.Add(buffer);

            return new SoundPlayer(buffer);
        }
    }
}
