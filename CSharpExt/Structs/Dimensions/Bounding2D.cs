using System;
using System.Collections.Generic;

namespace Noggog
{
    public struct Bounding2D : IEquatable<Bounding2D>
    {
        public static readonly Bounding2D Invalid = new Bounding2D(Int32.MaxValue, Int32.MinValue, Int32.MaxValue, Int32.MinValue);
        private static int max = Int32.MaxValue / 2;
        private static int min = Int32.MinValue / 2;
        public int Left;
        public int Right;
        public int Bottom;
        public int Top;

        public int Width
        {
            get { return Right - Left + 1; }
        }
        public int Height
        {
            get { return Top - Bottom + 1; }
        }
        public int Area
        {
            get { return Width * Height; }
        }
        public Dimension2D Dimensions { get { return new Dimension2D(Width, Height); } }
        public P2Int BottomLeft { get { return new P2Int(Left, Bottom); } }
        public P2Int BottomRight { get { return new P2Int(Right, Bottom); } }
        public P2Int TopRight { get { return new P2Int(Right, Top); } }
        public P2Int TopLeft { get { return new P2Int(Left, Top); } }
        public bool IsSingleUnit { get { return Width == 1 && Height == 1; } }

        #region Ctors
        public Bounding2D(int xl = Int32.MaxValue, int xr = Int32.MinValue, int yb = Int32.MaxValue, int yt = Int32.MinValue)
        {
            Left = xl;
            Right = xr;
            Bottom = yb;
            Top = yt;
        }

        public Bounding2D(P2Int leftdownOrigin, int width, int height)
            : this(leftdownOrigin.X, leftdownOrigin.X + width - 1,
            leftdownOrigin.Y, leftdownOrigin.Y + height - 1)
        {

        }

        public Bounding2D(P2Int center, int radius)
            : this(
                center.X - radius,
                center.X + radius,
                center.Y - radius,
                center.Y + radius)
        {

        }

        public Bounding2D(P2Int leftDownOrigin, P2Int topRight)
            : this(
                leftDownOrigin.X,
                topRight.X,
                leftDownOrigin.Y,
                topRight.Y)
        {
        }

        public Bounding2D(IP2IntGet p)
            : this(
                p.X,
                p.X,
                p.Y,
                p.Y)
        {

        }
        #endregion Ctors

        public bool IsValid()
        { // Don't know what I was doing here, exactly...
            return Left > min
                && Left < max;
        }

        public P2Int Center
        {
            get
            {
                if (IsValid())
                {
                    return new P2Int(Left + Width / 2, Bottom + Height / 2);
                }
                else
                {
                    return new P2Int();
                }
            }
        }

        public P2Double CenterDouble
        {
            get
            {
                if (IsValid())
                {
                    return new P2Double((Right + Left) * .5, (Bottom + Top) * .5);
                }
                else
                {
                    return new P2Double();
                }
            }
        }

        public Bounding2D CenterBounding()
        {
            if (this.Width % 2 == 0)
            {
                return new Bounding2D(Center.Shift(-1, -1), Center);
            }
            else
            {
                return new Bounding2D(Center, Center);
            }
        }

        public P2Double GetRealCenter()
        {
            if (IsValid())
            {
                return new P2Double(Left + (Width - 1) / 2F, Bottom + (Height - 1) / 2F);
            }
            else
            {
                return new P2Double(0, 0);
            }
        }

        #region Absorbs
        public Bounding2D Absorb(int x, int y)
        {
            return new Bounding2D(
                Math.Min(Left, x),
                Math.Max(Right, x),
                Math.Min(Bottom, y),
                Math.Max(Top, y));
        }

        public Bounding2D Absorb(P2Int val)
        {
            return Absorb(val.X, val.Y);
        }

        public Bounding2D Absorb(Bounding2D rhs)
        {
            return new Bounding2D(
                Math.Min(Left, rhs.Left),
                Math.Max(Right, rhs.Right),
                Math.Min(Bottom, rhs.Bottom),
                Math.Max(Top, rhs.Top));
        }

        public Bounding2D AbsorbX(int x)
        {
            return new Bounding2D(
                Math.Min(Left, x),
                Math.Max(Right, x),
                Bottom,
                Top);
        }

        public Bounding2D AbsorbY(int y)
        {
            return new Bounding2D(
                Left,
                Right,
                Math.Min(Bottom, y),
                Math.Max(Top, y));
        }
        #endregion Absorbs

        #region Intersects
        public void IntersectingDimensions(Bounding2D rhs, out int width, out int height)
        {
            if (IsValid() && rhs.IsValid())
            {
                IntersectingWidth(rhs, out width);
                IntersectingHeight(rhs, out height);
            }
            else
            {
                width = 0;
                height = 0;
            }
        }

