using System;

namespace stcalc
{
    public struct RightAscension
    {
        public RightAscension(double hours, double minutes, double seconds)
        {
            if (hours < 0 || hours > 23)
            {
                throw new ArgumentException("Hours must be between 0 and 23", "hours");
            }

            if (minutes < 0 || minutes > 59)
            {
                throw new ArgumentException("Minutes must be between 0 and 59", "minutes");
            }

            if (seconds < 0 || seconds > 59)
            {
                throw new ArgumentException("Seconds must be between 0 and 59", "seconds");
            }

            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public RightAscension(double decimalDegrees)
        {
            Hours = ToHours(decimalDegrees);
            Minutes = ToMinutes(decimalDegrees);
            Seconds = ToSeconds(decimalDegrees);
        }

        public double Hours { get; }
        public double Minutes { get; }
        public double Seconds { get; }
        
        public double ToDecimalDegrees() => (Hours + (Minutes / 60) + (Seconds / 3600)) * 15;
        public override string ToString()
        {
            var d = ToDecimalDegrees();
            var h = (int)d / 15;
            var m = (int)(((d / 15) - h) * 60);
            var s = ((((d / 15) - h) * 60) - m) * 60;
            var ss = Math.Round(s);
            return $"{h}h {m}m {ss}s";
        }

        private static double ToHours(double d)
        {
            return d / 15;
        }

        private static double ToMinutes(double d)
        {
            var h = ToHours(d);
            return ((d / 15) - h) * 60;
        }

        private static double ToSeconds(double d)
        {
            var h = ToHours(d);
            var m = ToMinutes(d);
            return ((((d / 15) - h) * 60) - m) * 60;
        }
    }
}
