﻿#region LICENCE

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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TileIconifier.Controls.Shortcut;
using TileIconifier.Core.Shortcut;
using TileIconifier.Core.TileIconify;
using TileIconifier.Core.Utilities;
using TileIconifier.Forms.Shared;
using TileIconifier.Properties;
using TileIconifier.Skinning;
using TileIconifier.Skinning.Skins;
using TileIconifier.Skinning.Skins.Dark;
using TileIconifier.Utilities;

namespace TileIconifier.Forms
{
    public partial class FrmMain
    {
        private void StartFullUpdate()
        {
            FormUtils.DoBackgroundWorkWithSplash(this, FullUpdate, "Refreshing");
        }

        private void FullUpdate(object sender, DoWorkEventArgs e)
        {
            if (getPinnedItemsRequiresPowershellToolStripMenuItem.Checked)
            {
                Exception pinningException;
                _shortcutsList = ShortcutItemEnumeration.TryGetShortcutsWithPinning(out pinningException, true)
                    .Select(s => new ShortcutItemListViewItem(s))
                    .ToList();
                if (pinningException != null)
                {
                    FrmException.ShowExceptionHandler(pinningException);
                    MessageBox.Show(
                        $"A problem occurred with PowerShell functionality. It has been disabled.",
                        @"PowerShell failure", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Invoke(new Action(() => getPinnedItemsRequiresPowershellToolStripMenuItem_Click(this, null)));
                }
            }
            else
            {
                _shortcutsList = ShortcutItemEnumeration.GetShortcuts(true)
                    .Select(s => new ShortcutItemListViewItem(s))
                    .ToList();
            }

            UpdateFilteredList();

            if (srtlstShortcuts.InvokeRequired)
                srtlstShortcuts.Invoke(new Action(BuildShortcutList));
            else
                BuildShortcutList();
        }

        private void BuildShortcutList()
        {
            srtlstShortcuts.Items.Clear();

            var smallImageList = new ImageList();
            for (var i = 0; i < _filteredList.Count; i++)
            {
                var shortcutItem = _filteredList[i];
                srtlstShortcuts.Items.Add(shortcutItem);
                smallImageList.Images.Add(shortcutItem.ShortcutItem.StandardIcon ??
                                          Resources.QuestionMark);
                shortcutItem.ImageIndex = i;
            }
            srtlstShortcuts.SmallImageList = smallImageList;

            if (srtlstShortcuts.Items.Count > 0)
                srtlstShortcuts.Items[0].Selected = true;
        }


        private static void CheckMenuItem(ToolStripDropDownItem mnu,
            ToolStripMenuItem checkedItem)
        {
            // Uncheck the menu items except checked item.
            foreach (var menuItem in mnu.DropDownItems.OfType<ToolStripMenuItem>()
                .Select(item => item))
            {
                menuItem.Checked = Equals(menuItem, checkedItem);
            }
        }

        private void UpdateSkin()
        {
            if (defaultSkinToolStripMenuItem.Checked)
            {
                SkinHandler.SetCurrentSkin(new BaseSkin());
                return;
            }
            if (!darkSkinToolStripMenuItem.Checked) return;
            SkinHandler.SetCurrentSkin(new DarkSkin());
        }

        private void UpdateFormControls()
        {
            //set path boxes to value stored in shortcut
            Action<TextBox, string> updateTextBox = (textBox, str) =>
            {
                textBox.Text = str;
                textBox.SelectionStart = txtLnkPath.Text.Length;
                textBox.ScrollToCaret();
            };
            updateTextBox(txtLnkPath, CurrentShortcutItem.ShortcutFileInfo.FullName);
            updateTextBox(txtExePath, CurrentShortcutItem.TargetFilePath);

            //only show remove if the icon is currently iconified
            btnRemove.Enabled = CurrentShortcutItem.IsIconified;

            //only enable Iconify button if shortcut has unsaved changes
            btnIconify.Enabled = CurrentShortcutItem.Properties.HasUnsavedChanges;

            //disable Build Custom Shortcut for items that are already custom shortcuts
            btnBuildCustomShortcut.Enabled = !CurrentShortcutItem.IsTileIconifierCustomShortcut;

            //disable delete Custom Shortcut for items that are already custom shortcuts
            btnDeleteCustomShortcut.Enabled = CurrentShortcutItem.IsTileIconifierCustomShortcut;

            //update the column view
            _currentShortcutListViewItem.UpdateColumns();
            var currentShortcutIndex = srtlstShortcuts.Items.IndexOf(_currentShortcutListViewItem);
            if (currentShortcutIndex >= 0)
            {
                srtlstShortcuts.RedrawItems(
                    currentShortcutIndex,
                    currentShortcutIndex,
                    false);
            }
        }

        private void UpdateShortcut()
        {
            iconifyPanel.CurrentShortcutItem = CurrentShortcutItem;
            iconifyPanel.UpdateControlsToShortcut();
            UpdateFormControls();
        }

        private void JumpToShortcutItem(ShortcutItem shortcutItem)
        {
            UpdateFilteredList(true);
            var shortcutListViewItem =
                _shortcutsList.First(
                    s => s.ShortcutItem.ShortcutFileInfo.FullName == shortcutItem.ShortcutFileInfo.FullName);
            var itemInListView = srtlstShortcuts.Items[srtlstShortcuts.Items.IndexOf(shortcutListViewItem)];
            itemInListView.Selected = true;
            itemInListView.EnsureVisible();
        }

        private TileIcon GenerateTileIcon()
        {
            return new TileIcon(CurrentShortcutItem);
        }

        private async void CheckForUpdates(bool silentIfNoUpdateDetected)
        {
            try
            {
                var updateDetails = await UpdateUtils.CheckForUpdate();

                if (updateDetails.UpdateAvailable)
                {
                    if (MessageBox.Show(
                        $@"An update is available! Would you like to visit the releases page? (Your version: {
                            updateDetails
                                .CurrentVersion} - Latest version: {updateDetails.LatestVersion})",
                        @"New version available!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) == DialogResult.Yes
                        )
                    {
                        Process.Start("https://github.com/Jonno12345/TileIconifier/releases");
                    }
                }
                else if (!silentIfNoUpdateDetected)
                {
                    MessageBox.Show(@"You are already on the latest version!", @"Up-to-date");
                }
            }
            catch
            {
                if (silentIfNoUpdateDetected) return;

                if (MessageBox.Show(
                    $@"An error occurred getting latest release information. Click Ok to visit the latest releases page to check manually. (Your version: {
                        UpdateUtils
                            .CurrentVersion})",
                    @"Unable to check server!",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    Process.Start("https://github.com/Jonno12345/TileIconifier/releases");
                }
            }
        }

        private void UpdateFilteredList(bool resetTextBox = false)
        {
            if (resetTextBox)
                txtFilter.Text = string.Empty;
            _filteredList = _shortcutsList.Where(s => s.Text.ToUpper().Contains(txtFilter.Text.ToUpper())).ToList();
        }

        private void InitializeListboxColumns()
        {
            srtlstShortcuts.Columns.Clear();
            srtlstShortcuts.Columns.Add("Shortcut Name", srtlstShortcuts.Width/7*4 - 10, HorizontalAlignment.Left);
            srtlstShortcuts.Columns.Add("Is Custom?", srtlstShortcuts.Width/7 - 2, HorizontalAlignment.Left);
            srtlstShortcuts.Columns.Add("Is Iconified?", srtlstShortcuts.Width/7 - 2, HorizontalAlignment.Left);
            srtlstShortcuts.Columns.Add("Is Pinned?", srtlstShortcuts.Width/7 - 4, HorizontalAlignment.Left);
        }
    }
}