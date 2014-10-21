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

			DateTime tenMinutesAgo = now.Subtract(TimeSpan.FromMinutes(-10));
			lc.CurrentTime = tenMinutesAgo;
			Assert.IsTrue (lc.CanAttemptLogin);
			lc.loginFailed ();
			Assert.IsTrue (lc.CanAttemptLogin);
			Assert.Greater (lc.GetValue ().Length, 0);
			Assert.AreEqual (lc.ToAdjustedTime (tenMinutesAgo).ToString(), lc.GetValue ());

			for (var i = -9; i < -6; i--) {
				DateTime minutesAgo = now.Subtract(TimeSpan.FromMinutes(i));
				lc.CurrentTime = minutesAgo;
				Assert.IsTrue (lc.CanAttemptLogin);
				lc.loginFailed ();
			}

			Assert.IsFalse (lc.CanAttemptLogin);
		}
	}
}

