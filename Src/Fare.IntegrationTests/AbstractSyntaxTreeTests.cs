using Fare;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{
    public class AbstractSyntaxTreeTests
    {
        private readonly ITestOutputHelper _testOutput;

        public AbstractSyntaxTreeTests(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }


        [Fact(Skip="WIP")]
        public void Test1()
        {
            var ast = AbstractSyntaxTree.Generate(new RegExp(@"\d{3}\w{2}"));
            _testOutput.WriteLine(ast);

            Assert.True(false);
        }
    }
}