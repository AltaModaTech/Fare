/*
 * dk.brics.automaton
 * 
 * Copyright (c) 2001-2011 Anders Moeller
 * All rights reserved.
 * http://github.com/moodmosaic/Fare/
 * Original Java code:
 * http://www.brics.dk/automaton/
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;


namespace Fare
{
    /// <summary>
    /// Regular Expression extension to Automaton.
    /// </summary>
    public class RegExp
    {
        private readonly string b;
        private readonly RegExpSyntaxOptions flags;

        private bool allowMutation;
        private int pos;


        internal char Char { get; private set; }
        internal int Digits { get;  private set; }
        internal RegExp Expr1 { get;  private set; }
        internal RegExp Expr2 { get; private set; }
        internal char FromChar { get;  private set; }
        internal Kind Kind { get;  private set; }
        internal int Max { get;  private set; }
        internal int Min { get; private set; }
        internal string SourceRegExpr { get; private set; }
        internal char ToChar { get; private set; }


        /// <summary>
        ///   Prevents a default instance of the <see cref = "RegExp" /> class from being created.
        /// </summary>
        private RegExp()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RegExp" /> class from a string.
        /// </summary>
        /// <param name = "s">A string with the regular expression.</param>
        public RegExp(string s) : this(s, RegExpSyntaxOptions.All)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RegExp" /> class from a string.
        /// </summary>
        /// <param name = "s">A string with the regular expression.</param>
        /// <param name = "syntaxFlags">Boolean 'or' of optional syntax constructs to be enabled.</param>
        public RegExp(string s, RegExpSyntaxOptions syntaxFlags)
        {
            this.b = s;
            this.flags = syntaxFlags;
            RegExp e;
            if (s.Length == 0)
            {
                e = RegExp.MakeString(string.Empty);
            }
            else
            {
                e = this.ParseUnionExp();
                if (this.pos < b.Length)
                {
                    throw new ArgumentException("end-of-string expected at position " + this.pos);
                }
            }

            this.Kind = e.Kind;
            this.Expr1 = e.Expr1;
            this.Expr2 = e.Expr2;
            this.SourceRegExpr = e.SourceRegExpr;
            this.Char = e.Char;
            this.Min= e.Min;
            this.Max = e.Max;
            this.Digits = e.Digits;
            this.FromChar = e.FromChar;
            this.ToChar = e.ToChar;
            this.b = null;
        }

        /// <summary>
        ///   Constructs new <code>Automaton</code> from this <code>RegExp</code>. 
        ///   Same as <code>toAutomaton(null)</code> (empty automaton map).
        /// </summary>
        /// <returns></returns>
        public Automaton ToAutomaton()
        {
            return this.ToAutomatonAllowMutate(null, null, true);
        }

        /// <summary>
        /// Constructs new <code>Automaton</code> from this <code>RegExp</code>.
        /// Same as <code>toAutomaton(null,minimize)</code> (empty automaton map).
        /// </summary>
        /// <param name="minimize">if set to <c>true</c> [minimize].</param>
        /// <returns></returns>
        public Automaton ToAutomaton(bool minimize)
        {
            return this.ToAutomatonAllowMutate(null, null, minimize);
        }

        /// <summary>
        ///   Constructs new <code>Automaton</code> from this <code>RegExp</code>. 
        ///   The constructed automaton is minimal and deterministic and has no 
        ///   transitions to dead states.
        /// </summary>
        /// <param name = "automatonProvider">The provider of automata for named identifiers.</param>
        /// <returns></returns>
        public Automaton ToAutomaton(IAutomatonProvider automatonProvider)
        {
            return this.ToAutomatonAllowMutate(null, automatonProvider, true);
        }

        /// <summary>
        ///   Constructs new <code>Automaton</code> from this <code>RegExp</code>. 
        ///   The constructed automaton has no transitions to dead states.
        /// </summary>
        /// <param name = "automatonProvider">The provider of automata for named identifiers.</param>
        /// <param name = "minimize">if set to <c>true</c> the automaton is minimized and determinized.</param>
        /// <returns></returns>
        public Automaton ToAutomaton(IAutomatonProvider automatonProvider, bool minimize)
        {
            return this.ToAutomatonAllowMutate(null, automatonProvider, minimize);
        }

        /// <summary>
        ///   Constructs new <code>Automaton</code> from this <code>RegExp</code>. 
        ///   The constructed automaton is minimal and deterministic and has no 
        ///   transitions to dead states.
        /// </summary>
        /// <param name = "automata">The a map from automaton identifiers to automata.</param>
        /// <returns></returns>
        public Automaton ToAutomaton(IDictionary<string, Automaton> automata)
        {
            return this.ToAutomatonAllowMutate(automata, null, true);
        }

        /// <summary>
        ///   Constructs new <code>Automaton</code> from this <code>RegExp</code>. 
        ///   The constructed automaton has no transitions to dead states.
        /// </summary>
        /// <param name = "automata">The map from automaton identifiers to automata.</param>
        /// <param name = "minimize">if set to <c>true</c> the automaton is minimized and determinized.</param>
        /// <returns></returns>
        public Automaton ToAutomaton(IDictionary<string, Automaton> automata, bool minimize)
        {
            return this.ToAutomatonAllowMutate(automata, null, minimize);
        }

        /// <summary>
        ///   Sets or resets allow mutate flag.
        ///   If this flag is set, then automata construction uses mutable automata,
        ///   which is slightly faster but not thread safe.
        /// </summary>
        /// <param name = "flag">if set to <c>true</c> the flag is set.</param>
        /// <returns>The previous value of the flag.</returns>
        public bool SetAllowMutate(bool flag)
        {
            bool previous = allowMutation;
            allowMutation = flag;
            return previous;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToStringBuilder(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Returns the set of automaton identifiers that occur in this regular expression.
        /// </summary>
        /// <returns>The set of automaton identifiers that occur in this regular expression.</returns>
        public HashSet<string> GetIdentifiers()
        {
            var set = new HashSet<string>();
            this.GetIdentifiers(set);
            return set;
        }

        private static RegExp MakeUnion(RegExp exp1, RegExp exp2)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpUnion;
            r.Expr1 = exp1;
            r.Expr2 = exp2;
            return r;
        }

        private static RegExp MakeIntersection(RegExp exp1, RegExp exp2)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpIntersection;
            r.Expr1 = exp1;
            r.Expr2 = exp2;
            return r;
        }

        private static RegExp MakeConcatenation(RegExp exp1, RegExp exp2)
        {
            if ((exp1.Kind == Kind.RegexpChar || exp1.Kind == Kind.RegexpString)
                && (exp2.Kind == Kind.RegexpChar || exp2.Kind == Kind.RegexpString))
            {
                return RegExp.MakeString(exp1, exp2);
            }

            var r = new RegExp();
            r.Kind = Kind.RegexpConcatenation;
            if (exp1.Kind == Kind.RegexpConcatenation
                && (exp1.Expr2.Kind == Kind.RegexpChar || exp1.Expr2.Kind == Kind.RegexpString)
                && (exp2.Kind == Kind.RegexpChar || exp2.Kind == Kind.RegexpString))
            {
                r.Expr1 = exp1.Expr1;
                r.Expr2 = RegExp.MakeString(exp1.Expr2, exp2);
            }
            else if ((exp1.Kind == Kind.RegexpChar || exp1.Kind == Kind.RegexpString)
                     && exp2.Kind == Kind.RegexpConcatenation
                     && (exp2.Expr1.Kind == Kind.RegexpChar || exp2.Expr1.Kind == Kind.RegexpString))
            {
                r.Expr1 = RegExp.MakeString(exp1, exp2.Expr1);
                r.Expr2 = exp2.Expr2;
            }
            else
            {
                r.Expr1 = exp1;
                r.Expr2 = exp2;
            }

            return r;
        }

        private static RegExp MakeRepeat(RegExp exp)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpRepeat;
            r.Expr1 = exp;
            return r;
        }

        private static RegExp MakeRepeat(RegExp exp, int min)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpRepeatMin;
            r.Expr1 = exp;
            r.Min= min;
            return r;
        }

        private static RegExp MakeRepeat(RegExp exp, int min, int max)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpRepeatMinMax;
            r.Expr1 = exp;
            r.Min= min;
            r.Max = max;
            return r;
        }

        private static RegExp MakeOptional(RegExp exp)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpOptional;
            r.Expr1 = exp;
            return r;
        }

        private static RegExp MakeChar(char @char)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpChar;
            r.Char = @char;
            return r;
        }

        private static RegExp MakeInterval(int min, int max, int digits)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpInterval;
            r.Min= min;
            r.Max = max;
            r.Digits = digits;
            return r;
        }

        private static RegExp MakeAutomaton(string s)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpAutomaton;
            r.SourceRegExpr = s;
            return r;
        }

        private static RegExp MakeAnyString()
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpAnyString;
            return r;
        }

        private static RegExp MakeEmpty()
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpEmpty;
            return r;
        }

        private static RegExp MakeAnyChar()
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpAnyChar;
            return r;
        }

        private static RegExp MakeAnyPrintableASCIIChar()
        {
            return MakeCharRange(' ', '~');
        }

        private static RegExp MakeCharRange(char from, char to)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpCharRange;
            r.FromChar = from;
            r.ToChar = to;
            return r;
        }

        private static RegExp MakeComplement(RegExp exp)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpComplement;
            r.Expr1 = exp;
            return r;
        }

        private static RegExp MakeString(string @string)
        {
            var r = new RegExp();
            r.Kind = Kind.RegexpString;
            r.SourceRegExpr = @string;
            return r;
        }

        private static RegExp MakeString(RegExp exp1, RegExp exp2)
        {
            var sb = new StringBuilder();
            if (exp1.Kind == Kind.RegexpString)
            {
                sb.Append(exp1.SourceRegExpr);
            }
            else
            {
                sb.Append(exp1.Char);
            }

            if (exp2.Kind == Kind.RegexpString)
            {
                sb.Append(exp2.SourceRegExpr);
            }
            else
            {
                sb.Append(exp2.Char);
            }

            return RegExp.MakeString(sb.ToString());
        }

        private Automaton ToAutomatonAllowMutate(
            IDictionary<string, Automaton> automata,
            IAutomatonProvider automatonProvider,
            bool minimize)
        {
            bool @bool = false;
            if (allowMutation)
            {
                @bool = this.SetAllowMutate(true); // This is not thead safe.
            }

            Automaton a = this.ToAutomaton(automata, automatonProvider, minimize);
            if (allowMutation)
            {
                this.SetAllowMutate(@bool);
            }

            return a;
        }

        private Automaton ToAutomaton(
            IDictionary<string, Automaton> automata,
            IAutomatonProvider automatonProvider,
            bool minimize)
        {
            IList<Automaton> list;
            Automaton a = null;
            switch (this.Kind)
            {
                case Kind.RegexpUnion:
                    list = new List<Automaton>();
                    this.FindLeaves(Expr1, Kind.RegexpUnion, list, automata, automatonProvider, minimize);
                    this.FindLeaves(Expr2, Kind.RegexpUnion, list, automata, automatonProvider, minimize);
                    a = BasicOperations.Union(list);
                    a.Minimize();
                    break;
                case Kind.RegexpConcatenation:
                    list = new List<Automaton>();
                    this.FindLeaves(Expr1, Kind.RegexpConcatenation, list, automata, automatonProvider, minimize);
                    this.FindLeaves(Expr2, Kind.RegexpConcatenation, list, automata, automatonProvider, minimize);
                    a = BasicOperations.Concatenate(list);
                    a.Minimize();
                    break;
                case Kind.RegexpIntersection:
                    a = Expr1.ToAutomaton(automata, automatonProvider, minimize)
                        .Intersection(Expr2.ToAutomaton(automata, automatonProvider, minimize));
                    a.Minimize();
                    break;
                case Kind.RegexpOptional:
                    a = Expr1.ToAutomaton(automata, automatonProvider, minimize).Optional();
                    a.Minimize();
                    break;
                case Kind.RegexpRepeat:
                    a = Expr1.ToAutomaton(automata, automatonProvider, minimize).Repeat();
                    a.Minimize();
                    break;
                case Kind.RegexpRepeatMin:
                    a = Expr1.ToAutomaton(automata, automatonProvider, minimize).Repeat(Min);
                    a.Minimize();
                    break;
                case Kind.RegexpRepeatMinMax:
                    a = Expr1.ToAutomaton(automata, automatonProvider, minimize).Repeat(Min, Max);
                    a.Minimize();
                    break;
                case Kind.RegexpComplement:
                    a = Expr1.ToAutomaton(automata, automatonProvider, minimize).Complement();
                    a.Minimize();
                    break;
                case Kind.RegexpChar:
                    a = BasicAutomata.MakeChar(this.Char);
                    break;
                case Kind.RegexpCharRange:
                    a = BasicAutomata.MakeCharRange(FromChar, ToChar);
                    break;
                case Kind.RegexpAnyChar:
                    a = BasicAutomata.MakeAnyChar();
                    break;
                case Kind.RegexpEmpty:
                    a = BasicAutomata.MakeEmpty();
                    break;
                case Kind.RegexpString:
                    a = BasicAutomata.MakeString(SourceRegExpr);
                    break;
                case Kind.RegexpAnyString:
                    a = BasicAutomata.MakeAnyString();
                    break;
                case Kind.RegexpAutomaton:
                    Automaton aa = null;
                    if (automata != null)
                    {
                        automata.TryGetValue(SourceRegExpr, out aa);
                    }

                    if (aa == null && automatonProvider != null)
                    {
                        try
                        {
                            aa = automatonProvider.GetAutomaton(SourceRegExpr);
                        }
                        catch (IOException e)
                        {
                            throw new ArgumentException(string.Empty, e);
                        }
                    }

                    if (aa == null)
                    {
                        throw new ArgumentException("'" + SourceRegExpr + "' not found");
                    }

                    a = aa.Clone(); // Always clone here (ignore allowMutate).
                    break;
                case Kind.RegexpInterval:
                    a = BasicAutomata.MakeInterval(Min, Max, Digits);
                    break;
            }

            return a;
        }

        private void FindLeaves(
            RegExp exp,
            Kind regExpKind,
            IList<Automaton> list,
            IDictionary<String, Automaton> automata,
            IAutomatonProvider automatonProvider,
            bool minimize)
        {
            if (exp.Kind == regExpKind)
            {
                this.FindLeaves(exp.Expr1, regExpKind, list, automata, automatonProvider, minimize);
                this.FindLeaves(exp.Expr2, regExpKind, list, automata, automatonProvider, minimize);
            }
            else
            {
                list.Add(exp.ToAutomaton(automata, automatonProvider, minimize));
            }
        }

        // TODO: make private after removing use from AST
        internal StringBuilder ToStringBuilder(StringBuilder sb)
        {
            switch (Kind)
            {
                case Kind.RegexpUnion:
                    sb.Append("(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append("|");
                    Expr2.ToStringBuilder(sb);
                    sb.Append(")");
                    break;
                case Kind.RegexpConcatenation:
                    Expr1.ToStringBuilder(sb);
                    Expr2.ToStringBuilder(sb);
                    break;
                case Kind.RegexpIntersection:
                    sb.Append("(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append("&");
                    Expr2.ToStringBuilder(sb);
                    sb.Append(")");
                    break;
                case Kind.RegexpOptional:
                    sb.Append("(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append(")?");
                    break;
                case Kind.RegexpRepeat:
                    sb.Append("(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append(")*");
                    break;
                case Kind.RegexpRepeatMin:
                    sb.Append("(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append("){").Append(Min).Append(",}");
                    break;
                case Kind.RegexpRepeatMinMax:
                    sb.Append("(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append("){").Append(Min).Append(",").Append(Max).Append("}");
                    break;
                case Kind.RegexpComplement:
                    sb.Append("~(");
                    Expr1.ToStringBuilder(sb);
                    sb.Append(")");
                    break;
                case Kind.RegexpChar:
                    sb.Append(this.Char);
                    break;
                case Kind.RegexpCharRange:
                    sb.Append("[").Append(FromChar).Append("-").Append(ToChar).Append("]");
                    break;
                case Kind.RegexpAnyChar:
                    sb.Append(".");
                    break;
                case Kind.RegexpEmpty:
                    sb.Append(string.Empty);    // TODO: consider not appending at all
                    break;
                case Kind.RegexpString:
//                     sb.Append("\"").Append(SourceRegExpr).Append("\"");  // TODO: REMOVE:
                    sb.Append(SourceRegExpr);
                    break;
                case Kind.RegexpAnyString:
                    sb.Append("@");
                    break;
                case Kind.RegexpAutomaton:
                    sb.Append("<").Append(SourceRegExpr).Append(">");
                    break;
                case Kind.RegexpInterval:
                    string s1 = Convert.ToDecimal(Min).ToString(NumberFormatInfo.CurrentInfo);
                    string s2 = Convert.ToDecimal(Max).ToString(NumberFormatInfo.CurrentInfo);
                    sb.Append("<");
                    if (Digits > 0)
                    {
                        for (int i = s1.Length; i < Digits; i++)
                        {
                            sb.Append('0');
                        }
                    }

                    sb.Append(s1).Append("-");
                    if (Digits > 0)
                    {
                        for (int i = s2.Length; i < Digits; i++)
                        {
                            sb.Append('0');
                        }
                    }

                    sb.Append(s2).Append(">");
                    break;
            }

            return sb;
        }

        private void GetIdentifiers(HashSet<string> set)
        {
            switch (Kind)
            {
                case Kind.RegexpUnion:
                case Kind.RegexpConcatenation:
                case Kind.RegexpIntersection:
                    Expr1.GetIdentifiers(set);
                    Expr2.GetIdentifiers(set);
                    break;
                case Kind.RegexpOptional:
                case Kind.RegexpRepeat:
                case Kind.RegexpRepeatMin:
                case Kind.RegexpRepeatMinMax:
                case Kind.RegexpComplement:
                    Expr1.GetIdentifiers(set);
                    break;
                case Kind.RegexpAutomaton:
                    set.Add(SourceRegExpr);
                    break;
            }
        }

        private RegExp ParseUnionExp()
        {
            RegExp e = this.ParseInterExp();
            if (this.Match('|'))
            {
                e = RegExp.MakeUnion(e, this.ParseUnionExp());
            }

            return e;
        }

        private bool Match(char @char)
        {
            if (pos >= b.Length)
            {
                return false;
            }

            if (b[pos] == @char)
            {
                pos++;
                return true;
            }

            return false;
        }

        private RegExp ParseInterExp()
        {
            RegExp e = this.ParseConcatExp();
            if (this.Check(RegExpSyntaxOptions.Intersection) && this.Match('&'))
            {
                e = RegExp.MakeIntersection(e, this.ParseInterExp());
            }

            return e;
        }

        private bool Check(RegExpSyntaxOptions flag)
        {
            return (flags & flag) != 0;
        }

        private RegExp ParseConcatExp()
        {
            RegExp e = this.ParseRepeatExp();
            if (this.More() && !this.Peek(")|") && (!this.Check(RegExpSyntaxOptions.Intersection) || !this.Peek("&")))
            {
                e = RegExp.MakeConcatenation(e, this.ParseConcatExp());
            }

            return e;
        }

        private bool More()
        {
            return pos < b.Length;
        }

        private bool Peek(string @string)
        {
            return this.More() && @string.IndexOf(b[pos]) != -1;
        }

        private RegExp ParseRepeatExp()
        {
            RegExp e = this.ParseComplExp();
            while (this.Peek("?*+{"))
            {
                if (this.Match('?'))
                {
                    e = RegExp.MakeOptional(e);
                }
                else if (this.Match('*'))
                {
                    e = RegExp.MakeRepeat(e);
                }
                else if (this.Match('+'))
                {
                    e = RegExp.MakeRepeat(e, 1);
                }
                else if (this.Match('{'))
                {
                    int start = pos;
                    while (this.Peek("0123456789"))
                    {
                        this.Next();
                    }

                    if (start == pos)
                    {
                        throw new ArgumentException("integer expected at position " + pos);
                    }

                    int n = int.Parse(b.Substring(start, pos - start), NumberFormatInfo.CurrentInfo);
                    int m = -1;
                    if (this.Match(','))
                    {
                        start = pos;
                        while (this.Peek("0123456789"))
                        {
                            this.Next();
                        }

                        if (start != pos)
                        {
                            m = int.Parse(b.Substring(start, pos - start), NumberFormatInfo.CurrentInfo);
                        }
                    }
                    else
                    {
                        m = n;
                    }

                    if (!this.Match('}'))
                    {
                        throw new ArgumentException("expected '}' at position " + pos);
                    }

                    e = m == -1 ? RegExp.MakeRepeat(e, n) : RegExp.MakeRepeat(e, n, m);
                }
            }

            return e;
        }

        private char Next()
        {
            if (!this.More())
            {
                throw new InvalidOperationException("unexpected end-of-string");
            }

            return b[pos++];
        }

        private RegExp ParseComplExp()
        {
            if (this.Check(RegExpSyntaxOptions.Complement) && this.Match('~'))
            {
                return RegExp.MakeComplement(this.ParseComplExp());
            }

            return this.ParseCharClassExp();
        }

        private RegExp ParseCharClassExp()
        {
            if (this.Match('['))
            {
                bool negate = false;
                if (this.Match('^'))
                {
                    negate = true;
                }

                RegExp e = this.ParseCharClasses();
                if (negate)
                {
                    e = ExcludeChars(e, MakeAnyPrintableASCIIChar());
                }

                if (!this.Match(']'))
                {
                    throw new ArgumentException("expected ']' at position " + pos);
                }

                return e;
            }

            return this.ParseSimpleExp();
        }

        private RegExp ParseSimpleExp()
        {
            if (this.Match('.'))
            {
                return MakeAnyPrintableASCIIChar();
            }

            /* Issue 32, https://github.com/moodmosaic/Fare/issues/32
            *   The intent of the original code is a little unclear.  The comment for the Empty value in the 
            *   enum is 'Enables empty language.'  Using '#' as token seems non-standard, and caused
            *   unhandled exception in some cases.  The best option at this point is to remove handling of the
            *   Empty option until a proper implementation is proposed.
             */
            // if (this.Check(RegExpSyntaxOptions.Empty) && this.Match('#'))
            // {
            //     return RegExp.MakeEmpty();
            // }

            if (this.Check(RegExpSyntaxOptions.Anystring) && this.Match('@'))
            {
                return RegExp.MakeAnyString();
            }

            if (this.Match('"'))
            {
                int start = pos;
                while (this.More() && !this.Peek("\""))
                {
                    this.Next();
                }

                if (!this.Match('"'))
                {
                    throw new ArgumentException("expected '\"' at position " + pos);
                }

                return RegExp.MakeString(b.Substring(start, ((pos - 1) - start)));
            }

            if (this.Match('('))
            {
                if (this.Match('?'))
                {
                    this.SkipNonCapturingSubpatternExp();
                }

                if (this.Match(')'))
                {
                    return RegExp.MakeString(string.Empty);
                }

                RegExp e = this.ParseUnionExp();
                if (!this.Match(')'))
                {
                    throw new ArgumentException("expected ')' at position " + pos);
                }

                return e;
            }

            if ((this.Check(RegExpSyntaxOptions.Automaton) || this.Check(RegExpSyntaxOptions.Interval)) && this.Match('<'))
            {
                int start = pos;
                while (this.More() && !this.Peek(">"))
                {
                    this.Next();
                }

                if (!this.Match('>'))
                {
                    throw new ArgumentException("expected '>' at position " + pos);
                }

                string str = b.Substring(start, ((pos - 1) - start));
                int i = str.IndexOf('-');
                if (i == -1)
                {
                    if (!this.Check(RegExpSyntaxOptions.Automaton))
                    {
                        throw new ArgumentException("interval syntax error at position " + (pos - 1));
                    }

                    return RegExp.MakeAutomaton(str);
                }

                if (!this.Check(RegExpSyntaxOptions.Interval))
                {
                    throw new ArgumentException("illegal identifier at position " + (pos - 1));
                }

                try
                {
                    if (i == 0 || i == str.Length - 1 || i != str.LastIndexOf('-'))
                    {
                        throw new FormatException();
                    }

                    string smin = str.Substring(0, i - 0);
                    string smax = str.Substring(i + 1, (str.Length - (i + 1)));
                    int imin = int.Parse(smin, System.Globalization.NumberFormatInfo.CurrentInfo);
                    int imax = int.Parse(smax, System.Globalization.NumberFormatInfo.CurrentInfo);
                    int numdigits = smin.Length == smax.Length ? smin.Length : 0;
                    if (imin > imax)
                    {
                        int t = imin;
                        imin = imax;
                        imax = t;
                    }

                    return RegExp.MakeInterval(imin, imax, numdigits);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("interval syntax error at position " + (pos - 1));
                }
            }

            if (this.Match('\\'))
            {
                // Escaped '\' character.
                if (this.Match('\\'))
                {
                    return MakeChar('\\');
                }

                bool inclusion;

                // Digits.
                if ((inclusion = this.Match('d')) || this.Match('D'))
                {
                    RegExp digitChars = MakeCharRange('0', '9');
                    return inclusion ? digitChars : ExcludeChars(digitChars, MakeAnyPrintableASCIIChar());
                }

                // Whitespace chars only.
                if ((inclusion = this.Match('s')) || this.Match('S'))
                {
                    // Do not add line breaks, as usually RegExp is single line.
                    RegExp whitespaceChars = MakeUnion(MakeChar(' '), MakeChar('\t'));
                    return inclusion ? whitespaceChars : ExcludeChars(whitespaceChars, MakeAnyPrintableASCIIChar());
                }

                // Word character. Range is [A-Za-z0-9_]
                if ((inclusion = this.Match('w')) || this.Match('W'))
                {
                    var ranges = new[] { MakeCharRange('A', 'Z'), MakeCharRange('a', 'z'), MakeCharRange('0', '9') };
                    RegExp wordChars = ranges.Aggregate(MakeChar('_'), MakeUnion);
                    
                    return inclusion ? wordChars : ExcludeChars(wordChars, MakeAnyPrintableASCIIChar());
                }
            }
            
            return RegExp.MakeChar(this.ParseCharExp());
        }

        private void SkipNonCapturingSubpatternExp()
        {
            RegExpMatchingOptions.All().Any(this.Match);
            this.Match(':');
        }

        private char ParseCharExp()
        {
            this.Match('\\');
            return this.Next();
        }

        private RegExp ParseCharClasses()
        {
            RegExp e = this.ParseCharClass();
            while (this.More() && !this.Peek("]"))
            {
                e = RegExp.MakeUnion(e, this.ParseCharClass());
            }

            return e;
        }

        private RegExp ParseCharClass()
        {
            char @char = this.ParseCharExp();
            if (this.Match('-'))
            {
                if (this.Peek("]"))
                {
                    return RegExp.MakeUnion(RegExp.MakeChar(@char), RegExp.MakeChar('-'));
                }

                return RegExp.MakeCharRange(@char, this.ParseCharExp());
            }

            return RegExp.MakeChar(@char);
        }

        private static RegExp ExcludeChars(RegExp exclusion, RegExp allChars)
        {
            return MakeIntersection(allChars, MakeComplement(exclusion));
        }

    }
}