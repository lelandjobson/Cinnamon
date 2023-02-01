using System.Collections.Generic;

namespace Cinnamon
{
    public static class DisplayConduitManager
    {
        static Dictionary<int, MessageConduit> messageConduits = new Dictionary<int, MessageConduit>();
        public static void RenderMessage(int id, string message)
        {
            if (messageConduits.ContainsKey(id))
            {
                messageConduits[id].Enabled = false;
                messageConduits[id] = new MessageConduit(message);
            }
            else
            {
                messageConduits.Add(id, new MessageConduit(message));
            }
            messageConduits[id].Enabled = true;
        }

        public static void HideMessage(int id)
        {
            if (messageConduits.ContainsKey(id))
            {
                messageConduits[id].Enabled = false;
            }
        }
    }
}
