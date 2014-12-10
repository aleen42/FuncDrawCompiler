using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;


namespace FuncDraw_WPF_
{
    public static class Program
    {
        public static class scanner
        {
            /*static void Main(string[] args)
            {
                Token token;
                string file_path = @"F:\School\Programing\C# Programing\FuncDraw\myfile.txt";
                scanner.InitScanner(file_path);
                scanner.SourceFileInit();
                scanner.InitTokenTab();
                Console.WriteLine("记号类别     字符串     常数值	   函数指针");
                Console.WriteLine("_______________________________________________________");
                while (true)
                {
                    token = scanner.GetToken();
                    if (token.type != Token_Type.NONTOKEN)
                        Console.WriteLine("{0}  {1} {2} {3}", token.type.ToString().PadRight(11, ' '), token.lexeme.ToString().PadRight(10, ' '), token.value.ToString().PadRight(10, ' '), token.FuncPtr.Method.Name.PadRight(20, ' '));
                    else
                        break;
                }
                Console.WriteLine("_______________________________________________________");
                scanner.CloseScanner();
                return;
            }*/
            //------------------------------------------------------scannerhead--------------------------------------------------------
            public enum Token_Type
            {
                ORIGIN, SCALE, ROT, IS,	// 保留字（一字一码）
                TO, STEP, DRAW, FOR, FROM,	// 保留字
                T,						// 参数
                SEMICOLON, L_BRACKET, R_BRACKET, COMMA, // 分隔符
                PLUS, MINUS, MUL, DIV, POWER,			// 运算符
                FUNC,					// 函数（调用）
                CONST_ID,				// 常数
                NONTOKEN,				// 空记号（源程序结束）
                ERRTOKEN				// 出错记号（非法输入）
            }

            public delegate double FuncPtr(double a);// 属性,若记号是函数则是函数委託

            public struct Token	// 记号的数据结构
            {
                public Token_Type type;	//类别
                public string lexeme;		//属性,原始输入的字符串
                public double value;		//属性,若记号是常数则是常数的值
                public FuncPtr FuncPtr;
                public Token(Token_Type type, string lexeme, double value, FuncPtr FuncPtr)
                {
                    this.type = type;
                    this.lexeme = lexeme;
                    this.value = value;
                    this.FuncPtr = FuncPtr;
                }
            };

            public const int MAXSIZE = 10000;          //记号最大類型
            public const int TOKEN_LEN = 100000;          //记号最大长度
            public static List<Token> TokenTab = new List<Token>(MAXSIZE);           //list可以加入結構體,arraylist不能
            public static uint LineNo;						                       //跟踪记号所在源文件行号
            public static uint Line_Count = 0;                                     //记录画的线条数目
            public static int index = 0;                                          //文件index   
            public static FileStream InFile;                                      //输入文件流
            public static ArrayList TokenBuffer = new ArrayList(TOKEN_LEN);       //记号字符缓冲
            public static char[] buffer = new char[TOKEN_LEN];                    //用於緩衝


            //----------------------------------------------------scannerfunction------------------------------------------------------


            public static double NULL(double a) { return -1; }
            //------------------初始化词法分析器
            public static void InitScanner(string FileName)	//初始化词法分析器
            {
                LineNo = 1;
                InFile = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            }


