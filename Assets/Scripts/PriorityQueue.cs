using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace DotNETPriorityQueue
{
    /// <summary>
    /// This is a custom Priority Queue that outputs elements in their sorted order using a binary heap.
    /// Comparison between elements is accomplished using a <c>delegate</c> that takes elements of type <c>T</c>.
    /// <para>
    ///     <c>Enqueue()</c> runs in <c>O(log(n))</c> time, 
    ///     while <c>LookTop()</c> and <c>Dequeue()</c> take <c>O(1)</c>.
    /// </para>
    /// Author: Zane Wang
    /// </summary>
    /// <typeparam name="T">The type of the elements which are to be inserted into this PriorityQueue.</typeparam>
    public class PriorityQueue<T> : IEnumerable<T>, ICollection, IReadOnlyCollection<T> //where T : IComparable
    {
        // Backing heap information
        /// <summary>
        /// The priority queue is backed by a binary heap, which is an array of generics.
        /// An item on the heap is in the correct spot if it is
        /// of higher priority than its children
        /// and
        /// of lower priority than its parent.
        /// </summary>
        private T[] heap;
        private int count; // How many elements are in the heap?
        private int capacity; // Maximum capacy of the heap
        private int masterIndex; // The next index to insert at

        // static constants
        private static readonly int ROOT = 0;
        private static readonly int INVALID = -1;
        private static readonly int DefaultCapacity = 256;

        /// <summary>
        /// Delegate: PriorityCompare (Type: CompareFunction)
        /// <para>
        ///     The priority queue contains a comparison delegate which compares the priorities of two of its elements.
        ///     This delegate can be provided through the constructor or it can be one of the following default implementations:
        ///     <list type="bullet">
        ///     <code>
        ///         <item>PriorityQueue.MaxHeapCompare</item>
        ///         <item>PriorityQueue.MinHeapCompare</item>
        ///     </code>
        ///     </list>
        /// </para>
        /// Priority Queues that use one of the default comparison functions must take elements that implement <c>IComparable</c>.
        /// </summary>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second elemnt.</param>
        /// <returns>
        ///     <c>true</c> if the first item is of greater priority, and <c>false</c> otherwise.
        /// </returns>
        public delegate bool CompareFunction(T first, T second);
        private CompareFunction PriorityCompare;

        /// <summary>
        /// Default comparison function for the PriorityQueue. 
        /// The inserted type must implement IComparable. This will cause the PriorityQueue to use a backing MaxHeap.
        /// (Greater elements will have higher priority.)
        /// </summary>
        /// <param name="first">The first item.</param>
        /// <param name="second">The second item.</param>
        /// <returns>True if the first item is greater, false if not.</returns>
        public static bool MaxHeapCompare(T first, T second)
        {
            if (first is IComparable && second is IComparable)
            {
                //if the first elem is greater
                if (((IComparable)first).CompareTo((IComparable)second) > 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported comparison of non-comparable types {first.GetType().Name} and {second.GetType().Name}"
                );
            }
        }

        /// <summary>
        /// Second default comparison function for the PriorityQueue. 
        /// The inserted type must implement IComparable. This will cause the PriorityQueue to use a backing MinHeap.
        /// (Lesser elements will have higher priority.)
        /// </summary>
        /// <param name="first">The first item.</param>
        /// <param name="second">The second item.</param>
        /// <returns>True if the first item is lesser, false if not.</returns>
        public static bool MinHeapCompare(T first, T second)
        {
            if (first is IComparable && second is IComparable)
            {
                //if the first elem is lesser
                if (((IComparable)first).CompareTo((IComparable)second) < 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported comparison of non-comparable types {first.GetType().Name} and {second.GetType().Name}"
                );
            }
        }

        //Properties for the PriorityQueue's metadata
        public T[] Heap { get => heap; private set => heap = value; }
        public int Count { get => count; }
        public bool IsSynchronized => Heap.IsSynchronized;
        public object SyncRoot => Heap.SyncRoot;

        /// <summary>
        /// The default constructor will initialize a PriorityQueue with a backing MaxHeap of size 256.
        /// </summary>
        public PriorityQueue()
        {
            count = 0;
            masterIndex = 0;

            //init heap
            capacity = DefaultCapacity;
            Heap = new T[DefaultCapacity];

            //init delegate
            PriorityCompare = MaxHeapCompare;
        }

        /// <summary>
        /// Constructor overload 1: specify initial capacity of the backing heap.
        /// </summary>
        /// <param name="size">Initial capacity of the backing heap.</param>
        public PriorityQueue(int size)
        {
            count = 0;
            masterIndex = 0;

            //init heap
            capacity = size;
            Heap = new T[size];

            //init delegate
            PriorityCompare = MaxHeapCompare;
        }

        /// <summary>
        /// Constructor overload 2: specify a comparison function <c>lambda</c>.
        /// </summary>
        /// <param name="lambda">The <c>CompareFunction</c> to use.</param>
        public PriorityQueue(CompareFunction lambda)
        {
            count = 0;
            masterIndex = 0;

            //init heap
            capacity = DefaultCapacity;
            Heap = new T[DefaultCapacity];

            //init delegate
            PriorityCompare = lambda;
        }

        /// <summary>
        /// Constructor overload 3: specify initial heap size and a comparison function <c>lambda</c>.
        /// </summary>
        /// <param name="size">Initial heap size.</param>
        /// <param name="lambda">The comparison function to use.</param>
        public PriorityQueue(int size, CompareFunction lambda)
        {
            count = 0;
            masterIndex = 0;

            //init heap
            capacity = size;
            Heap = new T[size];

            //init delegate
            PriorityCompare = lambda;
        }

        /// <summary>
        /// Adds an element to the Priority Queue.
        /// </summary>
        /// <param name="element">The element to insert.</param>
        public void Enqueue(T element)
        {
            if (IsFull())
            {
                //expand the backing array
                ExpandHeap();
            }

            //append to the heap
            Heap[masterIndex] = element;
            masterIndex++;
            count++;

            //send inserted item to correct index
            ReHeapUp();
        }

        /// <summary>
        /// Removes the element at the front of the PriorityQueue and returns it.
        /// </summary>
        /// <returns>The element at the top of the backing heap.</returns>
        public T Dequeue()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException($"This PriorityQueue<{typeof(T).Name}> is empty!");
            }
            else
            {
                //the index to remove from is the one behind the Master Index
                int toRemove = masterIndex - 1;
                T removed = ItemAt(ROOT);

                //overwrite the root with the spot behind the next insertion target,
                //then recalibrate occupancy
                Heap[ROOT] = ItemAt(toRemove);
                Heap[toRemove] = default(T);
                masterIndex--;
                count--;

                //restructure the heap
                ReHeapDown();

                return removed;
            }
        }

        /// <summary>
        /// Retrieves the element at the front of the PriorityQueue without removing it.
        /// </summary>
        /// <returns>The element at the top of the backing heap.</returns>
        public T LookTop()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException($"This PriorityQueue<{typeof(T).Name}> is empty!");
            }
            else
            {
                return Heap[ROOT];
            }
        }

        /// <summary>
        /// Checks to see if a query element exists in the priority queue.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool Contains(T query)
        {
            foreach (T elem in this)
            {
                if (elem.Equals(query))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the element at the front of the PriorityQueue and returns it through the <c>out</c> parameter.
        /// </summary>
        /// <param name="result">A pointer to an object of type T.</param>
        /// <returns>false if the backing heap was empty, true otherwise.</returns>
        public bool TryDequeue(out T result)
        {
            if (IsEmpty())
            {
                result = default(T);
                return false;
            }
            else
            {
                //the index to remove from is the one behind the Master Index
                int toRemove = masterIndex - 1;
                result = ItemAt(ROOT);

                //overwrite the root with the spot behind the next insertion target, recalibrate occupancy
                Heap[ROOT] = ItemAt(toRemove);
                Heap[toRemove] = default(T);
                masterIndex--;
                count--;

                //restructure the heap
                ReHeapDown();

                return true;
            }
        }

        /// <summary>
        /// Retrieves the element at the front of the PriorityQueue without removing it. 
        /// Returns the item through the <c>out</c> parameter.
        /// </summary>
        /// <param name="result">A pointer to an object of type T.</param>
        /// <returns>false if the backing heap was empty, true otherwise.</returns>
        public bool TryLookTop(out T result)
        {
            if (IsEmpty())
            {
                result = default(T);
                return false;
            }
            else
            {
                result = Heap[ROOT];
                return true;
            }
        }

        /// <summary>
        /// This method sees if the heap is full by comparing occupancy to size.
        /// </summary>
        /// <returns>True if the heap is full, false otherwise.</returns>
        private bool IsFull()
        {
            if (count == capacity)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method sees if the heap is empty by checking its occupancy.
        /// </summary>
        /// <returns>True if the heap is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            if (Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method restructures the tree to accomodate an insertion.
        /// The inserted item is compared to its parent and swapped 
        /// with it until it's of lower priority than its parent 
        /// and of higher priority than its children.
        /// </summary>
        private void ReHeapUp()
        {
            //new insert is behind the next empty index	
            int curr = masterIndex - 1;

            //as long as the item is not at the root
            //and not of higher priority than its parent
            while (curr > ROOT)
            {
                int parent = ParentIndexOf(curr);

                //if the current item is of higher priority than its parent
                if (PriorityCompare(ItemAt(curr), ItemAt(parent)))
                {
                    //swap and move index up 
                    Swap(parent, curr);
                    curr = parent;
                }
                else //otherwise, its in the right place
                {
                    break;
                }
            }
        }

        /// <summary>
        /// This method restructures the tree to accommodate a removal.
        /// The insertion target is compared to its children and swapped
        /// with the children of higher priority one until it's of 
        /// lower priority than its parent and of higher priority than its children.
        /// </summary>
        private void ReHeapDown()
        {
            int curr = ROOT;//init curr to the root
            int left = LeftIndexOf(curr);//the ind of left child
            int right = RightIndexOf(curr);//the ind of right child

            //as long as the left ind is not invalid
            while (left != INVALID)
            {
                //the child of higher priority defaults to left
                int toSwap = left;

                //if right exists and is of higher priority, use it instead
                if (right != INVALID && PriorityCompare(ItemAt(right), ItemAt(left)))
                {
                    toSwap = right;
                }

                //the current item is not of higher priority than its smaller child
                if (!PriorityCompare(ItemAt(curr), ItemAt(toSwap)))
                {
                    Swap(curr, toSwap);
                }
                else //this means everything is in order, so exit.
                {
                    break;
                }

                //move curr down tree
                curr = toSwap;

                //reset left and right
                left = LeftIndexOf(curr);
                right = RightIndexOf(curr);

            }
        }

        /// <summary>
        /// Expands the backing heap of the PriorityQueue.
        /// </summary>
        private void ExpandHeap()
        {
            int newCapacity = Heap.Length * 2;
            T[] newHeap = new T[newCapacity];
            capacity = newCapacity;

            for (int i = 0; i < Count; i++)
            {
                newHeap[i] = Heap[i];
            }

            Heap = newHeap;
        }

        /// <summary>
        /// This method will get the item at the specified index from the
        /// backing array.
        /// </summary>
        /// <param name="index">The index from which to retrieve the item.</param>
        /// <returns>The item at that index in the backing heap.</returns>
        private T ItemAt(int index)
        {
            return Heap[index];
        }

        /// <summary>
        /// Finds the location in the heap of the left child of the given index.
        /// This index is equal to <c>(2 * index) + 1</c>.
        /// </summary>
        /// <param name="index">The index whose child we're searching for.</param>
        /// <returns>The index of the left child if found, else -1.</returns>
        private int LeftIndexOf(int index)
        {
            //this arithmetic expression gives the
            //index of the left child
            int candidate = (2 * index) + 1;

            //if the candidate exceeds the number of elements in the heap,
            //indicate that it can't possibly exist
            if (candidate >= count || ItemAt(candidate) == null)
            {
                candidate = INVALID;
            }

            return candidate;
        }

        /// <summary>
        /// Finds the location in the heap of the right child of the given index.
        /// This index is equal to <c>(2 * index) + 2</c>.
        /// </summary>
        /// <param name="index">The index whose child we're searching for.</param>
        /// <returns>The index of the right child if found, else -1.</returns>
        private int RightIndexOf(int index)
        {
            //this arithmetic expression gives the
            //index of the right child
            int candidate = (2 * index) + 2;

            //if the candidate exceeds the number of elements in the heap,
            //indicate that it can't possibly exist
            if (candidate >= count || ItemAt(candidate) == null)
            {
                candidate = INVALID;
            }

            return candidate;
        }

        /// <summary>
        /// Finds the location in the heap of the parent of the given index.
        /// This index is equal to <c>floor((index - 1) / 2)</c>.
        /// </summary>
        /// <param name="index">The index whose parent we're searching for.</param>
        /// <returns>The index of the parent if found, else -1.</returns>
        private int ParentIndexOf(int index)
        {
            //ind is 1/2 given ind, floored
            return (int)Math.Floor((double)(index - 1) / 2);
        }

        /// <summary>
        /// Swaps two elements in the backing array.
        /// </summary>
        /// <param name="index1">First thing to swap.</param>
        /// <param name="index2">Second thing to swap.</param>
        private void Swap(int index1, int index2)
        {
            T temp = ItemAt(index1);
            Heap[index1] = ItemAt(index2);
            Heap[index2] = temp;
        }

        /// <summary>
        /// Empties the PriorityQueue.
        /// </summary>
        public void Clear()
        {
            masterIndex = 0;
            count = 0;
            Heap = new T[Heap.Length];
        }

        /// <summary>
        /// Performs implicit BFS on the heap to construct an array of its contents in near-sorted order.
        /// </summary>
        /// <returns>A sorted array representation of the backing heap.</returns>
        public T[] ToArraySortedBFS()
        {
            //todo: benchmark this. it's possible that BFS + near-sorted Array.Sort() is slightly faster.

            //the out-array holds the sorted heap representation
            T[] outArray = new T[this.Count];

            //the search-queue stores an element's children to perform BFS
            Queue<int> searchQueue = new Queue<int>(this.Count);
            searchQueue.Enqueue(ROOT);

            //Perform a Level-Ordered Traversal (i.e. BFS) of the heap and return a sorted array
            int ind = 0;
            while (searchQueue.Count > 0)
            {
                int currentEval = searchQueue.Dequeue();
                outArray[ind] = ItemAt(currentEval);
                int right = RightIndexOf(currentEval);
                int left = LeftIndexOf(currentEval);

                //queue up the children in order, if they exist

                //two-child case: if the right child exists, the left child also exists
                if (right != INVALID)
                {
                    if (PriorityCompare(ItemAt(left), ItemAt(right)))
                    {
                        searchQueue.Enqueue(left);
                        searchQueue.Enqueue(right);
                    }
                    else
                    {
                        searchQueue.Enqueue(right);
                        searchQueue.Enqueue(left);
                    }
                }
                //one child case: only left exists
                else if (left != INVALID)
                {
                    searchQueue.Enqueue(left);
                } //no child case: do nothing

                ind++;
            }
            Array.Sort(outArray);
            return outArray;

        }

        /// <summary>
        /// Retrieve an array of the backing heap's contents in sorted order.
        /// </summary>
        /// <returns>A sorted array representation of the backing heap.</returns>
        public T[] ToArraySorted()
        {
            T[] outArray = new T[this.Count];
            T[] oldHeap = (T[])Heap.Clone();
            int oldCount = Count;

            int ind = 0;
            while (!this.IsEmpty())
            {
                //populate array with ordered contents
                outArray[ind] = this.Dequeue();
                ind++;
            }

            //restore original heap
            this.Heap = oldHeap;
            count = oldCount;
            masterIndex = count;

            return outArray;
        }

        /// <summary>
        /// Returns a shallow copy of the backing heap.
        /// </summary>
        /// <returns>An unsorted array representation of the backing heap.</returns>
        public T[] ToArray()
        {
            return (T[])Heap.Clone();
        }

        public void CopyTo(Array array, int index)
        {
            Heap.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)Heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Heap.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return Heap.GetHashCode();
        }

        /// <summary>
        /// <c>Equals()</c> override for PriorityQueues.
        /// </summary>
        /// <param name="other">Another <c>object</c>, which can only be equal to <c>this</c> if it is also a PriorityQueue.</param>
        /// <returns><c>true</c> if the other object is also a PriorityQueue containing the same type and with the same contents, <c>false</c> otherwise.</returns>
        public override bool Equals(object other)
        {
            if (other.GetType() == typeof(PriorityQueue<T>))
            {
                return (Heap.Equals(((PriorityQueue<T>)other).Heap) && count == ((PriorityQueue<T>)other).Count);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <c>ToString()</c> override for PriorityQueues.
        /// </summary>
        /// <returns>The elements of the backing heap.</returns>
        public override string ToString()
        {

            //add to string Heap's datafields
            string theString = $"The heap contains {count} items and has capacity {capacity}.\n";

            // go through all heap elements, add them to string
            for (int index = ROOT; index < count; index++)
            {
                if (Heap[index] != null) //should never be the case
                {
                    theString += $"At index {index}:  ";
                    theString += $" {ItemAt(index)}.\n";
                }
            }
            return theString;
        }

        /// <summary>
        /// Sorts any given collection of <c>IComparables</c> into a <c>List</c> using a PriorityQueue.
        /// </summary>
        /// <param name="toSort">The collection to sort.</param>
        /// <param name="usingMinHeap"><c>true</c> if the elements are to be sorted in ascending order, <c>false</c> if descending.</param>
        /// <returns>A sorted <c>List</c> of <c>IComparables</c>.</returns>
        public static List<IComparable> HeapSort(in ICollection<IComparable> toSort, bool usingMinHeap)
        {
            //the out list stores the elements in sorted order
            var outList = new List<IComparable>(toSort.Count);

            //the inheap collects elements in sorted order
            PriorityQueue<IComparable> inHeap;
            if (usingMinHeap)
            {
                inHeap = new PriorityQueue<IComparable>(PriorityQueue<IComparable>.MinHeapCompare);
            }
            else
            {
                inHeap = new PriorityQueue<IComparable>(PriorityQueue<IComparable>.MaxHeapCompare);
            }

            //heapsort
            foreach (IComparable elem in toSort)
            {
                inHeap.Enqueue(elem);
            }
            while (!inHeap.IsEmpty())
            {
                outList.Add(inHeap.Dequeue());
            }

            return outList;
        }
    }
}