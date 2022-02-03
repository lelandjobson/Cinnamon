using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon.Models.Effects
{
    internal class LayerShowHideEffect : IEffect
    {
        public string Name { get; set; }

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();

        private readonly List<string> _layers;
        private readonly List<bool> _states;

        public bool IsValid => _layers != null && _layers.Count > 0 && _states != null && _states.Count > 0;

        public int LayersShownCount
        {
            get
            {
                int shown = 0;
                for(int i = 0; i < _layers.Count; i++)
                {
                    if (_states[i % _states.Count])
                    {
                        shown++;
                    }
                }
                return shown;
            }
        }

        public int LayersHiddenCount
        {
            get
            {
                int hidden = 0;
                for (int i = 0; i < _layers.Count; i++)
                {
                    if (!_states[i % _states.Count])
                    {
                        hidden++;
                    }
                }
                return hidden;
            }
        }


        public LayerShowHideEffect(List<string> layers, bool state)
        {
            _layers = layers;
            _states = new List<bool>();
            foreach(var l in _layers) { _states.Add(state); }
        }

        public LayerShowHideEffect(List<string> layers, List<bool> states)
        {
            _layers = layers;
            _states = states;
        }

        public IEffect Copy()
        {
            return new LayerShowHideEffect(_layers, _states);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            if (!IsValid) { return; }

            state.FrameActions.Add((s) =>
            {
                // Get layers
                int i = 0;
                foreach (var n in _layers)
                {
                    bool cur = _states[i % _states.Count];
                    var l = RhinoAppMappings.DocumentLayers.FindName(n);
                    if (l != null && l != default(Layer))
                    {
                        l.IsVisible = cur;
                    }
                    i++;
                }
            });
        }
    }
}
