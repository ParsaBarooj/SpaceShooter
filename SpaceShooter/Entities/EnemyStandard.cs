using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Entities
{
    public class EnemyStandard : Enemy
    {
        public EnemyStandard(float x, float y, int wave)
            : base(x, y, 34, 30, 20 + GameSettings.EnemyHPBonus(wave),
                  2f * GameSettings.EnemySpeedScale(wave))
        {
            ScoreValue = 100;
            CoinDropChance = 42;
            GoldCoinChance = 4;
        }

        public override void Update(float deltaTime)
        {
            Y += Speed * GameSettings.FrameScale(deltaTime);
        }

        public override void Draw(Graphics g)
        {
            int x = (int)X;
            int y = (int)Y;

            using var body = new SolidBrush(Color.FromArgb(180, 80, 200));
            using var dome = new SolidBrush(Color.FromArgb(220, 140, 255));

            g.FillEllipse(body, x, y + 10, Width, 16);
            g.FillEllipse(dome, x + 8, y + 2, Width - 16, 16);

            DrawHealthBar(g);
        }
    }
}
