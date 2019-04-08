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


        [Fact]
        public void Test1()
        {
            var automaton1 = new RegExp(@"\d{3}").ToAutomaton();

            var ast = AbstractSyntaxTree.Generate(automaton1);
            _testOutput.WriteLine(ast);

            Assert.True(false);
        }
    }
}