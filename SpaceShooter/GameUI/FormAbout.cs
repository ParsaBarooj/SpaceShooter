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
    public class FormAbout : Form
    {
        public FormAbout()
        {
            Text = "About";
            ClientSize = new Size(460, 380);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(12, 12, 32);
            BuildUI();
        }

        private void BuildUI()
        {
            var title = new Label
            {
                Text = "— ABOUT —",
                Font = new Font("Courier New", 16, FontStyle.Bold),
                ForeColor = Color.Cyan,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(420, 40),
                Location = new Point(20, 20)
            };

            string info =
                "Game Title  :  Space Shooter\n\n" +
                "Developer   :  Milad Zarei Maleki\n" +
                "Student ID  :  [replace before submission]\n\n" +
                "Course      :  Advanced Programming\n" +
                "Instructor  :  Dr. Marzieh Maleki Majd\n" +
                "University  :  Iran University of\n" +
                "                Science and Technology\n" +
                "Term        :  4042\n\n" +
                "Built with  :  C# · Windows Forms · SQLite";

            var lblInfo = new Label
            {
                Text = info,
                Font = new Font("Courier New", 9),
                ForeColor = Color.LightGray,
                BackColor = Color.FromArgb(18, 18, 45),
                AutoSize = false,
                Size = new Size(400, 230),
                Location = new Point(28, 72),
                Padding = new Padding(10)
            };

            var btnClose = new Button
            {
                Text = "Close",
                Font = new Font("Courier New", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 90),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 38),
                Location = new Point(165, 315),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();

            Controls.Add(title);
            Controls.Add(lblInfo);
            Controls.Add(btnClose);
        }
    }
}
