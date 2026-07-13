using SpaceShooter.GameCore;
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
    public class FormOptions : Form
    {
        public FormOptions()
        {
            Text = "Options";
            ClientSize = new Size(480, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(12, 12, 32);
            BuildUi();
        }

        private void BuildUi()
        {
            AddLabel("— OPTIONS —", 16, FontStyle.Bold, Color.Cyan, new Point(20, 20), new Size(420, 36));
            AddLabel("Audio Controls", 12, FontStyle.Bold, Color.White, new Point(20, 72), new Size(200, 24));

            var musicCheckBox = new CheckBox
            {
                Text = "Background Music",
                Font = new Font("Courier New", 10),
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                Checked = AudioManager.MusicEnabled,
                Location = new Point(30, 104),
                AutoSize = true
            };
            musicCheckBox.CheckedChanged += (_, _) => AudioManager.SetMusicEnabled(musicCheckBox.Checked);

            var sfxCheckBox = new CheckBox
            {
                Text = "Sound Effects (SFX)",
                Font = new Font("Courier New", 10),
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                Checked = AudioManager.SfxEnabled,
                Location = new Point(30, 134),
                AutoSize = true
            };
            sfxCheckBox.CheckedChanged += (_, _) => AudioManager.SetSfxEnabled(sfxCheckBox.Checked);

            Controls.Add(musicCheckBox);
            Controls.Add(sfxCheckBox);

            AddLabel("Controls Guide", 12, FontStyle.Bold, Color.White, new Point(20, 180), new Size(200, 24));

            string controls =
                "Move Ship      : Arrow Keys or W A S D\n" +
                "Shoot          : Space\n" +
                "Pause / Resume : Esc\n\n" +
                "Collect coins by flying over them.\n" +
                "Grab power-ups to gain special abilities.";

            var controlsLabel = new Label
            {
                Text = controls,
                Font = new Font("Courier New", 9),
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(20, 20, 50),
                AutoSize = false,
                Size = new Size(420, 130),
                Location = new Point(20, 210),
                Padding = new Padding(8)
            };
            Controls.Add(controlsLabel);

            var closeButton = new Button
            {
                Text = "Close",
                Font = new Font("Courier New", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 90),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(170, 420),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (_, _) => Close();
            Controls.Add(closeButton);
        }

        private void AddLabel(string text, int fontSize, FontStyle style, Color color, Point location, Size size)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Courier New", fontSize, style),
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = size,
                Location = location,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(label);
        }
    }
}