using Fare;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{

    public sealed class DotFormatterTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Regex _dagEdgeExpr = new Regex(@"\s->\s");
        private readonly Regex _dagVertexExpr = new Regex(@"S\d+");


        public DotFormatterTests(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }



        [Theory, ClassData(typeof(DotFormatterTestData))]
        public void ExpandedPatternIsCorrect(string pattern, int expectedDagVertices, int expectedDagEdges)
        {
            _testOutput.WriteLine(pattern);

            RegExp exp = new RegExp(pattern, RegExpSyntaxOptions.All);

            var dot = DotFormatter.ToDot(exp.ToAutomaton());
            _testOutput.WriteLine(dot);


            var actualDagVertices = _dagVertexExpr.Matches(dot)
                .Cast<Match>()
                .Select(m => m.Value)       
                .ToList()
                .Distinct()
                .Count();
            _testOutput.WriteLine($"{actualDagVertices} DAG vertices found; {expectedDagVertices} expected.");
            Assert.Equal(expectedDagVertices, actualDagVertices);


            var actualDagEdges = _dagEdgeExpr.Matches(dot)
                .Cast<Match>()
                .ToList()
                .Count();
            _testOutput.WriteLine($"{actualDagEdges} DAG edges found; {expectedDagEdges} expected.");
            Assert.Equal(expectedDagEdges, actualDagEdges);
        }

    }


    /// <summary>Regular expression patterns with values for expected states and transitions in DOT format </summary>
    public class DotFormatterTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {

            /* Shorthand handling
             *  .NET supports: \d, \s, \w
             *  
             */
            yield return new object[] { @"\d", 2, 1 };
            yield return new object[] { @"\d\d", 3, 2 };
            yield return new object[] { @"\d{3}", 4, 3 };

            // SSN
            yield return new object[] { @"\d{3}-\d{2}-\d{4}", 12, 11 };

            // Hex word value
            yield return new object[] { @"0x[0-9A-Fa-f]{4}", 7, 14};


            // Empty regex results in single vertex
            yield return new object[] { @"", 1, 0 };


            yield return new object[] { @"\d\w\d", 4, 6 };
            yield return new object[] { @"\d?\w\d", 5, 10 }; // BUGBUG: functional, but 1 excess node and 1 excess edge; same dot as \d?\w?d
            yield return new object[] { @"\d?\w?\d", 5, 10 };
            // BUGBUG: optionality lost on last element. Viewing digraph in webgraphviz.com indicates final \d is required
            /*
            yield return new object[] { @"\d?\w?\d?", 1, 1 };
            yield return new object[] { @"\d\w?\d?", 1, 1 };
            */
            
            // US Phone number, area code optional
// BUGBUG:            yield return new object[] { @"(\d{3}-)?\d{3}-\d{4}", 1, 1 };

            yield return new object[] { @"(JB|7466)", 6, 6 };


        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}