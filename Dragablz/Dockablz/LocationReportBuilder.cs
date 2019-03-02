using System;

namespace Dragablz.Dockablz
{
    internal class LocationReportBuilder
    {
        private readonly TabablzControl m_targetTabablzControl;
        private Branch m_branch;
        private bool m_isSecondLeaf;
        private Layout m_layout;

        public LocationReportBuilder(TabablzControl targetTabablzControl)
        {
            m_targetTabablzControl = targetTabablzControl;
        }

        public TabablzControl TargetTabablzControl
        {
            get { return m_targetTabablzControl; }
        }

        public bool IsFound { get; private set; }

        public void MarkFound()
        {
            if (IsFound)
                throw new InvalidOperationException("Already found.");

            IsFound = true;

            m_layout = CurrentLayout;
        }

        public void MarkFound(Branch branch, bool isSecondLeaf)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));
            if (IsFound)
                throw new InvalidOperationException("Already found.");

            IsFound = true;

            m_layout = CurrentLayout;
            m_branch = branch;
            m_isSecondLeaf = isSecondLeaf;
        }

        public Layout CurrentLayout { get; set; }

        public LocationReport ToLocationReport()
        {
            return new LocationReport(m_targetTabablzControl, m_layout, m_branch, m_isSecondLeaf);
        }
    }
}