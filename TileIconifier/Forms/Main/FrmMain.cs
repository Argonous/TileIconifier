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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TileIconifier.Controls.Shortcut;
using TileIconifier.Core.Custom;
using TileIconifier.Core.Shortcut;
using TileIconifier.Forms.CustomShortcutForms;
using TileIconifier.Utilities;

namespace TileIconifier.Forms
{
    public partial class FrmMain : SkinnableForm
    {
        private ShortcutItemListViewItem _currentShortcutListViewItem;
        private List<ShortcutItemListViewItem> _filteredList;
        private List<ShortcutItemListViewItem> _shortcutsList;

        public FrmMain()
        {
            InitializeComponent();
        }

        private ShortcutItem CurrentShortcutItem => _currentShortcutListViewItem.ShortcutItem;

        protected override void ApplySkin(object sender, EventArgs e)
        {
            base.ApplySkin(sender, e);
            iconifyPanel.UpdateSkinColors(CurrentBaseSkin);
        }

        private void frmDropper_Load(object sender, EventArgs e)
        {
            darkSkinToolStripMenuItem.Click += SkinToolStripMenuClick;
            defaultSkinToolStripMenuItem.Click += SkinToolStripMenuClick;
            iconifyPanel.OnIconifyPanelUpdate += (s, ev) => { UpdateFormControls(); };

            CheckForUpdates(true);
            InitializeListboxColumns();

            Show();
            StartFullUpdate();
        }

        private void btnIconify_Click(object sender, EventArgs e)
        {
            if (!iconifyPanel.DoValidation())
                return;

            var showForegroundColourWarning = CurrentShortcutItem.Properties.ForegroundTextColourChanged;
            var tileIconify = GenerateTileIcon();
            tileIconify.RunIconify();
            CurrentShortcutItem.Properties.CommitChanges();
            UpdateShortcut();
            if (showForegroundColourWarning)
                MessageBox.Show(
                    @"Foreground colour changes don't always instantly apply. If this change hasn't applied, try unpinning and repinning the shortcut.",
                    @"Foreground Colour Change", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you wish to remove iconification?", @"Confirm", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var tileDeIconify = GenerateTileIcon();
            tileDeIconify.DeIconify();
            CurrentShortcutItem.Properties.ResetParameters();
            UpdateShortcut();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void getPinnedItemsRequiresPowershellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getPinnedItemsRequiresPowershellToolStripMenuItem.Checked)
            {
                if (InvokeRequired)
                    Invoke(new Action(() => { getPinnedItemsRequiresPowershellToolStripMenuItem.Checked = false; }));
                else
                    getPinnedItemsRequiresPowershellToolStripMenuItem.Checked = false;
            }
            else
            {
                if (
                    MessageBox.Show(
                        @"Note- This feature uses Powershell and may take slightly longer to refresh. Continue?",
                        @"Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    getPinnedItemsRequiresPowershellToolStripMenuItem.Checked = true;
                }
            }
            StartFullUpdate();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtils.ShowCenteredDialogForm<FrmAbout>(this);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtils.ShowCenteredDialogForm<FrmHelp>(this);
        }

        private void customShortcutManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var customShortcutManager = new FrmCustomShortcutManagerMain())
            {
                customShortcutManager.ShowDialog(this);
                StartFullUpdate();
                if (customShortcutManager.GotoShortcutItem != null)
                    JumpToShortcutItem(customShortcutManager.GotoShortcutItem);
            }
        }

        private void refreshAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartFullUpdate();
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates(false);
        }

        private void SkinToolStripMenuClick(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            CheckMenuItem(skinToolStripMenuItem, item);
            UpdateSkin();
        }

        private void srtlstShortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (srtlstShortcuts.SelectedItems.Count != 1)
                return;

            _currentShortcutListViewItem = (ShortcutItemListViewItem) srtlstShortcuts.SelectedItems[0];
            UpdateShortcut();
        }

        private void btnBuildCustomShortcut_Click(object sender, EventArgs e)
        {
            var shortcutName = Path.GetFileNameWithoutExtension(CurrentShortcutItem.ShortcutFileInfo.Name).CleanInvalidFilenameChars();

            if (CurrentShortcutItem.IsTileIconifierCustomShortcut) return;

            var cloneConfirmation = new FrmCustomShortcutConfirm()
            {
                ShortcutName = shortcutName
            };
            if (cloneConfirmation.ShowDialog(this) != DialogResult.OK) return;

            shortcutName = cloneConfirmation.ShortcutName;

            var customShortcut =
                new CustomShortcut(shortcutName, CurrentShortcutItem.TargetFilePath, "",
                    CustomShortcutType.Other, WindowType.ActiveAndCurrent,
                    CustomShortcutGetters.CustomShortcutAllUsersPath, null,
                    CurrentShortcutItem.ShortcutFileInfo.Directory?.FullName);

            customShortcut.BuildCustomShortcut();

            StartFullUpdate();

            JumpToShortcutItem(customShortcut.ShortcutItem);

            //confirm to the user the shortcut has been created
            MessageBox.Show(
                $"A shortcut for {shortcutName.QuoteWrap()} has been created in your start menu under TileIconify. The item will need to be pinned manually.",
                @"Shortcut created!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //new CustomShortcut(Path.GetFileNameWithoutExtension(CurrentShortcutItem.ShortcutFileInfo.Name),
            //    CurrentShortcutItem.TargetFilePath, "", CustomShortcutType.Other, 
            //    WindowType.ActiveAndCurrent,
            //    ShortcutUser.CurrentUser, string.Empty, CurrentShortcutItem.ShortcutFileInfo.Directory.FullName);
        }

        private void btnDeleteCustomShortcut_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    $"Are you sure you wish to delete the custom shortcut for {CurrentShortcutItem.ShortcutFileInfo.Name.QuoteWrap()}?",
                    @"Are you sure?",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            try
            {
                var customShortcut = CustomShortcut.Load(CurrentShortcutItem.TargetFilePath);
                customShortcut.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Unable to clear up shortcuts." + ex);
            }

            StartFullUpdate();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateFilteredList();
            BuildShortcutList();
            UpdateShortcut();
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this,
                @"Donations for this application will NEVER be required but would be greatly appreciated. This application will ALWAYS be free, open source, and supported to the best of my ability. If you would like to help support me with this and future applications I would be very grateful!

Do you wish to proceed to the donation page (https://www.paypal.me/Jonno12345)?",
                @"Donation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Process.Start("https://www.paypal.me/Jonno12345");
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            InitializeListboxColumns();
        }
    }
}