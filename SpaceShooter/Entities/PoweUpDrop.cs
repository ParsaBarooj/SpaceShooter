using SpaceShooter.GameCore;
using System.Drawing;

namespace SpaceShooter.Entities
{
    public class PowerUpDrop : GameObject
    {
        public PowerUpType Type { get; }
        private int _animTick;

        public PowerUpDrop(float x, float y, PowerUpType type)
            : base(x, y, 22, 22, 1)
        {
            Type = type;
        }

        public override void Update(float deltaTime)
        {
            Y += 1.8f * GameSettings.FrameScale(deltaTime);
            _animTick++;
        }

        public override void Draw(Graphics g)
        {
            float pulse = 1f + 0.1f * (float)System.Math.Sin(_animTick * 0.15f);
            int pulseWidth = (int)(Width * pulse);
            int pulseHeight = (int)(Height * pulse);
            int offsetX = (pulseWidth - Width) / 2;
            int offsetY = (pulseHeight - Height) / 2;

            Color color = Type switch
            {
                PowerUpType.TripleShot => Color.DeepSkyBlue,
                PowerUpType.Shield => Color.Cyan,
                PowerUpType.HealthPack => Color.LimeGreen,
                PowerUpType.FireRateBoost => Color.Orange,
                _ => Color.White
            };

            using var brush = new SolidBrush(color);
            using var pen = new Pen(Color.White, 1.5f);
            g.FillRectangle(brush, X - offsetX, Y - offsetY, pulseWidth, pulseHeight);
            g.DrawRectangle(pen, X - offsetX, Y - offsetY, pulseWidth, pulseHeight);

            string label = Type switch
            {
                PowerUpType.TripleShot => "3x",
                PowerUpType.Shield => "S",
                PowerUpType.HealthPack => "+",
                PowerUpType.FireRateBoost => "F",
                _ => "?"
            };

            using var font = new Font("Arial", 8, FontStyle.Bold);
            g.DrawString(label, font, Brushes.White, X + 2, Y + 4);
        }

        public bool IsOffScreen() => Y > GameSettings.ScreenHeight + 30;
    }
}
