using SpaceShooter.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceShooter.GameUI
{

    public class FormShop : Form
    {
        private const string ExtraLifePackId = "extra_life";

        private readonly PlayerData _data;
        private Label _coinLabel = null!;

        public FormShop(PlayerData data)
        {
            _data = data;

            Text = "Shop";
            ClientSize = new Size(GameCore.GameSettings.ScreenWidth, GameCore.GameSettings.ScreenHeight);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(10, 10, 30);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            BuildUi();
        }

        private void BuildUi()
        {
            Controls.Clear();

            var title = new Label
            {
                Text = "— SHOP —",
                Font = new Font("Courier New", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(580, 50),
                Location = new Point(10, 30)
            };

            _coinLabel = new Label
            {
                Text = $"Your Coins: {_data.TotalCoins}",
                Font = new Font("Courier New", 12, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(580, 28),
                Location = new Point(10, 86)
            };

            Controls.Add(title);
            Controls.Add(_coinLabel);

            AddItem("Red Eagle Skin", "Changes the ship to the Red Eagle design.", 200, "skin_eagle", 120);
            AddItem("Laser Bullets", "Changes player bullets to a green laser style.", 150, "bullet_laser", 210);
            AddItem("Galaxy Background", "Adds a deep-galaxy background to each run.", 300, "bg_galaxy", 300);
            AddItem("Extra Life Pack", $"Consumable. In inventory: {_data.ExtraLifePacks}", 250, ExtraLifePackId, 390);

            var backButton = new Button
            {
                Text = "← Back",
                Font = new Font("Courier New", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 80),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 42),
                Location = new Point(20, 680),
                Cursor = Cursors.Hand
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += (_, _) => Close();
            Controls.Add(backButton);
        }

        private void AddItem(string name, string description, int price, string itemId, int y)
        {
            bool consumable = itemId == ExtraLifePackId;
            bool owned = !consumable && _data.Owns(itemId);
            bool equipped = !consumable && IsEquipped(itemId);

            var panel = new Panel
            {
                Size = new Size(540, 72),
                Location = new Point(30, y),
                BackColor = Color.FromArgb(20, 20, 50)
            };

            var nameLabel = new Label
            {
                Text = name,
                Font = new Font("Courier New", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(10, 8)
            };

            var descriptionLabel = new Label
            {
                Text = description,
                Font = new Font("Courier New", 9),
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(10, 32)
            };

            string state = consumable
                ? $"{price} coins"
                : owned ? (equipped ? "✓ Equipped" : "Owned") : $"{price} coins";

            var stateLabel = new Label
            {
                Text = state,
                Font = new Font("Courier New", 10, FontStyle.Bold),
                ForeColor = equipped ? Color.LimeGreen : owned ? Color.Cyan : Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(10, 50)
            };

            string buttonText = consumable ? "Buy" : equipped ? "Equipped" : owned ? "Equip" : "Buy";
            var actionButton = new Button
            {
                Text = buttonText,
                Font = new Font("Courier New", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = equipped ? Color.FromArgb(40, 100, 40) : owned ? Color.FromArgb(35, 80, 125) : Color.FromArgb(100, 70, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 36),
                Location = new Point(430, 18),
                Cursor = Cursors.Hand,
                Enabled = !equipped
            };
            actionButton.FlatAppearance.BorderSize = 0;
            actionButton.Click += (_, _) => HandleItemAction(itemId, price);

            panel.Controls.Add(nameLabel);
            panel.Controls.Add(descriptionLabel);
            panel.Controls.Add(stateLabel);
            panel.Controls.Add(actionButton);
            Controls.Add(panel);
        }

        private void HandleItemAction(string itemId, int price)
        {
            if (itemId == ExtraLifePackId)
            {
                if (!TrySpend(price))
                    return;

                _data.ExtraLifePacks++;
                Database.Save(_data);
                RefreshShop();
                return;
            }

            if (!_data.Owns(itemId))
            {
                if (!TrySpend(price))
                    return;

                _data.AddOwnedItem(itemId);
            }

            if (itemId.StartsWith("skin"))
                _data.EquippedSkin = itemId;
            else if (itemId.StartsWith("bullet"))
                _data.EquippedBullet = itemId;
            else if (itemId.StartsWith("bg"))
                _data.EquippedBg = itemId;

            Database.Save(_data);
            RefreshShop();
        }

        private bool TrySpend(int price)
        {
            if (_data.TotalCoins < price)
            {
                MessageBox.Show("Not enough coins!", "Shop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            _data.TotalCoins -= price;
            return true;
        }

        private void RefreshShop()
        {
            BuildUi();
        }

        private bool IsEquipped(string itemId)
        {
            if (itemId.StartsWith("skin"))
                return _data.EquippedSkin == itemId;
            if (itemId.StartsWith("bullet"))
                return _data.EquippedBullet == itemId;
            if (itemId.StartsWith("bg"))
                return _data.EquippedBg == itemId;

            return false;
        }
    }
}
