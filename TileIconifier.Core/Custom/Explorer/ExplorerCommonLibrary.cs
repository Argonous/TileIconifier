using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileIconifier.Core.Custom.Explorer
{
    public class ExplorerCommonLibrary
    {
        public static List<ExplorerItem> GetAllItems()
        {
            return CustomShortcutGetters.ExplorerGuids.Select(k => new ExplorerItem(k.Key, k.Value)).ToList();
        } 
    }
}
