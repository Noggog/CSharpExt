using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public enum AxisDirection
    {
        None,
        X,
        XNeg,
        Y,
        YNeg,
        Z,
        ZNeg,
        XY,
        XYNeg,
        XNegY,
        XNegYNeg,
        YZ,
        YZNeg,
        YNegZ,
        YNegZNeg,
        XZ,
        XZNeg,
        XNegZ,
        XNegZNeg
    }

    public static class AxisDirectionExt
    {
        public readonly static P3Double X = new P3Double(1, 0, 0);
        public readonly static P3Double XNeg = new P3Double(-1, 0, 0);
        public readonly static P3Double Y = new P3Double(0, 1, 0);
        public readonly static P3Double YNeg = new P3Double(0, -1, 0);
        public readonly static P3Double Z = new P3Double(0, 0, 1);
        public readonly static P3Double ZNeg = new P3Double(0, 0, -1);
        public readonly static P3Double XY = new P3Double(1, 1, 0);
        public readonly static P3Double XYNeg = new P3Double(1, -1, 0);
        public readonly static P3Double XNegY = new P3Double(-1, 1, 0);
        public readonly static P3Double XNegYNeg = new P3Double(-1, -1, 0);
        public readonly static P3Double YZ = new P3Double(0, 1, 1);
        public readonly static P3Double YZNeg = new P3Double(0, 1, -1);
        public readonly static P3Double YNegZ = new P3Double(0, -1, 1);
        public readonly static P3Double YNegZNeg = new P3Double(0, -1, -1);
        public readonly static P3Double XZ = new P3Double(1, 0, 1);
        public readonly static P3Double XZNeg = new P3Double(1, 0, -1);
        public readonly static P3Double XNegZ = new P3Double(-1, 0, 1);
        public readonly static P3Double XNegZNeg = new P3Double(-1, 0, -1);
        public readonly static P3Double None = new P3Double(0, 0, 0);

        public static AxisDirection GetAxisDirection(this GridLoc loc)
        {
            switch (loc)
            {
                case GridLoc.CENTER:
                    return AxisDirection.None;
                case GridLoc.TOP:
                    return AxisDirection.Z;
                case GridLoc.BOTTOM:
                    return AxisDirection.ZNeg;
                case GridLoc.LEFT:
                    return AxisDirection.XNeg;
                case GridLoc.RIGHT:
                    return AxisDirection.X;
                case GridLoc.TOPRIGHT:
                    return AxisDirection.XZ;
                case GridLoc.BOTTOMRIGHT:
                    return AxisDirection.XZNeg;
                case GridLoc.TOPLEFT:
                    return AxisDirection.XNegZ;
                case GridLoc.BOTTOMLEFT:
                    return AxisDirection.XNegZNeg;
                default:
                    throw new NotImplementedException();
            }
        }

        public static AxisDirection Rotate(this AxisDirection dir, Rotation rot)
        {
            switch (rot)
            {
                case Rotation.None:
                    return dir;
                case Rotation.ClockWise:
                    switch (dir)
                    {
                        case AxisDirection.None:
                            return AxisDirection.None;
                        case AxisDirection.X:
                            return AxisDirection.ZNeg;
                        case AxisDirection.XNeg:
                            return AxisDirection.Z;
                        case AxisDirection.Y:
                            return AxisDirection.Y;
                        case AxisDirection.YNeg:
                            return AxisDirection.YNeg;
                        case AxisDirection.Z:
                            return AxisDirection.X;
                        case AxisDirection.ZNeg:
                            return AxisDirection.XNeg;
                        case AxisDirection.XY:
                            return AxisDirection.YZNeg;
                        case AxisDirection.XYNeg:
                            return AxisDirection.YNegZNeg;
                        case AxisDirection.XNegY:
                            return AxisDirection.YZ;
                        case AxisDirection.XNegYNeg:
                            return AxisDirection.YNegZ;
                        case AxisDirection.YZ:
                            return AxisDirection.XY;
                        case AxisDirection.YZNeg:
                            return AxisDirection.XNegY;
                        case AxisDirection.YNegZ:
                            return AxisDirection.XYNeg;
                        case AxisDirection.YNegZNeg:
                            return AxisDirection.XNegYNeg;
                        case AxisDirection.XZ:
                            return AxisDirection.XZNeg;
                        case AxisDirection.XZNeg:
                            return AxisDirection.XNegZNeg;
                        case AxisDirection.XNegZ:
                            return AxisDirection.XZ;
                        case AxisDirection.XNegZNeg:
                            return AxisDirection.XNegZ;
                        default:
                            throw new NotImplementedException();
                    }
                case Rotation.CounterClockWise:
                    switch (dir)
                    {
                        case AxisDirection.None:
                            return AxisDirection.None;
                        case AxisDirection.X:
                            return AxisDirection.Z;
                        case AxisDirection.XNeg:
                            return AxisDirection.ZNeg;
                        case AxisDirection.Y:
                            return AxisDirection.Y;
                        case AxisDirection.YNeg:
                            return AxisDirection.YNeg;
                        case AxisDirection.Z:
                            return AxisDirection.XNeg;
                        case AxisDirection.ZNeg:
                            return AxisDirection.X;
                        case AxisDirection.XY:
                            return AxisDirection.YZ;
                        case AxisDirection.XYNeg:
                            return AxisDirection.YNegZ;
                        case AxisDirection.XNegY:
                            return AxisDirection.YZNeg;
                        case AxisDirection.XNegYNeg:
                            return AxisDirection.YNegZNeg;
                        case AxisDirection.YZ:
                            return AxisDirection.XNegY;
                        case AxisDirection.YZNeg:
                            return AxisDirection.XY;
                        case AxisDirection.YNegZ:
                            return AxisDirection.XNegYNeg;
                        case AxisDirection.YNegZNeg:
                            return AxisDirection.XYNeg;
                        case AxisDirection.XZ:
                            return AxisDirection.XNegZ;
                        case AxisDirection.XZNeg:
                            return AxisDirection.XZ;
                        case AxisDirection.XNegZ:
                            return AxisDirection.XNegZNeg;
                        case AxisDirection.XNegZNeg:
                            return AxisDirection.XZNeg;
                        default:
                            throw new NotImplementedException();
                    }
                case Rotation.OneEighty:
                    switch (dir)
                    {
                        case AxisDirection.None:
                            return AxisDirection.None;
                        case AxisDirection.X:
                            return AxisDirection.XNeg;
                        case AxisDirection.XNeg:
                            return AxisDirection.X;
                        case AxisDirection.Y:
                            return AxisDirection.Y;
                        case AxisDirection.YNeg:
                            return AxisDirection.YNeg;
                        case AxisDirection.Z:
                            return AxisDirection.ZNeg;
                        case AxisDirection.ZNeg:
                            return AxisDirection.Z;
                        case AxisDirection.XY:
                            return AxisDirection.XNegY;
                        case AxisDirection.XYNeg:
                            return AxisDirection.XNegYNeg;
                        case AxisDirection.XNegY:
                            return AxisDirection.XY;
                        case AxisDirection.XNegYNeg:
                            return AxisDirection.XYNeg;
                        case AxisDirection.YZ:
                            return AxisDirection.YZNeg;
                        case AxisDirection.YZNeg:
                            return AxisDirection.YZ;
                        case AxisDirection.YNegZ:
                            return AxisDirection.YNegZNeg;
                        case AxisDirection.YNegZNeg:
                            return AxisDirection.YNegZ;
                        case AxisDirection.XZ:
                            return AxisDirection.XNegZNeg;
                        case AxisDirection.XZNeg:
                            return AxisDirection.XNegZ;
                        case AxisDirection.XNegZ:
                            return AxisDirection.XZNeg;
                        case AxisDirection.XNegZNeg:
                            return AxisDirection.XZ;
                        default:
                            throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public static P3Double GetPoint3DDouble(this AxisDirection dir)
        {
            switch (dir)
            {
                case AxisDirection.X:
                    return X;
                case AxisDirection.XNeg:
                    return XNeg;
                case AxisDirection.Y:
                    return Y;
                case AxisDirection.YNeg:
                    return YNeg;
                case AxisDirection.Z:
                    return Z;
                case AxisDirection.ZNeg:
                    return ZNeg;
                case AxisDirection.XY:
                    return XY;
                case AxisDirection.XYNeg:
                    return XYNeg;
                case AxisDirection.XNegY:
                    return XNegY;
                case AxisDirection.XNegYNeg:
                    return XNegYNeg;
                case AxisDirection.YZ:
                    return YZ;
                case AxisDirection.YZNeg:
                    return YZNeg;
                case AxisDirection.YNegZ:
                    return YNegZ;
                case AxisDirection.YNegZNeg:
                    return YNegZNeg;
                case AxisDirection.XZ:
                    return XZ;
                case AxisDirection.XZNeg:
                    return XZNeg;
                case AxisDirection.XNegZ:
                    return XNegZ;
                case AxisDirection.XNegZNeg:
                    return XNegZNeg;
                default:
                    return None;
            }
        }
    }
}
