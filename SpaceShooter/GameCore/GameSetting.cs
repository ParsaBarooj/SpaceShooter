using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.GameCore
{
    public static class GameSettings
    {
        public const int ScreenWidth = 600;
        public const int ScreenHeight = 750;

        public const int PlayerStartHP = 100;
        public const int PlayerStartLives = 3;
        public const int PlayerSpeed = 5;
        public const int BulletSpeed = 10;
        public const int FireCooldownMs = 220;

        public const int TotalWaves = 10;
        public const int TimerIntervalMs = 20;

        public static float EnemySpeedScale(int wave) => 1f + 0.1f * wave;
        public static int EnemyHPBonus(int wave) => 2 * wave;
    }
}
