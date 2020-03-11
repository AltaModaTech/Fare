using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Fare.IntegrationTests
{
    public sealed class RegExTests
    {
        private readonly ITestOutputHelper _testOutput;

        public RegExTests(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }
        

        /*
         *  Verifies RegExp reproduces an equivalent, expanded representation after 
         *   converting input pattern to its internal automata.  This helps ensure
         *   the conversion to automata is accurate for generating text to match
         *   the pattern.
         */
        [Theory, ClassData(typeof(ExpressionWithExpansionTestData))]
        public void ExpandedPatternIsCorrect(string pattern, string expanded)
        {
            RegExp exp = new RegExp(pattern, RegExpSyntaxOptions.All);


            Assert.Equal(expanded, exp.ToString());
        }

    }

}