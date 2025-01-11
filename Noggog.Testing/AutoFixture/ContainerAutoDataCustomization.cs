using Autofac;
using Autofac.Core;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class ContainerAutoDataCustomization : ICustomization
{
    private readonly IContainer _container;
    
    public ContainerAutoDataCustomization(Type module)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule((IModule)Activator.CreateInstance(module)!);
        _container = builder.Build();
    }
    
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new ContainerSpecimenBuilder(_container));
        fixture.Register<IContainer>(() => _container);
    }

    class ContainerSpecimenBuilder : ISpecimenBuilder
    {
        private readonly IContainer _container;
        private readonly HashSet<Type> _blacklist = new();

        public ContainerSpecimenBuilder(IContainer container)
        {
            _container = container;
        }
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type t && !_blacklist.Contains(t))
            {
                try
                {
                    return _container.Resolve(t);
                }
                catch (Exception e)
                {
                    _blacklist.Add(t);
                    return new NoSpecimen();
                }
            }
            
            return new NoSpecimen();
        }
    }
}