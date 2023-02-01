using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon.Components.Capture.Managers
{
    internal static class TetherManager
    {
        internal static Dictionary<Guid, List<Guid>> Tethers = new Dictionary<Guid, List<Guid>>();
    }
}
