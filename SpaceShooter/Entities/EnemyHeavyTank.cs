using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Entities
{
    public class EnemyHeavyTank : Enemy
    {
        private int _shootTimer;
        private const int ShootInterval = 2500;

        public EnemyHeavyTank(float x, float y, int wave)
            : base(x, y, 54, 50, 200 + GameSettings.EnemyHPBonus(wave) * 3,
                  0.6f * GameSettings.EnemySpeedScale(wave))
        {
            ScoreValue = 800;
            CoinDropChance = 100;
            GoldCoinChance = 100;
            _shootTimer = 1000;
        }

        public override void Update(float deltaTime)
        {
            Y += Speed * GameSettings.FrameScale(deltaTime);
            _shootTimer -= (int)(deltaTime * 1000);
        }

        public override List<Bullet> TryShoot()
        {
            var bullets = new List<Bullet>();
            if (_shootTimer > 0)
                return bullets;

            _shootTimer = ShootInterval;
            float centerX = X + Width / 2f;
            float centerY = Y + Height / 2f;

            float[][] directions =
            {
                new[] { 0f, 1f }, new[] { 0f, -1f }, new[] { 1f, 0f }, new[] { -1f, 0f },
                new[] { 0.7f, 0.7f }, new[] { -0.7f, 0.7f }, new[] { 0.7f, -0.7f }, new[] { -0.7f, -0.7f }
            };

            foreach (float[] direction in directions)
            {
                var bullet = new Bullet(centerX - 3, centerY - 7, direction[0] * 4, direction[1] * 4, false, 25)
                {
                    BulletColor = Color.Yellow
                };
                bullets.Add(bullet);
            }

            return bullets;
        }

        public override void Draw(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;
            int w = Width;
            int h = Height;

            using var armor = new SolidBrush(Color.FromArgb(100, 100, 120));
            using var dark = new SolidBrush(Color.FromArgb(60, 60, 80));
            using var glow = new SolidBrush(Color.FromArgb(200, 220, 50));
            using var cannon = new SolidBrush(Color.FromArgb(80, 80, 90));

            g.FillRectangle(armor, x + 6, y + 6, w - 12, h - 12);
            g.FillRectangle(dark, x, y + 10, 8, h - 20);
            g.FillRectangle(dark, x + w - 8, y + 10, 8, h - 20);
            g.FillRectangle(cannon, x + w / 2 - 5, y + h - 16, 10, 16);
            g.FillEllipse(glow, x + w / 2 - 8, y + h / 2 - 8, 16, 16);
            g.FillRectangle(dark, x + 10, y + 10, 6, 6);
            g.FillRectangle(dark, x + w - 16, y + 10, 6, 6);

            DrawHealthBar(g, 50);
        }
    }
}
