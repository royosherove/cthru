using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CThru.Silverlight;
using Microsoft.Windows.Controls;
using NUnit.Framework;



namespace ControlTests.CS
{
    [TestFixture]
    public class AutoCompleteBoxTests
    {

        [Test,SilverlightUnitTest]
        public void SetProperty()
        {
            var box = new AutoCompleteBox();
            box.MinimumPrefixLength = -3;
            int length =  (int) box.MinimumPrefixLength;
            Assert.AreEqual(-1,length);
        }
        
        [Test,SilverlightUnitTest]
        public void SetChildTemplate()
        {
            var box = new AutoCompleteBox();
            SilverUnit.SetTemplateChild(box, "Text", new TextBox());
            SilverUnit.SetTemplateChild(box, "Popup", new Popup()); 
            SilverUnit.SetTemplateChild(box, "SelectionAdapter", new ListBox());
            SilverUnit.SetTemplateChild(box, "DropDownToggle", new ToggleButton());

            box.OnApplyTemplate();
            SilverUnit.Assert.VisualStatedWasChangedTo("Normal",box,false);
        }
    }
}
