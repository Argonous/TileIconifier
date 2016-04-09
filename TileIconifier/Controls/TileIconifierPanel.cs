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
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TileIconifier.Controls.PictureBox;
using TileIconifier.Core;
using TileIconifier.Core.Shortcut;
using TileIconifier.Core.Utilities;
using TileIconifier.Forms.Shared;
using TileIconifier.Skinning.Skins;

namespace TileIconifier.Controls
{
    public partial class TileIconifierPanel : UserControl
    {
        private readonly List<PannablePictureBoxMetaData> _pannablePictureBoxMetaDatas =
            new List<PannablePictureBoxMetaData>();

        private BaseSkin _currentBaseSkin;

        public TileIconifierPanel()
        {
            InitializeComponent();
            AddEventHandlers();
        }

        public ShortcutItem CurrentShortcutItem { get; set; }

        public event EventHandler OnIconifyPanelUpdate;

        public void UpdateControlsToShortcut()
        {
            if (CurrentShortcutItem == null) return;
            //disable event handlers whilst updating things programatically
            RemoveEventHandlers();

            //check if unsaved once per update
            var hasUnsavedChanges = CurrentShortcutItem.Properties.HasUnsavedChanges;

            //update the picture boxes to show the relevant images
            panPctMediumIcon.SetImage(CurrentShortcutItem.MediumImage(),
                CurrentShortcutItem.Properties.CurrentState.MediumImage.Width,
                CurrentShortcutItem.Properties.CurrentState.MediumImage.Height,
                CurrentShortcutItem.Properties.CurrentState.MediumImage.X,
                CurrentShortcutItem.Properties.CurrentState.MediumImage.Y);

            panPctSmallIcon.SetImage(CurrentShortcutItem.SmallImage(),
                CurrentShortcutItem.Properties.CurrentState.SmallImage.Width,
                CurrentShortcutItem.Properties.CurrentState.SmallImage.Height,
                CurrentShortcutItem.Properties.CurrentState.SmallImage.X,
                CurrentShortcutItem.Properties.CurrentState.SmallImage.Y);

            //set the assosciatedShortcutItemImages for each picturebox
            GetSenderPictureBoxToMetaData(panPctMediumIcon).ShortcutItemImage =
                CurrentShortcutItem.Properties.CurrentState.MediumImage;
            GetSenderPictureBoxToMetaData(panPctSmallIcon).ShortcutItemImage =
                CurrentShortcutItem.Properties.CurrentState.SmallImage;

            //update the picture box control panels
            pannablePictureBoxControlPanelMedium.UpdateControls();
            pannablePictureBoxControlPanelSmall.UpdateControls();

            //set relevant unsaved changes controls to required visibility/enabled states
            lblUnsaved.Visible = hasUnsavedChanges;
            btnReset.Enabled = hasUnsavedChanges;

            //reset the combo box - choose actual colour, or custom if none of the combobox items
            if (cmbColour.Items.Contains(CurrentShortcutItem.Properties.CurrentState.BackgroundColor))
            {
                cmbColour.SelectedItem = CurrentShortcutItem.Properties.CurrentState.BackgroundColor;
                txtBGColour.Enabled = false;
            }
            else
            {
                cmbColour.Text = @"Custom";
                txtBGColour.Enabled = true;
                txtBGColour.Text = CurrentShortcutItem.Properties.CurrentState.BackgroundColor;
            }

            //set the foreground text checkbox based on value stored for this shortcut
            chkFGTxtEnabled.Checked = CurrentShortcutItem.Properties.CurrentState.ShowNameOnSquare150X150Logo;

            //enable radio buttons if the foreground text is enabled
            radFGDark.Enabled = chkFGTxtEnabled.Checked;
            radFGLight.Enabled = chkFGTxtEnabled.Checked;

            //set the radio buttons based on the current shortcuts selection
            radFGDark.Checked = CurrentShortcutItem.Properties.CurrentState.ForegroundText == "dark";
            radFGLight.Checked = CurrentShortcutItem.Properties.CurrentState.ForegroundText == "light";

            //reset any validation failures
            ResetValidation();

            //re-add the event handlers now we've finished updating
            AddEventHandlers();
        }

        public void SetPictureBoxesBackColor()
        {
            var color = GetPictureBoxesBackColor();
            Action<PannablePictureBox> setBackColor = b =>
            {
                b.BackColor = b.PannablePictureBoxImage.Image == null ? _currentBaseSkin.BackColor : color;
                b.Refresh();
            };
            setBackColor(panPctMediumIcon);
            setBackColor(panPctSmallIcon);
        }


        public bool DoValidation()
        {
            ResetValidation();

            return ValidateColour();
        }

        public void UpdateSkinColors(BaseSkin currentBaseSkin)
        {
            _currentBaseSkin = currentBaseSkin;
            lblUnsaved.ForeColor = _currentBaseSkin.ErrorColor;
            SetPictureBoxesBackColor();
        }

