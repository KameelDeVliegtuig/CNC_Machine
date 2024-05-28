using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
	internal class Delay : IDisposable
	{

		public event EventHandler DelayComplete;

		// Delay for microseconds
		public async Task UsDelay(int microseconds, long StartTick)
		{
			var targetTicks = ((microseconds) * Stopwatch.Frequency) / 1000000;
			while (Stopwatch.GetTimestamp() - StartTick < targetTicks)
			{
				await Task.Yield(); // Non-blocking wait
			}

			OnDelayComplete(EventArgs.Empty);
		}

		protected virtual void OnDelayComplete(EventArgs e)
		{
			DelayComplete?.Invoke(this, e);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Dispose managed resources
				DelayComplete = null;
			}
		}
	}
}
