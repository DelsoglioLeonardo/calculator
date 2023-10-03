using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static calculator.Form1;

namespace calculator
{
    public partial class Form1 : Form
    {
        public enum symbolType
        {
            Number,
            Operator,
            DecimalPoint,
            PlusMinusSign,
            BackSpace,
            ClearAll,
            ClearEntry,
            Undefined
        }
        public struct btnStruct
        {
            public char Content;
            public symbolType type;
            public bool isBold;
            public btnStruct(char c, symbolType t= symbolType.Undefined, bool b=false)
            {
                this.Content = c;
                this.type = t;
                this.isBold = b;    
            }
        }
        private btnStruct[,] buttons =
        {
            { new btnStruct('%'), new btnStruct('\u0152',symbolType.ClearEntry), new btnStruct('C',symbolType.ClearAll), new btnStruct('\u232b',symbolType.BackSpace)},
            { new btnStruct('\u215f'),new btnStruct('\u00b2'),new btnStruct('\u221a'),new btnStruct('\u00f7',symbolType.Operator)},
            { new btnStruct('7',symbolType.Number,true),new btnStruct('8', symbolType.Number,true),new btnStruct('9', symbolType.Number, true),new btnStruct('\u00d7',symbolType.Operator)},
            { new btnStruct('4', symbolType.Number,true),new btnStruct('5', symbolType.Number,true),new btnStruct('6', symbolType.Number, true),new btnStruct('-', symbolType.Operator)},
            { new btnStruct('1', symbolType.Number,true),new btnStruct('2', symbolType.Number, true),new btnStruct('3', symbolType.Number, true),new btnStruct('+', symbolType.Operator)},
            { new btnStruct('\u00b1',symbolType.PlusMinusSign),new btnStruct('0', symbolType.Number,true),new btnStruct(',', symbolType.DecimalPoint,true),new btnStruct('=', symbolType.Operator)}
        };

        float lblResultBaseFormSize;
        const int lblResultWidthMargin = 24;
        const int lblResultMaxDigit = 25;

        char lastOperator = ' ';
        decimal operand1,operand2,result;
        btnStruct lastButtonClicked;

        public Form1()
        {
            InitializeComponent();
            lblResultBaseFormSize = lblResult.Font.Size;
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
                    myButton.Tag = buttons[j,i];
                    myButton.Click += MyButton_Click;

                }
                posY += btnHeight;
            }
        }
        private void MyButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            btnStruct clickedButtonStruct = (btnStruct)clickedButton.Tag;
            if (clickedButtonStruct.type == symbolType.Number)
            {

            }
            switch (clickedButtonStruct.type)
            {
                case symbolType.Number:
                    if (lblResult.Text == "0"  || lastButtonClicked.type==symbolType.Operator) lblResult.Text = "";
                    lblResult.Text += clickedButton.Text;
                    break;
                case symbolType.Operator:
                    ManageOperator(clickedButtonStruct);
                    break;
                case symbolType.DecimalPoint:
                    if (lblResult.Text.IndexOf(",") == -1)
                        lblResult.Text += clickedButton.Text;
                    break;
                case symbolType.PlusMinusSign:
                    if (lblResult.Text != "0")
                        if (lblResult.Text.IndexOf("-") == -1)
                            lblResult.Text = "-" + lblResult.Text;
                        else
                            lblResult.Text = lblResult.Text.Substring(1);
                    break;
                case symbolType.BackSpace:
                    lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);
                    if (lblResult.Text.Length == 0 || lblResult.Text == "-0")
                        lblResult.Text = "0";
                    break;
                case symbolType.ClearAll:
                    lblResult.Text = "0";
                    break;
                case symbolType.Undefined:
                    break;
                default:
                    break;
            }
            lastButtonClicked = clickedButtonStruct;
        }

        private void ManageOperator(btnStruct clickedButtonStruct)
        {
            if(lastOperator== ' ')
            {
                operand1 = decimal.Parse(lblResult.Text);
                lastOperator = clickedButtonStruct.Content;
            }
            else
            {
                operand2 = decimal.Parse(lblResult.Text);
                switch (lastOperator)
                {
                    case '+':
                        result = operand1 + operand2;
                        break;
                    case '-':
                        result = operand1 - operand2;
                        break;
                    case '\u00d7':
                        result = operand1 * operand2;
                        break;
                    case '\u00F7':
                        result = operand1 / operand2;
                        break;
                    default:
                        break;
                }
                operand1 = result;
                lastOperator = clickedButtonStruct.Content;
                lblResult.Text = result.ToString();
            }
        }

        private void lblResult_TextChanged(object sender, EventArgs e)
        {
            if(lblResult.Text=="-")
            {
                lblResult.Text = "0";
                return;
            }
            if (lblResult.Text.Length > 0)
            {
                double num = double.Parse(lblResult.Text);String stOut = "";
                NumberFormatInfo nfi = new CultureInfo("it-IT", false).NumberFormat;
                int decimalSeparatorPosition = lblResult.Text.IndexOf(",");
                nfi.NumberDecimalDigits = decimalSeparatorPosition == -1 ? 0 : lblResult.Text.Length-decimalSeparatorPosition-1;
                stOut = num.ToString("N",nfi);
                if (lblResult.Text.IndexOf(",") == lblResult.Text.Length - 1) stOut += ",";
                lblResult.Text = stOut;
                
            }
            if (lblResult.Text.Length > lblResultMaxDigit) lblResult.Text = lblResult.Text.Substring(0, lblResultMaxDigit);

            int textWidth = TextRenderer.MeasureText(lblResult.Text, lblResult.Font).Width;
            float newSize = lblResult.Font.Size * (((float)lblResult.Size.Width-lblResultWidthMargin)/textWidth);
            if (newSize > lblResultBaseFormSize) newSize = lblResultBaseFormSize;
            lblResult.Font = new Font("Segoe UI", newSize, FontStyle.Regular);
           
        }
    }
}
