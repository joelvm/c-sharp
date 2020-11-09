using System;
using System.Collections;
using System.Collections.Generic;
using NLog;

namespace Calculator
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Console.Write("Insira expressão para ser avaliada: ");
            string s = Console.ReadLine();
            List<object> e = CreateExpression(s);
            List<object> p = TransformToPostFix(e);
            CalculateExpression(p);

        }

        private static List<object> CreateExpression(String expression)
        {
            string aux = "";
            char[] a = expression.ToCharArray();
            List<object> e = new List<object>();

            for (int i = 0; i < a.Length; i++)
            {
                if (double.TryParse(a[i].ToString(), out _))
                    aux += a[i];
                else if (a[i] != ' ' && a[i] != '\t')
                {
                    if (aux.Length > 0)
                    {
                        e.Add(double.Parse(aux));
                        aux = "";
                    }

                    if (a[i] != '+' && a[i] != '-' && a[i] != '/' && a[i] != '*' && a[i] != '^' && a[i] != '%' && a[i] != '(' && a[i] != ')')
                        throw new ArgumentException("Apenas são permitos os operadores: +-*\\^()");
                    e.Add(a[i].ToString());
                }
            }
            if (aux.Length > 0)
                e.Add(double.Parse(aux));

            return e;
        }

        private static List<object> TransformToPostFix(List<object> expression)
        {
            List<object> postfix = new List<object>();
            Stack aux = new Stack();

            foreach(Object o in expression)
            {
                if (o is double)
                    postfix.Add(o);
                else
                {
                    string s = (string)o;
                    switch (s)
                    {
                        case "(":
                            aux.Push(s);
                            break;

                        case ")":
                            {
                                String s1 = (string)aux.Pop();
                                while (!s1.Equals("("))
                                {
                                    postfix.Add(s1);
                                    s1 = (string)aux.Pop();
                                }
                            }
                            break;

                        case "+":
                        case "-":
                            {
                                String s1 = aux.Count > 0 ? (string)aux.Peek() : null;
                                while (s1 != null && !s1.Equals("(")
                                    && (s1.Equals("*") || s1.Equals("/") || s1.Equals("%") || s1.Equals("^")
                                    || s1.Equals("+") || s1.Equals("-")))
                                {
                                    postfix.Add(aux.Pop());
                                    s1 = aux.Count > 0 ? (string)aux.Peek() : null;
                                }
                            }
                            aux.Push(s);
                            break;
                        case "*":
                        case "/":
                        case "%":
                             {
                                String s1 = aux.Count > 0 ? (string)aux.Peek() : null;
                                while (s1 != null && !s1.Equals("(")
                                    && (s1.Equals("*") || s1.Equals("/") || s1.Equals("%") || s1.Equals("^")))
                                {
                                    postfix.Add(aux.Pop());
                                    s1 = aux.Count > 0 ? (string)aux.Peek() : null;
                                }
                            }
                            aux.Push(s);
                            break;
                        case "^":
                            {
                                String s1 = aux.Count > 0 ? (string)aux.Peek() : null;
                                while (s1 != null && s1.Equals("^"))
                                {
                                    postfix.Add(aux.Pop());
                                    s1 = aux.Count > 0 ? (string)aux.Peek() : null;
                                }
                            }
                            aux.Push(s);
                            break;
                    }
                } 
            }

            object o2 = aux.Count > 0 ? aux.Pop() : null;
            while (o2 != null)
            {
                postfix.Add(o2);
                o2 = aux.Count > 0 ? aux.Pop() : null;
            }
            Console.Write("Postfix: ");
            foreach (Object o in postfix)
                Console.Write(o.ToString() + " ");
            Console.WriteLine();
            return postfix;
        }


        private static void CalculateExpression(List<object> expression)
        {
            Stack st = new Stack();
            foreach(Object o in expression){
                if (o is double)
                {
                    st.Push(o);
                    Console.WriteLine("Pushed: " + st.Peek());
                }
                else
                {
                    double i2 = (double)st.Pop();
                    double i1 = (double)st.Pop();
                    switch ((string)o)
                    {
                        case "+": st.Push(i1 + i2); break;
                        case "-": st.Push(i1 - i2); break;
                        case "*": st.Push(i1 * i2); break;
                        case "/": st.Push(i1 / i2); break;
                        case "^": st.Push(Math.Pow(i1, i2)); break;
                        case "%": st.Push(i1 % i2); break;
                    }
                    Console.WriteLine("Pushed: " + st.Peek());
                }
            }

            Console.WriteLine("O resultado é: " + st.Pop());
        }
    }
}
