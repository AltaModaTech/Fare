using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{
    public sealed class XegerTests
    {
        private readonly ITestOutputHelper _testOutput;

        public XegerTests(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }

        
        // Verify Xeger generated values match the regex pattern
        [Theory, ClassData(typeof(RegExPatternTestData))]
        public void GeneratedTextIsCorrect(string pattern)
        {
            string[] result = GenerateTextOfPattern(pattern, defaultRepeatCount);

            Assert.All(result, generatedText => Assert.Matches(pattern, generatedText));
        }


#if REX_AVAILABLE
        [Theory, ClassData(typeof(RegExPatternTestData))]
        public void GeneratedTextIsCorrectWithRexEngine(string pattern)
        {
            var settings = new Rex.RexSettings(pattern) { k = 1 };
            // Fix generated value doesn't always meet the pattern
            settings.encoding = Rex.CharacterEncoding.ASCII;

            var result = Enumerable.Repeat(0, defaultRepeatCount)
                .Select(_ =>
                {
                    var generatedValue  = Rex.RexEngine.GenerateMembers(settings).Single();
                    this._testOutput.WriteLine($"Generated value: {generatedValue}");
                    return generatedValue;
                })
                .ToArray();

            Assert.All(result, regex => Assert.Matches(pattern, regex));
        }
#endif


        /* This test verifies that RegExp.ToString provides an accurate representation of the input expression.
         *  RegExp converts the expression to automota, and ToString should be able to provide a functional
         *  equivalent.  If ToString's representation is not a functional match, major causes may be:
         *  1) RegExp's internal representation (automata) is wrong,
         *  2) RegExp's ToString method does not recreate functionally equivalent expression from the 
         *      internal representation.
         */
        // Verify Xeger generated values match the expanded regex pattern
        [Theory(Skip = "BROKEN: pattern expansion not functional match for input pattern"), ClassData(typeof(RegExPatternTestData))]
        public void GeneratedTextIsCorrectWithExpanded(string pattern)
        {
            // Generate matches from input pattern
            string[] result = GenerateTextOfPattern(pattern, defaultRepeatCount);

            // Recreate pattern based on RegExp's internal representation
            var r = new RegExp(pattern);
            var expanded = new RegExp(pattern).ToString();

            // Generated matches should also match the expanded expression
            Assert.All(result, generatedText => Assert.Matches(expanded, generatedText));
        }


        // Uses Xeger to generate multiple values which should match the regex pattern
        private string[] GenerateTextOfPattern(string pattern, int repeatCount)
        {
            var randomSeed = Environment.TickCount;
            this._testOutput.WriteLine($"Random seed: {randomSeed}");

            var random = new Random(randomSeed);

            var sut = new Fare.Xeger(pattern, random);

            // Act
            var result = Enumerable.Repeat(0, repeatCount)
                .Select(_ =>
                {
                    var generatedValue = sut.Generate();
                    this._testOutput.WriteLine($"Generated value {generatedValue} for {pattern}");
                    return generatedValue;
                })
                .ToArray();
            return result;
        }


        private const int defaultRepeatCount = 3;
    }
}