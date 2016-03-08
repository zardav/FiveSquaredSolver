using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _5InSquare
{
    public partial class Board : UserControl
    {
        Dictionary<int, Color> colorDict = new Dictionary<int, Color>();
        Button[,] slots = new Button[SIZE, SIZE];
        const int SIZE = 5;
        public Board()
        {
            InitializeComponent();
            
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    slots[i, j] = new Button();
                    slots[i, j].Size = new Size(61, 61);
                    slots[i, j].Location = new Point(i * 60, j * 60);
                    slots[i, j].ImageAlign = ContentAlignment.MiddleCenter;
                    slots[i, j].Text = "";
                    slots[i, j].FlatStyle = FlatStyle.Flat;
                    slots[i, j].FlatAppearance.BorderSize = 1;
                    this.Controls.Add(slots[i, j]);
                }
            }
            colorDict.Add(-1, slots[0, 0].BackColor);
            colorDict.Add(0, Color.Green);
            colorDict.Add(1, Color.LawnGreen);
            colorDict.Add(2, Color.Blue);
            colorDict.Add(3, Color.DarkCyan);
            colorDict.Add(4, Color.DeepPink);
            colorDict.Add(5, Color.Red);
            colorDict.Add(6, Color.Brown);
            colorDict.Add(7, Color.DarkSeaGreen);
            colorDict.Add(8, Color.Orange);
        }
        public void setSlot(int i, int j, int value, int color1)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(()=>setSlot(i, j, value, color1)));
                return;
            }
            switch (value)
            {
                case 0:
                    slots[i, j].Image = null;
                    break;
                case 1:
                    slots[i, j].Image = Properties.Resources._1;
                    break;
                case 2:
                    slots[i, j].Image = Properties.Resources._2;
                    break;
                case 3:
                    slots[i, j].Image = Properties.Resources._3;
                    break;
                case 4:
                    slots[i, j].Image = Properties.Resources._4;
                    break;
                case 5:
                    slots[i, j].Image = Properties.Resources._5;
                    break;
                default:
                    break;
            }
            slots[i, j].BackColor = colorDict[color1];
            slots[i, j].Refresh();
            slots[i, j].Update();
        }
    }
}
