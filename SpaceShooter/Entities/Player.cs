using SpaceShooter.GameCore;
using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SpaceShooter.Entities
{
    public enum PowerUpType
    {
        TripleShot,
        Shield,
        HealthPack,
        FireRateBoost
    }

    public class Player : GameObject
    {
        public int Lives { get; set; }
        public int Score { get; set; }
        public int Coins { get; set; }

        public bool MovingLeft;
        public bool MovingRight;
        public bool MovingUp;
        public bool MovingDown;

        private float _vx;
        private float _vy;
        private const float Acceleration = 6f;
        private const float MaxSpeed = 7f;
        private const float Friction = 0.80f;

        private int _fireCooldown;
        private int _fireRateMs;
        public bool TripleShot { get; private set; }
        private int _tripleShotTimer;

        public bool HasShield { get; private set; }
        private int _shieldTimer;

        private int _fireRateTimer;

        public string ShipSkin { get; private set; } = "default";
        private Color _shipColor = Color.DodgerBlue;
        private Color _bulletColor = Color.Cyan;

        public Player(float x, float y)
            : base(x, y, 40, 40, GameSettings.PlayerStartHP)
        {
            Lives = GameSettings.PlayerStartLives;
            _fireRateMs = GameSettings.FireCooldownMs;
        }

        public void ApplySkin(string skinId)
        {
            ShipSkin = skinId == "skin_eagle" ? "skin_eagle" : "default";
            _shipColor = ShipSkin == "skin_eagle" ? Color.Crimson : Color.DodgerBlue;
        }

        public void ApplyBulletStyle(string bulletId)
        {
            _bulletColor = bulletId == "bullet_laser" ? Color.LimeGreen : Color.Cyan;
        }

        public void ResetMotion()
        {
            _vx = 0;
            _vy = 0;
        }

        public override void Update(float deltaTime)
        {
            if (MovingLeft)
                _vx -= Acceleration;
            if (MovingRight)
                _vx += Acceleration;
            if (MovingUp)
                _vy -= Acceleration;
            if (MovingDown)
                _vy += Acceleration;

            _vx = Math.Clamp(_vx, -MaxSpeed, MaxSpeed);
            _vy = Math.Clamp(_vy, -MaxSpeed, MaxSpeed);

            if (!MovingLeft && !MovingRight)
                _vx *= Friction;
            if (!MovingUp && !MovingDown)
                _vy *= Friction;

            if (Math.Abs(_vx) < 0.1f)
                _vx = 0;
            if (Math.Abs(_vy) < 0.1f)
                _vy = 0;

            X += _vx;
            Y += _vy;

            X = Math.Clamp(X, 0, GameSettings.ScreenWidth - Width);
            Y = Math.Clamp(Y, 0, GameSettings.ScreenHeight - Height);

            if (_fireCooldown > 0)
                _fireCooldown -= (int)(deltaTime * 1000);

            UpdatePowerUpTimers(deltaTime);
        }

        public List<Bullet> TryShoot()
        {
            var bullets = new List<Bullet>();
            if (_fireCooldown > 0)
                return bullets;

            _fireCooldown = _fireRateMs;
            float centerX = X + Width / 2f - 3;

            if (TripleShot)
            {
                bullets.Add(CreatePlayerBullet(centerX, Y - 14, 0));
                bullets.Add(CreatePlayerBullet(centerX, Y - 14, -2.5f));
                bullets.Add(CreatePlayerBullet(centerX, Y - 14, 2.5f));
            }
            else
            {
                bullets.Add(CreatePlayerBullet(centerX, Y - 14, 0));
            }

            return bullets;
        }

        public void ApplyPowerUp(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.TripleShot:
                    TripleShot = true;
                    _tripleShotTimer = 10000;
                    break;

                case PowerUpType.Shield:
                    HasShield = true;
                    _shieldTimer = 5000;
                    break;

                case PowerUpType.HealthPack:
                    HP = Math.Min(HP + 40, MaxHP);
                    break;

                case PowerUpType.FireRateBoost:
                    _fireRateMs = 100;
                    _fireRateTimer = 10000;
                    break;
            }
        }

        public int ShieldTimeLeft => _shieldTimer;
        public int TripleShotTimeLeft => _tripleShotTimer;
        public int FireBoostTimeLeft => _fireRateTimer;

        public override void Draw(Graphics g)
        {
            if (HasShield)
            {
                using var shieldPen = new Pen(Color.FromArgb(180, Color.Cyan), 3);
                g.DrawEllipse(shieldPen, X - 8, Y - 8, Width + 16, Height + 16);
            }

            if (ShipSkin == "skin_eagle")
                DrawRedEagle(g);
            else
                DrawDefaultShip(g);
        }

        private Bullet CreatePlayerBullet(float x, float y, float horizontalVelocity)
        {
            return new Bullet(x, y, horizontalVelocity, -GameSettings.BulletSpeed, true)
            {
                BulletColor = _bulletColor
            };
        }

        private void UpdatePowerUpTimers(float deltaTime)
        {
            if (_tripleShotTimer > 0)
            {
                _tripleShotTimer -= (int)(deltaTime * 1000);
                if (_tripleShotTimer <= 0)
                    TripleShot = false;
            }

            if (_shieldTimer > 0)
            {
                _shieldTimer -= (int)(deltaTime * 1000);
                if (_shieldTimer <= 0)
                    HasShield = false;
            }

            if (_fireRateTimer > 0)
            {
                _fireRateTimer -= (int)(deltaTime * 1000);
                if (_fireRateTimer <= 0)
                    _fireRateMs = GameSettings.FireCooldownMs;
            }
        }

        private void DrawDefaultShip(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;
            int w = Width;
            int h = Height;

            using var body = new SolidBrush(_shipColor);
            using var accent = new SolidBrush(Color.White);
            using var engine = new SolidBrush(Color.FromArgb(255, 120, 50));

            g.FillEllipse(engine, x + w / 2 - 5, y + h - 4, 10, 8);

            Point[] hull =
            {
                new Point(x + w / 2, y),
                new Point(x + w - 4, y + h),
                new Point(x + 4, y + h)
            };
            g.FillPolygon(body, hull);
            g.FillEllipse(accent, x + w / 2 - 6, y + h / 3, 12, 10);

            Point[] leftWing =
            {
                new Point(x, y + h - 10),
                new Point(x + w / 2 - 4, y + h / 2),
                new Point(x + w / 2 - 4, y + h)
            };
            Point[] rightWing =
            {
                new Point(x + w, y + h - 10),
                new Point(x + w / 2 + 4, y + h / 2),
                new Point(x + w / 2 + 4, y + h)
            };
            g.FillPolygon(body, leftWing);
            g.FillPolygon(body, rightWing);
        }

        private void DrawRedEagle(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;
            int w = Width;
            int h = Height;

            using var body = new SolidBrush(Color.Crimson);
            using var wing = new SolidBrush(Color.DarkRed);
            using var trim = new SolidBrush(Color.Gold);
            using var cockpit = new SolidBrush(Color.LightCyan);
            using var engine = new SolidBrush(Color.OrangeRed);

            g.FillEllipse(engine, x + w / 2 - 6, y + h - 5, 12, 9);

            Point[] hull =
            {
                new Point(x + w / 2, y),
                new Point(x + w - 8, y + h - 6),
                new Point(x + 8, y + h - 6)
            };
            g.FillPolygon(body, hull);

            Point[] leftWing =
            {
                new Point(x, y + h - 8),
                new Point(x + w / 2 - 5, y + h / 2),
                new Point(x + w / 2 - 3, y + h)
            };
            Point[] rightWing =
            {
                new Point(x + w, y + h - 8),
                new Point(x + w / 2 + 5, y + h / 2),
                new Point(x + w / 2 + 3, y + h)
            };
            g.FillPolygon(wing, leftWing);
            g.FillPolygon(wing, rightWing);

            g.FillRectangle(trim, x + w / 2 - 2, y + 10, 4, h - 16);
            g.FillEllipse(cockpit, x + w / 2 - 6, y + 12, 12, 10);
        }
    }
}
