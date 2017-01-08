using System;

namespace System
{
    public struct Dimension2D : IP2IntGet, IEquatable<Dimension2D>
    {
        public readonly static Dimension2D One = new Dimension2D(1, 1);

        public int Width;
        public int Height;

        public int X
        {
            get { return Width; }
        }

        public int Y
        {
            get { return Height; }
        }

        public int Area
        {
            get { return Width * Height; }
        }

        public bool IsZero
        {
            get { return Width == 0 && Height == 0; }
        }

        public bool IsSquare { get { return Width == Height; } }

        public int MaxEdge { get { return Math.Max(Width, Height); } }

        public P2Int Point
        {
            get { return new P2Int(Width, Height); }
        }

        public Dimension2D(int size)
            : this(size, size)
        {

        }

        public Dimension2D(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Dimension2D(IP2IntGet point)
        {
            this.Width = point.X;
            this.Height = point.Y;
        }

        public bool IsSize(int size)
        {
            return Width == size && Height == size;
        }

        public Dimension2D Max(int size)
        {
            return new Dimension2D(
                Math.Max(size, this.Width),
                Math.Max(size, this.Height));
        }

        public Dimension2D Min(int size)
        {
            return new Dimension2D(
                Math.Min(size, this.Width),
                Math.Min(size, this.Height));
        }

        public Dimension2D Expand(int size)
        {
            return new Dimension2D(this.Width + size, this.Height + size);
        }

        public Dimension2D Flip()
        {
            return new Dimension2D(this.Height, this.Width);
        }

        public bool EqualsEvenFlipped(Dimension2D rhs)
        {
            if (rhs.Width == this.Width
                && rhs.Height == this.Height) return true;
            if (rhs.Height == this.Width
                && rhs.Width == this.Height) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Dimension2D)) return false;
            return Equals((Dimension2D)obj);
        }

        public bool Equals(Dimension2D other)
        {
            return this.Width == other.Width
                && this.Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Width, this.Height);
        }

        public override string ToString()
        {
            return $"Dimension2D(W: {Width}, H: {Height})";
        }

        public static implicit operator P2Int(Dimension2D dim)
        {
            return new P2Int(dim.Width, dim.Height);
        }

        public static implicit operator Dimension2D(P2Int dim)
        {
            return new Dimension2D(dim.X, dim.Y);
        }

        public static Dimension2D operator -(Dimension2D dim, int amount)
        {
            return new Dimension2D(dim.Width - amount, dim.Height - amount);
        }

        public static Dimension2D operator +(Dimension2D dim, int amount)
        {
            return new Dimension2D(dim.Width + amount, dim.Height + amount);
        }
    }
}
