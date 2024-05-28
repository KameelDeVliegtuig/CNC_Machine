using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
	internal class Delay : IDisposable
	{


		// A minimum of ~400us delay

		public event EventHandler DelayComplete;

		private bool enabled = false;

		public bool Enabled { get { return enabled; } }

		// Delay for microseconds
		public async Task UsDelay(int microseconds, long StartTick)
		{
			long endTick = ((microseconds) * Stopwatch.Frequency) / 1000000;
			//long endTick = Stopwatch.Frequency / 1000000;
			while (Stopwatch.GetTimestamp() - StartTick < endTick)
			{
				//await Task.Run(() => 1-0);
				await Task.Delay(0);
			}
			if (enabled)
			{
				DelayComplete?.Invoke(this, EventArgs.Empty);
			}
		}

		public void AutoDispose(object? sender, EventArgs e)
		{
			Dispose();
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

		public void Enable()
		{
			enabled = true;
		}

		public void Disable()
		{
			enabled = false;
		}
	}
}
