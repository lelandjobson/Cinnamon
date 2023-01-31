using Cinnamon.Models;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Capture
{

    public static class Document_CaptureManagers
    {
        private static Dictionary<Guid, CaptureManager_Object> Managers 
            = new Dictionary<Guid, CaptureManager_Object>();

        public static bool TryGetOrCreateCaptureManager(Guid id, out CaptureManager_Object capMan)
        {
            if (!Managers.ContainsKey(id))
            {
                if(ObjectOrientationState.TryCreate(id, out var baseOrientation))
                {
                    Managers.Add(id, new CaptureManager_Object(id));
                    DocumentBaseState.ActiveBase.AddObjectBaseState(id, baseOrientation);
                }
                else
                {
                    capMan = null;
                    return false;
                }
            }
            capMan = Managers[id];
            return true;
        }

        internal static bool ContainsCapture(Guid gid)
        {
            return Managers.ContainsKey(gid);
        }
    }
}