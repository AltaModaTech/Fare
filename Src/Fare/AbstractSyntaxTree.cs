using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fare
{

    public class AbstractSyntaxTree
    {
        public static string Generate(Automaton root)
        {
            StringBuilder bldr = new StringBuilder();

            if (null != root)
            {
                bldr.Append(CreateASTLine("root", 0));
                bldr.Append(CreateASTLine("root-1", 1));                
            }

            return bldr.ToString();
        }


        private static string CreateASTLine(string str, uint indent)
        {
            return string.Format("{0}{1}\n", CreateIndentString(indent), str);
        }


        private static string CreateIndentString(uint indent)
        {
            var indention = string.Empty;

            for (uint i = 0; i < indent; ++i)
            {
                indention += "\t";
            }

            return indention;
        }
    }
}