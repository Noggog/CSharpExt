using System.Collections;

namespace Noggog.Testing.TestClassData;

public class ErrorResponseSuccessFailData : IEnumerable<object[]>
{
    public static IEnumerable<object[]> Data { get; } = new[]
    {
        new object[] {ErrorResponse.Success},
        new object[] {ErrorResponse.Failure},
    };

    public IEnumerator<object[]> GetEnumerator() => Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
}