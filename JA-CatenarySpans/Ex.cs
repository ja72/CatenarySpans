using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JA
{
    public static class Ex
    {
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
    }

    public class EventArgs<T> : EventArgs
    {
        readonly T item;
        public EventArgs(T item) { this.item=item; }
        public T Item { get { return item; } }

        public delegate void Handler(object sender, EventArgs<T> e);
    }
}