        // Returns the min number, and whether thisNum was the min.
        public bool GetMinDim(int thisNum, int rhsNum, out int result)
        {
            int thisAbs = Math.Abs(thisNum) + 1;
            int rhsAbs = Math.Abs(rhsNum) + 1;
            if (thisAbs < rhsAbs)
            {
                result = thisAbs;
                if (Math.Sign(thisNum) < 0)
                {
                    result = -result;
                }
                return true;
            }
            result = rhsAbs;
            if (Math.Sign(rhsNum) < 0)
            {
                result = -result;
            }
            return false;
        }

        // True if completely contained in RHS horizontally
        public bool InsideHoriz(Bounding2D rhs)
        {
            return Left > rhs.Left && Right < rhs.Right;
        }

        // True if completely contained in RHS vertically
        public bool InsideVert(Bounding2D rhs)
        {
            return Bottom > rhs.Bottom && Top < rhs.Top;
        }

        // Gets the minimum intersection outHeight, and leftmost point
        public int IntersectingWidth(Bounding2D rhs, out int outWidth)
        {
            if (InsideHoriz(rhs))
            { // If completely inside RHS
                outWidth = Width;
                return Left;
            }
            if (rhs.InsideHoriz(this))
            { // If completely containing RHS
                outWidth = rhs.Width;
                return rhs.Left;
            }
            return GetMinDim(rhs.Right - Left, Right - rhs.Left, out outWidth) ? Left : rhs.Left;
        }

        // Gets the minimum intersection outHeight, and downmost point
        public int IntersectingHeight(Bounding2D rhs, out int outHeight)
        {
            if (InsideVert(rhs))
            { // If completely inside RHS
                outHeight = Height;
                return Bottom;
            }
            if (rhs.InsideVert(this))
            { // If completely containing RHS
                outHeight = rhs.Height;
                return rhs.Bottom;
            }
            return GetMinDim(rhs.Top - Bottom, Top - rhs.Bottom, out outHeight) ? Bottom : rhs.Bottom;
        }

        public int IntersectArea(Bounding2D rhs)
        {
            int width;
            int height;
            IntersectingDimensions(rhs, out width, out height);

            // If either x or y intersect is negative, there's no intersection
            if (width > 0 && height > 0)
            {
                return width * height;
            }
            return 0;
        }

        public bool Intersects(Bounding2D rhs)
        {
            return IntersectArea(rhs) > 0;
        }

