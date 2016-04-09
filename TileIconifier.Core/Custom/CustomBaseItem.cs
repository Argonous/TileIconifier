using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileIconifier.Core.Custom
{
    public interface ICustomBaseItem
    {
        string DisplayName { get; }
        string ExecutionArgument { get; }
        byte[] IconAsBytes { get; }
        CustomShortcutType ShortcutType { get; }
    }
}
