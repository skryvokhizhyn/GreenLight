
namespace GreenLightTracker.Src
{
    public class DoublyLinkedList<T>
    {
        public class Node
        {
            public Node Next { get; set; }
            public Node Prev { get; set; }
            public T Value { get; set; }
        }

        public Node Front { get; private set; }
        public Node Last { get; private set; }

        public void AddFirst(T val)
        {
            var prevFront = Front;
            Front = new Node() { Value = val };
            Front.Next = prevFront;
            prevFront.Prev = Front;
        }

        public void AddLast(T val)
        {
            Last.Next = new Node() { Value = val };
            Last.Next.Prev = Last;
            Last = Last.Next;
        }

        public void AddAfter(Node node, T val)
        {
            var nodeNext = node.Next;
            var newNode = new Node() { Value = val };

            node.Next = newNode;
            newNode.Prev = node;

            if (nodeNext != null)
            {
                nodeNext.Prev = newNode;
                newNode.Next = nodeNext;
            }
            else
            {
                Last = newNode;
            }
        }

        public void Remove(Node node)
        {
            if (node == null)
            {
                return;
            }

            if (node.Prev != null)
            {
                node.Prev.Next = node.Next;
            }
            else
            {
                Front = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
            else
            {
                Last = node.Prev;
            }
        }
    }
}