        public bool Intersects(IEnumerable<Bounding2D> boundings)
        {
            foreach (var bounding in boundings)
            {
                if (IntersectArea(bounding) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Returns bounding box of area, but positioning is on the origin
        public Bounding2D IntersectBoundRelative(Bounding2D rhs)
        {
            int width;
            int height;
            IntersectingDimensions(rhs, out width, out height);
            return new Bounding2D(Int32.MinValue, width, Int32.MinValue, height);
        }

        // Returns bounding box of intersecting area
        public Bounding2D IntersectBounds(Bounding2D rhs)
        {
            int width, height;
            int leftmost = IntersectingWidth(rhs, out width);
            int downmost = IntersectingHeight(rhs, out height);
            return new Bounding2D(new P2Int(leftmost, downmost), width, height);
        }

        // Gets center point of intersecting bounds
        public P2Int GetIntersectCenter(Bounding2D rhs)
        {
            return IntersectBounds(rhs).Center;
        }
        #endregion Intersects

        public Bounding2D Expand(P2Int amount)
        {
            if (IsValid())
            {
                return new Bounding2D(Left - amount.X, Right + amount.X, Bottom - amount.Y, Top + amount.Y);
            }
            return Bounding2D.Invalid;
        }

        public Bounding2D Expand(int amount)
        {
            return Expand(new P2Int(amount, amount));
        }

        public P2Int GetShiftNonNeg(int buffer)
        {
            int x = 0;
            int y = 0;
            if (IsValid())
            {
                if (Left < buffer)
                {
                    x = buffer - Left;
                }
                if (Bottom < buffer)
                {
                    y = buffer - Bottom;
                }
            }
            return new P2Int(x, y);
        }

        public P2Int GetShiftNonNeg()
        {
            return GetShiftNonNeg(0);
        }

        public Bounding2D Shift(int x, int y)
        {
            if (IsValid())
            {
                return new Bounding2D(Left + x, Right + x, Bottom + y, Top + y);
            }
            return new Bounding2D();
        }

        public Bounding2D Shift(P2Int shift)
        {
            return Shift(shift.X, shift.Y);
        }

        public Bounding2D ShiftNonNeg()
        {
            return Shift(GetShiftNonNeg());
        }

        public Bounding2D ShiftToOrigin()
        {
            return new Bounding2D(
                0,
                this.Right - this.Left,
                0,
                this.Top - this.Bottom);
        }

        public bool Contains(int x, int y)
        {
            return Contains(new P2Int(x, y));
        }

        public bool Contains(P2Int p)
        {
            return p.X >= Left && p.X <= Right && p.Y >= Bottom && p.Y <= Top;
        }

        public bool ContainsFully(Bounding2D rhs)
        {
            return Left < rhs.Left && Right > rhs.Right && Bottom < rhs.Bottom && Top > rhs.Top;
        }

        public Bounding2D InBounds<T>(T[,] arr)
        {
            return IntersectBounds(arr.GetBounds());
        }

        public int GetMin(bool horiz)
        {
            return horiz ? Left : Bottom;
        }

        public int GetMax(bool horiz)
        {
            return horiz ? Right : Top;
        }

        public void DistanceTo(P2Int p, out double closestDist, out double farthestDist)
        {
            if (p.X < Left)
            { // On left
                if (p.Y < Bottom)
                { // Bottom left
                    closestDist = p.Distance(Left, Bottom);
                    farthestDist = p.Distance(Right, Top);
                }
                else if (p.Y > Bottom)
                { // Top left
                    closestDist = p.Distance(Left, Top);
                    farthestDist = p.Distance(Right, Bottom);
                }
                else
                { // Left
                    closestDist = Left - p.X;
                    farthestDist = Right - p.X;
                }
            }
            else if (p.X > Right)
            { // On right
                if (p.Y < Bottom)
                { // Bottom right
                    closestDist = p.Distance(Right, Bottom);
                    farthestDist = p.Distance(Left, Top);
                }
                else if (p.Y > Bottom)
                { // Top right
                    closestDist = p.Distance(Right, Top);
                    farthestDist = p.Distance(Left, Bottom);
                }
                else
                { // right
                    closestDist = p.X - Right;
                    farthestDist = p.X - Left;
                }
            }
            else
            { // Inside horizontally
                if (p.Y < Bottom)
                { // Bottom
                    closestDist = Bottom - p.Y;
                    farthestDist = Top - p.Y;
                }
                else if (p.Y > Bottom)
                { // Top
                    closestDist = p.Y - Top;
                    farthestDist = p.Y - Bottom;
                }
                else
                { // Inside Vertically
                    closestDist = -1;
                    farthestDist = -1;
                }
            }
        }

        public void Foreach(Action<P2Int> action)
        {
            for (int x = this.Left; x <= this.Right; x++)
            {
                for (int y = this.Bottom; y <= this.Top; y++)
                {
                    action(new P2Int(x, y));
                }
            }
        }

        public override string ToString()
        {
            return $"Bounding2D (L: {Left} B: {Bottom} R: {Right} T: {Top})";
        }
        
        public bool Equals(Bounding2D rhs)
        {
            return this.Left == rhs.Left
                && this.Right == rhs.Right
                && this.Top == rhs.Top
                && this.Bottom == rhs.Bottom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Bounding2D)) return false;
            return Equals((Bounding2D)obj);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Left, this.Right, this.Bottom, this.Top);
        }

        public GridLoc GetLocationRelativeTo(Bounding2D bounding)
        {
            if (this.IsCompletelyLeftOf(bounding))
            {
                return GridLoc.LEFT;
            }
            if (this.IsCompletelyRightOf(bounding))
            {
                return GridLoc.RIGHT;
            }
            if (this.IsCompletelyAbove(bounding))
            {
                return GridLoc.TOP;
            }
            if (this.IsCompletelyBelow(bounding))
            {
                return GridLoc.BOTTOM;
            }

            return GridLoc.CENTER;
        }

        private bool IsCompletelyLeftOf(Bounding2D bounding)
        {
            return Right < bounding.Left;
        }

        private bool IsCompletelyRightOf(Bounding2D bounding)
        {
            return Left > bounding.Right;
        }

        private bool IsCompletelyAbove(Bounding2D bounding)
        {
            return Bottom > bounding.Top;
        }

        private bool IsCompletelyBelow(Bounding2D bounding)
        {
            return Top < bounding.Bottom;
        }

        public bool IsSameShapeAs(Dimension2D dimension)
        {
            var p = new P2Int(BottomLeft.X + dimension.X - 1,
                BottomLeft.Y + dimension.Y - 1);

            if (p == TopRight)
            {
                return true;
            }
            return false;
        }
    }

    public class Bounding2DObj
    {
        public Bounding2D Bounding = Bounding2D.Invalid;

        public void Absorb(P2Int p)
        {
            Absorb(p.X, p.Y);
        }

        public void Absorb(int x, int y)
        {
            Bounding = Bounding.Absorb(x, y);
        }

        public void Absorb(Bounding2D bounds)
        {
            Bounding = Bounding.Absorb(bounds);
        }
    }
}