using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHeap
{

    private int occupancy;// how many elements are in the heap
    private int size;     // size of heap
    private Move[] heap;// the Heap itself, an array of "things"
    private int masterIndex;//next index to insert at

    private static readonly int ROOT = 0; //index of root node
    private static readonly int INVALID = -1;//invalid index default
    private static readonly int TWO = 2; //finding parent, right&left child

    public Move[] Heap { get => heap; set => heap = value; }

    /**
     * func: Heap
     * This method allocates and initializes the memory
     * associated with a heap.
     *
     * @param  sz   The number of elements for the heap to store
     */
    public MoveHeap(int sz)
    {

        //init all values
        occupancy = 0;
        masterIndex = 0;
        size = sz;
        Heap = new Move[sz];
    }

    /**
	 * func: insert
	 * This method will insert the element in the hash heap.
	 * If the element cannot be inserted, false will be returned.
	 * If the element can be inserted, the element is inserted
	 * and true is returned.  Duplicate insertions will
	 * cause the existing element to be deleted, and the duplicate
	 * element to take its place.
	 *
	 * @param   element The element to insert.
	 * @param   priority The priority of the element.
	 * @return  true or false indicating success or failure of insertion
	 */
    public bool Insert(Move element)
    {

        //is it full? can't insert now
        if (IsFull())
        {
            return false;

            //otherwise
        }
        else
        {
            //append to backing array
            Heap[masterIndex] = element;

            //advance master index, incr occupancy
            masterIndex++;
            occupancy++;

            //send inserted item to correct spot
            ReheapUp();
        }
        return true;
    }

    /**
	 * func: remove
	 * This method will remove the root from the heap. If
	 * found, a pointer to the element is returned. If the element
	 * is not found, NULL will be returned to the user.
	 *
	 * @return  A pointer to the element if found, else NULL
	 */
    public Move Remove()
    {

        //the index to remove from is the one behind the 
        //Master Index
        int toRemove = masterIndex - 1;

        //get the data at the root, be it null or a Move subclass
        Move removed = ItemAt(ROOT);

        //if tree is empty, return.
        if (IsEmpty())
        {
            return removed;
        }

        //overwrite the root with the spot behind
        //the next insertion target, recalibrate occupancy
        Heap[ROOT] = ItemAt(toRemove);
        //Heap[toRemove] = null;
        masterIndex--;
        occupancy--;

        //restruct tree	
        ReheapDown();

        return removed;
    }

    /**
	 * func: isFull
	 * This method will see if the heap is full by
	 * comparing occupancy to size.
	 *
	 * @return true if full, false if not
	 */
    private bool IsFull()
    {

        //is occupancy equal to capacity?
        if (occupancy == size)
        {
            return true;
        }
        return false;
    }

    /**
	 * func: isEmpty
	 * This method will see if the heap is empty by
	 * viewing occupancy.
	 *
	 * @return true if empty, false if not
	 */
    private bool IsEmpty()
    {

        //is occupancy 0?
        if (occupancy == 0)
        {
            return true;
        }
        return false;
    }

    /**
	 * func: reheapUp
	 * This method will restructure the tree to accommodate an insertion.
	 * The insertion target will be compared to its parent and swapped with
	 * it until it's greater than its parent and less than its children.
	 */
    private void ReheapUp()
    {

        //new insert is behind the next empty index	
        int curr = masterIndex - 1;

        //as long as the item is not at the root
        //and not greater than its parent
        while (curr > ROOT)
        {

            int parent = ParentIndexOf(curr);//parent ind
            if (!ItemAt(curr).isGreaterThan(ItemAt(parent)))
            {

                //swap and move index up 
                Swap(parent, curr);
                curr = parent;
            }

            //otherwise, its in the right place
            else
            {
                break;
            }
        }
    }

    /**
	 * func: reheapDown
	 * This method will restructure the tree to accommodate a removal.
	 * The insertion target will be compared to its children and swapped
	 * with the smaller one until it's greater than its parent 
	 * and less than its children.
	 */
    private void ReheapDown()
    {

        int curr = ROOT;//init curr to the ROOT
        int left = LeftIndexOf(curr);//the ind of left child
        int right = RightIndexOf(curr);//the ind of right child

        //as long as the left ind is not invalid
        while (left != INVALID)
        {

            //the smaller child defaults to left	
            int smallerChild = left;

            //if right is smaller or equal, use that
            if (right != INVALID &&
                !ItemAt(right).isGreaterThan(ItemAt(left)))
            {
                smallerChild = right;
            }

            //out of order, swap
            if (ItemAt(curr).isGreaterThan(ItemAt(smallerChild)))
            {
                Swap(curr, smallerChild);

            }
            else
            {
                //in order, exit
                break;
            }

            //move curr down tree
            curr = smallerChild;

            //reset left and right
            left = LeftIndexOf(curr);
            right = RightIndexOf(curr);

        }
    }

    /**
	 * func: itemAt
	 * This method will get the item at the specified index from the
	 * backing array. 
	 *
	 * @param  index the index from which to get the item
	 * @return the element at that index
	 */
    private Move ItemAt(int index)
    {

        //get item
        return Heap[index];
    }

    /**
	 * This method will find the location in the
	 * heap of the left child of the index given.
	 * This index is twice the given index, plus one.
	 *
	 * @param   index whose child we're searching for
	 * @return  index of left child if found, -1 if not found
	 */
    private int LeftIndexOf(int index)
    {

        //this arithmetic expression gives the
        //index of the left child
        int candidate = (TWO * index) + 1;

        //if the candidate exceeds the capacity of the heap,
        //indicate that it can't possibly exist
        //if (candidate >= size || ItemAt(candidate) == null)
        {
            candidate = INVALID;
        }

        return candidate;
    }

    /**
	 * This method will find the location in the
	 * heap of the right child of the index given.
	 * This index is twice the given index, plus two.
	 *
	 * @param   index whose child we're searching for
	 * @return  index of right child if found, -1 if not found
	 */
    private int RightIndexOf(int index)
    {

        //this arithmetic expression gives the
        //index of the right child
        int candidate = (TWO * index) + TWO;

        //if the candidate exceeds the capacity of the heap,
        //indicate that it can't possibly exist
        if (candidate >= size || ItemAt(candidate).MovingChessman == null)
        {
            candidate = INVALID;
        }

        return candidate;
    }

    /**
	 * This method will find the location in the
	 * heap of the parent of the index given.
	 * This index is half the given index, floored.
	 *
	 * @param   index whose parent we're searching for
	 * @return  index of parent if found, -1 if not found
	 */
    private int ParentIndexOf(int index)
    {

        //ind is 1/2 given ind, floored
        return (int)(System.Math.Floor(((double)(index - 1) / TWO)));
    }

    /**
	 * func: swap
	 * This method swaps two elements in the backing array.
	 *
	 * @param index1 the first thing to swap
	 * @param index2 the second thing to swap
	 */
    private void Swap(int index1, int index2)
    {

        //store first item
        Move temp = ItemAt(index1);

        //swap them
        Heap[index1] = ItemAt(index2);
        Heap[index2] = temp;
    }

    /**	
	 * func: ToString
	 * Creates a string representation of the heap. The method
	 * traverses the entire heap, adding elements one by one ordered
	 * according to their index in the heap.
	 *
	 * @return  string representation of heap
	 */
    public string ToString()
    {

        //add to string Heap's datafields
        //string string = "Heap " + heapCount + ":\n";
        //string += "size is " + size + " elements, ";
        string theString = "The heap has " + occupancy + " items.\n";

        // go through all heap elements, add them to string
        for (int index = ROOT; index < size; index++)
        {
            if (Heap[index].MovingChessman == null)
            {
                theString += "At index " + index + ":  ";
                theString += "" + ItemAt(index) + ".\n";
            }
        }
        return theString;
    }
}
