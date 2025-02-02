using AutoFixture.Kernel;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.AutoFixture.Testing;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class SplitEnumerableIntoSubtypesTests
{
    class Queries
    {
        public void Enumerable(IEnumerable<string> item)
        {
        }
            
        public void List(List<string> item)
        {
        }
            
        public void IList(IList<string> item)
        {
        }
            
        public void IReadOnlyList(IReadOnlyList<string> item)
        {
        }
            
        public void Set(HashSet<string> item)
        {
        }
            
        public void ISet(ISet<string> item)
        {
        }
            
        public void IReadOnlySet(IReadOnlySet<string> item)
        {
        }
            
        public void Array(string[] item)
        {
        }
    }
        
    [Theory, DefaultAutoData]
    public void ExistsReturnsEnumerableModKeys(
        ISpecimenContext context,
        SplitEnumerableIntoSubtypes sut)
    {
        context.MockToReturn<IEnumerable<string>>();
        foreach (var method in typeof(Queries).Methods())
        {
            var param = method.GetParameters().First();
            var ret = sut.Split<string>(context, param.ParameterType);
            ret.ShouldBeAssignableTo(param.ParameterType);
        }
    }
}