            //---------------初始化TokenTab
            public static void InitTokenTab()
            {
                Token a = new Token(Token_Type.CONST_ID, "PI", 3.1415926, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.CONST_ID, "E", 2.71828, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.T, "T", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.FUNC, "SIN", 0.0, new FuncPtr(System.Math.Sin));
                TokenTab.Add(a);
                a = new Token(Token_Type.FUNC, "COS", 0.0, new FuncPtr(System.Math.Cos));
                TokenTab.Add(a);
                a = new Token(Token_Type.FUNC, "TAN", 0.0, new FuncPtr(System.Math.Tan));
                TokenTab.Add(a);
                a = new Token(Token_Type.FUNC, "LN", 0.0, new FuncPtr(System.Math.Log));
                TokenTab.Add(a);
                a = new Token(Token_Type.FUNC, "EXP", 0.0, new FuncPtr(System.Math.Exp));
                TokenTab.Add(a);
                a = new Token(Token_Type.FUNC, "SQRT", 0.0, new FuncPtr(System.Math.Sqrt));
                TokenTab.Add(a);
                a = new Token(Token_Type.ORIGIN, "ORIGIN", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.SCALE, "SCALE", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.ROT, "ROT", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.IS, "IS", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.FOR, "FOR", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.FROM, "FROM", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.TO, "TO", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.STEP, "STEP", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
                a = new Token(Token_Type.DRAW, "DRAW", 0.0, new FuncPtr(NULL));
                TokenTab.Add(a);
            }



            //------------------关闭词法分析器
            public static void CloseScanner()
            {
                if (InFile != null)
                    InFile.Close();
                scanner.index = 0;
            }



            //------------------从输入源程序中读入一个字符
            public static void SourceFileInit()
            {
                StreamReader srInFile = new StreamReader(InFile);
                srInFile.Read(buffer, 0, TOKEN_LEN);  //讀取所有字符  
            }

            public static char GetChar()
            {
                return buffer[index++];
            }



            //------------------清空记号缓冲区
            public static void EmptyToenString()
            {
                TokenBuffer.Clear();
            }




            //------------------加入字符到记号缓冲区
            /*public static int strlen(char[] p)
            {
                int num=0;
                while(p[num]!='\0')
                    num++;
                return num;
            }*/

            public static void AddCharTokenString(char Char)
            {
                int TokenLength = TokenBuffer.Count;
                if (TokenBuffer.Count == TOKEN_LEN)                        //字符缓冲区滿了则加入失败并返回
                    return;
                TokenBuffer.Add(Char);
            }


            //------------------判断所给的字符串是否在符号表中
            public static Token JudgeKeyToken(string IDstring)
            {
                int loop = 0;

                while (loop < TokenTab.Count)
                {
                    if (TokenTab[loop].lexeme == IDstring)
                        return TokenTab[loop];
                    loop++;
                }
                Token errortoken = new Token(Token_Type.ERRTOKEN, "", 0.0, NULL);
                return errortoken;
            }


            //------------------获取一个记号
            public static Token GetToken()				        //获取记号函数
            {
                Token token;
                char Char;

                EmptyToenString();
                int i;
                string s = "";
                for (i = 0; i < TokenBuffer.Count; i++)
                {
                    s += TokenBuffer[i].ToString();
                }
                token.lexeme = s.ToUpper();          //轉換成全部大寫
                while (true)
                {
                    Char = GetChar();
                    if ((int)Char == 0)             //Char==EOF 文件结束符
                    {
                        token = new Token(Token_Type.NONTOKEN, "", 0.0, NULL);
                        return token;
                    }
                    if (Char == '\r')               //回车\r\n
                    {
                        Char = GetChar();           //获得\n
                        LineNo++;       //回车时行号+1
                        //Console.WriteLine();
                        continue;
                    }
                    if (Char == '\t')
                        continue;

                    if (Char != ' ')                //不是空格則跳出
                        break;
                }
                AddCharTokenString(Char);	//若不是空格、tab、回车、文件结束符等，则先加入到记号的字符缓冲区中

                //-------DFA---------
                if (Regex.Match(Char.ToString(), "^[a-zA-Z]+$").Success)      //用正則表達式判斷是否為字符
                {
                    //函数，关键字，PI，E等
                    while (true)
                    {
                        Char = GetChar();
                        if (Regex.Match(Char.ToString(), "^\\d+$").Success || Regex.Match(Char.ToString(), "^[a-zA-Z]+$").Success)   //用正則表達式判斷是否為數字
                            AddCharTokenString(Char);
                        else
                            break;
                    }
                    index--;
                    for (i = 0; i < TokenBuffer.Count; i++)
                    {
                        s += TokenBuffer[i].ToString();
                    }
                    token = JudgeKeyToken(s.ToUpper());
                    token.lexeme = s.ToUpper();
                    return token;
                }
                else if (Regex.Match(Char.ToString(), "^\\d+$").Success)     //常量
                {
                    while (true)
                    {
                        Char = GetChar();
                        if (Regex.Match(Char.ToString(), "^\\d+$").Success)
                            AddCharTokenString(Char);
                        else
                            break;
                    }
                    if (Char == '.')
                    {
                        AddCharTokenString(Char);
                        while (true)
                        {
                            Char = GetChar();
                            if (Regex.Match(Char.ToString(), "^\\d+$").Success)
                                AddCharTokenString(Char);
                            else
                                break;
                        }
                    }
                    index--;
                    for (i = 0; i < TokenBuffer.Count; i++)
                    {
                        s += TokenBuffer[i].ToString();
                    }
                    double b = double.Parse(s);
                    token = new Token(Token_Type.CONST_ID, b.ToString(), b, NULL);
                    return token;
                }
                else
                {
                    switch (Char)
                    {
                        case ';':
                            token = new Token(Token_Type.SEMICOLON, ";", 0.0, NULL); break;
                        case '(':
                            token = new Token(Token_Type.L_BRACKET, "(", 0.0, NULL); break;
                        case ')':
                            token = new Token(Token_Type.R_BRACKET, ")", 0.0, NULL); break;
                        case ',':
                            token = new Token(Token_Type.COMMA, ",", 0.0, NULL); break;
                        case '+':
                            token = new Token(Token_Type.PLUS, "+", 0.0, NULL); break;
                        case '-':
                            Char = GetChar();
                            if (Char == '-')       //多個-操作
                            {
                                while (Char != '\n' && (int)Char != 0)
                                    Char = GetChar();
                                index--;
                                return GetToken();          //遞歸調用
                            }
                            else
                            {
                                index--;
                                token = new Token(Token_Type.MINUS, "-", 0.0, NULL);
                                break;
                            }
                        case '/':
                            Char = GetChar();
                            if (Char == '/')       //多個/操作
                            {
                                while (Char != '\n' && (int)Char != 0)
                                    Char = GetChar();
                                index--;
                                return GetToken();          //遞歸調用
                            }
                            else
                            {
                                index--;
                                token = new Token(Token_Type.DIV, "/", 0.0, NULL);
                                break;
                            }
                        case '*':
                            Char = GetChar();
                            if (Char == '*')       //多個*操作
                            {
                                token = new Token(Token_Type.POWER, "^", 0.0, NULL);
                                break;
                            }
                            else
                            {
                                index--;
                                token = new Token(Token_Type.MUL, "*", 0.0, NULL);
                                break;
                            }
                        default:
                            token = new Token(Token_Type.ERRTOKEN, "", 0.0, NULL);
                            break;
                    }
                }
                return token;
            }
        }
        public static class parser
        {
            public static int PARSER_DEBUG = 0;         //相當於宏定義
            public static int error_line_no = -1;       //用於終止程序
            public static int ind = -1;                  //用於縮進

