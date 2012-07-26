// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.Primitives;

namespace Microsoft.Windows.Controls.Testing
{
   
    public class OverriddenSelectionAdapter : SelectorSelectionAdapter
    {
        public static OverriddenSelectionAdapter Current { get; set; }

        public OverriddenSelectionAdapter(Selector selector)
            : base(selector)
        {
            Current = this;
        }

        public void SelectFirst()
        {
            SelectorControl.SelectedIndex = 0;
        }

        public void SelectNext()
        {
            SelectedIndexIncrement();
        }

        public void SelectPrevious()
        {
            SelectedIndexDecrement();
        }

        public void TestCommit()
        {
            OnCommit();
        }

        public void TestCancel()
        {
            OnCancel();
        }
    }
}