using System;

namespace Dragablz.Dockablz
{
  /// <summary>
  /// Provides information about where a tab control is withing a layout structure.
  /// </summary>
  public class LocationReport
  {
    private readonly TabablzControl m_tabablzControl;
    private readonly Layout m_rootLayout;
    private readonly Branch m_parentBranch;
    private readonly bool m_isLeaf;
    private readonly bool m_isSecondLeaf;

    //TODO I've internalised constructor for now, so I can come back and add Window without breaking.

    internal LocationReport(TabablzControl tabablzControl, Layout rootLayout)
      : this(tabablzControl, rootLayout, null, false)
    { }

    internal LocationReport(TabablzControl tabablzControl, Layout rootLayout, Branch parentBranch, bool isSecondLeaf)
    {
      m_tabablzControl = tabablzControl ?? throw new ArgumentNullException(nameof(tabablzControl));
      m_rootLayout = rootLayout ?? throw new ArgumentNullException(nameof(rootLayout));
      m_parentBranch = parentBranch;
      m_isLeaf = m_parentBranch != null;
      m_isSecondLeaf = isSecondLeaf;
    }

    public TabablzControl TabablzControl => m_tabablzControl;

    public Layout RootLayout => m_rootLayout;

    /// <summary>
    /// Gets the parent branch if this is a leaf. If the <see cref="TabablzControl"/> is directly under the <see cref="RootLayout"/> will be <c>null</c>.
    /// </summary>
    public Branch ParentBranch => m_parentBranch;

    /// <summary>
    /// Idicates if this is a leaf in a branch. <c>True</c> if <see cref="ParentBranch"/> is not null.
    /// </summary>
    public bool IsLeaf => m_isLeaf;

    public bool IsSecondLeaf => m_isSecondLeaf;
  }
}