            public class reference
            {
                public double Parameter = 0;
            }

            //static void Main(string[] args)
            //{
            //    string file_path = @"F:\School\Programing\C# Programing\FuncDraw\myfile.txt";
            //    Parser(file_path);
            //    return;
            //}

            //----------------------------------parserhead-----------------------------------------}
            public struct CaseOperator
            {
                public ExprNode Left, Right;
                public CaseOperator(ExprNode Left, ExprNode Right)
                {
                    this.Left = Left;
                    this.Right = Right;
                }
            }

            public struct CaseFunc
            {
                public ExprNode Child;
                public scanner.FuncPtr MathFuncPtr;
                public CaseFunc(ExprNode Child, scanner.FuncPtr MathFuncPtr)
                {
                    this.Child = Child;
                    this.MathFuncPtr = MathFuncPtr;
                }
            }

            public class Content
            {
                public CaseOperator CaseOperator;
                public CaseFunc CaseFunc;
                public double CaseConst;
                public reference CaseParmPtr;
            }

            public static Content Null = new Content();


            public class ExprNode				//語法樹節點類型
            {
                public scanner.Token_Type OpCode;
                public Content Content;
                //PLUS,MINUS,MUL,DIV,POWER,FUNC,CONST_ID等

                //等價於union
                /*[StructLayout(LayoutKind.Explicit)]
                class Content
                {
                    [FieldOffset(0)]
                    public struct CaseOperator
                    {
                        public ExprNode Left, Right;
                    }

                    [FieldOffset(0)]
                    public struct CaseFunc
                    {
                        public ExprNode Child;
                        public scanner.FuncPtr MathFuncPtr;
                    }

                    [FieldOffset(0)]
                    public double CaseConst;

                    [FieldOffset(0)]
                    public double CaseParmPtr;
                }
                public Content Content = new Content();*/
                public ExprNode(scanner.Token_Type OpCode, Content Content)
                {
                    this.OpCode = OpCode;
                    this.Content = Content;
                }
            };

