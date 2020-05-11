using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JA
{
    public static class Ex
    {
        public static TEnum Parse<TEnum>(this string value, bool ignoreCase) where TEnum : struct
        {
            if (Enum.TryParse(value, ignoreCase, out TEnum system))
            {
                return system;
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), 0);
        }

        static SHA1 sha1 = SHA1.Create();
        public static EventArgs<T> AsEventArgs<T>(this T item)
        {            
            return new EventArgs<T>(item);
        }

        public static ulong HashSha1(this byte[] buffer)
        {
            var hash = sha1.ComputeHash(buffer);
            return BitConverter.ToUInt64(hash, 0);
        }
        public static ulong HashSha1(this IEnumerable<byte> buffer)
        {
            var hash = sha1.ComputeHash(buffer.ToArray());
            return BitConverter.ToUInt64(hash, 0);
        }
        public static ulong HashSha1(this string text)
        {
            var buffer = Encoding.UTF7.GetBytes(text);
            var hash = sha1.ComputeHash(buffer);
            return BitConverter.ToUInt64(hash, 0);
        }

        public static System.Collections.IComparer AbsComparer(double tolerance) => new DblComparerClass(tolerance);
        public static System.Collections.IComparer EpsilonComparer(int bits = 1) => new DblComparerClass(Math.Pow(2, -52+bits));        

        private class DblComparerClass : Comparer<double>, IEqualityComparer<double>
        {
            public DblComparerClass(double tolerance) 
            {
                this.Tolerance = Math.Abs(tolerance);
            }

            public double Tolerance { get; }

            public override int Compare(double x, double y)
            {
                if (x<(y-Tolerance)) return -1;
                if (x>(y+Tolerance)) return +1;
                return 0;
            }

            public bool Equals(double x, double y)
            {
                return Compare(x, y)==0;
            }

            public int GetHashCode(double obj)
            {
                if (Tolerance>0)
                {
                    unchecked
                    {
                        return (int)(obj/Tolerance);
                    }
                }
                return obj.GetHashCode();
            }
        }
    }

    public class EventArgs<T> : EventArgs
    {
        readonly T item;
        public EventArgs(T item) { this.item=item; }
        public T Item { get { return item; } }

        public delegate void Handler(object sender, EventArgs<T> e);
    }
}
