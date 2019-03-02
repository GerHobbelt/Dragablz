using System;
using System.Collections.Generic;

namespace Dragablz.Dockablz
{
    /// <summary>
    /// Provides information about the <see cref="Layout"/> instance.
    /// </summary>
    public class LayoutAccessor
    {
        private readonly Layout m_layout;
        private readonly BranchAccessor m_branchAccessor;
        private readonly TabablzControl m_tabablzControl;

        public LayoutAccessor(Layout layout)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            
            m_layout = layout;

            var branch = Layout.Content as Branch;
            if (branch != null)
                m_branchAccessor = new BranchAccessor(branch);
            else            
                m_tabablzControl = Layout.Content as TabablzControl;            
        }

        public Layout Layout
        {
            get { return m_layout; }
        }

        public IEnumerable<DragablzItem> FloatingItems
        {
            get { return m_layout.FloatingDragablzItems(); }
        }

        /// <summary>
        /// <see cref="BranchAccessor"/> and <see cref="TabablzControl"/> are mutually exclusive, according to whether the layout has been split, or just contains a tab control.
        /// </summary>
        public BranchAccessor BranchAccessor
        {
            get { return m_branchAccessor; }
        }

        /// <summary>
        /// <see cref="BranchAccessor"/> and <see cref="TabablzControl"/> are mutually exclusive, according to whether the layout has been split, or just contains a tab control.
        /// </summary>
        public TabablzControl TabablzControl
        {
            get { return m_tabablzControl; }
        }

        /// <summary>
        /// Visits the content of the layout, according to its content type.  No more than one of the provided <see cref="Action"/>
        /// callbacks will be called.  
        /// </summary>        
        public LayoutAccessor Visit(
            Action<BranchAccessor> branchVisitor = null,
            Action<TabablzControl> tabablzControlVisitor = null,
            Action<object> contentVisitor = null)
        {
            if (m_branchAccessor != null)
            {
                if (branchVisitor != null)
                {
                    branchVisitor(m_branchAccessor);
                }
                    
                return this;
            }

            if (m_tabablzControl != null)
            {
                if (tabablzControlVisitor != null)
                    tabablzControlVisitor(m_tabablzControl);

                return this;
            }

            if (m_layout.Content != null && contentVisitor != null)
                contentVisitor(m_layout.Content);

            return this;
        }

        /// <summary>
        /// Gets all the Tabablz controls in a Layout, regardless of location.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TabablzControl> TabablzControls()
        {
            var tabablzControls = new List<TabablzControl>();
            this.Visit(tabablzControls, BranchAccessorVisitor, TabablzControlVisitor);
            return tabablzControls;
        }

        private static void TabablzControlVisitor(IList<TabablzControl> resultSet, TabablzControl tabablzControl)
        {
            resultSet.Add(tabablzControl);
        }

        private static void BranchAccessorVisitor(IList<TabablzControl> resultSet, BranchAccessor branchAccessor)
        {
            branchAccessor
                .Visit(resultSet, BranchItem.First, BranchAccessorVisitor, TabablzControlVisitor)
                .Visit(resultSet, BranchItem.Second, BranchAccessorVisitor, TabablzControlVisitor);
        }
    }
}