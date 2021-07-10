using System;
using System.Collections.Generic;
using FakeItEasy;

namespace Noggog.Testing.FakeItEasy
{
    public static class ArgumentConstraintManagerExt
    {
        public static ISet<T> SetEquals<T>(
            this IArgumentConstraintManager<ISet<T>> manager,
            IEnumerable<T> rhs)
        {
            return manager.Matches(x => x.SetEquals(rhs));
        }
        
        public static HashSet<T> SetEquals<T>(
            this IArgumentConstraintManager<HashSet<T>> manager, 
            IEnumerable<T> rhs)
        {
            return manager.Matches(x => x.SetEquals(rhs));
        }
        
        public static ISet<T> SetEquals<T>(
            this IArgumentConstraintManager<ISet<T>> manager,
            params T[] rhs)
        {
            return manager.Matches(x => x.SetEquals(rhs));
        }
        
        public static HashSet<T> SetEquals<T>(
            this IArgumentConstraintManager<HashSet<T>> manager,
            params T[] rhs)
        {
            return manager.Matches(x => x.SetEquals(rhs));
        }
    }
}