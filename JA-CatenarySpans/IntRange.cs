using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JA
{
    public readonly struct IntRange : IVector<int>
    {
        public IntRange(int offset, int count, int stride)
        {
            this.Start=offset;
            this.Count=count;
            this.Stride=stride;
        }
        public static readonly IntRange Empty=new IntRange();

        public bool IsEmpty { get { return Count==0; } }
        public int Start { get; }
        public int Count { get; }
        public int Stride { get; }
        public int End { get { return Start+Count*Stride; } }
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
            get { return Start+index*Stride; }
        }

        public int[] ToArray()
        {
            int[] array = new int[Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Start + i*Stride;
            }
            return array;
        }

        public static implicit operator IntRange(int index)
        {
            return Span(index, 1);
        }

        public static IntRange operator>>(IntRange range, int end)
        {
            return new IntRange(range.Start, (end-range.Start)/range.Stride, range.Stride);
        }
        public static IntRange operator<<(IntRange range, int start)
        {
            return new IntRange(start, (range.End-start)/range.Stride, -range.Stride);
        }
        public static IntRange operator+(IntRange range, int count)
        {
            return new IntRange(range.Start, count, range.Count);
        }
        public static IntRange operator*(IntRange range, int stride)
        {
            return new IntRange(range.Start, range.Count, stride);
        }

        #region IEnumerable<int> Members

        public IEnumerator<int> GetEnumerator()
        {
            for (int i=0; i<Count; i++)
            {
                yield return Start+i*Stride;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<int>.Add(int item)
        {
            throw new NotSupportedException();
        }

        void ICollection<int>.Clear()
        {
            throw new NotSupportedException();
        }
        bool ICollection<int>.Remove(int item)
        {
            throw new NotSupportedException();
        }
        public bool IsZero => Start==0 && Count==0 && Stride==0;
        public bool Contains(int item)
        {
            // Start + index*Stride = item
            int index;
            if (Stride==1)
            {
                index = item-Start;
                return index>=0 && index<Count;
            }
            else if (Stride!=0)
            {
                index = Math.DivRem(item-Start, Stride, out int remain);
                return remain==0 && index>=0 && index<Count;
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            Array.Copy(ToArray(), 0, array, arrayIndex, Count);
        }
        public void CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(ToArray(), 0, array, arrayIndex, Count);
        }
        int System.Collections.ICollection.Count => Count;
        int ICollection<int>.Count => Count;
        public bool IsReadOnly { get => true; }
        bool System.Collections.ICollection.IsSynchronized => false;
        object System.Collections.ICollection.SyncRoot => null;

        #endregion
    }
}
