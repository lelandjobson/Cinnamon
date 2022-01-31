using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Models
{
    [Serializable]
    public class Scene
    {
        public TimelineTime Range { get; private set; } = TimelineTime.Empty;

        readonly List<Moment> Moments = new List<Moment>();

        public readonly string Name = String.Empty;

        public Scene(string name, List<Moment> moments)
        {
            Name = name ?? this.Name;
            Moments = moments ?? this.Moments;

            if(Moments != null && moments.Count > 0)
            {
                Range = new TimelineTime()
                {
                    Start = moments.Min(m => m.Time.Start),
                    End = moments.Max(m => m.Time.End)
                };
            }
        }

        public IEnumerable<Moment> GetMoments()
        {
            foreach(var m in Moments) { yield return m; }
            yield break;
        }

        public Scene Copy()
        {
            return new Scene(this.Name, this.Moments.Select(m => m.Copy()).ToList());
        }

        public void AddMomentToEnd(Moment m)
        {
            Moments.Add(new Moment(m.Time.Push(this.Range.End), m.Curve, m.GetEffects()));
        }

        public void AddMomentsToEnd(List<Moment> moments)
        {
            moments?.ForEach(m => AddMomentToEnd(m));
        }

        public static Scene Compile(List<Scene> scenes, SceneCompilationStrategy strat, double gap = 0)
        {
            string combinedName = String.Join(" + ", scenes.Select(s => s.Name));
            switch (strat)
            {
                case SceneCompilationStrategy.AtOnce:
                    var result = new Scene(combinedName, scenes.SelectMany(s => s.Moments).ToList());
                    if(gap != 0)
                    {
                        result.AddMomentToEnd(Moment.Empty(gap));
                    }
                    return result;
                case SceneCompilationStrategy.Sequence:
                    Scene startScene = scenes[0].Copy();
                    for(int i =1; i< scenes.Count; i++)
                    {
                        startScene.AddMomentToEnd(Moment.Empty(gap));
                        startScene.AddMomentsToEnd(scenes[i].Moments);
                    }
                    return startScene;
                default:
                    throw new Exception("Compilation strategy not implemented");
            }

        }
    }
}
