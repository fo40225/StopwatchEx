# StopwatchEx
A enhanced stopwatch

StopwatchEx can measure elapsed time, CPU cycles, GC times and easy to use.

Example

    using (new StopwatchEx("Sleep(1000)"))
    {
        Thread.Sleep(1000);
    }

output:

    Sleep(1000) Start !
    Sleep(1000) End !
    Time Elapsed: 00:00:00.9997008
    CPU Cycles: 1207143
    Gen 0: 0
    Gen 1: 0
    Gen 2: 0

see StopwatchEx.cs to get implement detail

The main method is a test of the array access performance by pointer.
