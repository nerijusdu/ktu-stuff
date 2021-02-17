using System;
using System.Text;

namespace Lab3
{
    abstract class Bst
    {
        public abstract void Insert(short x);
        public abstract bool Contains(short x);
        public abstract void Print();

        public void GenerateData(int quantity, int seed)
        {
            Random rand = new Random(seed);
            for (int i = 0; i < quantity; i++)
            {
                var data = (short)rand.Next(short.MinValue, short.MaxValue);
                Insert(data);
            }

        }
    }

    class Node : IComparable<Node>
    {
        public short Data { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Parent { get; set; }
        public Color Color { get; set; }
        public int Number { get; set; }

        public Node(short data)
        {
            this.Data = data;
        }

        public Node(Color color)
        {
            this.Color = color;
        }

        public Node(short data, Color color)
        {
            this.Data = data;
            this.Color = color;
        }

        public Node(short data, int number)
        {
            this.Data = data;
            this.Number = number;
        }

        public int CompareTo(Node other)
        {
            return Data.CompareTo(other.Data);
        }
    }

    enum Color
    {
        Red,
        Black
    }
}
