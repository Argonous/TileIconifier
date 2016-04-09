using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileIconifier.Core.Utilities;

namespace TileIconifier.Core.Custom
{
    public class CustomShortcutLibrary
    {
        private static List<CustomShortcut> _customShortcuts;

        public static List<CustomShortcut> GetCustomShortcuts(bool useCache = true)
        {
            if (useCache && _customShortcuts != null)
                return _customShortcuts;

            if (!Directory.Exists(CustomShortcutGetters.CustomShortcutVbsPath))
            {
                _customShortcuts = new List<CustomShortcut>();
                return _customShortcuts;
            }

            //get all VBS files built by TileIconifier
            _customShortcuts = new DirectoryInfo(CustomShortcutGetters.CustomShortcutVbsPath)
                .GetFiles("*.vbs", SearchOption.AllDirectories)
                .Select(vbsFile => CustomShortcut.Load(vbsFile.FullName))
                .Where(customShortcut => DirectoryUtils.GetShortcutUserType(customShortcut.ShortcutPath) != ShortcutUser.Unknown)
                .ToList();
            return _customShortcuts;
        }
    }
}
