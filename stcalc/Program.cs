using System;
using System.Globalization;

namespace stcalc
{
    public class Program
    {
        private const string _usage = "Enter your longitude and (optionally) local date and time. Usage: stcalc -122.675 2020-03-03 13:00";
        static void Main(string[] args)
        {
            // Test if input arguments were supplied.
            if (args.Length < 1)
            {
                Console.WriteLine("Please enter your longitude. If you leave date and time blank the current moment is used.");
                Console.WriteLine(_usage);
                return;
            }

            if (!IsValidLongitude(args[0], out double longitude))
            {
                Console.WriteLine("Please enter a valid longitude between -180 and 180.");
                Console.WriteLine(_usage);
                return;
            }

            var dateValue = (args.Length > 1) ? args[1] : string.Empty;
            if (!IsValidDate(dateValue, out DateTime d))
            {
                Console.WriteLine("Please enter a valid date in the format YYYY-MM-DD");
                Console.WriteLine(_usage);
                return;
            }

            var timeValue = (args.Length > 2) ? args[2] : string.Empty;
            if (!IsValidTime(timeValue, out TimeSpan ts))
            {
                Console.WriteLine("Please enter a valid time in the format HH:MM");
                Console.WriteLine(_usage);
                return;
            }

            DateTime localDateTime = d + ts;
            DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo.Local);

            // lines 55 thru 59 are from Meeus, Chap 12 (pp 87-89)             
            var moment = new Moment(utcDateTime);
            var JD = moment.JulianDay;
            var T = ((JD - 2451545.0) / 36525);
            var theta0 = 280.46061837 + 360.98564736629 * (JD - 2451545.0) + (0.000387933 * T * T) - (T * T * T / 38710000.0); // (12.4)
            var gmstRA = new RightAscension(ReduceAngle(theta0));
            
            /*           
             From Textbook on Spherical Astronomy, 6th Ed. by W.M. Smart (p. 41):

             Sidereal time at Greenwich = GST + longitude l of your location east or west of Greenwich.
             In Portland, Oregon (-122.67) this results in a subtraction from GST. The result is then 
             converted into hours, minutes, and seconds via 15 deg = 1h, 15' = 1m, and 15'' = 1s. This 
             is the local sidereal time (LST).
             */ 

            var lmstRA = new RightAscension(gmstRA.ToDecimalDegrees() + longitude);

            Console.WriteLine("Longitude: " + longitude);
            Console.WriteLine("Local DateTime: " + localDateTime.ToString());
            Console.WriteLine("UTC DateTime: " + utcDateTime.ToString());
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("JD: " + JD);
            Console.WriteLine("T: " + T);
            Console.WriteLine("theta0 from Meeus formula (12.4): " + theta0);
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("Greenwich Mean Sidereal Time (dec deg): " + ReduceAngle(theta0));
            Console.WriteLine("Greenwich Mean Sidereal Time (HMS):     " + gmstRA.ToString());
            Console.WriteLine("Local Mean Sidereal Time (dec deg):     " + lmstRA.ToDecimalDegrees());
            Console.WriteLine("Local Mean Sidereal Time (HMS):         " + lmstRA.ToString());
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        private static bool IsValidLongitude(string value, out double d)
        {
            bool test = double.TryParse(value, out d);
            if (!test)
            {
                return false;
            }

            return (d >= -180 && d <= 180);
        }

        private static bool IsValidDate(string value, out DateTime d)
        {
            if (string.IsNullOrEmpty(value))
            {
                d = DateTime.Today;
                return false;
            }

            string pattern = "yyyy-MM-dd";
            CultureInfo ci = CultureInfo.InvariantCulture;
            DateTimeStyles dts = DateTimeStyles.None;
            try
            {
                return DateTime.TryParseExact(value, pattern, ci, dts, out d);            
            }
            catch
            {
                Console.WriteLine("Invalid date or not in format YYYY-MM-DD.");
                d = DateTime.Today;
                return false;
            }
        }

        private static bool IsValidTime(string value, out TimeSpan ts)
        {
            if (string.IsNullOrEmpty(value))
            {
                ts = DateTime.Now.TimeOfDay;
                return false;
            }
            
            try
            {
                return TimeSpan.TryParse(value, out ts);
            }
            catch
            {
                Console.WriteLine("Invalid time or not in format HH:MM.");
                ts = DateTime.Now.TimeOfDay;
                return false;
            }         
        }

        /// <summary>
        /// For very large angles reduce to put in interval between 0 and 360
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double ReduceAngle(double d)
        {
            d %= 360;
            if (d < 0)
            {
                d += 360;
            }

            return d;
        }
    }
}
