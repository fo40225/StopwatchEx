namespace StopwatchEx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A Stopwatch which can count CPU cycle and GC times
    /// </summary>
    public class StopwatchEx : IDisposable
    {
        private readonly string name;
        private readonly SafeThreadHandle currentThreadHandle;
        private readonly int[] gcCountsInit;
        private readonly int[] gcCountsEnd;
        private readonly Stopwatch sw;
        private ulong initCPUCycleCount;
        private ulong endCPUCycleCount;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchEx"/> class and start immediately.
        /// </summary>
        /// <param name="name"> Stopwatch's name </param>
        public StopwatchEx(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "StopwatchEx";
            }

            this.name = name;
            this.currentThreadHandle = NativeMethods.GetCurrentThread();
            this.gcCountsInit = new int[GC.MaxGeneration + 1];
            this.gcCountsEnd = new int[GC.MaxGeneration + 1];
            this.sw = new Stopwatch();
            this.Start(true);
            this.End(true);
            this.sw.Reset();

            this.Start(false);
        }

        ~StopwatchEx()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.End(false);
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.currentThreadHandle.Dispose();
                }

                this.disposed = true;
            }
        }

        private void Start(bool isWarmUp)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                this.gcCountsInit[i] = GC.CollectionCount(i);
            }

            if (!isWarmUp)
            {
                Console.WriteLine(this.name + " Start !");
            }

            this.sw.Start();
            NativeMethods.QueryThreadCycleTime(this.currentThreadHandle, ref this.initCPUCycleCount);
        }

        private void End(bool isWarmUp)
        {
            NativeMethods.QueryThreadCycleTime(this.currentThreadHandle, ref this.endCPUCycleCount);
            this.sw.Stop();
            var cpuCycles = this.endCPUCycleCount - this.initCPUCycleCount;
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                this.gcCountsEnd[i] = GC.CollectionCount(i);
            }

            if (!isWarmUp)
            {
                Console.WriteLine(this.name + " End !");
                Console.WriteLine("Time Elapsed: " + this.sw.Elapsed.ToString());
                Console.WriteLine("CPU Cycles: " + cpuCycles.ToString());

                for (int i = 0; i <= GC.MaxGeneration; i++)
                {
                    int count = this.gcCountsEnd[i] - this.gcCountsInit[i];
                    Console.WriteLine("Gen " + i + ": " + count);
                }

                Console.WriteLine();
            }
        }
    }
}