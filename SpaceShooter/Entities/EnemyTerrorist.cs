using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Entities
{
    public class EnemyTerrorist : Enemy
    {
        private float _dirX;
        private float _dirY = 1f;

        public EnemyTerrorist(float x, float y, int wave)
            : base(x, y, 32, 32, 60 + GameSettings.EnemyHPBonus(wave),
                  2.8f * GameSettings.EnemySpeedScale(wave))
        {
            ScoreValue = 400;
            CoinDropChance = 72;
            GoldCoinChance = 45;
        }

        public void TrackPlayer(float playerCenterX, float playerCenterY)
        {
            float enemyCenterX = X + Width / 2f;
            float enemyCenterY = Y + Height / 2f;
            float dx = playerCenterX - enemyCenterX;
            float dy = playerCenterY - enemyCenterY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance < 0.01f)
                return;

            _dirX = dx / distance;
            _dirY = dy / distance;
        }

        public override void Update(float deltaTime)
        {
            float frameScale = GameSettings.FrameScale(deltaTime);
            X += _dirX * Speed * frameScale;
            Y += _dirY * Speed * frameScale;
        }

        public override void Draw(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;
            int w = Width;
            int h = Height;

            using var body = new SolidBrush(Color.FromArgb(220, 60, 40));
            using var spike = new SolidBrush(Color.FromArgb(255, 100, 60));
            using var core = new SolidBrush(Color.White);

            g.FillEllipse(body, x + 4, y + 4, w - 8, h - 8);

            Point[] top = { new Point(x + w / 2, y), new Point(x + w / 2 - 5, y + 10), new Point(x + w / 2 + 5, y + 10) };
            Point[] bottom = { new Point(x + w / 2, y + h), new Point(x + w / 2 - 5, y + h - 10), new Point(x + w / 2 + 5, y + h - 10) };
            Point[] left = { new Point(x, y + h / 2), new Point(x + 10, y + h / 2 - 5), new Point(x + 10, y + h / 2 + 5) };
            Point[] right = { new Point(x + w, y + h / 2), new Point(x + w - 10, y + h / 2 - 5), new Point(x + w - 10, y + h / 2 + 5) };
            g.FillPolygon(spike, top);
            g.FillPolygon(spike, bottom);
            g.FillPolygon(spike, left);
            g.FillPolygon(spike, right);
            g.FillEllipse(core, x + w / 2 - 4, y + h / 2 - 4, 8, 8);

            DrawHealthBar(g);
        }
    }
}
