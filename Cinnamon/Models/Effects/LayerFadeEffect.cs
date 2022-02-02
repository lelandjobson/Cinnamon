using Rhino.DocObjects;
using System;
using System.Collections.Generic;

namespace Cinnamon.Models.Effects
{
    // For layer fade away FX, it would make the most sense
    // to append the layer material transparency state to
    // the framestate. 


    internal class LayerFadeEffect : IEffect
    {
        public string Name { get; set; }

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();

        private readonly List<string> _layers;

        private readonly double _startTrans;

        private readonly double _endTrans;

        public bool IsValid => 
            _layers != null && _layers.Count > 0 && 
            !double.IsNaN(_startTrans) && !double.IsNaN(_endTrans);

        public LayerFadeEffect(List<string> layers, double startTrans, double endTrans)
        {
            _layers = layers;
            _startTrans = startTrans;
            _endTrans = endTrans;
        }

        public IEffect Copy()
        {
            return new LayerFadeEffect(_layers, _startTrans,_endTrans);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            if (!IsValid) { return; }

            // Get layers
            foreach (var n in _layers)
            {
                double transparencyVal = percentage.PercToValue(_startTrans, _endTrans);
                var l = RhinoAppMappings.DocumentLayers.FindName(n);
                if (l == null || l == default(Layer))
                {
                    continue;
                }
                l.SetTransparency(transparencyVal);
            }
        }
    }
}
