using System;

namespace System
{
    public enum Rotation
    {
        None,
        ClockWise,
        CounterClockWise,
        OneEighty
    }

    public static class RotationExt
    {
        public static Rotation Rotate(this Rotation rot, Rotation rhs)
        {
            switch (rhs)
            {
                case Rotation.None:
                    return rot;
                case Rotation.ClockWise:
                    switch (rot)
                    {
                        case Rotation.None:
                            return Rotation.ClockWise;
                        case Rotation.ClockWise:
                            return Rotation.OneEighty;
                        case Rotation.CounterClockWise:
                            return Rotation.None;
                        case Rotation.OneEighty:
                            return Rotation.CounterClockWise;
                        default:
                            break;
                    }
                    break;
                case Rotation.CounterClockWise:
                    switch (rot)
                    {
                        case Rotation.None:
                            return Rotation.CounterClockWise;
                        case Rotation.ClockWise:
                            return Rotation.None;
                        case Rotation.CounterClockWise:
                            return Rotation.OneEighty;
                        case Rotation.OneEighty:
                            return Rotation.ClockWise;
                        default:
                            break;
                    }
                    break;
                case Rotation.OneEighty:
                    switch (rot)
                    {
                        case Rotation.None:
                            return Rotation.OneEighty;
                        case Rotation.ClockWise:
                            return Rotation.CounterClockWise;
                        case Rotation.CounterClockWise:
                            return Rotation.ClockWise;
                        case Rotation.OneEighty:
                            return Rotation.None;
                        default:
                            break;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public static Rotation Undo(this Rotation rot)
        {
            switch (rot)
            {
                case Rotation.None:
                    return Rotation.None;
                case Rotation.ClockWise:
                    return Rotation.CounterClockWise;
                case Rotation.CounterClockWise:
                    return Rotation.ClockWise;
                case Rotation.OneEighty:
                    return Rotation.OneEighty;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}