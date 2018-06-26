using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoEngine.Utils
{
    public class FastArrayList<E>
    {
        private E[] _values;
        private int _size;

                
        public FastArrayList() {
            _values = new E[16];
        }
        
        public FastArrayList(E[] values) {
            _values = new E[values.Length];
            Array.Copy(values, 0, _values, 0, values.Length);
            _size = values.Length;
        }
        
        public FastArrayList(int initialCapacity) {
            _values = new E[initialCapacity];
        }
        
        //=============================

        //public void sort(IComparer<E> comparator) {
        //    Util.quickSort(_values, _size, comparator);
        //}
        
        public bool add(E value) {
            addLast(value);
            return true;
        }
        
        public void add(int index, E value) {
            if (index > _size || index < 0) throw new IndexOutOfRangeException();
            addUnsafe(index, value);
        }
        
        public bool addAll(IEnumerable<E> collection) {
            int count = collection.Count();
            if (count == 0) return false;
            //
            ensureCapacity(_size + count);
            //
            IEnumerator<E> iter = collection.GetEnumerator();
            while (iter.MoveNext()) {
                addLastUnsafe(iter.Current);
            }
            //
            return true;
        }
        
        public bool addAll(E[] array) {
            int lenght = array.Length;
            if (lenght == 0) return false;
            //
            int newSize = _size + lenght;
            ensureCapacity(newSize);
            Array.Copy(array, 0, _values, _size, lenght);
            _size = newSize;
            return true;
        }
        
        public bool addAll(FastArrayList<E> list) {
            if (list.isEmpty()) return false;
            //
            ensureCapacity(_size + list._size);
            Array.Copy(list._values, 0, _values, _size, list._size);
            _size += list._size;
            //
            return true;
        }
        
        public void addAllIfAbsent(FastArrayList<E> list) {
            E value;
            for (int i = list.size(); i-- > 0;) {
                value = list.getUnsafe(i);
                if (!contains(value)) addLast(value);
            }
        }
        
        public bool addAll(int index, IEnumerable<E> collection) {
            int size = collection.Count();
            if (size == 0) return false;
            if (index > _size || index < 0) throw new IndexOutOfRangeException();
            //
            int spaceNeeded = _size + size;
            IEnumerator<E> iter = collection.GetEnumerator();
            int i = 0;
            //
            if (_values.Length < spaceNeeded) {
                E[] values = new E[spaceNeeded];
                Array.Copy(_values, 0, values, 0, index);
                //
                while (iter.MoveNext()) {
                    values[i++] = iter.Current;
                }
                //
                Array.Copy(_values, index, values, index + size, _size - index);
                _values = values;
            } else {
                Array.Copy(_values, index, _values, index + size, _size - index);
                //
                while (iter.MoveNext()) {
                    _values[i++] = iter.Current;
                }
                //
                Array.Copy(_values, 0, _values, 0, index);
            }
            //
            return false;
        }
        
        public void addFirst(E value) {
            ensureCapacity(_size + 1);
            addFirstUnsafe(value);
        }
        
        public void addFirstUnsafe(E value) {
            Array.Copy(_values, 0, _values, 1, _size);
            _values[0] = value;
        }
        
        public void addLast(E value) {
            ensureCapacity(_size + 1);
            addLastUnsafe(value);
        }
        
        public void addLastUnsafe(E value) {
            _values[_size++] = value;
        }
        
        public void addUnsafe(int index, E value) {
            int spaceNeeded = _size + 1;
            //
            if (_values.Length < spaceNeeded) {
                E[] values = new E[spaceNeeded];
                Array.Copy(_values, 0, values, 0, index);
                values[index] = value;
                Array.Copy(_values, index, values, index + 1, _size - index);
                _values = values;
            } else {
                Array.Copy(_values, index, _values, index + 1, _size - index);
                _values[index] = value;
            }
            //
            _size = spaceNeeded;
        }
        
        public void clear() {
            for (int i = _size; i-- > 0;) {
                _values[i] = default(E);
            }
            reset();
        }
        
        public void reset() {
            _size = 0;
        }
        
        public bool contains(object obj) {
            if (obj != null) {
                for (int i = _size; i-- > 0;) {
                    if (obj.Equals(_values[i])) return true;
                }
            } else {
                for (int i = _size; i-- > 0;) {
                    if (_values[i] == null) return true;
                }
            }
            //
            return false;
        }
        
        public E[] directArray() {return _values;}
        
        public void setCapacity(int capacity) {
            if (_values.Length != capacity) {
                E[] values = new E[capacity];
                Array.Copy(_values, 0, values, 0, _size);
                _values = values;
            }
        }
        
        public void ensureCapacity(int capacity) {
            if (_values.Length < capacity) {
                int newCapacity = capacity << 1;
                E[] values = new E[newCapacity > capacity ? newCapacity : capacity];
                Array.Copy(_values, 0, values, 0, _size);
                _values = values;
            }
        }
        
        public E get(int index) {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException();
            return getUnsafe(index);
        }
        
        public E getUnsafe(int index) {
            return _values[index];
        }
        
        public E getLastUnsafe() {
            return _values[_size - 1];
        }
        
        public int indexOf(object obj) {
            if (obj != null) {
                for (int i = 0; i < _size; i++) {
                    if (obj.Equals(_values[i])) return i;
                }
            } else {
                for (int i = 0; i < _size; i++) {
                    if (_values[i] == null) return i;
                }
            }
            return -1;
        }
        
        public int lastIndexOf(object obj) {
            if (obj != null) {
                for (int i = _size; i-- > 0;) {
                    if (obj.Equals(_values[i])) return i;
                }
            } else {
                for (int i = _size; i-- > 0;) {
                    if (_values[i] == null) return i;
                }
            }
            return -1;
        }
                
        public E remove(int index) {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException();
            return removeUnsafe(index);
        }
        
        public bool remove(object obj) {
            if (_size == 0) return false;
            return removeUnsafe(obj);
        }
        
        public bool removeAll(IEnumerable<E> c) {
            if (_size == 0 || c.Count() == 0) return false;
            //
            bool cng = false;
            IEnumerator<E> e = c.GetEnumerator();
            while (e.MoveNext()) {
                if (removeUnsafe(e.Current)) cng = true;
            }
            return cng;
        }
        
        public void removeAll(FastArrayList<E> list) {
            for (int i = list.size(); i-- > 0;) {
                removeUnsafeVoid(indexOf(list.getUnsafe(i)));
            }
        }
        
        public E removeFirst() {
            if (_size == 0) throw new Exception("Element not Found!");
            return removeFirstUnsafe();
        }
        
        public E removeFirstUnsafe() {
            E value = _values[0];
            Array.Copy(_values, 1, _values, 0, --_size);
            return value;
        }
        
        public E removeLast() {
            if (_size == 0) throw new Exception("Element not Found!");
            return removeLastUnsafe();
        }
        
        public E removeLastUnsafe() {
            E value = _values[--_size];
            _values[_size] = default(E);
            return value;
        }
        
        public E removeUnsafe(int index) {
            E value = _values[index];
            Array.Copy(_values, index + 1, _values, index, --_size - index);
            _values[_size] = default(E);
            return value;
        }
        
        public bool removeUnsafe(Object value) {
            int index = indexOf(value);
            if (index >= 0) {
                removeUnsafe(index);
                return true;
            }
            //
            return false;
        }
        
        public void removeUnsafeVoid(int index) {
            if (index < --_size) {
                Array.Copy(_values, index + 1, _values, index, _size - index);
            }
            _values[_size] = default(E);
        }
        
        public void removeVoid(int index) {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException();
            removeUnsafeVoid(index);
        }
                
        public E set(int index, E value) {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException();
            return setUnsafe(index, value);
        }
        
        public E setUnsafe(int index, E value) {
            E old = _values[index];
            _values[index] = value;
            return old;
        }
        
        public void setUnsafeVoid(int index, E value) {
            _values[index] = value;
        }
        
        public void setVoid(int index, E value) {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException();
            setUnsafeVoid(index, value);
        }
        
        public void shuffle(Random random) {
            for (int i = _size; i-- > 0;) {
                swapUnsafe(i, random.Next(_size));
            }
        }
        
        public void sort(IComparer<E> c) {
            Array.Sort(_values, c);
        }
        
        public void swap(int index1, int index2) {
            if (index1 < 0 || index1 >= _size) {
                throw new IndexOutOfRangeException("Index1 < 0 or >= _size");
            }
            if (index2 < 0 || index2 >= _size) {
                throw new IndexOutOfRangeException("Index1 < 0 or >= _size");
            }
            swapUnsafe(index1, index2);
        }
        
        public void swapUnsafe(int index1, int index2) {
            E value = _values[index1];
            _values[index1] = _values[index2];
            _values[index2] = value;
        }
                
        public T[] ToArray<T>(T[] array) {
            Array.Copy(_values, 0, array, 0, _size);
            return array;
        }
        
        public T[] ToArray<T>(T[] array, int offset) {
            Array.Copy(_values, 0, array, offset, _size);
            return array;
        }
        
        public override string ToString() {
            if (isEmpty()) return "[]";
            //
            E[] values = _values;
            int size = _size;
            StringBuilder isb = new StringBuilder(128);
            isb.Append('[');
            for (int i = 0; i < size; i++) {
                isb.Append(values[i]);
                if (i != size - 1) {
                    isb.Append(' ');
                    isb.Append(',');
                }
            }
            isb.Append(']');
            return isb.ToString();
        }
        
        public void trimToSize() {
            if (_values.Length != _size) {
                E[] values = new E[_size];
                Array.Copy(_values, 0, values, 0, _size);
                _values = values;
            }
        }
        
        public int size() {return _size;}
        
        public bool isEmpty() {return (_size == 0);}
        
        public bool containsAll(FastArrayList<E> list) {
            for (int i = list.size(); i-- > 0;) {
                if (!contains(list.getUnsafe(i))) return false;
            }
            return true;
        }
        
        public bool containsAll(IEnumerable<E> c) {
            IEnumerator<E> e = c.GetEnumerator();
            while (e.MoveNext()) {
                if (!contains(e.Current)) return false;
            }
            return true;
        }
    }
}