﻿using FluentValidation.AspNetCore;
using System.Reflection;
using Web.Utilities;

namespace Web
{
    public static class Dependencies
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<ISerializationUtility, SerializationUtility>();
            services.AddSingleton<IFileUtility, FileUtility>();
        }

        public static void AddFluentValidation(this WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidation(options =>
            {
                // Validate child properties and root collection elements
                options.ImplicitlyValidateChildProperties = true;
                options.ImplicitlyValidateRootCollectionElements = true;

                // Automatic registration of validators in assembly
                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });
        }
    }
}