            public static ExprNode NULL = new ExprNode(scanner.Token_Type.NONTOKEN, Null);


            //----------------------------------parserfunction--------------------------------------

            public static void enter(string x)
            {
                if (PARSER_DEBUG == 1)
                {
                    ind++;
                    int temp;
                    for (temp = 1; temp <= ind; temp++)
                        Console.Write('\t');		//縮進
                    Console.WriteLine("enter in  {0}", x);
                }
            }
            public static void back(string x)
            {
                if (PARSER_DEBUG == 1)
                {
                    int temp;
                    for (temp = 1; temp <= ind; temp++)
                        Console.Write('\t');		//縮進
                    Console.WriteLine("exit from  {0}", x);
                    ind--;
                }
            }
            public static void call_match(string x)
            {
                if (PARSER_DEBUG == 1)
                    Console.WriteLine("matchtoken  {0}", x);
            }
            public static void Tree_trace(ExprNode x)
            {
                if (PARSER_DEBUG == 1)
                    PrintSyntaxTree(x, 1);
            }

            public static reference Pa = new reference();								//	參數T的存儲空間
            public static double    Origin_x = 300, Origin_y = 300,						//	橫/縱平移距離
                                    Scale_x = 1, Scale_y = 1,						//	橫/縱比例因子
                                    Rot_angle = 0;									//	旋轉角度

            public static scanner.Token token;

            //------通過詞法分析器接口GetToken獲取一個記號--------
            public static void FetchToken()
            {
                token = scanner.GetToken();
                if (token.type == scanner.Token_Type.ERRTOKEN)
                {
                    SyntaxError(1);
                    error_line_no = (int)scanner.LineNo;
                }

            }

            //-------匹配記號-------
            public static void MatchToken(scanner.Token_Type The_Token)
            {
                if (token.type != The_Token)
                {
                    SyntaxError(2);
                    error_line_no = (int)scanner.LineNo;
                }

                FetchToken();
            }

            //-----語法錯誤處理------
            public static void SyntaxError(int case_of)
            {
                if ((int)scanner.LineNo != error_line_no)
                {
                    switch (case_of)
                    {
                        case 1:
                            ind++;
                            for (int temp = 1; temp <= ind; temp++)
                                Console.Write('\t');		//縮進
                            ErrMsg(scanner.LineNo, " 錯誤記號 ", token.lexeme);
                            ind--;
                            break;
                        case 2:
                            ind++;
                            for (int temp = 1; temp <= ind; temp++)
                                Console.Write('\t');		//縮進
                            ErrMsg(scanner.LineNo, " 不是預期記號 ", token.lexeme);
                            ind--;
                            break;
                        default:
                            break;
                    }
                }

            }

            //------打印錯誤信息------
            public static void ErrMsg(uint LineNo, string descrip, string st)
            {
                if (PARSER_DEBUG == 1)
                    Console.WriteLine("Line No " + LineNo.ToString() + ":  " + st + "" + descrip + "!");
                else
                {
                    string msg;
                    msg = "Line No " + LineNo.ToString() + ":  " + st + "" + descrip + "!";

                }
                scanner.CloseScanner();
            }

