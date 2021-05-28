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
        public int LineNumber = 1;
        public int NoOfLexeme = 1;
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
                if (token == null) 
                {
                    LineNumber++;
                    NoOfLexeme = 1;
                    continue; 
                }
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;
                textBox1.Text += $"{token.Kind}: '{token.Text}' #Lexem No Line: {NoOfLexeme++} in Line Number: {LineNumber}";
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
                // Condition if the token return null (Pressing Enter Key to enter new line)
                if (token == null) 
                {
                    LineNumber++;
                    NoOfLexeme = 1;
                    continue; 
                }

                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;
                textBox1.Text += $"{token.Kind}: '{token.Text}' #Lexems{NoOfLexeme++} in Line Number {LineNumber}";
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
        SInteger,
        String,
        Float,
        SFloat,
        Condition,
        Void,
        Loop,
        Return,
        Break,
        Struct,
        LogicOperators,
        RelationalOperators,
        AssignmentOperator,
        AccessOperator,
        QuotationMark,
        Inclusion,
        Comment,
        TokenDelimiter,
        LineDelimiter,
        Start,
        End,
        Character,
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
        private const char CR = '\r';
        private const char LF = '\n';
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

            // Condition for new line (Pressing Enter Key)
            if (Current == LF || Current == CR) {
                Next();
                return null; 
            }

            if (isWhiteSpace(Current))
            {
                var start = _position;
                while (isWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                //int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (isDigit(Current))
            {
                var start = _position;
                while (isDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                //int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.Constant, start, text, null);
            }



            if (isTokenDelimiter(Current) == 1)
                return new SyntaxToken(SyntaxKind.TokenDelimiter, _position++, "$", null);
            else if (isTokenDelimiter(Current) == 2)
                return new SyntaxToken(SyntaxKind.LineDelimiter, _position++, ".", null);


            else if (isArithmeticOperation(Current))
                return new SyntaxToken(SyntaxKind.ArithmeticOperation, _position++, _text.Substring(_position - 1, 1), null);

            else if (isBraces(Current))
                return new SyntaxToken(SyntaxKind.Braces, _position++, _text.Substring(_position - 1, 1), null);


            if (isKeyWord(Current, i, "Yesif-Otherwise"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "Yesif-Otherwise"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Yesif-Otherwise")
                    return new SyntaxToken(SyntaxKind.Condition, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Omw"))
            {
                var start = _position;
                while (isKeyWord(Current, i, "Omw"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if(text == "Omw")
                    return new SyntaxToken(SyntaxKind.Integer, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "SIMww"))
            {
                var start = _position;
                while (isKeyWord(Current, i, "SIMww"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "SIMww")
                    return new SyntaxToken(SyntaxKind.SInteger, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Chji"))
            {
                var start = _position;
                while (isKeyWord(Current, i, "Chji"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Chji")
                    return new SyntaxToken(SyntaxKind.Character, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Seriestl"))
            {
                var start = _position;
                while (isKeyWord(Current, i, "Seriestl"))
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

            if (isKeyWord(Current, i, "IMwf"))
            {
                var start = _position;
                while (isKeyWord(Current, i, "IMwf"))
                {
                    Next();
                    i++;
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "IMwf")
                    return new SyntaxToken(SyntaxKind.Float, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }

            }

            if (isKeyWord(Current, i, "SIMwf"))
            {
                
                var start = _position;
                while (isKeyWord(Current, i, "SIMwf"))
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

            if (isKeyWord(Current, i, "NOReturn"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "NOReturn"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "NOReturn")
                    return new SyntaxToken(SyntaxKind.Void, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "RepeatWhen"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "RepeatWhen"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "RepeatWhen")
                    return new SyntaxToken(SyntaxKind.Loop, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Reiterate"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "Reiterate"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Reiterate")
                    return new SyntaxToken(SyntaxKind.Loop, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "GetBack"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "GetBack"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "GetBack")
                    return new SyntaxToken(SyntaxKind.Return, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "OutLoop"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "OutLoop"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "OutLoop")
                    return new SyntaxToken(SyntaxKind.Break, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Loli"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "Loli"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Loli")
                    return new SyntaxToken(SyntaxKind.Struct, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "&&"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "&&"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "&&")
                    return new SyntaxToken(SyntaxKind.LogicOperators, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "||"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "||"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "||")
                    return new SyntaxToken(SyntaxKind.LogicOperators, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "->"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "->"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "->")
                    return new SyntaxToken(SyntaxKind.AccessOperator, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Start"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "Start"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Start")
                    return new SyntaxToken(SyntaxKind.Start, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }

            if (isKeyWord(Current, i, "Last"))
            {

                var start = _position;
                while (isKeyWord(Current, i, "Last"))
                {
                    Next();
                    i++;
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (text == "Last")
                    return new SyntaxToken(SyntaxKind.End, start, text, null);
                else
                {
                    _position = start;
                    i = 0;
                }
            }


            if(isKeyWord(Current, i, "="))
            {
                Next();
                if (Current == '=')
                {
                    string matchRlationalOp = "=" + Current;
                    if (isRelationalOp(matchRlationalOp))
                        return new SyntaxToken(SyntaxKind.RelationalOperators, _position++, matchRlationalOp, null);
                }
                else _position--;
            }

            if (isKeyWord(Current, i, "!"))
            {
                Next();
                if (Current == '=')
                {
                    string matchRlationalOp = "!" + Current;
                    if (isRelationalOp(matchRlationalOp))
                        return new SyntaxToken(SyntaxKind.RelationalOperators, _position++, matchRlationalOp, null);
                }
                else _position--;
            }

            if (isKeyWord(Current, i, "<"))
            {
                Next();
                if (Current == '=')
                {
                    string matchRlationalOp = "<" + Current;
                    if (isRelationalOp(matchRlationalOp))
                        return new SyntaxToken(SyntaxKind.RelationalOperators, _position++, matchRlationalOp, null);
                }
                else _position--;
            }

            if (isKeyWord(Current, i, ">"))
            {
                Next();
                if (Current == '=')
                {
                    string matchRlationalOp = ">" + Current;
                    if (isRelationalOp(matchRlationalOp))
                        return new SyntaxToken(SyntaxKind.RelationalOperators, _position++, matchRlationalOp, null);
                }
                else _position--;
            }

            if (isKeyWord(Current, i, "~"))
                return new SyntaxToken(SyntaxKind.LogicOperators, _position++, "~", null);

            if (isKeyWord(Current, i, "="))
            {
                return new SyntaxToken(SyntaxKind.AssignmentOperator, _position++, "=", null);
            }
            if(isRelationalOp(Char.ToString(Current)))
                return new SyntaxToken(SyntaxKind.RelationalOperators, _position++, _text.Substring(_position - 1, 1), null);

            if (isQuotationMark(Char.ToString(Current)))
                return new SyntaxToken(SyntaxKind.QuotationMark, _position++, _text.Substring(_position - 1, 1), null);

            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
        


        private static bool isDigit(char text)
        {
            Regex regex = new Regex(@"^\d$");
            string test = Char.ToString(text);
            if (regex.IsMatch(test)) return true;
            return false;
        }
        private static int isTokenDelimiter(char text)
        {
            if (text == '$') return 1;
            else if (text == '.') return 2;
            return 0;
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

        private static bool isRelationalOp(string text)
        {
            Regex regex = new Regex(@"(?:<=?|>=?|==|!=)");
            if (regex.IsMatch(text)) return true;
            return false;
        }

        private static bool isQuotationMark(string text)
        {
            Regex regex = new Regex(@"(""|')");
            if (regex.IsMatch(text)) return true;
            return false;
        }
        private static bool isKeyWord(char character, int location, string word)
        {
            string str = word;
            if (location < str.Length && character == str[location]) return true;
            return false;
        }
    }
}
