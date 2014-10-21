using System;
using System.Collections.Generic;

namespace SimpleUtilsLib
{
	public class LoginLockoutCalculator
	{
		private static DateTime baseTime = new DateTime(2014,10,1,0,0,0,DateTimeKind.Utc);
		public static int AttemptWindowMinutes = 5;
		public static int MaximumFailures = 5;
		public static int LockoutPeriodMinutes = 30;

		private List<DateTime> timestamps;

		public DateTime CurrentTime { get; set; }

		/// <summary>
		/// The event time argument is mostly for unit testing, you should 
		/// typically use the default (i.e. don't pass eventTime).
		/// </summary>
		/// <param name="existingValue">Existing value.</param>
		/// <param name="eventTime">Event time.</param>
		public LoginLockoutCalculator(String existingValue, DateTime? eventTime = null) {
			if (!eventTime.HasValue) {
				eventTime = DateTime.UtcNow;
			}
			CurrentTime = eventTime.Value;
			timestamps = new List<DateTime> ();
			if (existingValue != null) {
				foreach (var t in new List<String> (existingValue.Split (','))) {
					// Add the value in the list to the base date as an offset in seconds
					// only if it's in our window
					DateTime attemptDate = baseTime.Add (new TimeSpan (0, 0, int.Parse (t)));
					if (CurrentTime.Subtract (attemptDate).TotalMinutes < LockoutPeriodMinutes) {
						timestamps.Add (attemptDate);
					}
				}
			}
		}

		public bool CanAttemptLogin {
			get {
				if (timestamps.Count >= MaximumFailures) {
					if (CurrentTime.Subtract (timestamps [timestamps.Count - 1]).TotalMinutes > LockoutPeriodMinutes) {
						return true;
					}
					return false;
				}
				return true;
			}
		}
	
		public void loginFailed() {
			timestamps.Add (CurrentTime);
		}

		public void loginSucceeded() {
			timestamps.Clear ();
		}

		public String GetValue() {
			List<String> strList = new List<String> ();
			foreach (var dt in timestamps) {
				strList.Add (ToAdjustedTime(dt).ToString ());
			}
			return String.Join (",", strList);
		}

		public int ToAdjustedTime(DateTime d) {
			return Convert.ToInt32 (d.Subtract (baseTime).TotalSeconds);
		}
	}
}

