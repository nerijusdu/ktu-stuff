using System;
using System.Text;

namespace Lab3
{
    class MyBst : Bst
    {
        protected Node root;
        protected int count;

        public MyBst()
        {
            count = 0;
        }

        public override void Insert(short x)
        {
            var newNode = new Node(x, count);
            root = InsertNode(root, newNode);
            root.Color = Color.Black;
            count += 1;
        }

        private Node InsertNode(Node node, Node newNode)
        {
            if (node == null) return newNode;

            var cmp = newNode.CompareTo(node);
            if (cmp < 0)
            {
                node.Left = InsertNode(node.Left, newNode);
            }
            else if (cmp > 0)
            {
                node.Right = InsertNode(node.Right, newNode);
            }

            return Balance(node);
        }

        private Node Balance(Node node)
        {
            if (IsRed(node.Right))
            {
                node = RotateLeft(node);
            }

            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                node = RotateRight(node);
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                ColorFlip(node);
                ColorFlip(node.Left);
                ColorFlip(node.Right);
            }

            return node;
        }

        private void ColorFlip(Node node)
        {
            node.Color = IsRed(node) ? Color.Black : Color.Red;
        }

        private Node RotateRight(Node node)
        {
            var child = node.Left;
            node.Left = child.Right;
            child.Right = node;

            child.Color = node.Color;
            node.Color = Color.Red;

            return child;
        }

        private Node RotateLeft(Node node)
        {
            var child = node.Right;
            node.Right = child.Left;
            child.Left = node;

            child.Color = node.Color;
            node.Color = Color.Red;

            return child;
        }

        public override bool Contains(short x)
        {
            var temp = root;
            var item = new Node(x);

            while (temp != null)
            {
                var comp = temp.CompareTo(item);
                if (comp == 0) return true;

                temp = comp > 0 ? temp.Left : temp.Right;
            }

            return false;
        }

        public override void Print()
        {
            Print(root);
            Console.WriteLine();
        }

        private void Print(Node X)
        {
            if (X == null) return;

            Print(X.Left);
            Console.Write("{0} ", X.Data);
            Print(X.Right);
        }

        private bool IsRed(Node node) => node != null && node.Color == Color.Red;
    }
}
