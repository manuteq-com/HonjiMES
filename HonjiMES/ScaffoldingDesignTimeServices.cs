using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES
{
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddHandlebarsScaffolding(options =>
            {
                // Add custom template data
                options.TemplateData = new Dictionary<string, object>
    {
        { "models-namespace", "ScaffoldingSample.Models" },
        { "base-class", "EntityBase" }
    };
            });

        }
    }
}
