using Fare;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{

    public sealed class DumpTree
    {
        private readonly ITestOutputHelper _testOutput;

        public DumpTree(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }


        [Fact(Skip = "Only used for dumping info (not a test)")]
        public void DumpAST()
        {

            string rawRegEx = @"\d{3}\d3}";

            Fare.RegExp e = new Fare.RegExp(rawRegEx);
            var a = e.ToAutomaton();

            _testOutput.WriteLine($"expr: {rawRegEx} has {a.NumberOfStates} states and {a.NumberOfTransitions} transistions");
            
            foreach (var state in a.GetStates())
            {
                _testOutput.WriteLine($"\tState (Id:{state.Id}, Number:{state.Number}) has {state.Transitions.Count} transitions" );
                foreach (var trans in state.GetSortedTransitions(false))
                {
                    _testOutput.WriteLine($"\t\tTransition to: {trans.To.Id} [{trans.Min} - {trans.Max}]" );
                }
            }

            // fail test to get output
            Assert.True(false);

        }


        [Fact(Skip = "Only used for dumping info (not a test)")]
        public void DumpRegEx()
        {

            string rawRegEx = @"\d{3}\d3}";

            Fare.RegExp e = new Fare.RegExp(rawRegEx);

            _testOutput.WriteLine(e.ToString());
            

            // fail test to get output
            Assert.True(false);

        }



        
    }

}