            //-----先序遍曆並打印表達式的語法樹-------
            public static void PrintSyntaxTree(ExprNode root, int indent)
            {
                for (int temp = 1; temp <= ind + 1; temp++)
                    Console.Write('\t');		//縮進
                for (int temp = 1; temp <= indent; temp++)
                    Console.Write('\t');		//縮進
                switch (root.OpCode)
                {
                    case scanner.Token_Type.PLUS:
                        Console.WriteLine('+');
                        break;
                    case scanner.Token_Type.MINUS:
                        Console.WriteLine('-');
                        break;
                    case scanner.Token_Type.DIV:
                        Console.WriteLine('/');
                        break;
                    case scanner.Token_Type.POWER:
                        Console.WriteLine("**");
                        break;
                    case scanner.Token_Type.FUNC:
                        Console.WriteLine(root.Content.CaseFunc.MathFuncPtr.Method.Name);
                        break;
                    case scanner.Token_Type.CONST_ID:
                        Console.WriteLine(root.Content.CaseConst);
                        break;
                    case scanner.Token_Type.T:
                        Console.WriteLine('T');
                        break;
                    default:
                        Console.WriteLine("Error Tree Node!");
                        return;
                }
                if (root.OpCode == scanner.Token_Type.CONST_ID || root.OpCode == scanner.Token_Type.T)
                    return;
                if (root.OpCode == scanner.Token_Type.FUNC)
                    PrintSyntaxTree(root.Content.CaseFunc.Child, indent + 1);
                else
                {
                    PrintSyntaxTree(root.Content.CaseOperator.Left, indent + 1);
                    PrintSyntaxTree(root.Content.CaseOperator.Right, indent + 1);
                }
            }

            //------FuncDraw解釋器入口(與主程序的外部接口)
            public static void Parser(string SrcFilePtr)
            {
                enter("Parser");
                scanner.InitScanner(SrcFilePtr);
                scanner.SourceFileInit();
                scanner.InitTokenTab();
                FetchToken();
                Program();
                scanner.CloseScanner();
                back("Parse");
                return;
            }

            //------Program的遞歸子程序------
            public static void Program()
            {
                enter("Program");
                while (token.type != scanner.Token_Type.NONTOKEN)
                {
                    if ((int)scanner.LineNo != error_line_no)
                        Statement();
                    MatchToken(scanner.Token_Type.SEMICOLON);
                }
                back("Program");
            }

            //-------Statement的遞歸子程序-----
            public static void Statement()
            {
                enter("Statement");
                switch (token.type)
                {
                    case scanner.Token_Type.ORIGIN:
                        OriginStatement();
                        break;
                    case scanner.Token_Type.SCALE:
                        ScaleStatement();
                        break;
                    case scanner.Token_Type.ROT:
                        RotStatement();
                        break;
                    case scanner.Token_Type.FOR:
                        ForStatement();
                        break;
                    default:
                        SyntaxError(2);
                        error_line_no = (int)scanner.LineNo;
                        break;
                }
                back("Statement");
            }

            //--------OriginStatement的遞歸子程序---------
            public static void OriginStatement()
            {
                ExprNode tmp;
                enter("OrginStatement");
                MatchToken(scanner.Token_Type.ORIGIN);
                MatchToken(scanner.Token_Type.IS);
                MatchToken(scanner.Token_Type.L_BRACKET);
                tmp = Expression();
                if (PARSER_DEBUG == 0)
                {
                    Origin_x = semantic.GetExprValue(tmp);
                    semantic.DelExprTree(tmp);
                }
                MatchToken(scanner.Token_Type.COMMA);
                tmp = Expression();
                if (PARSER_DEBUG == 0)
                {
                    Origin_y = semantic.GetExprValue(tmp);
                    semantic.DelExprTree(tmp);
                }
                MatchToken(scanner.Token_Type.R_BRACKET);
                back("OriginStatement");
            }

            //--------ScaleStatement的遞歸子程序---------
            public static void ScaleStatement()
            {
                ExprNode tmp;
                enter("ScaleStatement");
                MatchToken(scanner.Token_Type.SCALE);
                MatchToken(scanner.Token_Type.IS);
                MatchToken(scanner.Token_Type.L_BRACKET);
                tmp = Expression();
                if (PARSER_DEBUG == 0)
                {
                    Scale_x = semantic.GetExprValue(tmp);				//獲取橫座標的比例因子
                    semantic.DelExprTree(tmp);
                }
                MatchToken(scanner.Token_Type.COMMA);
                tmp = Expression();
                if (PARSER_DEBUG == 0)
                {
                    Scale_y = semantic.GetExprValue(tmp);				//獲取縱座標的比例因子
                    semantic.DelExprTree(tmp);
                }
                MatchToken(scanner.Token_Type.R_BRACKET);
                back("ScaleStatement");
            }

