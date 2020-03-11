using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{
    public sealed class StateTests
    {
        private readonly ITestOutputHelper _testOutput;

        public StateTests(ITestOutputHelper testOutput)
        {
            this._testOutput = testOutput;
        }
        

        #region Equality / inequality

        [Fact]
        public void ValidateEquality()
        {
            var s1 = new State();

            // Verify instance of state never equals null
            Assert.False(s1.Equals(null));
            Assert.False(null == s1);
            Assert.True(null != s1);

            // Verify 2 refs to the same instance are equal
            var s1Ref = s1;
            Assert.True(s1.Equals(s1Ref));
            Assert.True(s1 == s1Ref);
            Assert.False(s1 != s1Ref);
            // Possibly redundant
            Assert.True(s1Ref.Equals(s1));
            Assert.True(s1Ref == s1);
            Assert.False(s1Ref != s1);
            // Verify using object ref
            Assert.True(s1.Equals((object)s1Ref));
            Assert.True(s1 == (object)s1Ref);
            Assert.False(s1 != (object)s1Ref);



            /* 
                Different instances are considered equal if they have the same Id, Number, and Accept
                BUGBUG: State has no copy ctor, and Id cannot be set, so Number & Accept cmps seems irrelevant.
            */
            // Verify inequality due to differing Id 
            var s2 = new State();   // instantiation should create diff Id
            Assert.True(s1.Id != s2.Id);  // verify Ids differ
            // Actual verifications
            Assert.False(s1.Equals(s2));
            Assert.False(s1 == s2);
            Assert.True(s1 != s2);
        }

        #endregion Hopcroft minimization tests


        #region Comparison tests

        [Fact]
        public void ValidateCompareTo()
        {
            var s1 = new State();
            var s1Ref = s1;
            var s2 = new State();
            
            /*
                IComparable<State>.CompareTo
            */
            // An instance is always greater than null
            Assert.Equal(1, s1.CompareTo((State)null));
            // An instance is always equal to itself
            Assert.Equal(0, s1.CompareTo(s1));
            Assert.Equal(0, s1.CompareTo(s1Ref));
            // Different instances compare by Id values
            Assert.Equal(s2.Id - s1.Id, s1.CompareTo(s2));

            /*
                IComparable.CompareTo
            */
            // An instance is always greater than null
            Assert.Equal(1, s1.CompareTo((object)null));
            // An instance is always equal to itself
            Assert.Equal(0, s1.CompareTo((object)s1));
            Assert.Equal(0, s1.CompareTo((object)s1Ref));
            // Different instances compare by Id values
            Assert.Equal(s2.Id - s1.Id, s1.CompareTo((object)s2));

        }

        #endregion Comparison tests


        #region State Step tests

        [Fact(Skip="WIP")]
        public void StepTests()
        {
            var s1 = new State();
            var s1Ref = s1;
            var s2 = new State();
            
            
            s1.Transitions.Add(null);
            Assert.True(s1.Transitions.Contains(null));
            
            s1.Transitions.Add(null);


            s1.Step((char)0x00, null);
        }

        #endregion State Step tests

        
    }

}