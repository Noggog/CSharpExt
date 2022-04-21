using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class ObservableEmptyBehavior : ISpecimenBuilderTransformation
{
    public ISpecimenBuilderNode Transform(ISpecimenBuilder builder)
    {
        return new Postprocessor(
            builder,
            new ObservableSpecimenCommand());
    }
}