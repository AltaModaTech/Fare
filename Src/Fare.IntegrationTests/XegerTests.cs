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

        
        [Theory, ClassData(typeof(RegExPatternTestData))]
        public void GeneratedTextIsCorrect(string pattern)
        {
            string[] result = GenerateTestOfPattern(pattern);

            Assert.All(result, regex => Assert.Matches(pattern, regex));
        }

#if REX_AVAILABLE
        [Theory, ClassData(typeof(RegExPatternTestData))]
        public void GeneratedTextIsCorrectWithRexEngine(string pattern)
        {
            const int repeatCount = 3;
            var settings = new Rex.RexSettings(pattern) { k = 1 };
            // Fix generated value doesn't always meet the pattern
            settings.encoding = Rex.CharacterEncoding.ASCII;

            var result = Enumerable.Repeat(0, repeatCount)
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

        [Theory(Skip = "BROKEN: pattern expansion not functional match for input pattern"), ClassData(typeof(RegExPatternTestData))]
        public void GeneratedTextIsCorrectWithExpanded(string pattern)
        {
            string[] result = GenerateTestOfPattern(pattern);

            // Assert
            Assert.All(result, regex => Assert.Matches(pattern, regex));

            var r = new RegExp(pattern);
            var expanded = new RegExp(pattern).ToString();
            Assert.All(result, regex => Assert.Matches(expanded, regex));

        }


        private string[] GenerateTestOfPattern(string pattern)
        {
            const int repeatCount = 3;

            var randomSeed = Environment.TickCount;
            this._testOutput.WriteLine($"Random seed: {randomSeed}");

            var random = new Random(randomSeed);

            var sut = new Fare.Xeger(pattern, random);

            // Act
            var result = Enumerable.Repeat(0, repeatCount)
                .Select(_ =>
                {
                    var generatedValue = sut.Generate();
                    this._testOutput.WriteLine($"Generated value: {generatedValue}");
                    return generatedValue;
                })
                .ToArray();
            return result;
        }
    }
}