using SpaceShooter.Core;
using SpaceShooter.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.GameCore
{
    public enum GameState
    {
        Playing,
        BetweenWaves,
        GameOver,
        Victory
    }
    public class GameEngine
    {
        public Player Player { get; private set; } = null!;
        public List<Enemy> Enemies { get; } = new();
        public List<Bullet> Bullets { get; } = new();
        public List<CoinDrop> Coins { get; } = new();
        public List<PowerUpDrop> PowerUps { get; } = new();
        public List<Explosion> Explosions { get; } = new();

        public GameState State { get; private set; } = GameState.Playing;
        public int Wave => _waveManager.CurrentWave;

        private WaveManager _waveManager = new();
        private readonly Random _rng = new();
        private int _betweenWaveTimer;
        private const int BetweenWaveDelay = 3000;

        public event Action<string>? OnMessage;
        public event Action? OnEnemyDestroyed;
        public event Action? OnCoinCollected;
        public event Action? OnPowerUpCollected;
        public event Action? OnPlayerDamaged;

        public GameEngine()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            Player = new Player(
                GameSettings.ScreenWidth / 2f - 20,
                GameSettings.ScreenHeight - 80);

            Enemies.Clear();
            Bullets.Clear();
            Coins.Clear();
            PowerUps.Clear();
            Explosions.Clear();

            State = GameState.Playing;
            _waveManager = new WaveManager();
            _waveManager.BuildWave(1);
        }

        private void ResolvePlayerBulletHit(int bulletIndex, Bullet bullet)
        {
            for (int enemyIndex = Enemies.Count - 1; enemyIndex >= 0; enemyIndex--)
            {
                Enemy enemy = Enemies[enemyIndex];
                if (!bullet.CollidesWith(enemy))
                    continue;

                enemy.TakeDamage(bullet.Damage);
                if (!enemy.IsAlive)
                    KillEnemy(enemyIndex);

                Bullets.RemoveAt(bulletIndex);
                return;
            }
        }

        private void ResolveEnemyBulletHit(int bulletIndex, Bullet bullet)
        {
            if (!bullet.CollidesWith(Player))
                return;

            if (!Player.HasShield)
            {
                Player.TakeDamage(bullet.Damage);
                OnPlayerDamaged?.Invoke();
            }

            Bullets.RemoveAt(bulletIndex);
        }

        private void UpdateEnemyCollisions()
        {
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                if (!Enemies[i].CollidesWith(Player))
                    continue;

                if (!Player.HasShield)
                {
                    Player.TakeDamage(30);
                    OnPlayerDamaged?.Invoke();
                }

                Explosions.Add(new Explosion(Enemies[i].X, Enemies[i].Y));
                Enemies.RemoveAt(i);
            }
        }

        private void UpdateCoins(float deltaTime)
        {
            for (int i = Coins.Count - 1; i >= 0; i--)
            {
                CoinDrop coin = Coins[i];
                coin.Update(deltaTime);

                if (coin.CollidesWith(Player))
                {
                    Player.Coins += coin.Value;
                    Coins.RemoveAt(i);
                    OnCoinCollected?.Invoke();
                }
                else if (coin.IsOffScreen())
                {
                    Coins.RemoveAt(i);
                }
            }
        }

        private void UpdatePowerUps(float deltaTime)
        {
            for (int i = PowerUps.Count - 1; i >= 0; i--)
            {
                PowerUpDrop powerUp = PowerUps[i];
                powerUp.Update(deltaTime);

                if (powerUp.CollidesWith(Player))
                {
                    Player.ApplyPowerUp(powerUp.Type);
                    PowerUps.RemoveAt(i);
                    OnPowerUpCollected?.Invoke();
                }
                else if (powerUp.IsOffScreen())
                {
                    PowerUps.RemoveAt(i);
                }
            }
        }

        private void CheckPlayerDeath()
        {
            if (Player.IsAlive)
                return;

            Player.Lives--;
            Explosions.Add(new Explosion(Player.X, Player.Y));

            if (Player.Lives <= 0)
            {
                State = GameState.GameOver;
                return;
            }

            Player.HP = GameSettings.PlayerStartHP;
            Player.X = GameSettings.ScreenWidth / 2f - 20;
            Player.Y = GameSettings.ScreenHeight - 80;
            Player.ResetMotion();
        }

        private void CheckWaveCompletion()
        {
            if (!_waveManager.WaveCleared(Enemies))
                return;

            if (Wave >= GameSettings.TotalWaves)
            {
                State = GameState.Victory;
                return;
            }

            State = GameState.BetweenWaves;
            _betweenWaveTimer = BetweenWaveDelay;
            OnMessage?.Invoke($"Wave {Wave} cleared! Next wave in 3s...");
        }

        private void KillEnemy(int index)
        {
            Enemy enemy = Enemies[index];
            Player.Score += enemy.ScoreValue;
            Explosions.Add(new Explosion(enemy.X, enemy.Y));
            OnEnemyDestroyed?.Invoke();

            DropReward(enemy);
            Enemies.RemoveAt(index);
        }

        private void DropReward(Enemy enemy)
        {
            float dropX = enemy.X + enemy.Width / 2f - 7;

            if (enemy is EnemyHeavyTank)
            {
                Coins.Add(new CoinDrop(dropX, enemy.Y, 5));
                return;
            }

            int powerUpChance = Math.Min(18, 6 + Wave);
            if (_rng.Next(100) < powerUpChance)
            {
                PowerUpType[] types = (PowerUpType[])Enum.GetValues(typeof(PowerUpType));
                PowerUpType type = types[_rng.Next(types.Length)];
                PowerUps.Add(new PowerUpDrop(dropX - 4, enemy.Y, type));
                return;
            }

            int coinChance = Math.Min(95, enemy.CoinDropChance + (Wave - 1) * 2);
            if (_rng.Next(100) >= coinChance)
                return;

            int goldChance = Math.Min(85, enemy.GoldCoinChance + (Wave - 1) * 3);
            int coinValue = _rng.Next(100) < goldChance ? 5 : 1;
            Coins.Add(new CoinDrop(dropX, enemy.Y, coinValue));
        }
    }
    public class Explosion
    {
        public float X;
        public float Y;
        public int Frame;

        private const int MaxFrames = 18;
        public bool Done => Frame >= MaxFrames;

        public Explosion(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Update(float deltaTime)
        {
            Frame += 2;
        }

        public void Draw(System.Drawing.Graphics g)
        {
            float progress = (float)Frame / MaxFrames;
            int alpha = (int)(255 * (1 - progress));
            int radius = (int)(30 * progress);
            var color = System.Drawing.Color.FromArgb(alpha, 255, (int)(160 * (1 - progress)), 0);

            using var brush = new System.Drawing.SolidBrush(color);
            g.FillEllipse(brush, X - radius, Y - radius, radius * 2 + 20, radius * 2 + 20);
        }
    }
}
