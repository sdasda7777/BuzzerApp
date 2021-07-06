using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuzzerApp
{
    public partial class Form1 : Form
    {
        struct PlayerInfo
        {
            public Keys Trigger;
            public string Name;
            public string Sound;
            public Color FrontColor;
            public Color BackColor;
        }

        bool GameView = false;
        bool GameState_Guess = false;
        PlayerInfo GuessingPlayer;

        Color DefaultFrontColor = Color.Black;
        Color DefaultBackColor = Color.White;
        

        List<PlayerInfo> PlayersInfo = new List<PlayerInfo>();

        public Form1()
        {
            InitializeComponent();
        }

        private void fullscreenF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchGV();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F11)
            {
                SwitchGV();
            }
            else if(GameState_Guess &&  e.KeyCode == Keys.Enter)
            {
                GameState_Guess = false;
                panel1.Invalidate();
            }
            else if (!GameState_Guess){
                foreach (PlayerInfo pi in PlayersInfo)
                {
                    if (pi.Trigger == e.KeyCode)
                    {
                        GameState_Guess = true;
                        GuessingPlayer = pi;
                        panel1.Invalidate();

                        if (File.Exists(pi.Sound))
                        {
                            SoundPlayer simpleSound = new SoundPlayer(pi.Sound);
                            simpleSound.Play();
                        }
                    }
                }
            }
        }

        void SwitchGV()
        {
            GameView = !GameView;
            if (GameView)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                menuStrip1.Visible = false;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                menuStrip1.Visible = true;
            }   
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileContent = string.Empty;
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Application.StartupPath;
                openFileDialog.Filter = "BuzzerApp settings (*.bas)|*.bas|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();

                        string[] lines = fileContent.Split(new string[] { Environment.NewLine.ToString() }, StringSplitOptions.None);

                        try
                        {
                            DefaultFrontColor = Color.FromName(lines[0]);
                        }
                        catch (Exception ex) { return; }

                        try
                        {
                            DefaultBackColor = Color.FromName(lines[1]);
                        }
                        catch (Exception ex) { return; }

                        PlayersInfo = new List<PlayerInfo>();

                        for (int i = 2; i < lines.Length; i++)
                        {
                            string[] playeroptions = lines[i].Split(new string[] { "," }, StringSplitOptions.None);

                            try
                            {
                                PlayerInfo pi = new PlayerInfo();
                                pi.Trigger = (Keys)Enum.Parse(typeof(Keys), playeroptions[0].Substring(1, playeroptions[0].Length - 2), true);
                                pi.Name = playeroptions[1].Substring(1, playeroptions[1].Length - 2);
                                pi.Sound = playeroptions[2].Substring(1, playeroptions[2].Length - 2);
                                pi.FrontColor = Color.FromName(playeroptions[3].Substring(1, playeroptions[3].Length - 2));
                                pi.BackColor = Color.FromName(playeroptions[4].Substring(1, playeroptions[4].Length - 2));
                                PlayersInfo.Add(pi);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }

            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(DefaultBackColor), 0, 0, panel1.Width, panel1.Height);
            
            if (GameState_Guess)
            {
                e.Graphics.FillRectangle(new SolidBrush(GuessingPlayer.BackColor), 0, 0, panel1.Width, panel1.Height);

                Font font1 = new Font(FontFamily.GenericSansSerif, 80);
                Font font2 = new Font(FontFamily.GenericSansSerif, 15);

                SizeF textsize = e.Graphics.MeasureString(GuessingPlayer.Name, font1);
                SizeF textsize2 = e.Graphics.MeasureString("Press Enter to continue", font2);

                e.Graphics.DrawString(GuessingPlayer.Name, font1, new SolidBrush(DefaultFrontColor), 
                    panel1.Width/2-textsize.Width/2, panel1.Height / 2 - textsize.Height / 2);
                e.Graphics.DrawString(GuessingPlayer.Name, font1, new SolidBrush(GuessingPlayer.FrontColor),
                    panel1.Width / 2 - textsize.Width / 2, panel1.Height / 2 - textsize.Height / 2);

                e.Graphics.DrawString("Press Enter to continue", font2, new SolidBrush(DefaultFrontColor),
                    panel1.Width / 2 - textsize2.Width / 2, panel1.Height - textsize2.Height * 2);
            }
        }
    }
}
