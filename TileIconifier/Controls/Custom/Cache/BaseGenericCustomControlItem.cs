#region LICENCE

// /*
//         The MIT License (MIT)
// 
//         Copyright (c) 2016 Johnathon M
// 
//         Permission is hereby granted, free of charge, to any person obtaining a copy
//         of this software and associated documentation files (the "Software"), to deal
//         in the Software without restriction, including without limitation the rights
//         to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//         copies of the Software, and to permit persons to whom the Software is
//         furnished to do so, subject to the following conditions:
// 
//         The above copyright notice and this permission notice shall be included in
//         all copies or substantial portions of the Software.
// 
//         THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//         IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//         FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//         AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//         LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//         OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//         THE SOFTWARE.
// 
// */

#endregion

using System.IO;
using System.Linq;
using TileIconifier.Core.Custom;
using TileIconifier.Core.Shortcut;

namespace TileIconifier.Controls.Custom.Cache
{
    public class BaseGenericCustomControlItem<T> where T : ICustomBaseItem
    {
        public T BaseItem;
        public CachedShortcutItem CachedShortcutItem = new CachedShortcutItem();

        protected BaseGenericCustomControlItem(T t)
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