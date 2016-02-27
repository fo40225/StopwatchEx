namespace StopwatchEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static unsafe void Main(string[] args)
        {
            Console.WriteLine("2GB array copy");
            var a = new int[536870911];
            var rnd = new Random();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = rnd.Next();
            }

            var b = new int[536870911];
            using (new StopwatchEx("Copy array in managed"))
            {
                for (int i = 0; i < 536870911; i++)
                {
                    b[i] = a[i];
                }
            }

            b = new int[536870911];
            using (new StopwatchEx("unsafe array style pointer"))
            {
                fixed (int* ptrA = a, ptrB = b)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        ptrB[i] = ptrA[i];
                    }
                }
            }

            b = new int[536870911];

            // https://github.com/dotnet/coreclr/issues/2480
            using (new StopwatchEx("unsafe array style pointer (no fixed pointer)"))
            {
                fixed (int* ptrA = a, ptrB = b)
                {
                    int* ta = ptrA;
                    int* tb = ptrB;
                    for (int i = 0; i < a.Length; i++)
                    {
                        tb[i] = ta[i];
                    }
                }
            }

            b = new int[536870911];
            using (new StopwatchEx("pointer arithmetic"))
            {
                fixed (int* ptrA = a, ptrB = b)
                {
                    int* ta = ptrA;
                    int* tb = ptrB;
                    for (int i = 0; i < a.Length; i++, ta++, tb++)
                    {
                        *tb = *ta;
                    }
                }
            }

            b = new int[536870911];
            using (new StopwatchEx("Array.Copy"))
            {
                Array.Copy(a, b, a.Length);
            }

            b = new int[536870911];
            using (new StopwatchEx("Buffer.BlockCopy"))
            {
                Buffer.BlockCopy(a, 0, b, 0, Buffer.ByteLength(a));
            }

            b = new int[536870911];
            using (new StopwatchEx(".net 4.6 Buffer.MemoryCopy"))
            {
                fixed (void* ptrA = a, ptrB = b)
                {
                    Buffer.MemoryCopy(ptrA, ptrB, Buffer.ByteLength(b), Buffer.ByteLength(a));
                }
            }

            Console.WriteLine("large array copy");
            var x = new byte[0x7FEFFFFF];

            rnd.NextBytes(x);

            var y = new byte[0x7FEFFFFF];
            using (new StopwatchEx("Copy array in managed"))
            {
                for (int i = 0; i < x.Length; i++)
                {
                    y[i] = x[i];
                }
            }

            y = new byte[0x7FEFFFFF];
            using (new StopwatchEx("unsafe array style pointer"))
            {
                fixed (byte* ptrX = x, ptrY = y)
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        ptrY[i] = ptrX[i];
                    }
                }
            }

            y = new byte[0x7FEFFFFF];
            using (new StopwatchEx("unsafe array style pointer (no fixed pointer)"))
            {
                fixed (byte* ptrX = x, ptrY = y)
                {
                    byte* tx = ptrX;
                    byte* ty = ptrY;
                    for (int i = 0; i < x.Length; i++)
                    {
                        ty[i] = tx[i];
                    }
                }
            }

            y = new byte[0x7FEFFFFF];
            using (new StopwatchEx("pointer arithmetic"))
            {
                fixed (byte* ptrX = x, ptrY = y)
                {
                    byte* tx = ptrX;
                    byte* ty = ptrY;
                    for (int i = 0; i < x.Length; i++, tx++, ty++)
                    {
                        *ty = *tx;
                    }
                }
            }

            y = new byte[0x7FEFFFFF];
            using (new StopwatchEx("Array.Copy"))
            {
                Array.Copy(x, y, x.Length);
            }

            y = new byte[0x7FEFFFFF];
            using (new StopwatchEx("Buffer.BlockCopy"))
            {
                Console.WriteLine("Buffer.BlockCopy can't copy array large than 2GB");
            }

            y = new byte[0x7FEFFFFF];
            using (new StopwatchEx(".net 4.6 Buffer.MemoryCopy"))
            {
                fixed (void* ptrX = x, ptrY = y)
                {
                    Buffer.MemoryCopy(ptrX, ptrY, sizeof(byte) * y.LongLength, sizeof(byte) * x.LongLength);
                }
            }

            Console.WriteLine("small array access");
            var smallAry = new byte[32];

            rnd.NextBytes(smallAry);

            int testTimes = 10000000;
            Bag bag = new Bag();
            using (new StopwatchEx("managed access array"))
            {
                for (int i = 0; i < testTimes; i++)
                {
                    for (int j = 0; j < smallAry.Length; j++)
                    {
                        bag.Temp = smallAry[j];
                        GC.KeepAlive(bag);
                    }
                }
            }

            using (new StopwatchEx("fixed pointer access array"))
            {
                fixed (byte* ptrX = smallAry)
                {
                    for (int i = 0; i < testTimes; i++)
                    {
                        for (int j = 0; j < smallAry.Length; j++)
                        {
                            bag.Temp = ptrX[j];
                            GC.KeepAlive(bag);
                        }
                    }
                }
            }

            using (new StopwatchEx("non-fixed pointer access array"))
            {
                fixed (byte* ptrX = smallAry)
                {
                    byte* tx = ptrX;
                    for (int i = 0; i < testTimes; i++)
                    {
                        for (int j = 0; j < smallAry.Length; j++)
                        {
                            bag.Temp = tx[j];
                            GC.KeepAlive(bag);
                        }
                    }
                }
            }

            using (new StopwatchEx("arithmetic pointer access array"))
            {
                fixed (byte* ptrX = smallAry)
                {
                    for (int i = 0; i < testTimes; i++)
                    {
                        byte* tx = ptrX;
                        for (int j = 0; j < smallAry.Length; j++, tx++)
                        {
                            bag.Temp = *tx;
                            GC.KeepAlive(bag);
                        }
                    }
                }
            }

            b = new int[536870911];
            using (new StopwatchEx("Empty 1"))
            {
            }

            b = new int[536870911];
            using (new StopwatchEx("Empty 2"))
            {
            }

            Console.WriteLine("Test end");
            Console.ReadLine();
        }
    }
}