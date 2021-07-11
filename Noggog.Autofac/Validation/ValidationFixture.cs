using System;
using Autofac;

namespace Noggog.Autofac.Validation
{
    public class ValidationFixture : IDisposable
    {
        private IContainer _container;

        public ValidationFixture()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ValidationModule>();
            _container = builder.Build();
        }

        public IDisposable GetValidator(IContainer container, out IValidator validator)
        {
            var scope = _container.BeginLifetimeScope(cfg =>
            {
                cfg.RegisterInstance(container).As<IContainer>();
            });
            validator = scope.Resolve<IValidator>();
            return scope;
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}