            //--------RotStatement的遞歸子程序-------
            public static void RotStatement()
            {
                ExprNode tmp;
                enter("RotStatement");
                MatchToken(scanner.Token_Type.ROT);
                MatchToken(scanner.Token_Type.IS);
                tmp = Expression();
                if (PARSER_DEBUG == 0)
                {
                    Rot_angle = semantic.GetExprValue(tmp);				//獲取旋轉角度
                    semantic.DelExprTree(tmp);
                }
                back("RotStatement");
            }

            //------ForStatement的遞歸子程序-------
            public static void ForStatement()
            {
                double Start = 0, End = 0, Step = 0;				//繪圖起點,終點,步長
                ExprNode start_ptr, end_ptr, step_ptr, x_ptr, y_ptr;		//各表達式語法樹根節點指針
                enter("ForStatement");
                MatchToken(scanner.Token_Type.FOR);
                call_match("FOR");
                MatchToken(scanner.Token_Type.T);
                call_match("T");
                MatchToken(scanner.Token_Type.FROM);
                call_match("FROM");
                start_ptr = Expression();					    //計算參數起點表達式的值
                if (PARSER_DEBUG == 0)								//釋放參數起點語法樹所占空間
                {
                    Start = semantic.GetExprValue(start_ptr);
                    semantic.DelExprTree(start_ptr);
                }
                MatchToken(scanner.Token_Type.TO);
                call_match("TO");
                end_ptr = Expression();						    //構造參數終點表達式語法樹
                if (PARSER_DEBUG == 0)
                {
                    End = semantic.GetExprValue(end_ptr);
                    semantic.DelExprTree(end_ptr);
                }
                MatchToken(scanner.Token_Type.STEP);
                step_ptr = Expression();
                if (PARSER_DEBUG == 0)
                {
                    Step = semantic.GetExprValue(step_ptr);
                    semantic.DelExprTree(step_ptr);
                }
                MatchToken(scanner.Token_Type.DRAW);
                call_match("DRAW");
                MatchToken(scanner.Token_Type.L_BRACKET);
                call_match("(");
                x_ptr = Expression();
                MatchToken(scanner.Token_Type.COMMA);
                call_match(",");
                y_ptr = Expression();
                MatchToken(scanner.Token_Type.R_BRACKET);
                call_match(")");
                if (PARSER_DEBUG == 0)
                {
                    semantic.DrawLoop(Start, End, Step, x_ptr, y_ptr);
                    semantic.DelExprTree(x_ptr);
                    semantic.DelExprTree(y_ptr);
                }
                back("ForStatement");
            }

            //---------Expression 的递归子程序---------
            public static ExprNode Expression()
            {
                ExprNode left, right;								//左右子樹節點的指針
                scanner.Token_Type token_tmp;						//當前記號
                enter("Expression");
                left = Term();												//分析左操作數且得到其語法樹
                while (token.type == scanner.Token_Type.PLUS || token.type == scanner.Token_Type.MINUS)
                {
                    token_tmp = token.type;
                    MatchToken(token_tmp);
                    right = Term();											//分析右操作數且得到其語法樹
                    left = MakeExprNode(token_tmp, left, right, new scanner.FuncPtr(scanner.NULL));			//構造運算的語法樹,結果為左子树
                }
                Tree_trace(left);											//打印表達式的語法樹
                back("Expression");
                return left;												//返回最終表達式的語法樹
            }

            //--------Term 的的遞歸子程序--------
            public static ExprNode Term()
            {
                ExprNode left, right;
                scanner.Token_Type token_tmp;
                enter("Term");
                left = Factor();
                while (token.type == scanner.Token_Type.MUL || token.type == scanner.Token_Type.DIV)
                {
                    token_tmp = token.type;
                    MatchToken(token_tmp);
                    right = Factor();
                    left = MakeExprNode(token_tmp, left, right, new scanner.FuncPtr(scanner.NULL));
                }
                back("Term");
                return left;
            }

