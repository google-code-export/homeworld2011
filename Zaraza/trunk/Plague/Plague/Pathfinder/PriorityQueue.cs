using System;
using System.Collections.Generic;

namespace PlagueEngine.Pathfinder
{
    public class PriorityQueue<T>
        where T : IComparable
    {
        public List<T> StoredValues;
        public Queue<T> StoredValues2;
        private readonly bool _prioritized;
        public PriorityQueue(bool prioritized)
        {
            _prioritized = prioritized;
            if (prioritized)
            {
                StoredValues = new List<T>();
            }
            else
            {
                StoredValues2 = new Queue<T>();
            }
        }

        public int Count
        {
            get { return _prioritized ? StoredValues.Count : StoredValues2.Count; }
        }

        public void Enqueue(T value)
        {
            if (_prioritized)
            {
                foreach (var storedValue in StoredValues)
                {
                    if (!storedValue.Equals(value)) continue;
                    var stv = storedValue as Node;
                    var v = value as Node;
                    if (v == null || stv == null) continue;
                    if (v.Value < stv.Value)
                    {
                        stv.Value = v.Value;
                        SortUp(StoredValues.IndexOf(value));
                    }
                    return;
                }
                StoredValues.Add(value);
                SortUp(StoredValues.Count - 1);
            }
            else
            {
                StoredValues2.Enqueue(value);
            }
        }
        public T Dequeue()
        {
            if (_prioritized)
            {
                if (Count == 0)
                    return default(T);
                var minValue = StoredValues[0];
                StoredValues.RemoveAt(0);
                return minValue;
            }
            return StoredValues2.Dequeue();
        }

        private void SortUp(int listIndex)
        {
            if (!_prioritized) return;
            while (listIndex != 0 && StoredValues[listIndex].CompareTo(StoredValues[listIndex - 1]) == -1)
            {
                Swap(listIndex, listIndex - 1);
                listIndex--;
            }
        }
        public void Swap(int index1, int index2)
        {
            var temp = StoredValues[index1];
            StoredValues[index1] = StoredValues[index2];
            StoredValues[index2] = temp;
        }
    }
    
}
