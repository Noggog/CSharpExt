using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class ContainerAutoDataCustomization : ICustomization
{
    private readonly IContainer _container;
    
    public ContainerAutoDataCustomization(Type module)
        : this((IModule)Activator.CreateInstance(module)!)
    {
    }
    
    public ContainerAutoDataCustomization(IModule module)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule(module);
        _container = builder.Build();
    }
    
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new ContainerSpecimenBuilder(_container, throwOnUnresolved: false));
        fixture.ResidueCollectors.Add(new ContainerSpecimenBuilder(_container, throwOnUnresolved: true));
        fixture.Register<IContainer>(() => _container);
    }

    class ContainerSpecimenBuilder : ISpecimenBuilder
    {
        private readonly IContainer _container;
        private readonly bool _throwOnUnresolved;

        public ContainerSpecimenBuilder(IContainer container, bool throwOnUnresolved)
        {
            _container = container;
            _throwOnUnresolved = throwOnUnresolved;
        }
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type t)
            {
                try
                {
                    return _container.Resolve(t);
                }
                catch (Exception)
                when (!_throwOnUnresolved)
                {
                    return new NoSpecimen();
                }
            }
            
            return new NoSpecimen();
        }
    }
}