using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CThru.Silverlight;
using Microsoft.Windows.Controls;
using NUnit.Framework;



namespace ControlTests.CS
{
    [TestFixture]
    public class StackPanelTests
    {

        [Test,SilverlightUnitTest]
        public void SetProperty()
        {
            var panel = new StackPanel();
            Assert.AreEqual(0,panel.Children.Count);
        }
        
        [Test,SilverlightUnitTest]
        public void AddChildren()
        {
            var panel = new StackPanel();
            panel.Children.Add(new TextBox());
            Assert.AreEqual(1,panel.Children.Count);
        }
        
        [Test,SilverlightUnitTest]
        public void AddChildrenAndRemove()
        {
            var panel = new StackPanel();
            var value = new TextBox();
            panel.Children.Add(value);
            panel.Children.Remove(value);
            Assert.AreEqual(0,panel.Children.Count);
        }
    }
}
