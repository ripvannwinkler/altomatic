using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altomatic.UI.Utilities
{
	public class AsyncLock
	{
		private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
		AsyncLocal<int> ReferenceCount = new AsyncLocal<int> { Value = 0 };

		public async Task Do(Func<Task> action)
		{
			if (ReferenceCount.Value > 0)
			{
				await action();
			}
			else
			{
				ReferenceCount.Value++;
				await semaphore.WaitAsync();

				try { await action(); }
				finally
				{
					semaphore.Release();
					ReferenceCount.Value--;
				}
			}
		}

		public async Task<T> Do<T>(Func<Task<T>> action)
		{
			if (ReferenceCount.Value > 0)
			{
				return await action();
			}
			else
			{
				ReferenceCount.Value++;
				await semaphore.WaitAsync();

				try { return await action(); }
				finally
				{
					semaphore.Release();
					ReferenceCount.Value--;
				}
			}
		}
	}
}
