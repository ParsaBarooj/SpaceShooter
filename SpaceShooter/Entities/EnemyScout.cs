using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Entities
{
    public class EnemyScout : Enemy
    {
        private float _time;
        private readonly float _amplitude;

        public EnemyScout(float x, float y, int wave)
            : base(x, y, 30, 28, 15 + GameSettings.EnemyHPBonus(wave),
                  2.5f * GameSettings.EnemySpeedScale(wave))
        {
            ScoreValue = 150;
            CoinDropChance = 52;
            GoldCoinChance = 10;
            _amplitude = 80f;
        }

        public override void Update(float deltaTime)
        {
            _time += deltaTime * 3f;
            Y += Speed * GameSettings.FrameScale(deltaTime);
            X += (float)Math.Sin(_time) * _amplitude * deltaTime;
            X = Math.Clamp(X, 0, GameSettings.ScreenWidth - Width);
        }

        public override void Draw(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;

            using var body = new SolidBrush(Color.FromArgb(50, 200, 150));
            using var wing = new SolidBrush(Color.FromArgb(30, 140, 100));
            using var eye = new SolidBrush(Color.LimeGreen);

            g.FillEllipse(body, x + 5, y + 5, Width - 10, Height - 10);

            Point[] left = { new Point(x, y + 14), new Point(x + 10, y + 8), new Point(x + 10, y + 20) };
            Point[] right = { new Point(x + Width, y + 14), new Point(x + Width - 10, y + 8), new Point(x + Width - 10, y + 20) };
            g.FillPolygon(wing, left);
            g.FillPolygon(wing, right);
            g.FillEllipse(eye, x + Width / 2 - 4, y + Height / 2 - 4, 8, 8);

            DrawHealthBar(g);
        }
    }
}
