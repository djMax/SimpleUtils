using NUnit.Framework;
using System;
using SimpleUtilsLib;

namespace SimpleUtilsTests
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void SimpleCase ()
		{
			DateTime now = DateTime.UtcNow;
			LoginLockoutCalculator lc = new LoginLockoutCalculator ("1,2,3,4,5");

			Assert.AreSame ("", lc.GetValue ());
			Assert.IsTrue (lc.CanAttemptLogin);

			DateTime fiveMinutesAgo = now.Subtract(TimeSpan.FromMinutes(5));
			lc.CurrentTime = fiveMinutesAgo;
			Assert.IsTrue (lc.CanAttemptLogin);
			lc.loginFailed ();
			Assert.IsTrue (lc.CanAttemptLogin);
			Assert.Greater (lc.GetValue ().Length, 0);
			Assert.AreEqual (lc.ToAdjustedTime (fiveMinutesAgo).ToString(), lc.GetValue ());

			for (var i = -4; i < 0; i++) {
				DateTime minutesAgo = now.Add(TimeSpan.FromMinutes(i));
				lc.CurrentTime = minutesAgo;
				Assert.IsTrue (lc.CanAttemptLogin);
				lc.loginFailed ();
			}

			Assert.IsFalse (lc.CanAttemptLogin);
			lc.CurrentTime = now;
			// 10 minutes is still in the penalty box
			Assert.IsFalse (lc.CanAttemptLogin);

			// After the 30 minutes is up, you can go in again.
			lc.CurrentTime = now.Add (TimeSpan.FromMinutes (31));
			Assert.IsTrue (lc.CanAttemptLogin);
		}
	}
}

