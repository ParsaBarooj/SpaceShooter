using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Entities
{
    public class EnemyShooter : Enemy
    {
        private int _shootTimer;
        private const int ShootInterval = 1500;

        public EnemyShooter(float x, float y, int wave)
            : base(x, y, 36, 34, 40 + GameSettings.EnemyHPBonus(wave),
                  1.2f * GameSettings.EnemySpeedScale(wave))
        {
            ScoreValue = 250;
            CoinDropChance = 62;
            GoldCoinChance = 22;
            _shootTimer = ShootInterval / 2;
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
            float centerX = X + Width / 2f - 3;
            bullets.Add(new Bullet(centerX, Y + Height, 0, 6, false, 15));
            return bullets;
        }

        public override void Draw(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;

            using var body = new SolidBrush(Color.FromArgb(200, 80, 60));
            using var cannon = new SolidBrush(Color.FromArgb(130, 40, 30));
            using var detail = new SolidBrush(Color.FromArgb(240, 160, 140));

            g.FillRectangle(body, x + 4, y + 4, Width - 8, Height - 12);
            g.FillRectangle(cannon, x + Width / 2 - 4, y + Height - 14, 8, 14);
            g.FillEllipse(detail, x + Width / 2 - 7, y + 6, 14, 10);
            g.FillRectangle(body, x, y + 10, 6, 16);
            g.FillRectangle(body, x + Width - 6, y + 10, 6, 16);

            DrawHealthBar(g);
        }
    }
}
