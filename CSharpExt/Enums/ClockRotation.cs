using System;

namespace Noggog
{
    public enum ClockRotation
    {
        None,
        ClockWise,
        CounterClockWise,
        OneEighty
    }

    public static class ClockRotationExt
    {
        public static ClockRotation Rotate(this ClockRotation rot, ClockRotation rhs)
        {
            switch (rhs)
            {
                case ClockRotation.None:
                    return rot;
                case ClockRotation.ClockWise:
                    switch (rot)
                    {
                        case ClockRotation.None:
                            return ClockRotation.ClockWise;
                        case ClockRotation.ClockWise:
                            return ClockRotation.OneEighty;
                        case ClockRotation.CounterClockWise:
                            return ClockRotation.None;
                        case ClockRotation.OneEighty:
                            return ClockRotation.CounterClockWise;
                        default:
                            break;
                    }
                    break;
                case ClockRotation.CounterClockWise:
                    switch (rot)
                    {
                        case ClockRotation.None:
                            return ClockRotation.CounterClockWise;
                        case ClockRotation.ClockWise:
                            return ClockRotation.None;
                        case ClockRotation.CounterClockWise:
                            return ClockRotation.OneEighty;
                        case ClockRotation.OneEighty:
                            return ClockRotation.ClockWise;
                        default:
                            break;
                    }
                    break;
                case ClockRotation.OneEighty:
                    switch (rot)
                    {
                        case ClockRotation.None:
                            return ClockRotation.OneEighty;
                        case ClockRotation.ClockWise:
                            return ClockRotation.CounterClockWise;
                        case ClockRotation.CounterClockWise:
                            return ClockRotation.ClockWise;
                        case ClockRotation.OneEighty:
                            return ClockRotation.None;
                        default:
                            break;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public static ClockRotation Undo(this ClockRotation rot)
        {
            switch (rot)
            {
                case ClockRotation.None:
                    return ClockRotation.None;
                case ClockRotation.ClockWise:
                    return ClockRotation.CounterClockWise;
                case ClockRotation.CounterClockWise:
                    return ClockRotation.ClockWise;
                case ClockRotation.OneEighty:
                    return ClockRotation.OneEighty;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}