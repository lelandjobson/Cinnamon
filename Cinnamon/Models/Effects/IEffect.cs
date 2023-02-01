using System;

namespace Cinnamon.Models.Effects
{
    public interface IEffect : ICanUseInPlayer
    { 
        Guid Id { get; }

        string Name { get; set; }
        bool Local { get; set; }

        void SetFrameStateValue(double percentage, FrameState state);

        IEffect Copy();
    }
}