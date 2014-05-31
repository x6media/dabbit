using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;


namespace dabbit.Win
{
    class MultiSelect
    {
        private static readonly PropertyInfo IsSelectionChangeActiveProperty
            = typeof(TreeView).GetProperty
            (
         "IsSelectionChangeActive",
         BindingFlags.NonPublic | BindingFlags.Instance
            );
        public delegate void OnItemsSelectedChange(object sender, List<TreeViewItem> selectedItems);

        public static void AllowMultiSelection(TreeView treeView, OnItemsSelectedChange callback)
        {
            if (IsSelectionChangeActiveProperty == null) return;

            var selectedItems = new List<TreeViewItem>();
            treeView.SelectedItemChanged += (a, b) =>
            {
                var treeViewItem = treeView.SelectedItem as TreeViewItem;
                if (treeViewItem == null) return;

                // allow multiple selection
                // when control key is pressed
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    // suppress selection change notification
                    // select all selected items
                    // then restore selection change notifications
                    var isSelectionChangeActive =
                      IsSelectionChangeActiveProperty.GetValue(treeView, null);

                    IsSelectionChangeActiveProperty.SetValue(treeView, true, null);
                    selectedItems.ForEach(item => item.IsSelected = true);

                    IsSelectionChangeActiveProperty.SetValue
                    (
                      treeView,
                      isSelectionChangeActive,
                      null
                    );
                }
                else
                {
                    // deselect all selected items except the current one
                    selectedItems.ForEach(item => item.IsSelected = (item == treeViewItem));
                    selectedItems.Clear();
                }

                if (!selectedItems.Contains(treeViewItem))
                {
                    selectedItems.Add(treeViewItem);
                }
                else
                {
                    // deselect if already selected
                    treeViewItem.IsSelected = false;
                    selectedItems.Remove(treeViewItem);
                }

                callback(a, selectedItems);
            };

        }
    }
}