            //--------Factor 的遞歸子程序---------
            public static ExprNode Factor()
            {
                ExprNode left, right;
                enter("Factor");
                if (token.type == scanner.Token_Type.PLUS)									//匹配一元加運算
                {
                    MatchToken(scanner.Token_Type.PLUS);
                    right = Factor();									//表達式退化為僅有右操作數的表達式
                }
                else if (token.type == scanner.Token_Type.MINUS)							//匹配一元減運算
                {
                    MatchToken(scanner.Token_Type.MINUS);
                    right = Factor();									//表達式轉化為二元减运算的表達式
                    Content transparent = new Content();
                    transparent.CaseConst = 0.0;
                    left = new ExprNode(scanner.Token_Type.CONST_ID, transparent);
                    right = MakeExprNode(scanner.Token_Type.MINUS, left, right, new scanner.FuncPtr(scanner.NULL));
                }
                else
                    right = Component();								//匹配非終結符Component
                back("Factor");
                return right;
            }

            //--------Component 的遞歸子程序--------
            public static ExprNode Component()
            {
                ExprNode left, right;
                enter("Component");
                left = Atom();
                if (token.type == scanner.Token_Type.POWER)
                {
                    MatchToken(scanner.Token_Type.POWER);
                    right = Component();								//遞歸調用Component以實現power的右結合
                    left = MakeExprNode(scanner.Token_Type.POWER, left, right, new scanner.FuncPtr(scanner.NULL));
                }
                back("Component");
                return left;
            }

            //---------Atom 的遞歸子程序---------
            public static ExprNode Atom()
            {
                scanner.Token t = token;
                ExprNode address, tmp;
                enter("Atom");
                switch (token.type)
                {
                    case scanner.Token_Type.CONST_ID:
                        MatchToken(scanner.Token_Type.CONST_ID);
                        address = MakeExprNode(scanner.Token_Type.CONST_ID, t.value, null, new scanner.FuncPtr(scanner.NULL));
                        break;
                    case scanner.Token_Type.T:
                        MatchToken(scanner.Token_Type.T);
                        address = MakeExprNode(scanner.Token_Type.T, null, null, new scanner.FuncPtr(scanner.NULL));
                        break;
                    case scanner.Token_Type.FUNC:
                        MatchToken(scanner.Token_Type.FUNC);
                        MatchToken(scanner.Token_Type.L_BRACKET);
                        tmp = Expression();
                        address = MakeExprNode(scanner.Token_Type.FUNC, null, tmp, t.FuncPtr);
                        MatchToken(scanner.Token_Type.R_BRACKET);
                        break;
                    case scanner.Token_Type.L_BRACKET:
                        MatchToken(scanner.Token_Type.L_BRACKET);
                        address = Expression();
                        MatchToken(scanner.Token_Type.R_BRACKET);
                        break;
                    default:
                        SyntaxError(2);
                        error_line_no = (int)scanner.LineNo;
                        return null;
                }
                back("Atom");
                return address;
            }

            //----------建立语法树---------------
            public static ExprNode MakeExprNode(scanner.Token_Type OpCode, object arg1, object arg2, scanner.FuncPtr Funcptr)
            {
                Content Content = new Content();
                ExprNode node = new ExprNode(OpCode, Content);
                switch (OpCode)
                {
                    case scanner.Token_Type.CONST_ID:
                        node.Content.CaseConst = (double)arg1;
                        break;
                    case scanner.Token_Type.T:
                        node.Content.CaseParmPtr = parser.Pa;
                        break;
                    case scanner.Token_Type.FUNC:
                        node.Content.CaseFunc.MathFuncPtr = Funcptr;
                        node.Content.CaseFunc.Child = (ExprNode)arg2;
                        break;
                    default:
                        node.Content.CaseOperator.Left = (ExprNode)arg1;
                        node.Content.CaseOperator.Right = (ExprNode)arg2;
                        break;
                }
                return node;
            }

        }
        public static class semantic
        {
            //---------出錯處理-------
            public static void Errmsg(string st)
            {
                return;
            }

