using SpaceShooter.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Entities
{
    public abstract class Enemy : GameObject
    {
        public int ScoreValue { get; protected set; }
        public float Speed { get; protected set; }
        public int CoinDropChance { get; protected set; }
        public int GoldCoinChance { get; protected set; }

        protected Enemy(float x, float y, int width, int height, int hp, float speed)
            : base(x, y, width, height, hp)
        {
            Speed = speed;
        }

        public abstract override void Update(float deltaTime);

        public virtual List<Bullet> TryShoot() => new();

        public bool IsOffScreen()
        {
            return Y > GameSettings.ScreenHeight + Height;
        }

        protected void DrawHealthBar(Graphics g, int barW = 36)
        {
            if (MaxHP <= 1)
                return;

            float ratio = (float)HP / MaxHP;
            int barX = (int)(X + Width / 2 - barW / 2);
            int barY = (int)Y - 8;

            g.FillRectangle(Brushes.DimGray, barX, barY, barW, 4);
            using var hBrush = new SolidBrush(
                ratio > 0.5f ? Color.LimeGreen : ratio > 0.25f ? Color.Orange : Color.Red);
            g.FillRectangle(hBrush, barX, barY, (int)(barW * ratio), 4);
        }
    }
}
