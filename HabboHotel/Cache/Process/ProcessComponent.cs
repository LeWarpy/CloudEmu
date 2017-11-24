using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;


using Cloud.HabboHotel.Users;
using Cloud.Core;
using Cloud.HabboHotel.Cache.Type;

namespace Cloud.HabboHotel.Cache.Process
{
	sealed class ProcessComponent
	{
		/// <summary>
		/// ThreadPooled Timer.
		/// </summary>
		private Timer _timer = null;

		/// <summary>
		/// Prevents the timer from overlapping itself.
		/// </summary>
		private bool _timerRunning = false;

		/// <summary>
		/// Checks if the timer is lagging behind (server can't keep up).
		/// </summary>
		private bool _timerLagging = false;

		/// <summary>
		/// Enable/Disable the timer WITHOUT disabling the timer itself.
		/// </summary>
		private bool _disabled = false;

		/// <summary>
		/// Used for disposing the ProcessComponent safely.
		/// </summary>
		private AutoResetEvent _resetEvent = new AutoResetEvent(true);

		/// <summary>
		/// How often the timer should execute.
		/// </summary>
		private static int _runtimeInSec = 1200;

		/// <summary>
		/// Default.
		/// </summary>
		public ProcessComponent()
		{
		}

		/// <summary>
		/// Initializes the ProcessComponent.
		/// </summary>
		public void Init()
		{
			_timer = new Timer(new TimerCallback(Run), null, _runtimeInSec * 1000, _runtimeInSec * 1000);
		}

		/// <summary>
		/// Called for each time the timer ticks.
		/// </summary>
		/// <param name="State"></param>
		public void Run(object State)
		{
			try
			{
				if (_disabled)
					return;

				if (_timerRunning)
				{
					_timerLagging = true;
					return;
				}

				_resetEvent.Reset();

				// BEGIN CODE
				List<UserCache> CacheList = CloudServer.GetGame().GetCacheManager().GetUserCache().ToList();
				if (CacheList.Count > 0)
				{
					foreach (UserCache Cache in CacheList)
					{
						try
						{
							if (Cache == null)
								continue;

							UserCache Temp = null;

							if (Cache.isExpired())
								CloudServer.GetGame().GetCacheManager().TryRemoveUser(Cache.Id, out Temp);

							Temp = null;
						}
						catch (Exception e)
						{
							ExceptionLogger.LogException(e);
						}
					}
				}

				CacheList = null;

				List<Habbo> CachedUsers = CloudServer.GetUsersCached().ToList();
				if (CachedUsers.Count > 0)
				{
					foreach (Habbo Data in CachedUsers)
					{
						try
						{
							if (Data == null)
								continue;

							Habbo Temp = null;

							if (Data.CacheExpired())
								CloudServer.RemoveFromCache(Data.Id, out Temp);

							if (Temp != null)
								Temp.Dispose();

							Temp = null;
						}
						catch (Exception e)
						{
							ExceptionLogger.LogException(e);
						}
					}
				}

				CachedUsers = null;
				// END CODE

				// Reset the values
				_timerRunning = false;
				_timerLagging = false;

				_resetEvent.Set();
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
			}
		}

		/// <summary>
		/// Stops the timer and disposes everything.
		/// </summary>
		public void Dispose()
		{
			// Wait until any processing is complete first.
			try
			{
				_resetEvent.WaitOne(TimeSpan.FromMinutes(5));
			}
			catch { } // give up

			// Set the timer to disabled
			_disabled = true;

			// Dispose the timer to disable it.
			try
			{
				if (_timer != null)
					_timer.Dispose();
			}
			catch { }

			// Remove reference to the timer.
			_timer = null;
		}
	}
}