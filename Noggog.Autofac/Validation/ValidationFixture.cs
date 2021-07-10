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

        public IDisposable GetValidator(IContainer container, out IValidate validate)
        {
            var scope = _container.BeginLifetimeScope(cfg =>
            {
                cfg.RegisterInstance(container).As<IContainer>();
            });
            validate = scope.Resolve<IValidate>();
            return scope;
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}