using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dragablz.Core;

namespace Dragablz.Dockablz
{ 
    public class BranchAccessor
    {
        private readonly Branch m_branch;
        private readonly BranchAccessor m_firstItemBranchAccessor;
        private readonly BranchAccessor m_secondItemBranchAccessor;
        private readonly TabablzControl m_firstItemTabablzControl;
        private readonly TabablzControl m_secondItemTabablzControl;

        public BranchAccessor(Branch branch)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));

            m_branch = branch;

            var firstChildBranch = branch.FirstItem as Branch;
            if (firstChildBranch != null)
                m_firstItemBranchAccessor = new BranchAccessor(firstChildBranch);
            else
                m_firstItemTabablzControl = FindTabablzControl(branch.FirstItem, branch.FirstContentPresenter);

            var secondChildBranch = branch.SecondItem as Branch;            
            if (secondChildBranch != null)
                m_secondItemBranchAccessor = new BranchAccessor(secondChildBranch);
            else
                m_secondItemTabablzControl = FindTabablzControl(branch.SecondItem, branch.SecondContentPresenter);
        }

        private static TabablzControl FindTabablzControl(object item, DependencyObject contentPresenter)
        {
            var result = item as TabablzControl;
            return result ?? contentPresenter.VisualTreeDepthFirstTraversal().OfType<TabablzControl>().FirstOrDefault();
        }

        public Branch Branch
        {
            get { return m_branch; }
        }

        public BranchAccessor FirstItemBranchAccessor
        {
            get { return m_firstItemBranchAccessor; }
        }

        public BranchAccessor SecondItemBranchAccessor
        {
            get { return m_secondItemBranchAccessor; }
        }

        public TabablzControl FirstItemTabablzControl
        {
            get { return m_firstItemTabablzControl; }
        }

        public TabablzControl SecondItemTabablzControl
        {
            get { return m_secondItemTabablzControl; }
        }

        /// <summary>
        /// Visits the content of the first or second side of a branch, according to its content type.  No more than one of the provided <see cref="Action"/>
        /// callbacks will be called.  
        /// </summary>
        /// <param name="childItem"></param>
        /// <param name="childBranchVisitor"></param>
        /// <param name="childTabablzControlVisitor"></param>
        /// <param name="childContentVisitor"></param>
        /// <returns></returns>
        public BranchAccessor Visit(BranchItem childItem,
            Action<BranchAccessor> childBranchVisitor = null,
            Action<TabablzControl> childTabablzControlVisitor = null,
            Action<object> childContentVisitor = null)
        {
            Func<BranchAccessor> branchGetter;
            Func<TabablzControl> tabGetter;
            Func<object> contentGetter;

            switch (childItem)
            {
                case BranchItem.First:
                    branchGetter = () => m_firstItemBranchAccessor;
                    tabGetter = () => m_firstItemTabablzControl;
                    contentGetter = () => m_branch.FirstItem;
                    break;
                case BranchItem.Second:
                    branchGetter = () => m_secondItemBranchAccessor;
                    tabGetter = () => m_secondItemTabablzControl;
                    contentGetter = () => m_branch.SecondItem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(childItem));
            }

            var branchDescription = branchGetter();
            if (branchDescription != null)
            {
                if (childBranchVisitor != null)
                    childBranchVisitor(branchDescription);
                return this;
            }
            
            var tabablzControl = tabGetter();
            if (tabablzControl != null)
            {
                if (childTabablzControlVisitor != null)
                    childTabablzControlVisitor(tabablzControl);

                return this;
            }

            if (childContentVisitor == null) return this;

            var content = contentGetter();
            if (content != null)
                childContentVisitor(content);

            return this;
        }
    }    
}
