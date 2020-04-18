using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Dragablz
{
    internal class StoryboardCompletionListener
    {
        private readonly Storyboard m_storyboard;
        private readonly Action<Storyboard> m_continuation;

        public StoryboardCompletionListener(Storyboard storyboard, Action<Storyboard> continuation)
        {
            if (storyboard == null) throw new ArgumentNullException(nameof(storyboard));
            if (continuation == null) throw new ArgumentNullException(nameof(continuation));

            m_storyboard = storyboard;
            m_continuation = continuation;

            m_storyboard.Completed += StoryboardOnCompleted;
        }

        private void StoryboardOnCompleted(object sender, EventArgs eventArgs)
        {
            m_storyboard.Completed -= StoryboardOnCompleted;
            m_continuation(m_storyboard);
        }
    }

    internal static class StoryboardCompletionListenerExtension
    {
        private static readonly IDictionary<Storyboard, Action<Storyboard>> CONTINUATION_INDEX = new Dictionary<Storyboard, Action<Storyboard>>();

        public static void WhenComplete(this Storyboard storyboard, Action<Storyboard> continuation)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new StoryboardCompletionListener(storyboard, continuation);
        }
    }
}