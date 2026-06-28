using SpaceShooter.GameCore;
using System.Drawing;

namespace SpaceShooter.Entities
{
    public class CoinDrop : GameObject
    {
        public int Value { get; }
        private readonly bool _isGold;

        public CoinDrop(float x, float y, int value)
            : base(x, y, 14, 14, 1)
        {
            Value = value;
            _isGold = value >= 5;
        }

        public override void Update(float deltaTime)
        {
            Y += 2.5f * GameSettings.FrameScale(deltaTime);
        }

        public override void Draw(Graphics g)
        {
            Color color = _isGold ? Color.Gold : Color.Silver;
            using var brush = new SolidBrush(color);
            using var pen = new Pen(Color.FromArgb(180, Color.DarkGoldenrod), 1.5f);
            g.FillEllipse(brush, X, Y, Width, Height);
            g.DrawEllipse(pen, X, Y, Width, Height);
        }

        public bool IsOffScreen() => Y > GameSettings.ScreenHeight + 20;
    }
}
