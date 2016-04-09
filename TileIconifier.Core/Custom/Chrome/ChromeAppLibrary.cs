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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace TileIconifier.Core.Custom.Chrome
{
    public class ChromeAppLibrary
    {
        public static string GetChromeInstallationPath()
        {
            try
            {
                return
                    (string)
                        Registry.GetValue(
                            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                            string.Empty, null);
            }
            catch
            {
                foreach (
                    var defaultChromeInstallationPath in
                        CustomShortcutGetters.DefaultChromeInstallationPaths.Where(File.Exists))
                    return defaultChromeInstallationPath;
            }

            throw new FileNotFoundException(@"Unable to find Chrome installation path!");
        }

        public static List<ChromeApp> GetChromeAppItems(string appLibraryPath)
        {
            if (!Directory.Exists(appLibraryPath))
                throw new DirectoryNotFoundException(appLibraryPath);

            return (from chromeAppDir in new DirectoryInfo(appLibraryPath).GetDirectories()
                let chromeAppId =
                    Regex.Match(chromeAppDir.Name, @"_crx_([a-zA-Z0-9]{32})", RegexOptions.None).Groups[1].Value
                let chromeIconPaths = chromeAppDir.GetFiles(@"*.ico")
                from chromeIconPath in chromeIconPaths
                where chromeIconPath != null && CustomShortcutGetters.ExcludedChromeAppIds.All(s => s != chromeAppId)
                select
                    new ChromeApp(chromeAppId, Path.GetFileNameWithoutExtension(chromeIconPath.Name),
                        chromeIconPath.FullName
                        )).ToList();
        }
    }
}