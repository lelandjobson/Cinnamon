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

            // Get layers
            int i = 0;
            foreach (var n in _layers)
            {
                bool cur = _states[i % _states.Count];
                var l = RhinoAppMappings.DocumentLayers.FindName(n);
                if (l == null || l == default(Layer))
                {
                    continue;
                }
                l.IsVisible = cur;
                i++;
            }
        }
    }
}