        private void RunUpdate()
        {
            UpdateControlsToShortcut();
            OnIconifyPanelUpdate?.Invoke(this, null);
        }

        private void IconSet(object sender)
        {
            byte[] imageBytes;
            try
            {
                imageBytes = FrmIconSelector.GetImage(this, CurrentShortcutItem?.TargetFilePath);
            }
            catch (UserCancellationException)
            {
                return;
            }

            var pictureBoxMetaDataToUse = chkUseSameImg.Checked
                ? _pannablePictureBoxMetaDatas
                : new List<PannablePictureBoxMetaData> {GetSenderPictureBoxToMetaData(sender)};

            foreach (var pictureBoxMetaData in pictureBoxMetaDataToUse)
            {
                pictureBoxMetaData.ShortcutItemImage.SetImage(imageBytes, pictureBoxMetaData.Size);
                pictureBoxMetaData.PannablePictureBox.CenterImage();
            }

            RunUpdate();
        }

        private PannablePictureBoxMetaData GetSenderPictureBoxToMetaData(object sender)
        {
            PannablePictureBox senderPictureBox = null;
            if (sender.GetType() == typeof (PannablePictureBoxControlPanel))
                senderPictureBox = ((PannablePictureBoxControlPanel) sender).PannablePictureBox;
            if (sender.GetType() == typeof (PannablePictureBox))
                senderPictureBox = (PannablePictureBox) sender;
            if (senderPictureBox == null)
                throw new InvalidCastException($@"Sender not valid type! Received {sender.GetType()}");

            return _pannablePictureBoxMetaDatas.Single(p => p.PannablePictureBox == senderPictureBox);
        }

        private void BuildPannableShortcutBoxControlPanels()
        {
            pannablePictureBoxControlPanelMedium.SetPannablePictureBoxControl(panPctMediumIcon);
            pannablePictureBoxControlPanelSmall.SetPannablePictureBoxControl(panPctSmallIcon);
            pannablePictureBoxControlPanelMedium.ChangeImageClick += (sender, args) => { IconSet(sender); };
            pannablePictureBoxControlPanelSmall.ChangeImageClick += (sender, args) => { IconSet(sender); };
            pannablePictureBoxControlPanelMedium.UpdateTrackBarAndZoom();
            pannablePictureBoxControlPanelSmall.UpdateTrackBarAndZoom();
        }

        private void TileIconifierPanel_Load(object sender, EventArgs e)
        {
            SetupPannablePictureBoxes();
            BuildPannableShortcutBoxControlPanels();
        }

        private void SetupPannablePictureBoxes()
        {
            _pannablePictureBoxMetaDatas.Add(new PannablePictureBoxMetaData
            {
                PannablePictureBox = panPctMediumIcon,
                Size = ShortcutConstantsAndEnums.MediumShortcutSize
            });
            _pannablePictureBoxMetaDatas.Add(new PannablePictureBoxMetaData
            {
                PannablePictureBox = panPctSmallIcon,
                Size = ShortcutConstantsAndEnums.SmallShortcutSize
            });
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            CurrentShortcutItem.Properties.UndoChanges();

            RunUpdate();
        }

        private void radFGLight_CheckedChanged(object sender, EventArgs e)
        {
            CurrentShortcutItem.Properties.CurrentState.ForegroundText = radFGLight.Checked ? "light" : "dark";

            RunUpdate();
        }

        private void txtBGColour_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length != textBox.MaxLength)
                return;


