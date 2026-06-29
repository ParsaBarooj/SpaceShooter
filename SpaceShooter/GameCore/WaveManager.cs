using SpaceShooter.Entities;
using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;

namespace SpaceShooter.Core
{
    public class WaveManager
    {
        public int CurrentWave { get; private set; } = 1;

        private readonly Queue<Enemy> _spawnQueue = new();
        private readonly Random _rng = new();
        private int _spawnTimer;
        private const int SpawnInterval = 900;

        public bool WaveCleared(List<Enemy> activeEnemies) =>
            _spawnQueue.Count == 0 && activeEnemies.Count == 0;

        public void BuildWave(int wave)
        {
            CurrentWave = wave;
            _spawnQueue.Clear();
            _spawnTimer = 0;

            var enemies = new List<Enemy>();
            int standardCount = 3 + wave;
            int scoutCount = 1 + wave / 2;
            int shooterCount = wave >= 2 ? 1 + wave / 3 : 0;
            int terroristCount = wave >= 4 ? wave / 4 : 0;

            int tankCount = wave == GameSettings.TotalWaves ? 2 : 0;

            for (int i = 0; i < standardCount; i++)
                enemies.Add(new EnemyStandard(RandomX(34), -40 - i * 20, wave));

            for (int i = 0; i < scoutCount; i++)
                enemies.Add(new EnemyScout(RandomX(30), -60 - i * 25, wave));

            for (int i = 0; i < shooterCount; i++)
                enemies.Add(new EnemyShooter(RandomX(36), -80 - i * 30, wave));

            for (int i = 0; i < terroristCount; i++)
                enemies.Add(new EnemyTerrorist(RandomX(32), -100 - i * 25, wave));

            for (int i = 0; i < tankCount; i++)
                enemies.Add(new EnemyHeavyTank(RandomX(54), -120 - i * 60, wave));

            Shuffle(enemies);
            foreach (Enemy enemy in enemies)
                _spawnQueue.Enqueue(enemy);
        }

        public List<Enemy> Update(int deltaMs)
        {
            var spawned = new List<Enemy>();
            _spawnTimer -= deltaMs;

            if (_spawnTimer <= 0 && _spawnQueue.Count > 0)
            {
                _spawnTimer = SpawnInterval;
                spawned.Add(_spawnQueue.Dequeue());
            }

            return spawned;
        }

        private float RandomX(int width)
        {
            return _rng.Next(10, GameSettings.ScreenWidth - width - 10);
        }

        private void Shuffle(List<Enemy> enemies)
        {
            for (int i = enemies.Count - 1; i > 0; i--)
            {
                int randomIndex = _rng.Next(i + 1);
                (enemies[i], enemies[randomIndex]) = (enemies[randomIndex], enemies[i]);
            }
        }
    }
}
