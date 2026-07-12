using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceShooter.Data;
using System.Windows.Forms;
using SpaceShooter.Entities;
using SpaceShooter.GameCore;


namespace SpaceShooter.GameUI
{
    public class FormMain : Form
    {
        private Label _titleLabel = null!;
        private Button _btnPlay = null!, _btnShop = null!, _btnOptions = null!, _btnAbout = null!, _btnQuit = null!;
        private Label _coinLabel = null!;

        private System.Windows.Forms.Timer _animTimer = new System.Windows.Forms.Timer { Interval = 60 };
        private int[] _starY = new int[60];
        private int[] _starX = new int[60];
        private int[] _starS = new int[60];
        private Random _rng = new Random();

        public FormMain()
        {
            Text = "Space Shooter";
            ClientSize = new Size(GameSettings.ScreenWidth, GameSettings.ScreenHeight);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(8, 8, 24);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            Data.Database.Init();
            AudioManager.Initialize();
            AudioManager.StartBackgroundMusic();
            BuildUI();
            InitStars();

            _animTimer.Tick += (s, e) => { MoveStars(); Invalidate(); };
            _animTimer.Start();
        }

        private void BuildUI()
        {
            _titleLabel = new Label
            {
                Text = "★  SPACE SHOOTER  ★",
                Font = new Font("Courier New", 22, FontStyle.Bold),
                ForeColor = Color.Cyan,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(580, 60),
                Location = new Point(10, 80)
            };

            _coinLabel = new Label
            {
                Text = $"Coins: {Database.Load().TotalCoins}  |  Best: {Database.Load().HighScore}",
                Font = new Font("Courier New", 10),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(580, 25),
                Location = new Point(10, 148)
            };

            int bw = 220, bx = (ClientSize.Width - bw) / 2;
            int startY = 230;
            int gap = 64;

            _btnPlay = MakeButton("▶  Play", bx, startY, Color.FromArgb(0, 160, 80));
            _btnShop = MakeButton("🛒  Shop", bx, startY + gap, Color.FromArgb(160, 100, 0));
            _btnOptions = MakeButton("⚙  Options", bx, startY + gap * 2, Color.FromArgb(40, 80, 160));
            _btnAbout = MakeButton("ℹ  About", bx, startY + gap * 3, Color.FromArgb(80, 40, 120));
            _btnQuit = MakeButton("✕  Quit", bx, startY + gap * 4, Color.FromArgb(140, 30, 30));

            _btnPlay.Click += (s, e) => OpenGame();
            _btnShop.Click += (s, e) => OpenShop();
            _btnOptions.Click += (s, e) => OpenOptions();
            _btnAbout.Click += (s, e) => OpenAbout();
            _btnQuit.Click += (s, e) => Application.Exit();

            Controls.Add(_titleLabel);
            Controls.Add(_coinLabel);
            Controls.Add(_btnPlay);
            Controls.Add(_btnShop);
            Controls.Add(_btnOptions);
            Controls.Add(_btnAbout);
            Controls.Add(_btnQuit);
        }

        private Button MakeButton(string text, int x, int y, Color bg)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Courier New", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = bg,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(220, 48),
                Location = new Point(x, y),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void OpenGame()
        {
            _animTimer.Stop();

            var data = Database.Load();
            var game = new FormGame(data);
            game.FormClosed += (s, e) =>
            {
                // Refresh the menu state after the game writes its progress.
                var fresh = Database.Load();
                _coinLabel.Text = $"Coins: {fresh.TotalCoins}  |  Best: {fresh.HighScore}";
                Show();
                _animTimer.Start();
            };

            Hide();
            game.Show();
        }

        private void OpenShop()
        {
            _animTimer.Stop();

            var data = Database.Load();
            var shop = new FormShop(data);
            shop.FormClosed += (s, e) =>
            {
                var fresh = Database.Load();
                _coinLabel.Text = $"Coins: {fresh.TotalCoins}  |  Best: {fresh.HighScore}";
                Show();
                _animTimer.Start();
            };

            Hide();
            shop.Show();
        }

        private void OpenOptions()
        {
            var opt = new FormOptions();
            opt.ShowDialog(this);
        }

        private void OpenAbout()
        {
            var about = new FormAbout();
            about.ShowDialog(this);
        }

        // --- star background ---
        private void InitStars()
        {
            for (int i = 0; i < _starY.Length; i++)
            {
                _starX[i] = _rng.Next(Width);
                _starY[i] = _rng.Next(Height);
                _starS[i] = _rng.Next(1, 4);
            }
        }

        private void MoveStars()
        {
            for (int i = 0; i < _starY.Length; i++)
            {
                _starY[i] += _starS[i];
                if (_starY[i] > Height) { _starY[i] = 0; _starX[i] = _rng.Next(Width); }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            foreach (int i in Range(_starY.Length))
            {
                int alpha = Math.Min(255, 80 + _starS[i] * 50);
                using var brush = new SolidBrush(Color.FromArgb(alpha, Color.White));
                g.FillEllipse(brush, _starX[i], _starY[i], _starS[i], _starS[i]);
            }
        }

        private static System.Collections.Generic.IEnumerable<int> Range(int n)
        { for (int i = 0; i < n; i++) yield return i; }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _animTimer.Stop();
            AudioManager.Shutdown();
            base.OnFormClosed(e);
        }
    }
}
