using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Robot
{
    public enum Directions
    {
        left,
        rigth,
        up,
        down,
        none
    }
    public class Labyrinth
    {
        public int[,] labyrinth;
        private HashSet<Directions>[,] directions;

        readonly public int r = 10;
        readonly public int c = 10;
        bool treasureFind = false;
        public delegate void RobotlocationHandler(int oldi, int oldj, int i, int j);
        public event RobotlocationHandler OnRobotlocation;

        public Labyrinth()
        {
            labyrinth = new int[r, c];
            directions = new HashSet<Directions>[r, c];
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    directions[i, j] = new HashSet<Directions>();
                }
            }
            var rand = new Random();
            //Рандомный лабиринт
            for (int i = 0; i < r; i++)
            {
                for (int j = 1; j < c; j++)
                {
                    labyrinth[i, j] = rand.Next(2); //Стена или дорога
                }
            }
            PutTreasure();
        }

        private void PutTreasure()
        {
            int i, j = 0;
            var rand = new Random();
            do
            {
                i = rand.Next(10);
                j = rand.Next(10);
                if (labyrinth[i, j] != 1)
                {
                    labyrinth[i, j] = 5; // treasure
                }
            } while (labyrinth[i, j] != 5);
        }

        public void LoadLabyrinth()
        {
            string path = "Labyrinth.txt";

            using (StreamReader reader = new StreamReader(path))
            {
                int cr = 0;
                for (int i = 0; i < r; i++)
                {
                    for (int j = 0; j < c; j++)
                    {
                        cr = reader.Read();
                        if ((cr == 13) || (cr == 10))
                        {
                            j--;
                            continue;
                        }
                        labyrinth[i, j] = int.Parse(((char)cr).ToString());
                        if (reader.Peek() == -1) break;
                    }
                }
            }
        }

        public void StartFindTreasure()
        {
            treasureFind = false;
            FindTreasureRecursionClassic(0, 0, 0, 0, Directions.none);
        }


        private void FindTreasureRecursionClassic(int oldi, int oldj, int i, int j, Directions dir)

        {
            if (labyrinth[i, j] == 5)
            {
                treasureFind = true;
                MessageBox.Show("Клад найден!");
                OnRobotlocation?.Invoke(oldi, oldj, i, j);
                return;
            }
            OnRobotlocation?.Invoke(oldi, oldj, i, j); //Прямой ход рекурсии

            if (j != c - 1)
                if (dir != Directions.left)
                    if (labyrinth[i, j + 1] != 1)
                    {
                        FindTreasureRecursionClassic(i, j, i, j + 1, Directions.rigth); //вправо
                        OnRobotlocation?.Invoke(i, j + 1, i, j);  //Обратный ход рекурсии до перекрестка. i,j стековые переменные
                        if (treasureFind) return;
                    }
            if (i != r - 1)
                if (dir != Directions.up)
                    if (labyrinth[i + 1, j] != 1)
                    {
                        FindTreasureRecursionClassic(i, j, i + 1, j, Directions.down); //Вниз
                        OnRobotlocation?.Invoke(i + 1, j, i, j);
                        if (treasureFind) return;
                    }
            if (j != 0)
                if (dir != Directions.rigth)
                    if (labyrinth[i, j - 1] != 1)
                    {
                        FindTreasureRecursionClassic(i, j, i, j - 1, Directions.left); //влево
                        OnRobotlocation?.Invoke(i, j - 1, i, j);
                        if (treasureFind) return;
                    }
            if (i != 0)
                if (dir != Directions.down)
                    if (labyrinth[i - 1, j] != 1)
                    {
                        FindTreasureRecursionClassic(i, j, i - 1, j, Directions.up); //Вверх
                        OnRobotlocation?.Invoke(i - 1, j, i, j);
                        if (treasureFind) return;
                    }
        }

    }
}
