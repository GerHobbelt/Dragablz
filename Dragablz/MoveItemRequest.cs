namespace Dragablz
{
    public class MoveItemRequest
    {
        private readonly object m_item;
        private readonly object m_context;
        private readonly AddLocationHint m_addLocationHint;

        public MoveItemRequest(object item, object context, AddLocationHint addLocationHint)
        {
            m_item = item;
            m_context = context;
            m_addLocationHint = addLocationHint;
        }

        public object Item
        {
            get { return m_item; }
        }

        public object Context
        {
            get { return m_context; }
        }

        public AddLocationHint AddLocationHint
        {
            get { return m_addLocationHint; }
        }
    }
}