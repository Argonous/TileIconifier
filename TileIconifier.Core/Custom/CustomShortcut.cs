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
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using TileIconifier.Core.Properties;
using TileIconifier.Core.Shortcut;
using TileIconifier.Core.Utilities;

namespace TileIconifier.Core.Custom
{
    public class CustomShortcut
    {
        private string _basicShortcutIcon;

        public CustomShortcut(
            string shortcutName,
            string shortcutPath,
            string targetPath,
            string targetArguments,
            CustomShortcutType shortcutType,
            string basicShortcutIcon = null,
            string vbsFilePath = null,
            string vbsFolderPath = null)
        {
            ShortcutName = shortcutName.CleanInvalidFilenameChars();
            ShortcutPath = shortcutPath;
            TargetPath = targetPath;
            TargetArguments = targetArguments;
            ShortcutType = shortcutType;
            BasicShortcutIcon = basicShortcutIcon;
            VbsFilePath = vbsFilePath;
            VbsFolderPath = vbsFolderPath;
        }

        public CustomShortcut(
            string shortcutName,
            string targetPath,
            string targetArguments,
            CustomShortcutType shortcutType,
            WindowType windowType,
            string shortcutRootFolder,
            string basicIconToUse = null,
            string workingFolder = null
            )
        {
            var vbsFolderPath =
                DirectoryUtils.GetUniqueDirName(CustomShortcutGetters.CustomShortcutVbsPath + shortcutName) + "\\";
            ShortcutPath = $"{shortcutRootFolder}{new DirectoryInfo(vbsFolderPath).Name}\\{shortcutName}.lnk";


            ShortcutName = shortcutName.CleanInvalidFilenameChars();
            TargetPath = targetPath;
            TargetArguments = targetArguments;
            ShortcutType = shortcutType;
            VbsFolderPath = vbsFolderPath;
            WorkingFolder = workingFolder;
            WindowType = windowType;

            Directory.CreateDirectory(VbsFolderPath);

            if (basicIconToUse != null && File.Exists(basicIconToUse)) BasicShortcutIcon = basicIconToUse;
        }

        public string ShortcutPath { get; set; }
        private string VbsFilePath { get; set; }
        private string VbsFolderPath { get; }
        public string ShortcutName { get; }
        public string TargetPath { get; }
        public string TargetArguments { get; }
        private string WorkingFolder { get; }
        public CustomShortcutType ShortcutType { get; }
        public WindowType WindowType { get; set; }

        private string BasicShortcutIcon
        {
            get { return !string.IsNullOrEmpty(_basicShortcutIcon) ? _basicShortcutIcon : TargetPath; }
            set { _basicShortcutIcon = value; }
        }

        /// <summary>
        ///     Create a custom shortcut and save its icon alongside it
        /// </summary>
        /// <param name="basicIconTouse"></param>
        public void BuildCustomShortcut(Image basicIconTouse)
        {
            BasicShortcutIcon = VbsFolderPath + ShortcutName + ".ico";
            try
            {
                using (var iconWriter = new StreamWriter(BasicShortcutIcon))
                {
                    using (var ico = Icon.FromHandle(((Bitmap) basicIconTouse).GetHicon()))
                    {
                        ico.Save(iconWriter.BaseStream);
                    }
                }
            }
            catch
            {
                BasicShortcutIcon = null;
            }

            BuildCustomShortcut();
        }

        public void BuildCustomShortcut()
        {
            VbsFilePath = VbsFolderPath + ShortcutName + ".vbs";

            File.WriteAllText(VbsFilePath,
                string.Format(Resources.CustomShortcutVbsTemplate,
                    ShortcutName.EscapeVba(),
                    ShortcutPath.EscapeVba(),
                    TargetPath.QuoteWrap().EscapeVba(),
                    TargetArguments.EscapeVba(),
                    ShortcutType,
                    (int) WindowType
                    ));

            ShortcutUtils.CreateLnkFile(ShortcutPath, VbsFilePath,
                ShortcutName + " shortcut created by TileIconifier",
                iconPath: BasicShortcutIcon,
                workingDirectory: WorkingFolder
                );
        }

        public void Delete()
        {
            var shortcutFileInfo = new FileInfo(ShortcutPath);
            if (shortcutFileInfo.Directory != null && shortcutFileInfo.Directory.Exists)
            {
                try
                {
                    shortcutFileInfo.Directory.Delete(true);
                }
                catch
                {
                    // ignored
                }
            }

            if (!Directory.Exists(VbsFolderPath)) return;
            try
            {
                Directory.Delete(VbsFolderPath, true);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///     Parses data from a VBS file and returns a custom shortcut
        /// </summary>
        /// <exception cref="InvalidCustomShortcutException">If a property Regex could not be matched</exception>
        public static CustomShortcut Load(string vbsFilePath)
        {
            if (!File.Exists(vbsFilePath))
                throw new FileNotFoundException(vbsFilePath);

            var vbsFileContents = File.ReadAllText(vbsFilePath);

            var regexMatch = Regex.Match(vbsFileContents,
                "'Custom Shortcut Type = \"(.*)\".*'Shortcut Name = \"(.*)\".*'Shortcut Path = \"(.*)\".*targetPath = \"(.*)\".*targetArguments = \"(.*)\"\r?\n.*",
                RegexOptions.Singleline);

            if (!regexMatch.Success)
                throw new InvalidCustomShortcutException();

            var directoryInfo = new FileInfo(vbsFilePath).Directory;
            if (directoryInfo != null)
                return new CustomShortcut(regexMatch.Groups[2].Value.UnescapeVba(),
                    regexMatch.Groups[3].Value.UnescapeVba(), regexMatch.Groups[4].Value.UnescapeVba(),
                    regexMatch.Groups[5].Value.UnescapeVba(),
                    (CustomShortcutType) Enum.Parse(typeof (CustomShortcutType), regexMatch.Groups[1].Value, true),
                    vbsFilePath: vbsFilePath,
                    vbsFolderPath: directoryInfo.FullName + "\\");

            throw new DirectoryNotFoundException();
        }
    }
}