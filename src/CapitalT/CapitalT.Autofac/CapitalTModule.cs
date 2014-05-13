using Autofac;
using Autofac.Core;
using CapitalT.Culture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace CapitalT.Autofac
{
    public class CapitalTModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserCulture>()
                .InstancePerLifetimeScope()
                .PreserveExistingDefaults();

            builder.Register((context) => Capital.Config.LocalizedStringService)
                .As<CapitalT.Translate.ILocalizedStringService>()
                .PreserveExistingDefaults();

            builder.Register((context) => Capital.Config.TranslationProvider)
                .As<CapitalT.Translate.ITranslationProvider>()
                .PreserveExistingDefaults();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var hasLocalizerInConstructor = registration.Activator.LimitType
                .GetConstructors()
                .Any(c => c.GetParameters().Any(p => p.ParameterType == typeof(Localizer)));

            if (hasLocalizerInConstructor)
            {
                var scope = registration.Activator.LimitType.FullName;
                registration.Preparing += (sender, args) =>
                {
                    args.Parameters = args.Parameters.Union(new Parameter[] { 
                        new ResolvedParameter(
                            (param, context) => param.ParameterType == typeof(Localizer), 
                            (param, context) => Capital.TFor(scope, context.Resolve<UserCulture>())
                        )
                    });
                };
            }
            else
            {
                var capitalTProperty = FindCapitalTProperty(registration.Activator.LimitType);
                if (capitalTProperty != null)
                {
                    var scope = registration.Activator.LimitType.FullName;

                    registration.Activated += (sender, e) =>
                    {
                        var localizer = Capital.TFor(scope, e.Context.Resolve<UserCulture>());
                        capitalTProperty.SetValue(e.Instance, localizer, null);
                    };
                }
            }
        }

        private static PropertyInfo FindCapitalTProperty(Type type)
        {
            return type.GetProperty("T", typeof(Localizer));
        }
    }
}
