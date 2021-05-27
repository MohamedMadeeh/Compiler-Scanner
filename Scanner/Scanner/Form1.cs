using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Scanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string code;
        private void button2_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open a file";
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Clear();
                using (StreamReader sr = new StreamReader(openFile.FileName))
                {
                    code = sr.ReadToEnd();
                    sr.Close();
                }
            }
            var lexer = new Lexer(code);
            while (true)
            {
                var token = lexer.NextToken();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;
                textBox1.Text += $"{token.Kind}: '{token.Text}'";
                if (token.Value != null)
                    textBox1.Text += $"{token.Value}";
                textBox1.Text += Environment.NewLine;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            code = richTextBox1.Text;
            var lexer = new Lexer(code);
            while (true)
            {
                var token = lexer.NextToken();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;
                textBox1.Text += $"{token.Kind}: '{token.Text}'";
                if (token.Value != null)
                    textBox1.Text += $"{token.Value}";
                textBox1.Text += Environment.NewLine;
            }
        }
    }

    enum SyntaxKind
    {
        Constant,
        WhiteSpaceToken,
        ArithmeticOperation,
        Braces,
        Integer,
        String,
        SFloat,
        EndOfFileToken,
        BadToken
    }

    class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }
        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }
        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';
                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }
        public SyntaxToken NextToken()
        {

            int i = 0;
            if(_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }
                
            if(isInteger(Current, i))
            {
                var start = _position;
                while(isInteger(Current, i))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.Integer, start, text, null);
            }
            
            if(isDigit(Current))
            {
                var start = _position;
                while (isDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                //int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.Constant, start, text, null);
            }

            

            if(isWhiteSpace(Current))
            {
                var start = _position;
                while (isWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }
            else if (isArithmeticOperation(Current))
                return new SyntaxToken(SyntaxKind.ArithmeticOperation, _position++, _text.Substring(_position - 1, 1), null);
            
            else if (isBraces(Current))
                return new SyntaxToken(SyntaxKind.Braces, _position++, _text.Substring(_position - 1, 1), null);
            if (isString(Current, i))
            {
                var start = _position;
                while (isString(Current, i))
                {
                    Next();
                    i++;
                }
                
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Seriestl")
                    return new SyntaxToken(SyntaxKind.String, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
                
            }
            if (isSSFloat(Current, i))
            {
                
                var start = _position;
                while (isSSFloat(Current, i))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if(text == "SIMwf")
                    return new SyntaxToken(SyntaxKind.SFloat, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }

        
        private static bool isDigit(char text)
        {
            Regex regex = new Regex(@"^\d$");
            string test = Char.ToString(text);
            if (regex.IsMatch(test)) return true;
            return false;
        }
        private static bool isWhiteSpace(char text)
        {
            if (text == ' ') return true;
            return false;
        }
        private static bool isArithmeticOperation(char text)
        {
            Regex regex = new Regex(@"[+/-/*\/]");
            string test = Char.ToString(text);
            if (regex.IsMatch(test)) return true;
            return false;
        }
        private static bool isBraces(char text)
        {
            //Regex regex = new Regex(@"(\{.*?\}|\[.*?\]|\(.*?\))");
            Regex regex = new Regex(@"(\{|\}|\[|\]|\(|\))");
            string test = Char.ToString(text);
            if (regex.IsMatch(test)) return true;
            return false;
        }
        

        private static bool isInteger(char text, int location)
        {
            string str = "omw";
            if (location < str.Length && text == str[location]) return true;
            return false;
        }


        private static bool isString(char text, int location)
        {
            string str = "Seriestl";
            if (location < str.Length && text == str[location]) return true;
            return false;
        }
        private static bool isSSFloat(char text, int location)
        {
            string str = "SIMwf";
            if (location < str.Length && text == str[location]) return true;
            return false;
        }
    }
}
