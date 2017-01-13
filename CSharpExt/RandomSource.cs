using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog
{
    public class RandomSource
    {
        static System.Random randSource = new Random();
        int numQueries;
        public int NumQueries { get { return numQueries; } }
        System.Random rand;
        public int OriginalSeed { get; protected set; }

        public RandomSource()
        {
            this.OriginalSeed = randSource.Next();
            rand = new System.Random(OriginalSeed);
        }

        public RandomSource(int seed)
        {
            this.OriginalSeed = seed;
            rand = new System.Random(seed);
        }

        public int Next()
        {
            numQueries++;
            return rand.Next();
        }

        public int Next(int min, int max)
        {
            numQueries++;
            return rand.Next(min, max);
        }

        public int Next(int max)
        {
            numQueries++;
            return rand.Next(max);
        }

        public double NextDouble()
        {
            numQueries++;
            return rand.NextDouble();
        }

        public bool NextPercent(double percent)
        {
            numQueries++;
            return percent > rand.NextDouble();
        }

        public bool NextPercent(int percent)
        {
            numQueries++;
            return percent >= rand.Next(1, 100);
        }

        public Percent NextPercent()
        {
            numQueries++;
            return new Percent(rand.NextDouble());
        }

        public bool NextBool()
        {
            numQueries++;
            return rand.Next(2) == 1;
        }

        public float NextAngle()
        {
            numQueries++;
            return (float)(rand.NextDouble() * 360);
        }

        public int NextNegative()
        {
            return NextBool() ? -1 : 1;
        }

        public double NextDouble(double magn)
        {
            numQueries++;
            return rand.NextDouble() * magn * 2 - magn;
        }

        public float NextRotationDegree()
        {
            numQueries++;
            return (float)(rand.NextDouble() * 360);
        }
        
        private double? spareRoll;
        private double NextMargsalia(bool useSpare)
        {
            double magn;
            if (useSpare && spareRoll.HasValue)
            {
                magn = spareRoll.Value;
                spareRoll = null;
            }
            else
            {
                double roll1 = this.NextDouble() * 2 - 1;
                double roll2 = this.NextDouble() * 2 - 1;
                double s = Math.Pow(roll1, 2) + Math.Pow(roll2, 2);
                if (s >= 1)
                { // Retry
                    return NextNormalDist(useSpare);
                }
                double commonTerm = Math.Sqrt(-2 * Math.Log(s) / s);
                magn = roll1 * commonTerm;
                if (!spareRoll.HasValue)
                {
                    spareRoll = roll2 * commonTerm;
                }
            }
            return magn;
        }

        public double NextNormalDist(double mean, double stdDev, bool useSpare)
        {
            return NextMargsalia(useSpare) * stdDev + mean;
        }

        public double NextNormalDist(bool useSpare = true)
        {
            return NextMargsalia(useSpare);
        }

        public double NextNormalDist(double min, double max, double wingCutoff = 2, bool useSpare = true)
        {
            if (max < min)
            {
                throw new ArgumentException("Max must be greater than or equal to min");
            }
            if (max == min)
            {
                return max;
            }
            double magn = NextNormalDist(useSpare);
            while (Math.Abs(magn) > wingCutoff)
            {
                magn = NextNormalDist(true);
            }
            magn = (magn + wingCutoff) / 2 / wingCutoff;
            if (magn.EqualsWithin(1d))
            {
                return max;
            }
            return magn * (max - min) + min;
        }

        public int NextNormalDist(int min, int max, double wingCutoff = 2, bool useSpare = true)
        {
            return (int)NextNormalDist(min, (double)max, wingCutoff, useSpare);
        }

        public ushort NextNormalDist(ushort min, ushort max, double wingCutoff = 2, bool useSpare = true)
        {
            return (ushort)NextNormalDist((int)min, (int)max, wingCutoff, useSpare);
        }

        public int NextNormalDist(RangeInt range, double wingCutoff = 2, bool useSpare = true)
        {
            return NextNormalDist(range.Min, range.Max, wingCutoff, useSpare);
        }

        public override string ToString()
        {
            return "RandomSource (Original Seed: " + OriginalSeed + ", Num Queries: " + numQueries + ")";
        }
    }
}