            //---------計算被繪製點的座標-------
            public static void CalcCoord(parser.ExprNode Hor_Exp, parser.ExprNode Ver_Exp, ref double Hor_x, ref double Ver_y)
            {
                double HorCord, VerCord, Hor_tmp;
                //計算表達式的值,得到點的原始座標
                HorCord = GetExprValue(Hor_Exp);
                VerCord = GetExprValue(Ver_Exp);
                //進行比例變換
                HorCord *= parser.Scale_x;
                VerCord *= parser.Scale_y;
                //進行旋轉變換
                Hor_tmp = HorCord * System.Math.Cos(parser.Rot_angle) + VerCord * System.Math.Sin(parser.Rot_angle);
                VerCord = VerCord * System.Math.Cos(parser.Rot_angle) - HorCord * System.Math.Sin(parser.Rot_angle);
                HorCord = Hor_tmp;
                //進行平移變換
                HorCord += parser.Origin_x;
                VerCord += parser.Origin_y;
                //返回變換後點的座標
                Hor_x = HorCord;
                Ver_y = VerCord;
            }

            //-------循環繪製點座標--------
            public static void DrawLoop(double Start, double End, double Step, parser.ExprNode HorPtr, parser.ExprNode VerPtr)
            {
                double x = 0, y = 0;
                Draw.Initlizer_draw((int)scanner.Line_Count);
                for (parser.Pa.Parameter = Start; parser.Pa.Parameter <= End; parser.Pa.Parameter += Step)
                {
                    CalcCoord(HorPtr, VerPtr, ref x, ref y);
                    DrawPixel(x, y);
                }
                scanner.Line_Count++;
            }

            //-------計算表達式的值-------
            public static double GetExprValue(parser.ExprNode root)
            {
                if (root == parser.NULL)
                    return 0.0;
                switch (root.OpCode)
                {
                    case scanner.Token_Type.PLUS:
                        return GetExprValue(root.Content.CaseOperator.Left) + GetExprValue(root.Content.CaseOperator.Right);
                    case scanner.Token_Type.MINUS:
                        return GetExprValue(root.Content.CaseOperator.Left) - GetExprValue(root.Content.CaseOperator.Right);
                    case scanner.Token_Type.MUL:
                        return GetExprValue(root.Content.CaseOperator.Left) * GetExprValue(root.Content.CaseOperator.Right);
                    case scanner.Token_Type.DIV:
                        return GetExprValue(root.Content.CaseOperator.Left) / GetExprValue(root.Content.CaseOperator.Right);
                    case scanner.Token_Type.POWER:
                        return System.Math.Pow(GetExprValue(root.Content.CaseOperator.Left), GetExprValue(root.Content.CaseOperator.Right));
                    case scanner.Token_Type.FUNC:
                        return root.Content.CaseFunc.MathFuncPtr(GetExprValue(root.Content.CaseFunc.Child));
                    case scanner.Token_Type.CONST_ID:
                        return root.Content.CaseConst;
                    case scanner.Token_Type.T:
                        return root.Content.CaseParmPtr.Parameter;
                    default:
                        return 0.0;
                }
            }

            //------刪除一棵語法樹--------
            public static void DelExprTree(parser.ExprNode root)
            {
                if (root == parser.NULL)
                    return;
                switch (root.OpCode)
                {
                    case scanner.Token_Type.PLUS:														//兩個孩子的內部節點
                    case scanner.Token_Type.MINUS:
                    case scanner.Token_Type.MUL:
                    case scanner.Token_Type.DIV:
                    case scanner.Token_Type.POWER:
                        DelExprTree(root.Content.CaseOperator.Left);
                        DelExprTree(root.Content.CaseOperator.Right);
                        break;
                    case scanner.Token_Type.FUNC:														//一個孩子的內部節點
                        DelExprTree(root.Content.CaseFunc.Child);
                        break;
                    default:														//葉子節點
                        break;
                }
                delete(root);													//刪除節點
            }

            public static void delete(parser.ExprNode root)
            {
                root = parser.NULL;
            }

            //---------繪製一個點--------
            public static void DrawPixel(double x, double y)
            {
                Draw.draw(x, y);
            }
        }
    }
}
