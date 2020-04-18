using System;
using Dragablz;

namespace DragablzDemo
{
    public static class BoundExampleNewItem
    {
        public static Func<object, HeaderedItemViewModel> Factory
        {
            get
            {
                return
                    p =>
                    {
                        var dateTime = DateTime.Now;

                        return new HeaderedItemViewModel()
                        {
                            Header = dateTime.ToLongTimeString(),
                            Content = dateTime.ToString("R")
                        };
                    };
            }
        }
    }
}