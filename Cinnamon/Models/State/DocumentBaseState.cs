using Cinnamon.Components.Capture;
using Newtonsoft.Json;
using Rhino;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Cinnamon.Models
{
    /// <summary>
    /// The base state of the document, including
    /// the initial positions of all animated objects,
    /// and the viewport which is being manipulated.
    /// </summary>
    public class DocumentBaseState 
    {
        public event EventHandler BaseStateChanged;

        [JsonIgnore]
        public static bool HasBeenInitialized => ActiveBase != null;

        [JsonIgnore]
        public string SavePath { get; private set; }

        [JsonProperty("uuid")]
        public Guid UUid { get; private set; } = Guid.NewGuid();

        [JsonProperty("created")]
        public DateTime CreatedDate { get; private set; } = DateTime.Now;

        [JsonProperty("modified")]
        public DateTime ModifiedDate { get; private set; } = DateTime.Now;

        [JsonIgnore]
        public Rhino.Display.RhinoViewport Viewport => View.ActiveViewport;
        [JsonIgnore]
        public Rhino.Display.RhinoView View { get; private set; }

        public Guid ViewportId { get; private set; }

        public static DocumentBaseState ActiveBase {
            get
            {
                if(_activeBase == null)
                {
                    LoadOrCreateBaseState();
                    SubscribeToDocEvents();
                }
                return _activeBase;
            }
            set {
                _activeBase = value;
            } 
        }

        private static void SubscribeToDocEvents()
        {
            RhinoAppMappings.ActiveDoc.AddCustomUndoEvent("Favorite Number", OnUndo);
        }

        // event handler for custom undo
        static void OnUndo(object sender, Rhino.Commands.CustomUndoEventArgs e)
        {
            // !!!!!!!!!!
            // NEVER change any setting in the Rhino document or application.  Rhino
            // handles ALL changes to the application and document and you will break
            // the Undo/Redo commands if you make any changes to the application or
            // document. This is meant only for your own private plug-in data
            // !!!!!!!!!!

            // On Undo we want to regen our document
            // capture objects from the document.
            DocumentCaptureManagers.RegenAll();

            // This function can be called either by undo or redo
            // In order to get redo to work, add another custom undo event with the
            // current value.  If you don't want redo to work, just skip adding
            // a custom undo event here
            e.Document.AddCustomUndoEvent("Cinnamon Undo", OnUndo);

        }

        private static void LoadOrCreateBaseState()
        {
            if (!LoadBaseFromRhinoPath())
            {
                CreateBaseState();
            }
        }

        private static DocumentBaseState _activeBase;

        [JsonProperty("objectStates")]
        private Dictionary<Guid, ObjectOrientationState> _objectStates = new Dictionary<Guid, ObjectOrientationState>();


        public void AddObjectBaseState(Guid id, ObjectOrientationState state)
        {
            if (!_objectStates.ContainsKey(id)) { _objectStates.Add(id, state); }
            else { _objectStates[id] = state; }
            Notify();
        }

        public bool TryGetObjectBaseState(Guid id, out ObjectOrientationState state) => _objectStates.TryGetValue(id, out state);   

        internal void Reset()
        {
            RestoreObjectsBaseStates();
            _objectStates.Clear();
            this.View = RhinoAppMappings.ActiveView;
            Notify();
        }

        /// <summary>
        /// Creates a new base state from the active viewport
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static void CreateBaseState()
        {
            DocumentBaseState.ActiveBase = new DocumentBaseState(RhinoAppMappings.ActiveView.ActiveViewportID);
        }

        public static string GetCinnamonSavePathForActiveDoc()
        {
            string docPath = RhinoDoc.ActiveDoc.Path;
            if (String.IsNullOrEmpty(docPath))
            {
                throw new Exception("Rhino document must be saved in order to use Cinnamon!");
            }
            if (!docPath.ToLower().EndsWith(".3dm"))
            {
                throw new Exception("Rhino document must be a .3dm file in order to use Cinnamon!");
            }
            string cinnamonPath = docPath.Replace(".3dm", ".cin");
            return cinnamonPath;
        }

        public static bool LoadBaseFromRhinoPath()
        {
            string docPath = RhinoDoc.ActiveDoc.Path;
            if (!String.IsNullOrEmpty(docPath))
            {
                var cinnamonFiles = System.IO.Directory.GetFiles(System.IO.Directory.GetParent(docPath).FullName)
                    .Where(p => p.EndsWith(".cin", StringComparison.OrdinalIgnoreCase));

                foreach(var file in cinnamonFiles)
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.Equals(RhinoDoc.ActiveDoc.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // Found it
                        LoadBaseStateFromFile(file);
                        return true;
                    }
                }
            }
            return false;
        }

        internal static CameraState GetActiveCameraState()
        {
            ActiveBase.ActivateView();
            return new CameraState(
                RhinoAppMappings.ActiveViewport.CameraLocation,
                RhinoAppMappings.ActiveViewport.CameraTarget,
                RhinoAppMappings.ActiveViewport.Camera35mmLensLength
                );
        }

        void Notify()
        {
            ModifiedDate = DateTime.Now;
            BaseStateChanged?.Invoke(this, null);
            Task.Run(() => { Save(); });
        }

        object _saveLock = new object();


        public FrameState FrameState { get; private set; }
        public int ObjectBaseStateCount => _objectStates.Count;

        void Save()
        {
            if (string.IsNullOrEmpty(SavePath)) { return; }
            lock (_saveLock)
            {
                try
                {
                    System.IO.File.WriteAllText(SavePath, JsonConvert.SerializeObject(this));
                }
                catch
                {
                }
            }
        }

        private bool _hasSubscribedToDocumentSave = false;

        public DocumentBaseState(Guid viewportId)
        {
            this.CreatedDate = DateTime.Now;
            this.ViewportId = viewportId;
            CompleteInit();
        }

        private DocumentBaseState() { }

        [OnDeserialized]
        void OnDeserializationComplete(System.Runtime.Serialization.StreamingContext ctx) => CompleteInit();

        void CompleteInit()
        {
            var view = RhinoDoc.ActiveDoc.Views.Find(ViewportId);
            if(view != null)
            {
                View = view;
            }
            else
            {
                View = RhinoAppMappings.ActiveView;
            }

            ActivateView();

            if (!_hasSubscribedToDocumentSave)
            {
                RhinoDoc.EndSaveDocument += RhinoDoc_EndSaveDocument;
                _hasSubscribedToDocumentSave = true;
            }
            try
            {
                this.SavePath = GetCinnamonSavePathForActiveDoc();
                Notify();
            }
            catch {
                // Document not saved yet.
            }
        }

        void ActivateView()
        {
            return;
            //RhinoAppMappings.ActiveView = View;
        }

        /// <summary>
        /// Restores the base state of all of the objects
        /// tracked by this
        /// </summary>
        public void RestoreObjectsBaseStates()
        {
            foreach(var o  in _objectStates)
            {
                o.Value.Apply();
            }
        }

        private void RhinoDoc_EndSaveDocument(object sender, DocumentSaveEventArgs e)
        {
            try
            {
                this.SavePath = GetCinnamonSavePathForActiveDoc();
                Notify();
            }
            catch
            {
                // Event fired prematurely
            }
        }

        public void SetViewToActiveView()
        {
            View = RhinoAppMappings.ActiveView;
            Notify();
        }

        public static void LoadBaseStateFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException(filePath);
            }

            ActiveBase = JsonConvert.DeserializeObject<DocumentBaseState>(System.IO.File.ReadAllText(filePath));
            ActiveBase.SavePath = filePath;
        }
    }
}
