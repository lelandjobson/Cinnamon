using System;

namespace Cinnamon.Models
{
    [Serializable]
    public class TimelineTime
    {
        public double Duration => _end - _start;

        public bool IsValid => _start >= 0 && _end > 0 && !IsZeroDuration;
        public bool IsZeroDuration => _end == _start;
        public double Start
        {
            get => _start;
            set { _start = value; }

        }
        private double _start = 0;
        public double End
        {
            get => _end;
            set
            {
                _end = value;
            }
        }

        private double _end = 0;

        public static TimelineTime Empty => new TimelineTime() { Start = 0, End = 0 };

        public TimelineTime Push(double value) => new TimelineTime() { Start = _start + value, End = _end + value };
    }
}