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
                }
                return _activeBase;
            }
            set {
                _activeBase = value;
            } 
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

        /// <summary>
        /// Creates a new base state from the active viewport
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static void CreateBaseState()
        {
            DocumentBaseState.ActiveBase = new DocumentBaseState(RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewportID);
        }

        public static string GetCinnamonSavePathForActiveDoc()
        {
            string docPath = RhinoDoc.ActiveDoc.Path;
            if (!String.IsNullOrEmpty(docPath))
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

        [OnSerialized]
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
            RhinoAppMappings.ActiveView = View;
            if (!_hasSubscribedToDocumentSave)
            {
                RhinoDoc.EndSaveDocument += RhinoDoc_EndSaveDocument;
                _hasSubscribedToDocumentSave = true;
            }
            try
            {
                this.SavePath = GetCinnamonSavePathForActiveDoc();
            }
            catch {
                // Document not saved yet.
            }
        }

        /// <summary>
        /// Restores the base state of all of the objects
        /// tracked by this
        /// </summary>
        public void RestoreObjectsBaseState()
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
