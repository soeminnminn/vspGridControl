using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace S16.Utils
{
    #region Node<T> Class
    [Serializable]
    public class Node<T> : ICloneable
    {
        #region Variables
        internal int m_childCount;
        internal int m_index;

        internal Node<T>[] m_children = null;
        internal Node<T> m_parent;
        internal T m_value;

        private NodeCollection<T> m_nodes;
        #endregion

        #region Constructor
        public Node()
        {
        }

        public Node(T value)
            : this()
        {
            this.m_value = value;
        }

        public Node(T value, Node<T>[] children)
            : this(value)
        {
            this.m_nodes.AddRange(children);
        }
        #endregion

        #region Internal Methods
        internal int AddSorted(IComparer treeListNodeSorter, Node<T> node)
        {
            int index = 0;

            if (this.m_childCount > 0)
            {
                int num2;
                int childCount;
                int num4;

                if (treeListNodeSorter != null)
                {
                    num2 = 0;
                    childCount = this.m_childCount;

                    while (num2 < childCount)
                    {
                        num4 = (num2 + childCount) / 2;

                        if (treeListNodeSorter.Compare(this.m_children[num4], node) <= 0)
                        {
                            num2 = num4 + 1;
                        }
                        else
                        {
                            childCount = num4;
                        }
                    }

                    index = num2;
                }
            }

            node.SortChildren(treeListNodeSorter);
            this.InsertNodeAt(index, node);

            return index;
        }

        internal void Clear()
        {
            while (this.m_childCount > 0)
            {
                this.m_children[this.m_childCount - 1].Remove();
            }
            this.m_children = null;
        }

        internal void EnsureCapacity(int num)
        {
            int num2 = num;
            if (num2 < 4)
            {
                num2 = 4;
            }

            if (this.m_children == null)
            {
                this.m_children = new Node<T>[num2];
            }
            else if ((this.m_childCount + num) > this.m_children.Length)
            {
                int num3 = this.m_childCount + num;
                if (num == 1)
                {
                    num3 = this.m_childCount * 2;
                }

                Node<T>[] destinationArray = new Node<T>[num3];
                Array.Copy(this.m_children, 0, destinationArray, 0, this.m_childCount);
                this.m_children = destinationArray;
            }
        }

        internal void InsertNodeAt(int index, Node<T> node)
        {
            this.EnsureCapacity(1);
            node.m_parent = this;
            node.m_index = index;

            for (int i = this.m_childCount; i > index; i--)
            {
                Node<T> node2;
                this.m_children[i] = node2 = this.m_children[i - 1];
                node2.m_index = i;
            }

            this.m_children[index] = node;
            this.m_childCount++;
        }
        #endregion

        #region Public Methods
        public virtual object Clone()
        {
            Node<T> node = new Node<T>(this.m_value);
            if (this.m_childCount > 0)
            {
                node.m_children = new Node<T>[this.m_childCount];
                for (int i = 0; i < this.m_childCount; i++)
                {
                    node.Nodes.Add((Node<T>)this.m_children[i].Clone());
                }
            }
            return node;
        }

        public int GetNodeCount(bool includeSubTrees)
        {
            int childCount = this.m_childCount;

            if (includeSubTrees)
            {
                for (int i = 0; i < this.m_childCount; i++)
                {
                    childCount += this.m_children[i].GetNodeCount(true);
                }
            }

            return childCount;
        }

        public Node<T>[] GetAllNodes(bool includeSubTrees)
        {
            ArrayList list = new ArrayList();
            if (this.HasChildren)
            {
                if (includeSubTrees)
                {
                    for(int i=0; i<this.m_childCount; i++)
                    {
                        list.Add(this.m_children[i]);
                        if (this.m_children[i].HasChildren)
                        {
                            list.AddRange(this.m_children[i].GetAllNodes(true));
                        }
                    }
                }
                else
                {
                    list.AddRange(this.m_children);
                }
            }

            Node<T>[] allNodes = new Node<T>[list.Count];
            list.CopyTo(allNodes);

            return allNodes;
        }

        public void Remove()
        {
            for (int i = 0; i < this.m_childCount; i++)
            {
                this.m_children[i].Remove();
            }

            if (this.m_parent != null)
            {
                if (this.m_index == (this.m_parent.m_childCount - 1))
                {
                    this.m_parent.m_children[this.m_index] = null;
                }

                for (int j = this.m_index; j < (this.m_parent.m_childCount - 1); j++)
                {
                    Node<T> node;
                    this.m_parent.m_children[j] = node = this.m_parent.m_children[j + 1];
                    node.m_index = j;
                }

                this.m_parent.m_childCount--;
                this.m_parent = null;
            }
        }

        public void SortChildren(IComparer treeListNodeSorter)
        {
            if (this.m_childCount > 0)
            {
                Node<T>[] nodeArray = new Node<T>[this.m_childCount];
                if (treeListNodeSorter == null)
                {
                    throw new ArgumentNullException("Node.SortChildren.treeListNodeSorter");
                }
                else
                {
                    for (int k = 0; k < this.m_childCount; k++)
                    {
                        int num5 = -1;
                        for (int m = 0; m < this.m_childCount; m++)
                        {
                            if (this.m_children[m] != null)
                            {
                                if (num5 == -1)
                                {
                                    num5 = m;
                                }
                                else if (treeListNodeSorter.Compare(this.m_children[m], this.m_children[num5]) <= 0)
                                {
                                    num5 = m;
                                }
                            }
                        }

                        nodeArray[k] = this.m_children[num5];
                        this.m_children[num5] = null;
                        nodeArray[k].m_index = k;
                        nodeArray[k].SortChildren(treeListNodeSorter);
                    }
                    this.m_children = nodeArray;
                }
            }
        }
        #endregion

        #region Override Methods
        public override bool Equals(object obj)
        {
            if (obj is Node<T>)
            {
                Node<T> value = (Node<T>)obj;
                return this.m_value.Equals(value.m_value);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.m_value.GetHashCode();
        }
        #endregion

        #region Private Propeties
        private bool IsChildrenEmpty
        {
            get
            {
                if (this.m_children != null)
                {
                    if (this.m_children.Length < 1)
                    {
                        return true;
                    }
                    return false;
                }

                return true;
            }
        }
        #endregion

        #region Properties
        public T Value
        {
            get { return this.m_value; }
            set { this.m_value = value; }
        }

        public int Index
        {
            get
            {
                return this.m_index;
            }
        }

        public int Level
        {
            get
            {
                if (this.Parent == null)
                {
                    return 0;
                }
                return (this.Parent.Level + 1);
            }
        }

        public NodeCollection<T> Nodes
        {
            get
            {
                if (this.m_nodes == null)
                {
                    this.m_nodes = new NodeCollection<T>(this);
                }
                return this.m_nodes;
            }
        }

        public Node<T> Parent
        {
            get
            {
                return this.m_parent;
            }
        }

        public bool HasChildren
        {
            get { return (this.m_childCount > 0); }
        }

        public Node<T> FirstNode
        {
            get
            {
                if (this.m_childCount == 0)
                {
                    return null;
                }
                return this.m_children[0];
            }
        }

        private Node<T> FirstParent
        {
            get
            {
                Node<T> parent = this;
                while (parent != null)
                {
                    parent = parent.Parent;
                }
                return parent;
            }
        }

        public Node<T> LastNode
        {
            get
            {
                if (this.m_childCount == 0)
                {
                    return null;
                }
                return this.m_children[this.m_childCount - 1];
            }
        }

        public Node<T> NextNode
        {
            get
            {
                if ((this.m_index + 1) < this.m_parent.Nodes.Count)
                {
                    return this.m_parent.Nodes[this.m_index + 1];
                }
                return null;
            }
        }

        public Node<T> PrevNode
        {
            get
            {
                int index = this.m_index;
                int fixedIndex = this.m_parent.Nodes.FixedIndex;
                if (fixedIndex > 0)
                {
                    index = fixedIndex;
                }
                if ((index > 0) && (index <= this.m_parent.Nodes.Count))
                {
                    return this.m_parent.Nodes[index - 1];
                }
                return null;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.m_value == null && this.IsChildrenEmpty);
            }
        }
        #endregion

        #region Operators
        public static implicit operator T(Node<T> value)
        {
            return value.Value;
        }
        #endregion
    }
    #endregion

    #region NodeCollection<T> Class
    [Serializable]
    public class NodeCollection<T> : IList, ICollection, IEnumerable
    {
        #region Variables
        private int m_fixedIndex = -1;
        private int m_lastAccessedIndex = -1;
        private Node<T> m_owner = new Node<T>();
        #endregion

        #region Constructor
        internal NodeCollection(Node<T> owner)
        {
            this.m_owner = owner;
        }
        #endregion

        #region Private Methods
        private int AddInternal(Node<T> node, int delta)
        {
            if (node == null)
            {
                throw new ArgumentNullException("NodeCollection.AddInternal.node");
            }

            node.m_parent = this.m_owner;
            int fixedIndex = this.m_owner.Nodes.FixedIndex;
            if (fixedIndex != -1)
            {
                node.m_index = fixedIndex + delta;
            }
            else
            {
                this.m_owner.EnsureCapacity(1);
                node.m_index = this.m_owner.m_childCount;
            }

            this.m_owner.m_children[node.m_index] = node;
            this.m_owner.m_childCount++;

            return node.m_index;
        }

        private ArrayList FindInternal(T findValue, bool searchAllChildren, NodeCollection<T> collection, ArrayList foundNodes)
        {
            if ((collection == null) || (foundNodes == null))
            {
                return null;
            }
            for (int i = 0; i < collection.Count; i++)
            {
                if ((collection[i] != null) && (collection[i].Value.Equals(findValue)))
                {
                    foundNodes.Add(collection[i]);
                }
            }
            if (searchAllChildren)
            {
                for (int j = 0; j < collection.Count; j++)
                {
                    if (((collection[j] != null) && (collection[j].Nodes != null)) && (collection[j].Nodes.Count > 0))
                    {
                        foundNodes = this.FindInternal(findValue, searchAllChildren, collection[j].Nodes, foundNodes);
                    }
                }
            }
            return foundNodes;
        }

        private Node<T> FindInternal(Predicate<T> match, NodeCollection<T> collection)
        {
            if (collection == null)
            {
                return null;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (match(collection[i]))
                {
                    return collection[i];
                }

                Node<T> foundNode = this.FindInternal(match, collection[i].Nodes);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        private ArrayList FindAllInternal(Predicate<T> match, NodeCollection<T> collection, ArrayList foundNodes)
        {
            if (collection == null)
            {
                return null;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                if (match(collection[i]))
                {
                    foundNodes.Add(collection[i]);
                }

                foundNodes = this.FindAllInternal(match, collection[i].Nodes, foundNodes);
            }

            return foundNodes;
        }

        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < this.Count));
        }
        #endregion

        #region Public Methods
        public virtual Node<T> Add(T value)
        {
            Node<T> node = new Node<T>(value);
            this.Add(node);
            return node;
        }

        public virtual int Add(Node<T> node)
        {
            return this.AddInternal(node, 0);
        }

        public virtual void AddRange(Node<T>[] nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("NodeCollection.AddRange.nodes");
            }
            if (nodes.Length != 0)
            {
                this.m_owner.Nodes.FixedIndex = this.m_owner.m_childCount;
                this.m_owner.EnsureCapacity(nodes.Length);
                for (int i = nodes.Length - 1; i >= 0; i--)
                {
                    this.AddInternal(nodes[i], i);
                }
                this.m_owner.Nodes.FixedIndex = -1;
            }
        }

        public virtual void Clear()
        {
            this.m_owner.Clear();
        }

        public virtual bool Contains(Node<T> node)
        {
            return (this.IndexOf(node) != -1);
        }

        public virtual void CopyTo(Array dest, int index)
        {
            if (this.m_owner.m_childCount > 0)
            {
                Array.Copy(this.m_owner.m_children, 0, dest, index, this.m_owner.m_childCount);
            }
        }

        public virtual Node<T>[] Find(T findValue, bool searchAllChildren)
        {
            ArrayList list = this.FindInternal(findValue, searchAllChildren, this, new ArrayList());
            Node<T>[] array = new Node<T>[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public virtual Node<T>[] Find(Node<T> findNode, bool searchAllChildren)
        {
            ArrayList list = this.FindInternal(findNode.Value, searchAllChildren, this, new ArrayList());
            Node<T>[] array = new Node<T>[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public Node<T> Find(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            return this.FindInternal(match, this);
        }

        public Node<T>[] FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            ArrayList list = this.FindAllInternal(match, this, new ArrayList());
            Node<T>[] array = new Node<T>[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this.m_owner.m_children, this.m_owner.m_childCount);
        }

        public virtual int IndexOf(T value)
        {
            if (!value.Equals(default(T)))
            {
                if (this.IsValidIndex(this.m_lastAccessedIndex) && (this[this.m_lastAccessedIndex].Value.Equals(value)))
                {
                    return this.m_lastAccessedIndex;
                }
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[this.m_lastAccessedIndex].Value.Equals(value))
                    {
                        this.m_lastAccessedIndex = i;
                        return i;
                    }
                }
                this.m_lastAccessedIndex = -1;
            }
            return -1;
        }

        public virtual int IndexOf(Node<T> node)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == node)
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual Node<T> Insert(int index, T value)
        {
            Node<T> node = new Node<T>(value);
            this.Insert(index, node);
            return node;
        }

        public virtual void Insert(int index, Node<T> node)
        {
            if (index < 0)
            {
                index = 0;
            }
            if (index > this.m_owner.m_childCount)
            {
                index = this.m_owner.m_childCount;
            }
            this.m_owner.InsertNodeAt(index, node);
        }

        public virtual void Remove(T value)
        {
            int index = this.IndexOf(value);
            if (this.IsValidIndex(index))
            {
                this.RemoveAt(index);
            }
        }

        public virtual void Remove(Node<T> node)
        {
            node.Remove();
        }

        public virtual void RemoveAt(int index)
        {
            this[index].Remove();
        }

        int IList.Add(object node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("NodeCollection.Add.node");
            }
            if (node is Node<T>)
            {
                return this.Add((Node<T>)node);
            }
            return 0;
        }

        bool IList.Contains(object node)
        {
            return ((node is Node<T>) && this.Contains((Node<T>)node));
        }

        int IList.IndexOf(object node)
        {
            if (node is Node<T>)
            {
                return this.IndexOf((Node<T>)node);
            }
            return -1;
        }

        void IList.Insert(int index, object node)
        {
            if (!(node is Node<T>))
            {
                throw new ArgumentException("NodeCollection.Insert.node");
            }
            this.Insert(index, (Node<T>)node);
        }

        void IList.Remove(object node)
        {
            if (node is Node<T>)
            {
                this.Remove((Node<T>)node);
            }
        }
        #endregion

        #region Properties
        public int Count
        {
            get
            {
                return this.m_owner.m_childCount;
            }
        }

        internal int FixedIndex
        {
            get
            {
                return this.m_fixedIndex;
            }
            set
            {
                this.m_fixedIndex = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public virtual Node<T> this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.m_owner.m_childCount))
                {
                    throw new ArgumentOutOfRangeException("NodeCollection.index");
                }
                return this.m_owner.m_children[index];
            }
            set
            {
                if ((index < 0) || (index >= this.m_owner.m_childCount))
                {
                    throw new ArgumentOutOfRangeException("NodeCollection.index");
                }
                value.m_parent = this.m_owner;
                value.m_index = index;
                this.m_owner.m_children[index] = value;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (!(value is Node<T>))
                {
                    throw new ArgumentException("NodeCollection");
                }
                this[index] = (Node<T>)value;
            }
        }
        #endregion

        #region Nested Types

        #region Enumerator Class
        /// <summary>
        /// Type-specific enumeration class, used by NodeCollection.GetEnumerator.
        /// </summary>
        public class Enumerator : IEnumerator
        {
            #region Variables
            private object[] array = null;
            private int current = -1;
            private int total = 0;
            #endregion

            #region Constructor
            public Enumerator(object[] array, int count)
            {
                this.array = array;
                this.total = count;
                this.current = -1;
            }
            #endregion

            #region Public Methods
            public bool MoveNext()
            {
                if (this.current < (this.total - 1))
                {
                    this.current++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                this.current = -1;
            }
            #endregion

            #region Properties
            public object Current
            {
                get
                {
                    if (this.current == -1)
                    {
                        return null;
                    }
                    return this.array[this.current];
                }
            }
            #endregion
        }
        #endregion

        #endregion
    }
    #endregion
}
