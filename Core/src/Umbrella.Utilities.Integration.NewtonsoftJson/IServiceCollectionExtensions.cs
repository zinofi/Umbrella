using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Integration.NewtonsoftJson;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaUtilitiesNewtonsoftJson(this IServiceCollection services)
        {
            UmbrellaJsonIntegration.Initialize();

            return services;
        }
    }
}