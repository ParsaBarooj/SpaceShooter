using SpaceShooter.Data;
using SpaceShooter.Entities;
using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceShooter.GameUI
{
    public class FormGame : Form
    {
        private GameEngine _engine = null!;
        private System.Windows.Forms.Timer? _gameTimer;
        private readonly PlayerData _playerData;
        private DateTime _lastTick;

        private bool _shooting;
        private bool _paused;
        private bool _escapeKeyHeld;
        private string _backgroundTheme = "default";

        private string _hudMessage = "";
        private int _hudMessageTimer;

        private readonly HashSet<Keys> _heldKeys = new();

        private readonly int[] _starX = new int[80];
        private readonly int[] _starY = new int[80];
        private readonly Random _rng = new();

        public FormGame(PlayerData data)
        {
            _playerData = data;

            Text = "Space Shooter — Playing";
            ClientSize = new Size(GameSettings.ScreenWidth, GameSettings.ScreenHeight);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(5, 5, 18);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            KeyPreview = true;

            InitStars();
            StartNewGame();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Focus();
        }

        private void StartNewGame()
        {
            if (_gameTimer is not null)
            {
                _gameTimer.Stop();
                _gameTimer.Tick -= GameTick;
                _gameTimer.Dispose();
            }

            _heldKeys.Clear();
            _shooting = false;
            _paused = false;
            _escapeKeyHeld = false;

            _engine = new GameEngine();
            ApplyCosmetics();
            SubscribeToEngineEvents();

            bool usedLifePack = ConsumeExtraLifePackForRun();
            ShowHudMessage(usedLifePack
                ? "Extra Life Pack activated! Wave 1 — Good luck!"
                : "Wave 1 — Good luck!");

            _gameTimer = new System.Windows.Forms.Timer { Interval = GameSettings.TimerIntervalMs };
            _gameTimer.Tick += GameTick;
            _lastTick = DateTime.Now;
            _gameTimer.Start();

            AudioManager.StartBackgroundMusic();
        }

        private void ApplyCosmetics()
        {
            _backgroundTheme = _playerData.EquippedBg;
            _engine.Player.ApplySkin(_playerData.EquippedSkin);
            _engine.Player.ApplyBulletStyle(_playerData.EquippedBullet);
        }

        private void SubscribeToEngineEvents()
        {
            _engine.OnMessage += ShowHudMessage;
            _engine.OnEnemyDestroyed += () => AudioManager.Play(SoundEffect.Explosion);
            _engine.OnCoinCollected += () => AudioManager.Play(SoundEffect.Coin);
            _engine.OnPowerUpCollected += () => AudioManager.Play(SoundEffect.PowerUp);
            _engine.OnPlayerDamaged += () => AudioManager.Play(SoundEffect.PlayerHit);
        }

        private bool ConsumeExtraLifePackForRun()
        {
            if (_playerData.ExtraLifePacks <= 0)
                return false;

            _playerData.ExtraLifePacks--;
            _engine.Player.Lives++;
            Database.Save(_playerData);
            AudioManager.Play(SoundEffect.PowerUp);
            return true;
        }

        private void GameTick(object? sender, EventArgs e)
        {
            if (_paused)
            {
                _lastTick = DateTime.Now;
                return;
            }

            DateTime now = DateTime.Now;
            float deltaTime = (float)(now - _lastTick).TotalSeconds;
            if (deltaTime > 0.1f)
                deltaTime = 0.1f;
            _lastTick = now;

            Player player = _engine.Player;
            player.MovingLeft = _heldKeys.Contains(Keys.Left) || _heldKeys.Contains(Keys.A);
            player.MovingRight = _heldKeys.Contains(Keys.Right) || _heldKeys.Contains(Keys.D);
            player.MovingUp = _heldKeys.Contains(Keys.Up) || _heldKeys.Contains(Keys.W);
            player.MovingDown = _heldKeys.Contains(Keys.Down) || _heldKeys.Contains(Keys.S);

            if (_shooting)
            {
                List<Bullet> bullets = player.TryShoot();
                if (bullets.Count > 0)
                {
                    _engine.Bullets.AddRange(bullets);
                    AudioManager.Play(SoundEffect.Shot);
                }
            }

            _engine.Update(deltaTime);

            if (_hudMessageTimer > 0)
                _hudMessageTimer -= GameSettings.TimerIntervalMs;

            if (_engine.State is GameState.GameOver or GameState.Victory)
            {
                _gameTimer?.Stop();
                SaveProgress();
                ShowResult();
                return;
            }

            Invalidate();
        }

        private void SaveProgress()
        {
            _playerData.TotalCoins += _engine.Player.Coins;
            if (_engine.Player.Score > _playerData.HighScore)
                _playerData.HighScore = _engine.Player.Score;

            Database.Save(_playerData);
        }

        private void ShowResult()
        {
            string title = _engine.State == GameState.Victory ? "You Win!" : "Game Over";
            string message = $"{title}\n\nScore: {_engine.Player.Score}\nCoins collected: {_engine.Player.Coins}\n\nPlay again?";
            DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
                StartNewGame();
            else
                Close();
        }

        private void ShowHudMessage(string message)
        {
            _hudMessage = message;
            _hudMessageTimer = 2500;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _heldKeys.Add(e.KeyCode);

            if (e.KeyCode == Keys.Space)
            {
                _shooting = true;
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Escape && !_escapeKeyHeld)
            {
                _escapeKeyHeld = true;
                TogglePause();
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            _heldKeys.Remove(e.KeyCode);

            if (e.KeyCode == Keys.Space)
                _shooting = false;
            if (e.KeyCode == Keys.Escape)
                _escapeKeyHeld = false;
        }

        private void TogglePause()
        {
            _paused = !_paused;
            Text = _paused ? "Space Shooter — PAUSED" : "Space Shooter — Playing";
            _lastTick = DateTime.Now;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DrawBackground(graphics);

            if (_engine is null)
                return;

            foreach (Bullet bullet in _engine.Bullets)
                bullet.Draw(graphics);
            foreach (CoinDrop coin in _engine.Coins)
                coin.Draw(graphics);
            foreach (PowerUpDrop powerUp in _engine.PowerUps)
                powerUp.Draw(graphics);
            foreach (Enemy enemy in _engine.Enemies)
                enemy.Draw(graphics);
            foreach (Explosion explosion in _engine.Explosions)
                explosion.Draw(graphics);

            _engine.Player.Draw(graphics);
            DrawHud(graphics);

            if (_paused)
                DrawCenteredText(graphics, "— PAUSED —\nPress Esc to resume", Color.White, 18);

            if (_hudMessageTimer > 0 && !string.IsNullOrWhiteSpace(_hudMessage))
                DrawCenteredText(graphics, _hudMessage, Color.Yellow, 16);
        }

        private void DrawHud(Graphics graphics)
        {
            Player player = _engine.Player;
            using var hudFont = new Font("Courier New", 11, FontStyle.Bold);
            using var smallFont = new Font("Courier New", 9);
            using var topBar = new SolidBrush(Color.FromArgb(120, 0, 0, 0));

            graphics.FillRectangle(topBar, 0, 0, GameSettings.ScreenWidth, 50);
            graphics.DrawString($"Score: {player.Score}", hudFont, Brushes.White, 8, 6);
            graphics.DrawString($"Coins: {player.Coins}", hudFont, Brushes.Gold, 8, 26);
            graphics.DrawString($"Wave: {_engine.Wave}/{GameSettings.TotalWaves}", hudFont, Brushes.Cyan,
                GameSettings.ScreenWidth / 2 - 60, 14);

            int barWidth = 150;
            int barHeight = 14;
            int barX = GameSettings.ScreenWidth - barWidth - 10;
            int barY = 10;
            float hpRatio = (float)player.HP / player.MaxHP;
            Color hpColor = hpRatio > 0.5f ? Color.LimeGreen : hpRatio > 0.25f ? Color.Orange : Color.Red;

            graphics.FillRectangle(Brushes.DimGray, barX, barY, barWidth, barHeight);
            using var hpBrush = new SolidBrush(hpColor);
            graphics.FillRectangle(hpBrush, barX, barY, (int)(barWidth * hpRatio), barHeight);
            graphics.DrawString("HP", smallFont, Brushes.White, barX - 22, barY);

            for (int i = 0; i < player.Lives; i++)
            {
                using var lifeBrush = new SolidBrush(Color.Red);
                graphics.FillEllipse(lifeBrush, barX + i * 18, barY + 18, 12, 12);
            }

            int powerUpY = 55;
            if (player.HasShield)
            {
                graphics.DrawString($"[SHIELD {player.ShieldTimeLeft / 1000f:0.0}s]", smallFont, Brushes.Cyan, 8, powerUpY);
                powerUpY += 15;
            }

            if (player.TripleShot)
            {
                graphics.DrawString($"[TRIPLE {player.TripleShotTimeLeft / 1000f:0.0}s]", smallFont, Brushes.DeepSkyBlue, 8, powerUpY);
                powerUpY += 15;
            }

            if (player.FireBoostTimeLeft > 0)
                graphics.DrawString($"[FAST FIRE {player.FireBoostTimeLeft / 1000f:0.0}s]", smallFont, Brushes.Orange, 8, powerUpY);
        }

        private void DrawCenteredText(Graphics graphics, string text, Color color, int size)
        {
            using var font = new Font("Courier New", size, FontStyle.Bold);
            SizeF measured = graphics.MeasureString(text, font);
            float x = (GameSettings.ScreenWidth - measured.Width) / 2f;
            float y = (GameSettings.ScreenHeight - measured.Height) / 2f;
            using var background = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            using var brush = new SolidBrush(color);

            graphics.FillRectangle(background, x - 10, y - 8, measured.Width + 20, measured.Height + 16);
            graphics.DrawString(text, font, brush, x, y);
        }

        private void InitStars()
        {
            for (int i = 0; i < _starX.Length; i++)
            {
                _starX[i] = _rng.Next(GameSettings.ScreenWidth);
                _starY[i] = _rng.Next(GameSettings.ScreenHeight);
            }
        }

        private void DrawBackground(Graphics graphics)
        {
            if (_backgroundTheme == "bg_galaxy")
            {
                graphics.Clear(Color.FromArgb(8, 4, 28));
                using var purpleNebula = new SolidBrush(Color.FromArgb(45, 145, 70, 220));
                using var blueNebula = new SolidBrush(Color.FromArgb(40, 35, 145, 255));
                graphics.FillEllipse(purpleNebula, -150, 120, 420, 280);
                graphics.FillEllipse(blueNebula, 290, 360, 360, 250);
            }
            else
            {
                graphics.Clear(Color.FromArgb(5, 5, 18));
            }

            for (int i = 0; i < _starX.Length; i++)
            {
                int alpha = 80 + (i % 3) * 60;
                using var brush = new SolidBrush(Color.FromArgb(alpha, Color.White));
                graphics.FillEllipse(brush, _starX[i], _starY[i], 2, 2);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_gameTimer is not null)
            {
                _gameTimer.Stop();
                _gameTimer.Tick -= GameTick;
                _gameTimer.Dispose();
            }

            base.OnFormClosed(e);
        }
    }
}
