using System.Windows;
using System.Windows.Controls;

namespace Dragablz
{
    /// <summary>
    /// Selects style to apply to a <see cref="DragablzItem"/> according to the tab item content itself.
    /// </summary>
    public class TabablzItemStyleSelector : StyleSelector
    {
        private readonly Style m_defaultHeaderItemStyle;
        private readonly Style m_customHeaderItemStyle;

        public TabablzItemStyleSelector(Style defaultHeaderItemStyle, Style customHeaderItemStyle)
        {
            m_defaultHeaderItemStyle = defaultHeaderItemStyle;
            m_customHeaderItemStyle = customHeaderItemStyle;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is TabItem) return m_defaultHeaderItemStyle;

            return m_customHeaderItemStyle;
        }
    }
}