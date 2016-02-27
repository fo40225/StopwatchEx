namespace StopwatchEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Win32.SafeHandles;

    internal class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected SafeThreadHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(this.handle);
        }
    }
}