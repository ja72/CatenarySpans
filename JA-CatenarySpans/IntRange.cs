using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JA
{
    public struct IntRange : IEnumerable<int>
    {
        readonly int offset, count, stride;
        public IntRange(int offset, int count, int stride)
        {
            this.offset=offset;
            this.count=count;
            this.stride=stride;
        }
        public static IntRange Empty=new IntRange();
        public bool IsEmpty { get { return count==0; } }
        public int Start { get { return offset; } }
        public int Count { get { return count; } }
        public int Stride { get { return stride; } }
        public int End { get { return offset+count*stride; } }
        public static IntRange Span(int count)
        {
            return new IntRange(0, count, 1);
        }
        public static IntRange Span(int offset, int count)
        {
            return new IntRange(offset, count, 1);
        }

        public int this[int index]
        {
            get { return offset+index*stride; }
        }

        public static implicit operator IntRange(int index)
        {
            return Span(index, 1);
        }

        public static IntRange operator>>(IntRange range, int end)
        {
            return new IntRange(range.offset, (end-range.offset)/range.stride, range.stride);
        }
        public static IntRange operator<<(IntRange range, int start)
        {
            return new IntRange(start, (range.End-start)/range.stride, -range.stride);
        }
        public static IntRange operator+(IntRange range, int count)
        {
            return new IntRange(range.offset, count, range.count);
        }
        public static IntRange operator*(IntRange range, int stride)
        {
            return new IntRange(range.offset, range.count, stride);
        }

        #region IEnumerable<int> Members

        public IEnumerator<int> GetEnumerator()
        {
            for (int i=0; i<count; i++)
            {
                yield return offset+i*stride;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
