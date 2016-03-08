using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace _5InSquare
{
    public partial class Form1 : Form
    {
        const int SIZE = 5;
        public Form1()
        {
            InitializeComponent();
            Solver.whenPaused += Solver_whenPaused;
        }

        void Solver_whenPaused()
        {
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    board1.setSlot(i, j, Solver.board[i, j].Num, Solver.board[i, j].Style);
                }
            }
        }

        void Solver_whenPlaced(int arg1, int arg2, int arg3, int arg4)
        {
            board1.setSlot(arg1, arg2, arg3, arg4);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Solver.NextSolve();
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try
                {
                    Solver.whenPlaced += Solver_whenPlaced;
                }
                catch { }
            }
            else
            {
                try
                {
                    Solver.whenPlaced -= Solver_whenPlaced;
                }
                catch { }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Solver.DelayTime = (int)numericUpDown1.Value;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Solver.Reset();
        }
    }
}
