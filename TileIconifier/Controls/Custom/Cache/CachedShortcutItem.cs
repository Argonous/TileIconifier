using TileIconifier.Core.Custom;
using TileIconifier.Core.Shortcut;

namespace TileIconifier.Controls.Custom.Cache
{
    public class CachedShortcutItem
    {
        public ShortcutItem ShortcutItem = new ShortcutItem();
        public string ShortcutName;
        public ShortcutUser ShortcutUser = ShortcutUser.AllUsers;
        public CustomShortcut PreviousCustomShortcut;
    }
}
