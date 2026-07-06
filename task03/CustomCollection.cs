using System;
using System.Collections;
using System.Collections.Generic;

namespace task03
{
    public class CustomCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items;


        public CustomCollection()
        {
            _items = new List<T>();
        }

        public CustomCollection(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Входная коллекция не может быть null.");
            }

            _items = new List<T>(source);
        }


        public void Add(T item)
        {
            _items.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> GetReverseEnumerator()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                yield return _items[i];
            }
        }
    }
}