using Cinnamon.Models.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cinnamon.Models
{
    [Serializable]
    public class Moment
    {
        public bool IsValid => Time != null && Time.IsValid;

        public bool HasEffects { get; private set; } = false;

        public TimelineTime Time { get; set; } = TimelineTime.Empty;

        private List<IEffect> Effects => _effects ?? (_effects = new List<IEffect>());
        private List<IEffect> _effects;

        public IEnumerable<IEffect> GetEffects()
        {
            foreach(var effect in Effects) { yield return effect; }
        }

        public AnimationCurve Curve { get; set; }

        public Moment(TimelineTime time, AnimationCurve curve, IEnumerable<IEffect> effects)
        {
            Time = time;
            Curve = curve;
            if(effects != null) { foreach (var e in effects) { AddEffect(e); } }

            // This is to ensure any effects with new rhinoobjects
            // have their objects in the cache.
            Player.ExpireDocObjects();
        }

        public void AddEffect(IEffect effect)
        {
            Effects.Add(effect);
            HasEffects = true;
        }

        public Moment Copy()
        {
            return new Moment(
               new TimelineTime() { Start = Time.Start, End = Time.End },
                 this.Curve,
                Effects.Select(e => e.Copy())
                );
        }

        public static Moment Empty(double duration, double startTime = 0) => new Moment(new TimelineTime() { Start = startTime, End = startTime + duration }, AnimationCurve.Linear, null);
    }
}