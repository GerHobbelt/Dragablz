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

        public object Item => m_item;

        public object Context => m_context;

        public AddLocationHint AddLocationHint => m_addLocationHint;
    }
}