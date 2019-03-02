using System;

namespace Dragablz.Dockablz
{
    public class BranchResult
    {
        private readonly Branch m_branch;
        private readonly TabablzControl m_tabablzControl;

        public BranchResult(Branch branch, TabablzControl tabablzControl)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));
            if (tabablzControl == null) throw new ArgumentNullException(nameof(tabablzControl));
            
            m_branch = branch;
            m_tabablzControl = tabablzControl;
        }

        /// <summary>
        /// The new branch.
        /// </summary>
        public Branch Branch
        {
            get { return m_branch; }
        }

        /// <summary>
        /// The new tab control.
        /// </summary>
        public TabablzControl TabablzControl
        {
            get { return m_tabablzControl; }
        }
    }
}