using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Robot
{
    public partial class Form1 : Form
    {
        Labyrinth labyrinth;
        List<Thread> robotThreds = new List<Thread>();
        public Form1()
        {
            InitializeComponent();
            labyrinth = new Labyrinth();
            labyrinth.OnRobotlocation += Labyrinth_OnRobotlocation;
        }

        private void Labyrinth_OnRobotlocation(int oldi, int oldj, int i, int j)
        {
            PaintMove(oldi, oldj, i, j);            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread findTreasure = new Thread(labyrinth.StartFindTreasure);
            robotThreds.Add(findTreasure);
            findTreasure.Start();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            PaintLabirynth(e.Graphics);
        }

        private void PaintMove(int oldi, int oldj, int i, int j)
        {
            Graphics g = panel1.CreateGraphics();
            Image imageRoad = new Bitmap(Properties.Resources.Road);
            Image imageRobot = new Bitmap(Properties.Resources.Robot);
            int y = oldi * 55;
            int x = oldj * 55;
            g.DrawImage(imageRobot, x, y);
            if (oldi < i)
            {
                while (y != i * 55) //Вниз
                {
                    y++;
                    g.DrawImage(imageRobot, x, y);
                    g.DrawImage(imageRoad, x, y - 1, 55, 1);
                    Thread.Sleep(1);
                }
            }
            if (oldi > i)
            {
                while (y != i * 55)//Вверх
                {
                    y--;
                    g.DrawImage(imageRobot, x, y);
                    g.DrawImage(imageRoad, x, y + 55, 55, 1);
                    Thread.Sleep(1);
                }
            }
            if (oldj < j)
            {
                while (x != j * 55) //Вправо
                {
                    x++;
                    g.DrawImage(imageRobot, x, y);
                    g.DrawImage(imageRoad, x - 1, y, 1, 55);
                    Thread.Sleep(1);
                }
            }
            if (oldj > j)
            {
                while (x != j * 55) //Влево
                {
                    x--;
                    g.DrawImage(imageRobot, x, y);
                    g.DrawImage(imageRoad, x + 55, y, 1, 55);
                    Thread.Sleep(1);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var item in robotThreds)
            {
                item.Abort();
            }
        }

        private void LoadLabyrinth_Click(object sender, EventArgs e)
        {
            foreach (var item in robotThreds)
            {
                item.Abort();
            }
            labyrinth.LoadLabyrinth();
            Graphics g = panel1.CreateGraphics();
            PaintLabirynth(g);
        }

        private void PaintLabirynth(Graphics g)
        {
            for (int i = 0; i < labyrinth.r; i++)
            {
                for (int j = 0; j < labyrinth.c; j++)
                {
                    if (labyrinth.labyrinth[i, j] == 1)
                    {
                        g.DrawImage(Properties.Resources.Wall, j * 55, i * 55);
                    }
                    else
                    {
                        g.DrawImage(Properties.Resources.Road, j * 55, i * 55);
                    }

                    if (labyrinth.labyrinth[i, j] == 5)
                    {
                        g.DrawImage(Properties.Resources.Treasure, j * 55, i * 55);
                    }
                }
            }
        }

    }
}
