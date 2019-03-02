using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dragablz
{
    /// <summary>
    /// Helper class to create view models, particularly for tool/MDI windows.
    /// </summary>
    public class HeaderedItemViewModel : INotifyPropertyChanged
    {
        private bool m_isSelected;
        private object m_header;
        private object m_content;

        public HeaderedItemViewModel()
        {
        }

        public HeaderedItemViewModel(object header, object content, bool isSelected = false)
        {
            m_header = header;
            m_content = content;
            m_isSelected = isSelected;
        }

        public object Header
        {
            get { return m_header; }
            set
            {
                if (m_header == value) return;
                m_header = value;
#if NET40
                OnPropertyChanged("Header");
#endif
#if NET45
                OnPropertyChanged();
#endif
            }
        }

        public object Content
        {
            get { return m_content; }
            set
            {
                if (m_content == value) return;
                m_content = value;
#if NET40
                OnPropertyChanged("Content");
#endif
#if NET45
                OnPropertyChanged();
#endif
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected == value) return;
                m_isSelected = value;
#if NET40
                OnPropertyChanged("IsSelected");
#endif
#if NET45
                OnPropertyChanged();
#endif
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

#if NET40
        protected virtual void OnPropertyChanged(string propertyName)
#endif
#if NET45
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
#endif
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
