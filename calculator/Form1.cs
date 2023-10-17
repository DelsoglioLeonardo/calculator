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
            SpecialOperator,
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
            { new btnStruct('%', symbolType.SpecialOperator), new btnStruct('\u0152',symbolType.ClearEntry), new btnStruct('C',symbolType.ClearAll), new btnStruct('\u232b',symbolType.BackSpace)},
            { new btnStruct('\u215f',symbolType.SpecialOperator),new btnStruct('\u00b2',symbolType.SpecialOperator),new btnStruct('\u221a',symbolType.SpecialOperator),new btnStruct('\u00f7',symbolType.Operator)},
            { new btnStruct('7',symbolType.Number,true),new btnStruct('8', symbolType.Number,true),new btnStruct('9', symbolType.Number, true),new btnStruct('\u00d7',symbolType.Operator)},
            { new btnStruct('4', symbolType.Number,true),new btnStruct('5', symbolType.Number,true),new btnStruct('6', symbolType.Number, true),new btnStruct('-', symbolType.Operator)},
            { new btnStruct('1', symbolType.Number,true),new btnStruct('2', symbolType.Number, true),new btnStruct('3', symbolType.Number, true),new btnStruct('+', symbolType.Operator)},
            { new btnStruct('\u00b1',symbolType.PlusMinusSign),new btnStruct('0', symbolType.Number,true),new btnStruct(',', symbolType.DecimalPoint,true),new btnStruct('=', symbolType.Operator)}
        };

        float lblResultBaseFontSize;
        const int lblResultWidthMargin = 24;
        const int lblResultMaxDigit = 25;

        char lastOperator = ' ';
        decimal operand1,operand2,result;
        btnStruct lastButtonClicked;

        public Form1()
        {
            InitializeComponent();
            lblResultBaseFontSize = lblResult.Font.Size;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            makeButtons(buttons.GetLength(0), buttons.GetLength(1));
            lblOperazioneInCorso.Text = "";
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
            
            switch (clickedButtonStruct.type)
            {
                case symbolType.Number:
                    if (lblResult.Text == "0"  || lastButtonClicked.type==symbolType.Operator) lblResult.Text = "";
                    lblResult.Text += clickedButton.Text;
                    break;
                case symbolType.SpecialOperator:
                    ManageSpecialOperator(clickedButtonStruct);
                    break;
                case symbolType.Operator:
                    if (lastButtonClicked.type!=symbolType.Operator || lastButtonClicked.Content!='=')
                    {
                        ManageOperator(clickedButtonStruct);
                    }
                    else
                    {
                        lastOperator = clickedButtonStruct.Content;
                    }
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
                    if (lastButtonClicked.type == symbolType.Operator)
                    {
                        operand1 = -operand1;
                    }
                    break;
                case symbolType.BackSpace:
                if(lastButtonClicked.type!=symbolType.Operator && lastButtonClicked.type!=symbolType.SpecialOperator)
                  {
                    lblResult.Text = lblResult.Text.Substring(0, lblResult.Text.Length - 1);
                    if (lblResult.Text.Length == 0 || lblResult.Text == "-0")
                        lblResult.Text = "0"; 
                    }
                    break;
                case symbolType.ClearAll:
                    clearAllFunction();
                    break;
                case symbolType.Undefined:
                    break;
                case symbolType.ClearEntry:
                    if (lastButtonClicked.Content == '=')
                        clearAllFunction();
                    else
                        lblResult.Text = "0";
                    break;

                default:
                    break;
            }
            if(clickedButtonStruct.type!=symbolType.BackSpace  && clickedButtonStruct.type!=symbolType.PlusMinusSign)
            lastButtonClicked = clickedButtonStruct;
        }

        private void clearAllFunction()
        {
            lblResult.Text = "0";
            operand1 = 0;
            operand2 = 0;
            result = 0;
            lastOperator = ' ';
            lblOperazioneInCorso.Text = "";
        }
        private void ManageSpecialOperator(btnStruct clickedButtonStruct)
        {
                operand2 = decimal.Parse(lblResult.Text);
                decimal aus = operand2;
                switch (clickedButtonStruct.Content)
                {
                    case '%':
                        result = operand1 * operand2 / 100;
                        break;
                    case '\u215F':// 1/x
                        lblOperazioneInCorso.Text = aus +lastOperator.ToString()+ "1/(" + operand2 + ")";
                        result = 1 / operand2;
                        break;
                    case '\u00b2':// x^2
                        result = operand2*operand2;
                        break;
                    case '\u221a':// sqr(x)
                    if (lblOperazioneInCorso.Text == "")
                        lblOperazioneInCorso.Text = "sqrt(" + operand2.ToString() + ")";
                    else
                        lblOperazioneInCorso.Text="sqrt("+lblOperazioneInCorso.Text + ")";
                        result = (decimal)Math.Sqrt((double)operand2);
                        break;
                    default:
                        break;
                }
                lblResult.Text = result.ToString();
        }

        private void ManageOperator(btnStruct clickedButtonStruct)
        {
            if(lastOperator== ' ')
            {
                operand1 = decimal.Parse(lblResult.Text);
                if(clickedButtonStruct.Content!='=')lastOperator = clickedButtonStruct.Content;
                lblOperazioneInCorso.Text = operand1.ToString()+" "+lastOperator.ToString();
            }
            else
            {
                if(lastButtonClicked.Content!= '=') operand2 = decimal.Parse(lblResult.Text);
                lblOperazioneInCorso.Text += operand2.ToString() + " =";
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
                if (clickedButtonStruct.Content != '=')
                {
                    lastOperator = clickedButtonStruct.Content;
                    if (lastButtonClicked.Content == '=')
                        operand2 = 0;
                }
                lblResult.Text = result.ToString();
            }
        }
        bool lavorando = false;
        private void lblResult_TextChanged(object sender, EventArgs e)
        {
            if (!lavorando)
            {
                if (lblResult.Text.Length > 0)
                {
                    double num = double.Parse(lblResult.Text); string stOut = "";
                    NumberFormatInfo nfi = new CultureInfo("it-IT", false).NumberFormat;
                    int decimalSeparatorPosition = lblResult.Text.IndexOf(",");
                    nfi.NumberDecimalDigits = decimalSeparatorPosition == -1 ? 0
                        : lblResult.Text.Length - decimalSeparatorPosition - 1;
                    stOut = num.ToString("N", nfi);
                    char ultimoCarattereSt = stOut[stOut.Length - 1];
                    char ultimoCarattereLbl = lblResult.Text.Length > 0 ? lblResult.Text[lblResult.Text.Length - 1] : '\0';
                    if (ultimoCarattereSt.ToString() != ultimoCarattereLbl.ToString())
                    {
                        lavorando = true;
                        int lunghezza = lblResult.Text.Length;
                        int posVirgola=-999;
                        string copia="";
                        int i = 0;
                        while(i!=lunghezza)
                        {
                             if (lblResult.Text[i] !='.')
                                 copia += lblResult.Text[i];
                             if(lblResult.Text[i]==',')
                                 posVirgola = i;
                             i++;
                        }
                        int lunghezzaCopia = copia.Length;
                        if (lunghezzaCopia % 3==0)
                        {
                            if (posVirgola == -999)
                            {
                                lblResult.Text = "";
                                i = 0;
                                while(i!= copia.Length)
                                {
                                    lblResult.Text += copia[i];
                                    i++;
                                    if(i%3==0 && i != 0)
                                    {
                                        lblResult.Text += ".";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (posVirgola == -999)
                            {
                                int diff = lunghezzaCopia % 3;
                                lblResult.Text = "";
                                i = 0;
                                while (i != copia.Length)
                                {
                                    lblResult.Text += copia[i];
                                    i++;
                                    if(i==diff)
                                    {
                                        lblResult.Text += ".";
                                    }
                                    else if ((i-diff) % 3 == 0 && i != 0 && i!=copia.Length)
                                    {
                                        lblResult.Text += ".";
                                    }
                                }
                            }
                        }
                        lavorando = false;
                    }
                    else
                    {
                        if (lblResult.Text.IndexOf(",") == lblResult.Text.Length - 1) stOut += ",";
                        lblResult.Text = stOut;
                    }
                }
                if (lblResult.Text.Length > lblResultMaxDigit)
                    lblResult.Text = lblResult.Text.Substring(0, lblResultMaxDigit);

                int textWidth = TextRenderer.MeasureText(lblResult.Text, lblResult.Font).Width;
                float newSize = lblResult.Font.Size * (((float)lblResult.Size.Width - lblResultWidthMargin) / textWidth);
                if (newSize > lblResultBaseFontSize) newSize = lblResultBaseFontSize;
                lblResult.Font = new Font("Segoe UI", newSize, FontStyle.Regular);
            }
        }

      
    }
}
