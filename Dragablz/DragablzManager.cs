using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dragablz.Dockablz;

namespace Dragablz
{
  public class TabSelectionEventArgs : EventArgs
  {
    public object Content { get; set; }

    public TabSelectionEventArgs(object content)
    {
      Content = content;
    }
  }

  public static class DragablzManager
  {
    public static readonly HashSet<Layout> LOADED_LAYOUTS = new HashSet<Layout>();
    public static readonly HashSet<TabablzControl> LOADED_TABABLZ_INSTANCES = new HashSet<TabablzControl>();
    public static readonly HashSet<TabablzControl> VISIBLE_TABABLZ_INSTANCES = new HashSet<TabablzControl>();

    public static void LoadTabablzInstance(TabablzControl tabablzControl)
    {
      LOADED_TABABLZ_INSTANCES.Add(tabablzControl);
      tabablzControl.SelectionChanged += TabablzControlOnSelectionChanged;
    }

    public static void UnloadTabablzInstrance(TabablzControl tabablzControl)
    {
      LOADED_TABABLZ_INSTANCES.Remove(tabablzControl);
      tabablzControl.SelectionChanged -= TabablzControlOnSelectionChanged;
    }

    private static void TabablzControlOnSelectionChanged(object sender, TabSelectionEventArgs selectionChangedEventArgs)
    {
      OnTabSelectionChanged(sender,selectionChangedEventArgs);
    }

    public static event EventHandler<TabSelectionEventArgs> TabSelectionChanged;

    private static void OnTabSelectionChanged(object sender, TabSelectionEventArgs e)
      => TabSelectionChanged?.Invoke(sender, e);
  }
}
