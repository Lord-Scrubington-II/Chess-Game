using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This is a custom Priority Queue that outputs elements in their sorted order using a binary heap.
/// Comparison between elements is accomplished using a <c>delegate</c> that takes elements of type <c>T</c>.
/// <para>
///     <c>Enqueue()</c> runs in <c>O(log(n))</c> time, 
///     while <c>LookTop()</c> and <c>Dequeue()</c> take <c>O(1)</c>.
/// </para>
/// </summary>
/// <typeparam name="T">The type of the elements which are to be inserted into this PriorityQueue.</typeparam>
public class PriorityQueue<T> : IEnumerable<T>, ICollection, IReadOnlyCollection<T> //where T : IComparable
{
    // Backing heap information
    private T[] heap; // The priority queue is backed by a binary heap, which is an array of generics
    private int count; // How many elements are in the heap?
    private int capacity; // Maximum capacy of the heap
    private int masterIndex; // The next index to insert at

    // static constants
    private static readonly int ROOT = 0;
    private static readonly int INVALID = -1;
    private static readonly int DefaultCapacity = 256;

    /// <summary>
    /// Delegate: CompareFunction
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
    /// </summary>
    /// <param name="first">The first item.</param>
    /// <param name="second">The second item.</</param>
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

        //append to backing array
        Heap[masterIndex] = element;
        masterIndex++;
        count++;

        //send inserted item to correct spot
        ReHeapUp();
    }

    /// <summary>
    /// Removes the element at the top of the heap and returns it.
    /// </summary>
    /// <returns>The element at the top of the heap.</returns>
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

            //overwrite the root with the spot behind the next insertion target, recalibrate occupancy
            Heap[ROOT] = ItemAt(toRemove);
            Heap[toRemove] = default(T);
            masterIndex--;
            count--;

            //restruct tree	
            ReHeapDown();

            return removed;
        }
    }

    /// <summary>
    /// Retrieves the element at the top of the heap without removing it.
    /// </summary>
    /// <returns>The element at the top of the heap.</returns>
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
        foreach(T elem in this)
        {
            if(elem.Equals(query))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes the element at the top of the heap and returns it through the <c>out</c> parameter.
    /// </summary>
    /// <param name="result">A pointer to an object of type T.</param>
    /// <returns>false if the heap was empty, true otherwise.</returns>
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
    /// Retrieves the element at the top of the heap without removing it. 
    /// Returns the item through the <c>out</c> parameter.
    /// </summary>
    /// <param name="result">A pointer to an object of type T.</param>
    /// <returns>false if the heap was empty, true otherwise.</returns>
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
    /// Retructures the tree to accomodate an insertion.
    /// The inserted item will be compared to its parent and swapped 
    /// with it until it's of higher priority than its parent 
    /// and of lower priority than its children.
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
    /// This method will restructure the tree to accommodate a removal.
	/// The insertion target will be compared to its children and swapped
    /// with the smaller one until it's of lower priority than its parent 
	/// and of higher priority than its children.
    /// </summary>
    private void ReHeapDown()
    {

        int curr = ROOT;//init curr to the root
        int left = LeftIndexOf(curr);//the ind of left child
        int right = RightIndexOf(curr);//the ind of right child

        //as long as the left ind is not invalid
        while (left != INVALID)
        {
            //the child of lower priority defaults to left
            int smallerChild = left;

            //if right is of lower priority or equal, use that
            if (right != INVALID 
                && PriorityCompare(ItemAt(left), ItemAt(right)))
            {
                smallerChild = right;
            }

            //out of order, swap
            if (PriorityCompare(ItemAt(curr), ItemAt(smallerChild)))
            {
                Swap(curr, smallerChild);
            }
            else //this means everything is in order, so exit.
            {
                break;
            }

            //move curr down tree
            curr = smallerChild;

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
        T[] newHeap = new T[Heap.Length * 2];

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

        //if the candidate exceeds the capacity of the heap,
        //indicate that it can't possibly exist
        if (candidate >= capacity || ItemAt(candidate) == null)
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

        //if the candidate exceeds the capacity of the heap,
        //indicate that it can't possibly exist
        if (candidate >= capacity || ItemAt(candidate) == null)
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

    public T[] ToArraySorted()
    {
        //the out-array holds the sorted heap representation
        T[] outArray = new T[this.Count];
        
        //the search-queue stores an element's children to perform BFS
        Queue<int> searchQueue = new Queue<int>(this.Count);
        searchQueue.Enqueue(ROOT);

        //Perform a Level-Ordered Traversal (i.e. BFS) of the heap and return a sorted array
        int ind = 0;
        while(searchQueue.Count > 0)
        {
            int currentEval = searchQueue.Dequeue();
            outArray[ind] = ItemAt(currentEval);

            //queue up the children if they exist
            if(LeftIndexOf(currentEval) < this.Count)
            {
                searchQueue.Enqueue(LeftIndexOf(currentEval));

                if (RightIndexOf(currentEval) < this.Count)
                {
                    searchQueue.Enqueue(RightIndexOf(currentEval));
                }
            }
            ind++;
        }

        return outArray;
    }

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

    public override bool Equals(object other)
    {
        return Heap.Equals(other);
    }

    /// <summary>
    /// <c>ToString()</c> override for PriorityQueues.
    /// </summary>
    /// <returns>The elements of the backing heap.</returns>
    public override string ToString()
    {

        //add to string Heap's datafields
        string theString = $"The heap has {Count} items.\n";

        // go through all heap elements, add them to string
        for (int index = ROOT; index < Count; index++)
        {
            if (Heap[index] != null) //should never be the case
            {
                theString += $"At index {index}:  ";
                theString += $" {ItemAt(index)} .\n";
            }
        }
        return theString;
    }

    public static List<IComparable> HeapSort(ICollection<IComparable> toSort, bool usingMinHeap)
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
