using System;
using System.Collections.Generic;

namespace System
{
    public enum GridLoc
    {
        CENTER,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        TOPRIGHT,
        BOTTOMRIGHT,
        TOPLEFT,
        BOTTOMLEFT
    }

    public static class GridLocationExt
    {
        public static bool IsCorner(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOPRIGHT:
                case GridLoc.BOTTOMRIGHT:
                case GridLoc.TOPLEFT:
                case GridLoc.BOTTOMLEFT:
                    return true;
                default:
                    return false;
            }
        }

        private static GridLoc[] dirs = new[] { GridLoc.TOP, GridLoc.RIGHT, GridLoc.BOTTOM, GridLoc.LEFT };
        public static IEnumerable<GridLoc> Dirs { get { return dirs; } }

        public static GridLoc Opposite(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    return GridLoc.BOTTOM;
                case GridLoc.BOTTOM:
                    return GridLoc.TOP;
                case GridLoc.LEFT:
                    return GridLoc.RIGHT;
                case GridLoc.RIGHT:
                    return GridLoc.LEFT;
                case GridLoc.TOPRIGHT:
                    return GridLoc.BOTTOMLEFT;
                case GridLoc.BOTTOMRIGHT:
                    return GridLoc.TOPLEFT;
                case GridLoc.TOPLEFT:
                    return GridLoc.BOTTOMRIGHT;
                case GridLoc.BOTTOMLEFT:
                    return GridLoc.TOPRIGHT;
                default:
                    return GridLoc.CENTER;
            }
        }

        public static P2Int Point(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    return new P2Int(0, 1);
                case GridLoc.BOTTOM:
                    return new P2Int(0, -1);
                case GridLoc.LEFT:
                    return new P2Int(-1, 0);
                case GridLoc.RIGHT:
                    return new P2Int(1, 0);
                case GridLoc.TOPRIGHT:
                    return new P2Int(1, 1);
                case GridLoc.BOTTOMRIGHT:
                    return new P2Int(1, -1);
                case GridLoc.TOPLEFT:
                    return new P2Int(-1, 1);
                case GridLoc.BOTTOMLEFT:
                    return new P2Int(-1, -1);
                default:
                    return new P2Int(0, 0);
            }
        }

        public static GridLoc FromPoint(P2Int p)
        {
            P2Int normal = p.UnitDir();
            if (normal.X == 0)
            {
                if (normal.Y == 0)
                {
                    return GridLoc.CENTER;
                }
                else if (normal.Y == 1)
                {
                    return GridLoc.TOP;
                }
                return GridLoc.BOTTOM;
            }
            else if (normal.X == 1)
            {
                if (normal.Y == 0)
                {
                    return GridLoc.RIGHT;
                }
                else if (normal.Y == 1)
                {
                    return GridLoc.TOPRIGHT;
                }
                return GridLoc.BOTTOMRIGHT;
            }
            else
            {
                if (normal.Y == 0)
                {
                    return GridLoc.LEFT;
                }
                else if (normal.Y == 1)
                {
                    return GridLoc.TOPLEFT;
                }
                return GridLoc.BOTTOMLEFT;
            }
        }

        public static GridLoc OppositeRough(this GridLoc loc, System.Random rand)
        {
            switch (loc)
            {
                case GridLoc.CENTER:
                    return GridLoc.CENTER;
                case GridLoc.TOP:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.BOTTOMLEFT;
                        case 1:
                            return GridLoc.BOTTOM;
                        case 2:
                        default:
                            return GridLoc.BOTTOMRIGHT;
                    }
                case GridLoc.BOTTOM:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.TOPLEFT;
                        case 1:
                            return GridLoc.TOP;
                        case 2:
                        default:
                            return GridLoc.TOPRIGHT;
                    }
                case GridLoc.LEFT:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.TOPRIGHT;
                        case 1:
                            return GridLoc.RIGHT;
                        case 2:
                        default:
                            return GridLoc.BOTTOMRIGHT;
                    }
                case GridLoc.RIGHT:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.TOPLEFT;
                        case 1:
                            return GridLoc.LEFT;
                        case 2:
                        default:
                            return GridLoc.BOTTOMLEFT;
                    }
                case GridLoc.TOPRIGHT:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.BOTTOM;
                        case 1:
                            return GridLoc.BOTTOMLEFT;
                        case 2:
                        default:
                            return GridLoc.LEFT;
                    }
                case GridLoc.BOTTOMRIGHT:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.TOP;
                        case 1:
                            return GridLoc.TOPLEFT;
                        case 2:
                        default:
                            return GridLoc.LEFT;
                    }
                case GridLoc.TOPLEFT:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.BOTTOM;
                        case 1:
                            return GridLoc.BOTTOMRIGHT;
                        case 2:
                        default:
                            return GridLoc.RIGHT;
                    }
                case GridLoc.BOTTOMLEFT:
                    switch (rand.Next(3))
                    {
                        case 0:
                            return GridLoc.TOP;
                        case 1:
                            return GridLoc.TOPRIGHT;
                        case 2:
                        default:
                            return GridLoc.RIGHT;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public static GridLoc CounterClockwise(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    return GridLoc.TOPLEFT;
                case GridLoc.BOTTOM:
                    return GridLoc.BOTTOMRIGHT;
                case GridLoc.LEFT:
                    return GridLoc.BOTTOMLEFT;
                case GridLoc.RIGHT:
                    return GridLoc.TOPRIGHT;
                case GridLoc.TOPRIGHT:
                    return GridLoc.TOP;
                case GridLoc.BOTTOMRIGHT:
                    return GridLoc.RIGHT;
                case GridLoc.TOPLEFT:
                    return GridLoc.LEFT;
                case GridLoc.BOTTOMLEFT:
                    return GridLoc.BOTTOM;
                default:
                    return GridLoc.CENTER;
            }
        }

        public static GridLoc Clockwise(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    return GridLoc.TOPRIGHT;
                case GridLoc.BOTTOM:
                    return GridLoc.BOTTOMLEFT;
                case GridLoc.LEFT:
                    return GridLoc.TOPLEFT;
                case GridLoc.RIGHT:
                    return GridLoc.BOTTOMRIGHT;
                case GridLoc.TOPRIGHT:
                    return GridLoc.RIGHT;
                case GridLoc.BOTTOMRIGHT:
                    return GridLoc.BOTTOM;
                case GridLoc.TOPLEFT:
                    return GridLoc.TOP;
                case GridLoc.BOTTOMLEFT:
                    return GridLoc.LEFT;
                default:
                    return GridLoc.CENTER;
            }
        }

        public static GridLoc Clockwise90(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    return GridLoc.RIGHT;
                case GridLoc.BOTTOM:
                    return GridLoc.LEFT;
                case GridLoc.LEFT:
                    return GridLoc.TOP;
                case GridLoc.RIGHT:
                    return GridLoc.BOTTOM;
                case GridLoc.TOPRIGHT:
                    return GridLoc.BOTTOMRIGHT;
                case GridLoc.BOTTOMRIGHT:
                    return GridLoc.BOTTOMLEFT;
                case GridLoc.TOPLEFT:
                    return GridLoc.TOPRIGHT;
                case GridLoc.BOTTOMLEFT:
                    return GridLoc.TOPLEFT;
                default:
                    return GridLoc.CENTER;
            }
        }

        public static GridLoc CounterClockwise90(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    return GridLoc.LEFT;
                case GridLoc.BOTTOM:
                    return GridLoc.RIGHT;
                case GridLoc.LEFT:
                    return GridLoc.BOTTOM;
                case GridLoc.RIGHT:
                    return GridLoc.TOP;
                case GridLoc.TOPRIGHT:
                    return GridLoc.TOPLEFT;
                case GridLoc.BOTTOMRIGHT:
                    return GridLoc.TOPRIGHT;
                case GridLoc.TOPLEFT:
                    return GridLoc.BOTTOMLEFT;
                case GridLoc.BOTTOMLEFT:
                    return GridLoc.BOTTOMRIGHT;
                default:
                    return GridLoc.CENTER;
            }
        }

        public static GridLoc Rotate(this GridLoc loc, Rotation rot)
        {
            switch (rot)
            {
                case Rotation.None:
                    return loc;
                case Rotation.ClockWise:
                    return loc.Clockwise90();
                case Rotation.CounterClockWise:
                    return loc.CounterClockwise90();
                case Rotation.OneEighty:
                    return loc.Opposite();
                default:
                    throw new NotImplementedException();
            }
        }

        public static GridLoc Merge(this GridLoc loc, GridLoc rhs)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    switch (rhs)
                    {
                        case GridLoc.TOP:
                            return GridLoc.TOP;
                        case GridLoc.LEFT:
                            return GridLoc.TOPLEFT;
                        case GridLoc.RIGHT:
                            return GridLoc.TOPRIGHT;
                    }
                    break;
                case GridLoc.BOTTOM:
                    switch (rhs)
                    {
                        case GridLoc.BOTTOM:
                            return GridLoc.BOTTOM;
                        case GridLoc.LEFT:
                            return GridLoc.BOTTOMLEFT;
                        case GridLoc.RIGHT:
                            return GridLoc.BOTTOMRIGHT;
                    }
                    break;
                case GridLoc.LEFT:
                    switch (rhs)
                    {
                        case GridLoc.TOP:
                            return GridLoc.TOPLEFT;
                        case GridLoc.BOTTOM:
                            return GridLoc.BOTTOMLEFT;
                        case GridLoc.LEFT:
                            return GridLoc.LEFT;
                    }
                    break;
                case GridLoc.RIGHT:
                    switch (rhs)
                    {
                        case GridLoc.TOP:
                            return GridLoc.TOPRIGHT;
                        case GridLoc.BOTTOM:
                            return GridLoc.BOTTOMRIGHT;
                        case GridLoc.RIGHT:
                            return GridLoc.CENTER;
                    }
                    break;
                case GridLoc.TOPRIGHT:
                    switch (rhs)
                    {
                        case GridLoc.TOPRIGHT:
                            return GridLoc.TOPRIGHT;
                        case GridLoc.BOTTOMRIGHT:
                            return GridLoc.RIGHT;
                        case GridLoc.TOPLEFT:
                            return GridLoc.TOP;
                    }
                    break;
                case GridLoc.BOTTOMRIGHT:
                    switch (rhs)
                    {
                        case GridLoc.TOPRIGHT:
                            return GridLoc.RIGHT;
                        case GridLoc.BOTTOMRIGHT:
                            return GridLoc.BOTTOMRIGHT;
                        case GridLoc.BOTTOMLEFT:
                            return GridLoc.BOTTOM;
                    }
                    break;
                case GridLoc.TOPLEFT:
                    switch (rhs)
                    {
                        case GridLoc.TOPRIGHT:
                            return GridLoc.TOP;
                        case GridLoc.TOPLEFT:
                            return GridLoc.TOPLEFT;
                        case GridLoc.BOTTOMLEFT:
                            return GridLoc.LEFT;
                    }
                    break;
                case GridLoc.BOTTOMLEFT:
                    switch (rhs)
                    {
                        case GridLoc.BOTTOMRIGHT:
                            return GridLoc.BOTTOM;
                        case GridLoc.TOPLEFT:
                            return GridLoc.LEFT;
                        case GridLoc.BOTTOMLEFT:
                            return GridLoc.BOTTOMLEFT;
                    }
                    break;
            }
            return GridLoc.CENTER;
        }

        public static void Modify(this GridLoc loc, ref int x, ref int y)
        {
            switch (loc)
            {
                case GridLoc.TOP:
                    y++;
                    break;
                case GridLoc.BOTTOM:
                    y--;
                    break;
                case GridLoc.LEFT:
                    x--;
                    break;
                case GridLoc.RIGHT:
                    x++;
                    break;
                case GridLoc.TOPRIGHT:
                    x++;
                    y++;
                    break;
                case GridLoc.BOTTOMRIGHT:
                    x++;
                    y--;
                    break;
                case GridLoc.TOPLEFT:
                    x--;
                    y++;
                    break;
                case GridLoc.BOTTOMLEFT:
                    x--;
                    y--;
                    break;
                case GridLoc.CENTER:
                default:
                    break;
            }
        }

        public static void Modify(this GridLoc loc, int x, int y, out int xOut, out int yOut)
        {
            loc.Modify(ref x, ref y);
            xOut = x;
            yOut = y;
        }

        public static GridLoc GetGridLocation(this double angle)
        {
            angle = angle % 360f;
            if (angle < 202.5f)
            {
                if (angle < 112.5f)
                {
                    if (angle < 67.5)
                    {
                        if (angle < 22.5)
                        {
                            return GridLoc.TOP;
                        }
                        return GridLoc.TOPRIGHT;
                    }
                    return GridLoc.RIGHT;
                }
                else if (angle < 157.5f)
                {
                    return GridLoc.BOTTOMRIGHT;
                }
                return GridLoc.BOTTOM;
            }
            else
            {
                if (angle < 292.5f)
                {
                    if (angle < 247.5)
                    {
                        return GridLoc.BOTTOMLEFT;
                    }
                    return GridLoc.LEFT;
                }
                else if (angle < 337.5)
                {
                    return GridLoc.TOPLEFT;
                }
                return GridLoc.TOP;
            }
        }

        public static GridLoc GetLocationToward(int x, int y, int xTowards, int yTowards)
        {
            x = xTowards - x;
            y = yTowards - y;
            throw new NotImplementedException();
        }
    }
}