using SpaceShooter.GameCore;
using SpaceShooter.GameCore;
using System.Drawing;

namespace SpaceShooter.Entities
{
    public class Bullet : GameObject
    {
        private readonly float _vx;
        private readonly float _vy;

        public bool IsPlayerBullet { get; }
        public int Damage { get; }
        public Color BulletColor { get; set; }

        public Bullet(float x, float y, float vx, float vy, bool isPlayerBullet, int damage = 20)
            : base(x, y, 6, 14, 1)
        {
            _vx = vx;
            _vy = vy;
            IsPlayerBullet = isPlayerBullet;
            Damage = damage;
            BulletColor = isPlayerBullet ? Color.Cyan : Color.OrangeRed;
        }

        public override void Update(float deltaTime)
        {
            float frameScale = GameSettings.FrameScale(deltaTime);
            X += _vx * frameScale;
            Y += _vy * frameScale;
        }

        public override void Draw(Graphics g)
        {
            using var glowBrush = new SolidBrush(Color.FromArgb(80, BulletColor));
            using var brush = new SolidBrush(BulletColor);
            g.FillRectangle(glowBrush, X - 2, Y - 2, Width + 4, Height + 4);
            g.FillRectangle(brush, X, Y, Width, Height);
        }

        public bool IsOffScreen()
        {
            return Y < -Height || Y > GameSettings.ScreenHeight + Height ||
                   X < -Width || X > GameSettings.ScreenWidth + Width;
        }
    }
}
