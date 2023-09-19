using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace calculator
{
    public partial class Form1 : Form
    {
        public struct btnStruct
        {
            public char Content;
            public bool isBold;
            public btnStruct(char c, bool b)
            {
                this.Content = c;
                this.isBold = b;    
            }
        }
        private btnStruct[,] buttons =
        {
            { new btnStruct('%', false), new btnStruct('\u0152', false), new btnStruct('C', false), new btnStruct('\u232b', false)},
            { new btnStruct('\u215f', false),new btnStruct('\u00b2', false),new btnStruct('\u221a', false),new btnStruct('\u00f7', false)},
            { new btnStruct('7', true),new btnStruct('8', true),new btnStruct('9', true),new btnStruct('\u00d7', false)},
            { new btnStruct('4', true),new btnStruct('5', true),new btnStruct('6', true),new btnStruct('-', false)},
            { new btnStruct('1', true),new btnStruct('2', true),new btnStruct('3', true),new btnStruct('+', false)},
            { new btnStruct('\u00b1', true),new btnStruct('0', true),new btnStruct(',', true),new btnStruct('=', false)}
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            makeButtons(buttons.GetLength(0), buttons.GetLength(1));
        }

        private void makeButtons(int rows, int cols)
        {
            int btnWidth = 80;
            int btnHeight = 60;
            int posY = 139;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < cols; i++)
                {
                    Button myButton = new Button();
                    FontStyle fs = buttons[j,i].isBold? FontStyle.Bold : FontStyle.Regular;
                    myButton.BackColor = buttons[j,i].isBold? Color.White : Color.Transparent;
                    myButton.Font = new Font("Segoe UI", 16,fs);
                    myButton.Width=btnWidth;
                    myButton.Height=btnHeight;
                    myButton.Text = buttons[j,i].Content.ToString();
                    myButton.Top = posY;
                    myButton.Left = myButton.Width * i;
                    this.Controls.Add(myButton);
                    
                }
                posY += btnHeight;
            }
        }
    }
}
