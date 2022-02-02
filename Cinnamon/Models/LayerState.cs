using System;

namespace Cinnamon.Models
{
    /// <summary>
    /// Contains the state of the layer that is necessary
    /// to the application.
    /// </summary>
    [Serializable]
    public class LayerState
    {
        public bool HasTransparencyState => TransparencyState != -1;

        public bool IsVisible { get; set; } = true;

        public double TransparencyState { get; set; } = -1;

    }
}
