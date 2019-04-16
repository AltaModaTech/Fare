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
        


        [Theory, ClassData(typeof(ExpressionWithExpansionTestData))]
        public void GeneratedTextIsCorrect(string pattern, string expanded)
        {
            RegExp exp = new RegExp(pattern, RegExpSyntaxOptions.All);

            
            Assert.Equal(expanded, exp.ToString());
        }

    }

}