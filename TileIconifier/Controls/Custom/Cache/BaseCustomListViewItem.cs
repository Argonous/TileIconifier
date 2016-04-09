using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TileIconifier.Core.Custom;
using TileIconifier.Core.Shortcut;

namespace TileIconifier.Controls.Custom.Cache
{
    //Can this be merged with BaseGenericCustomControlItem...?
    public abstract class BaseCustomListViewItem<T> : ListViewItem where T : ICustomBaseItem
    {
        public CachedShortcutItem CachedShortcutItem = new CachedShortcutItem();
        public T BaseItem;

        protected BaseCustomListViewItem(T t)
        {
            BaseItem = t;
            UpdateCachedShortcutItem(t);
        }

        protected void UpdateCachedShortcutItem(ICustomBaseItem iCustomBaseItem)
        {
            try
            {
                CachedShortcutItem.PreviousCustomShortcut =
                    CustomShortcutLibrary.GetCustomShortcuts()
                        .First(
                            c =>
                                c.ShortcutType == iCustomBaseItem.ShortcutType &&
                                c.TargetArguments == iCustomBaseItem.ExecutionArgument);
                CachedShortcutItem.ShortcutItem = new ShortcutItem(CachedShortcutItem.PreviousCustomShortcut.ShortcutPath);
                CachedShortcutItem.ShortcutName = Path.GetFileNameWithoutExtension(CachedShortcutItem.ShortcutItem.ShortcutFileInfo.Name);
                CachedShortcutItem.ShortcutUser = CachedShortcutItem.ShortcutItem.ShortcutUser;
            }
            catch
            {
                // couldn't load a previous one, make a new one
                CachedShortcutItem.ShortcutName = iCustomBaseItem.DisplayName.CleanInvalidFilenameChars();
                CachedShortcutItem.ShortcutItem.Properties.CurrentState.MediumImage.SetImage(iCustomBaseItem.IconAsBytes, ShortcutConstantsAndEnums.MediumShortcutSize);
                CachedShortcutItem.ShortcutItem.Properties.CurrentState.SmallImage.SetImage(iCustomBaseItem.IconAsBytes, ShortcutConstantsAndEnums.SmallShortcutSize);

                // set these as the 'original' state
                CachedShortcutItem.ShortcutItem.Properties.CommitChanges();
            }
        }
    }
}
