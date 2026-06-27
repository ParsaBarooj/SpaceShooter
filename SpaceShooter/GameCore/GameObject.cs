using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.GameCore
{
    public abstract class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int HP { get; set; }
        public int MaxHP { get; protected set; }
        public bool IsAlive => HP > 0;

        public Rectangle Bounds => new Rectangle((int)X, (int)Y, Width, Height);

        protected GameObject(float x, float y, int width, int height, int hp)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            HP = hp;
            MaxHP = hp;
        }

        public abstract void Update(float deltaTime);
        public abstract void Draw(Graphics g);

        public bool CollidesWith(GameObject other)
        {
            return Bounds.IntersectsWith(other.Bounds);
        }

        public void TakeDamage(int amount)
        {
            HP -= amount;
            if (HP < 0) HP = 0;
        }
    }
}
