using System.Windows.Controls;
using CThru.Silverlight;
using NUnit.Framework;

namespace $safeprojectname$
{
    [TestFixture]
    public class ControlTests1
    {
        //NEXT STEPS:
        //1. Add a reference to a silverlight project with logic you'd like to test.
        //2. Write tests against objects using only their logic. No WPF, XAML or silverlight runtime configuration needed.
        // see the example test below for a simple example of testing a custom control


        /// <summary>
        /// An example test against a silverlight AutoCompleteBox control.
        /// Test name follows this convention:
        ///  [FunctionalityUnderTest]_[Scenario]_[ExpectedBehavior]()
        /// </summary>
        [Test,SilverlightUnitTest]
        public void MinimumPrefixLength_SetToInvalidValue_DefaultsToMinusOne()
        {
            AutoCompleteBox box = new AutoCompleteBox();
            
            box.MinimumPrefixLength = -3;
            Assert.AreEqual(-1,box.MinimumPrefixLength);
        }
    }
}