            CurrentShortcutItem.Properties.CurrentState.BackgroundColor = txtBGColour.Text;
            RunUpdate();
        }

        private void chkFGTxtEnabled_CheckedChanged(object sender, EventArgs e)
        {
            radFGDark.Enabled = chkFGTxtEnabled.Checked;
            radFGLight.Enabled = chkFGTxtEnabled.Checked;
            CurrentShortcutItem.Properties.CurrentState.ShowNameOnSquare150X150Logo = chkFGTxtEnabled.Checked;
            RunUpdate();
        }

        private void cmbColour_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBGColour.Enabled = cmbColour.Text == @"Custom";
            CurrentShortcutItem.Properties.CurrentState.BackgroundColor = cmbColour.Text == @"Custom"
                ? txtBGColour.Text
                : cmbColour.Text;
            RunUpdate();
        }

        private Color GetPictureBoxesBackColor()
        {
            if (!string.Equals(cmbColour.Text, "custom", StringComparison.InvariantCultureIgnoreCase))
                return Color.FromName(cmbColour.Text);
            try
            {
                if (txtBGColour.Text.Length == txtBGColour.MaxLength)
                    return ColorUtils.HexToColor(txtBGColour.Text);
            }
            catch
            {
                // ignored
            }
            return _currentBaseSkin.BackColor;
        }

        private void ResetValidation()
        {
            txtBGColour.BackColor = _currentBaseSkin.BackColor;
            SetPictureBoxesBackColor();
        }

        private bool ValidateColour()
        {
            var valid = true;

            Action<Control> controlInvalid = c =>
            {
                c.BackColor = Color.Red;
                valid = false;
            };

            if (cmbColour.Text == @"Custom" && !Regex.Match(txtBGColour.Text, @"^#[0-9a-fA-F]{6}$").Success)
                controlInvalid(txtBGColour);

            if (CurrentShortcutItem.Properties.CurrentState.MediumImage.Bytes == null)
                controlInvalid(panPctMediumIcon);

            if (CurrentShortcutItem.Properties.CurrentState.SmallImage.Bytes == null)
                controlInvalid(panPctSmallIcon);

            return valid;
        }

        private void AddEventHandlers()
        {
            txtBGColour.TextChanged += txtBGColour_TextChanged;
            cmbColour.SelectedIndexChanged += cmbColour_SelectedIndexChanged;
            chkFGTxtEnabled.CheckedChanged += chkFGTxtEnabled_CheckedChanged;
            radFGLight.CheckedChanged += radFGLight_CheckedChanged;
            panPctMediumIcon.OnPannablePictureImagePropertyChange +=
                PanPctMediumIcon_OnPannablePictureImagePropertyChange;
            panPctSmallIcon.OnPannablePictureImagePropertyChange += PanPctSmallIcon_OnPannablePictureImagePropertyChange;
        }

        private void RemoveEventHandlers()
        {
            txtBGColour.TextChanged -= txtBGColour_TextChanged;
            cmbColour.SelectedIndexChanged -= cmbColour_SelectedIndexChanged;
            chkFGTxtEnabled.CheckedChanged -= chkFGTxtEnabled_CheckedChanged;
            radFGLight.CheckedChanged -= radFGLight_CheckedChanged;
            panPctMediumIcon.OnPannablePictureImagePropertyChange -=
                PanPctMediumIcon_OnPannablePictureImagePropertyChange;
            panPctSmallIcon.OnPannablePictureImagePropertyChange -= PanPctSmallIcon_OnPannablePictureImagePropertyChange;
        }

        private void panPctSmallIcon_DoubleClick(object sender, EventArgs e)
        {
            IconSet(sender);
        }

        private void panPctMediumIcon_DoubleClick(object sender, EventArgs e)
        {
            IconSet(sender);
        }

        private void panPctSmallIcon_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs) e).Button != MouseButtons.Right)
                return;

            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem("Change image...",
                (o, args) => { IconSet(panPctSmallIcon); });
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new MenuItem("Center image",
                (o, args) => { panPctSmallIcon.CenterImage(); });
            contextMenu.MenuItems.Add(menuItem);
            contextMenu.Show(panPctSmallIcon, ((MouseEventArgs) e).Location);
        }

        private void panPctMediumIcon_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs) e).Button != MouseButtons.Right)
                return;

            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem("Change image...",
                (o, args) => { IconSet(panPctMediumIcon); });
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new MenuItem("Center image",
                (o, args) => { panPctMediumIcon.CenterImage(); });
            contextMenu.MenuItems.Add(menuItem);
            contextMenu.Show(panPctMediumIcon, ((MouseEventArgs) e).Location);
        }


        private void PanPctMediumIcon_OnPannablePictureImagePropertyChange(object sender, EventArgs e)
        {
            var item = (PannablePictureBoxImage) sender;
            CurrentShortcutItem.Properties.CurrentState.MediumImage.X = item.X;
            CurrentShortcutItem.Properties.CurrentState.MediumImage.Y = item.Y;
            CurrentShortcutItem.Properties.CurrentState.MediumImage.Width = item.Width;
            CurrentShortcutItem.Properties.CurrentState.MediumImage.Height = item.Height;

            RunUpdate();
        }

        private void PanPctSmallIcon_OnPannablePictureImagePropertyChange(object sender, EventArgs e)
        {
            var item = (PannablePictureBoxImage) sender;
            CurrentShortcutItem.Properties.CurrentState.SmallImage.X = item.X;
            CurrentShortcutItem.Properties.CurrentState.SmallImage.Y = item.Y;
            CurrentShortcutItem.Properties.CurrentState.SmallImage.Width = item.Width;
            CurrentShortcutItem.Properties.CurrentState.SmallImage.Height = item.Height;

            RunUpdate();
        }

        private void btnColourPicker_Click(object sender, EventArgs e)
        {
            if (clrDialog.ShowDialog(this) == DialogResult.OK)
            {
                txtBGColour.Text = ColorUtils.ColorToHex(clrDialog.Color);
            }
        }
    }
}