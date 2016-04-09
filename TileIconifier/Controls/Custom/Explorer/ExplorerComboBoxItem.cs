using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileIconifier.Controls.Custom.Cache;
using TileIconifier.Core.Custom.Explorer;

namespace TileIconifier.Controls.Custom.Explorer
{
    public class ExplorerComboBoxItem : BaseGenericCustomControlItem<ExplorerItem>
    {
        public ExplorerComboBoxItem(ExplorerItem explorerItem) : base(explorerItem)
        {

        }

        public override string ToString()
        {
            return BaseItem.DisplayName;
        }
    }
}
