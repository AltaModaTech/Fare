using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{
    public sealed class MinimizationOperationsTests
    {
        private readonly ITestOutputHelper _testOutput;

        public MinimizationOperationsTests(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }
        

        #region Hopcroft minimization tests

        [Theory, ClassData(typeof(RegExPatternTestData))]
        public void HopcroftTest1(string pattern)
        {
            var r = new RegExp(pattern);
            
            var initial = r.ToAutomaton();
            var forMinimize = r.ToAutomaton();

            MinimizationOperations.MinimizeHopcroft(forMinimize);

            Assert.True(AutomatonsAreEquivalent(initial, forMinimize));
        }

        #endregion Hopcroft minimization tests


        private bool AutomatonsAreEquivalent(Automaton a1, Automaton a2)
        {
            bool areEquivalent = false;

            if (null != a2 && null != a2)
            {
                Assert.Equal(a1.IsDebug, a2.IsDebug);
                Assert.Equal(a1.IsDeterministic, a2.IsDeterministic);
                Assert.Equal(a1.IsEmpty, a2.IsEmpty);
                Assert.Equal(a1.IsSingleton, a2.IsSingleton);
                Assert.Equal(a1.NumberOfStates, a2.NumberOfStates);
                Assert.Equal(a1.NumberOfTransitions, a2.NumberOfTransitions);
                Assert.Equal(a1.Singleton, a2.Singleton);
                
                areEquivalent = StatesAreEquivalent(a1.Initial, a2.Initial);
            }
            else if (null == a1 && null == a2)
            {
                areEquivalent = true;
            }

            return areEquivalent;
        }


        private bool StatesAreEquivalent(State s1, State s2)
        {
            bool areEquivalent = false;

            
            if (null != s1 && null != s2)
            {
                Assert.Equal(s1.Accept, s2.Accept);
                Assert.Equal(s1.Number, s2.Number);
                Assert.Equal(s1.Transitions.Count, s2.Transitions.Count);
                
                areEquivalent = true;
            }
            else if (null == s1 && null == s2)
            {
                areEquivalent = true;
            }

            return areEquivalent;
        }

    }

}