using System;

namespace Noggog
{
    public struct Dimension2DDouble : IEquatable<Dimension2DDouble>
    {
        public double Width;
        public double Height;

        public bool IsZero
        {
            get { return Width.EqualsWithin(0) && Height.EqualsWithin(0); }
        }

        public bool IsSquare { get { return Width.EqualsWithin(Height); } }

        public double MaxEdge { get { return Math.Max(Width, Height); } }

        public P2Double Point
        {
            get { return new P2Double(Width, Height); }
        }

        public Dimension2DDouble(double size)
            : this(size, size)
        {

        }

        public Dimension2DDouble(double width, double length)
        {
            this.Width = width;
            this.Height = length;
        }

        public bool IsSize(double size)
        {
            return Width.EqualsWithin(size) && Height.EqualsWithin(size);
        }

        public Dimension2DDouble Max(int size)
        {
            return new Dimension2DDouble(
                Math.Max(size, this.Width),
                Math.Max(size, this.Height));
        }

        public Dimension2DDouble Min(int size)
        {
            return new Dimension2DDouble(
                Math.Min(size, this.Width),
                Math.Min(size, this.Height));
        }

        public override string ToString()
        {
            return $"Dimension2DDouble (W: {Width}, H: {Height})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Dimension2DDouble)) return false;
            return Equals((Dimension2DDouble)obj);
        }

        public bool Equals(Dimension2DDouble other)
        {
            return this.Width.EqualsWithin(other.Width)
                && this.Height.EqualsWithin(other.Height);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Width, this.Height);
        }

        public static implicit operator P2Double(Dimension2DDouble dim)
        {
            return new P2Double(dim.Width, dim.Height);
        }

        public static implicit operator Dimension2DDouble(P2Double dim)
        {
            return new Dimension2DDouble(dim.X, dim.Y);
        }

        public static Dimension2DDouble operator -(Dimension2DDouble dim, int amount)
        {
            return new Dimension2DDouble(dim.Width - amount, dim.Height - amount);
        }

        public static Dimension2DDouble operator +(Dimension2DDouble dim, int amount)
        {
            return new Dimension2DDouble(dim.Width + amount, dim.Height + amount);
        }
    }
}
