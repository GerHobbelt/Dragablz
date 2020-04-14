namespace Dragablz.Core
{
    /// <summary>
    /// Non-client hit test values, HT*
    /// </summary>
    internal enum HitTest
    {
        HtError = -2,
        HtTransparent = -1,
        HtNowhere = 0,
        HtClient = 1,
        HtCaption = 2,
        HtSysmenu = 3,
        HtGrowbox = 4,
        HtMenu = 5,
        HtHscroll = 6,
        HtVscroll = 7,
        HtMinbutton = 8,
        HtMaxbutton = 9,
        HtLeft = 10,
        HtRight = 11,
        HtTop = 12,
        HtTopleft = 13,
        HtTopright = 14,
        HtBottom = 15,
        HtBottomleft = 16,
        HtBottomright = 17,
        HtBorder = 18,
        HtObject = 19,
        HtClose = 20,
        HtHelp = 21
    }
}