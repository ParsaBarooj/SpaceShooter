using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace SpaceShooter.Data
{
    public class PlayerData
    {
        public int TotalCoins { get; set; }
        public int HighScore { get; set; }

        public string OwnedItems { get; set; } = "";
        public string EquippedSkin { get; set; } = "default";
        public string EquippedBullet { get; set; } = "default";
        public string EquippedBg { get; set; } = "default";

        public int ExtraLifePacks { get; set; }

        public bool Owns(string itemId)
        {
            return GetOwnedItems().Contains(itemId);
        }

        public void AddOwnedItem(string itemId)
        {
            HashSet<string> items = GetOwnedItems();
            items.Add(itemId);
            OwnedItems = string.Join(",", items.OrderBy(x => x, StringComparer.Ordinal));
        }

        private HashSet<string> GetOwnedItems()
        {
            return OwnedItems
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.Ordinal);
        }
    }

    public static class Database
    {
        private static string DbPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game_data.db");

        private static string ConnectionString => $"Data Source={DbPath}";

        public static void Init()
        {
            using var con = new SqliteConnection(ConnectionString);
            con.Open();

            using (var create = con.CreateCommand())
            {
                create.CommandText = @"
                    CREATE TABLE IF NOT EXISTS player (
                        id              INTEGER PRIMARY KEY,
                        total_coins     INTEGER NOT NULL DEFAULT 0,
                        high_score      INTEGER NOT NULL DEFAULT 0,
                        owned_items     TEXT    NOT NULL DEFAULT '',
                        equipped_skin   TEXT    NOT NULL DEFAULT 'default',
                        equipped_bullet TEXT    NOT NULL DEFAULT 'default',
                        equipped_bg     TEXT    NOT NULL DEFAULT 'default',
                        extra_life_packs INTEGER NOT NULL DEFAULT 0
                    );
                    INSERT OR IGNORE INTO player (id) VALUES (1);";
                create.ExecuteNonQuery();
            }

            if (!ColumnExists(con, "player", "extra_life_packs"))
            {
                using var alter = con.CreateCommand();
                alter.CommandText = "ALTER TABLE player ADD COLUMN extra_life_packs INTEGER NOT NULL DEFAULT 0;";
                alter.ExecuteNonQuery();

                if (ColumnExists(con, "player", "extra_lives"))
                {
                    using var migrate = con.CreateCommand();
                    migrate.CommandText = @"
                        UPDATE player
                        SET extra_life_packs = extra_lives
                        WHERE extra_life_packs = 0;";
                    migrate.ExecuteNonQuery();
                }
            }
        }

        public static PlayerData Load()
        {
            Init();

            using var con = new SqliteConnection(ConnectionString);
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT total_coins, high_score, owned_items, equipped_skin,
                       equipped_bullet, equipped_bg, extra_life_packs
                FROM player
                WHERE id = 1;";

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new PlayerData
                {
                    TotalCoins = reader.GetInt32(0),
                    HighScore = reader.GetInt32(1),
                    OwnedItems = reader.GetString(2),
                    EquippedSkin = reader.GetString(3),
                    EquippedBullet = reader.GetString(4),
                    EquippedBg = reader.GetString(5),
                    ExtraLifePacks = reader.GetInt32(6)
                };
            }

            return new PlayerData();
        }

        public static void Save(PlayerData data)
        {
            Init();

            using var con = new SqliteConnection(ConnectionString);
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
                UPDATE player SET
                    total_coins = $coins,
                    high_score = $score,
                    owned_items = $owned,
                    equipped_skin = $skin,
                    equipped_bullet = $bullet,
                    equipped_bg = $bg,
                    extra_life_packs = $packs
                WHERE id = 1;";
            cmd.Parameters.AddWithValue("$coins", data.TotalCoins);
            cmd.Parameters.AddWithValue("$score", data.HighScore);
            cmd.Parameters.AddWithValue("$owned", data.OwnedItems);
            cmd.Parameters.AddWithValue("$skin", data.EquippedSkin);
            cmd.Parameters.AddWithValue("$bullet", data.EquippedBullet);
            cmd.Parameters.AddWithValue("$bg", data.EquippedBg);
            cmd.Parameters.AddWithValue("$packs", data.ExtraLifePacks);
            cmd.ExecuteNonQuery();
        }

        private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"PRAGMA table_info({tableName});";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
