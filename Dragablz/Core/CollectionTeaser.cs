using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dragablz.Core
{
    internal class CollectionTeaser
    {
        private readonly Action<object> m_addMethod;
        private readonly Action<object> m_removeMethod;

        private CollectionTeaser(Action<object> addMethod, Action<object> removeMethod)
        {
            m_addMethod = addMethod;
            m_removeMethod = removeMethod;
        }

        public static bool TryCreate(object items, out CollectionTeaser collectionTeaser)
        {
            collectionTeaser = null;

            var list = items as IList;
            if (list != null)
            {
                collectionTeaser = new CollectionTeaser(i => list.Add(i), list.Remove);
            }
            else if (items != null)
            {
                var itemsType = items.GetType();
                var genericCollectionType = typeof(ICollection<>);

                //TODO, *IF* we really wanted to we could get the consumer to inform us of the correct type
                //if there are multiple impls.  havent got time for this edge case right now though
                var collectionImplType = itemsType.GetInterfaces().SingleOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == genericCollectionType);

                if (collectionImplType != null)
                {
                    var genericArgType = collectionImplType.GetGenericArguments().First();

                    var addMethodInfo = collectionImplType.GetMethod("Add", new[] { genericArgType });
                    var removeMethodInfo = collectionImplType.GetMethod("Remove", new[] { genericArgType });

                    collectionTeaser = new CollectionTeaser(
                        i => addMethodInfo.Invoke(items, new[] { i }),
                        i => removeMethodInfo.Invoke(items, new[] { i }));
                }
            }

            return collectionTeaser != null;
        }

        public void Add(object item)
        {
            m_addMethod(item);
        }

        public void Remove(object item)
        {
            m_removeMethod(item);
        }
    }
}
