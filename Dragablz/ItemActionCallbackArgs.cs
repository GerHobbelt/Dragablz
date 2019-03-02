using System;
using System.Windows;

namespace Dragablz
{
    public delegate void ItemActionCallback(ItemActionCallbackArgs<TabablzControl> args);

    public class ItemActionCallbackArgs<TOwner> where TOwner : FrameworkElement
    {
        private readonly Window m_window;
        private readonly TOwner m_owner;
        private readonly DragablzItem m_dragablzItem;

        public ItemActionCallbackArgs(Window window, TOwner owner, DragablzItem dragablzItem)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (dragablzItem == null) throw new ArgumentNullException(nameof(dragablzItem));

            m_window = window;
            m_owner = owner;
            m_dragablzItem = dragablzItem;
        }

        public Window Window
        {
            get { return m_window; }
        }

        public TOwner Owner
        {
            get { return m_owner; }
        }

        public DragablzItem DragablzItem
        {
            get { return m_dragablzItem; }
        }

        public bool IsCancelled { get; private set; }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
