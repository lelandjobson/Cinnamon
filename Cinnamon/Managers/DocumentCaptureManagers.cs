using Cinnamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Capture
{

    public static class DocumentCaptureManagers
    {
        private static Dictionary<Guid, ObjectCaptureManager> Managers 
            = new Dictionary<Guid, ObjectCaptureManager>();

        public static bool TryGetOrCreateObjectCaptureManager(Guid id, out ObjectCaptureManager capMan)
        {
            if (!Managers.ContainsKey(id))
            {
                if(ObjectOrientationState.TryCreate(id, out var currentOrientation))
                {
                    Managers.Add(id, new ObjectCaptureManager(id));
                    DocumentBaseState.ActiveBase.AddObjectBaseState(id, Managers[id].FirstState ?? currentOrientation);
                }
                else
                {
                    // This object is non-orientable, so no dice
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

        public static void RegenAll()
        {
            foreach(var manager in Managers.Values) { manager.ForceRegen(); }
            CameraCaptureManager.Default.ForceRegen();
        }
    }
}