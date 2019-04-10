using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fare
{

    public class AbstractSyntaxTree
    {
        public static string Generate(RegExp regex)
        {

            return regex.ToString();
        }


        public static string Generate(Automaton root)
        {
            StringBuilder bldr = new StringBuilder();

            if (null != root)
            {
                var a = root;

                bldr.Append(CreateASTLine(a, 0));
                
                foreach (var state in a.GetStates())
                {
                    bldr.Append(CreateASTLine($"\tState Id:{state.Id}, Number:{state.Number}) has {state.Transitions.Count} transitions", 1 ));
                    foreach (var trans in state.GetSortedTransitions(false))
                    {
                        bldr.Append(CreateASTLine($"\t\tTransition to: {trans.To.Id} [{trans.Min} - {trans.Max}]", 2 ));
                    }
                }

            }

            return bldr.ToString();
        }


        private static string CreateASTLine(string str, uint indent)
        {
            return string.Format("{0}{1}\n", CreateIndentString(indent), str);
        }


        private static string CreateASTLine(Automaton a, uint indent)
        {
            return string.Format("{0}{1}\n", CreateIndentString(indent), GetAutomatonInfo(a));
        }


        private static string CreateIndentString(uint indent)
        {
            var indention = string.Empty;

            for (uint i = 0; i < indent; ++i)
            {
                indention += "  ";
            }

            return indention;
        }


        private static string GetAutomatonInfo(Automaton a)
        {
            return $"Node (initial state id {a.Initial.Id}) has {a.NumberOfStates} states and {a.NumberOfTransitions} transistions";
        }




        
        private StringBuilder ToStringBuilder(RegExp regex)
        {
            StringBuilder sb = new StringBuilder(2048);

            switch (regex.Kind)
            {
                case Kind.RegexpUnion:
                    sb.Append("(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append("|");
                    regex.Expr2.ToStringBuilder(sb);
                    sb.Append(")");
                    break;
                case Kind.RegexpConcatenation:
                    regex.Expr1.ToStringBuilder(sb);
                    regex.Expr2.ToStringBuilder(sb);
                    break;
                case Kind.RegexpIntersection:
                    sb.Append("(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append("&");
                    regex.Expr2.ToStringBuilder(sb);
                    sb.Append(")");
                    break;
                case Kind.RegexpOptional:
                    sb.Append("(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append(")?");
                    break;
                case Kind.RegexpRepeat:
                    sb.Append("(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append(")*");
                    break;
                case Kind.RegexpRepeatMin:
                    sb.Append("(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append("){").Append(regex.Min).Append(",}");
                    break;
                case Kind.RegexpRepeatMinMax:
                    sb.Append("(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append("){").Append(regex.Min).Append(",").Append(regex.Max).Append("}");
                    break;
                case Kind.RegexpComplement:
                    sb.Append("~(");
                    regex.Expr1.ToStringBuilder(sb);
                    sb.Append(")");
                    break;
                case Kind.RegexpChar:
                    sb.Append("\\").Append(regex.Char);
                    break;
                case Kind.RegexpCharRange:
                    sb.Append("[\\").Append(regex.FromChar).Append("-\\").Append(regex.ToChar).Append("]");
                    break;
                case Kind.RegexpAnyChar:
                    sb.Append(".");
                    break;
                case Kind.RegexpEmpty:
                    sb.Append("#");
                    break;
                case Kind.RegexpString:
                    sb.Append("\"").Append(regex.SourceRegExpr).Append("\"");
                    break;
                case Kind.RegexpAnyString:
                    sb.Append("@");
                    break;
                case Kind.RegexpAutomaton:
                    sb.Append("<").Append(regex.SourceRegExpr).Append(">");
                    break;
                case Kind.RegexpInterval:
                    string s1 = Convert.ToDecimal(regex.Min).ToString();
                    string s2 = Convert.ToDecimal(regex.Max).ToString();
                    sb.Append("<");
                    if (regex.Digits > 0)
                    {
                        for (int i = s1.Length; i < regex.Digits; i++)
                        {
                            sb.Append('0');
                        }
                    }

                    sb.Append(s1).Append("-");
                    if (regex.Digits > 0)
                    {
                        for (int i = s2.Length; i < regex.Digits; i++)
                        {
                            sb.Append('0');
                        }
                    }

                    sb.Append(s2).Append(">");
                    break;
            }

            return sb;
        }


    }

}