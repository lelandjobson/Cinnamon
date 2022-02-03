using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Object_Rec
{
    public static class Document_OrderManagers
    {
        private static Dictionary<Guid, OrderManager_Object> Managers 
            = new Dictionary<Guid, OrderManager_Object>();

        public static OrderManager_Object GetOrCreateOrderManager(Guid id)
        {
            if (!Managers.ContainsKey(id))
            {
                Managers.Add(id, new OrderManager_Object(id));
            }
            return Managers[id];    
        }

        internal static bool ContainsOrder(Guid gid)
        {
            return Managers.ContainsKey(gid);
        }
    }
}