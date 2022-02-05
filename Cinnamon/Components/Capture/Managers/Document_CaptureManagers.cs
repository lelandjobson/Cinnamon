using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Capture
{
    public static class Document_CaptureManagers
    {
        private static Dictionary<Guid, CaptureManager_Object> Managers 
            = new Dictionary<Guid, CaptureManager_Object>();

        public static CaptureManager_Object GetOrCreateCaptureManager(Guid id)
        {
            if (!Managers.ContainsKey(id))
            {
                Managers.Add(id, new CaptureManager_Object(id));
            }
            return Managers[id];    
        }

        internal static bool ContainsCapture(Guid gid)
        {
            return Managers.ContainsKey(gid);
        }
    }
}