using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;

namespace Middleware.Swagger
{
    public class SwaggerSetup
    {
        public static Action<SwaggerGenOptions> Config(bool isProduction) => (options) =>
            {
                // files with documentation to add description to properties
                var swaggerFiles = new string[] { "SwaggerAPI.xml", "SwaggerApplicationAPI.xml" }
                    .Select(fileName => Path.Combine(System.AppContext.BaseDirectory, fileName))
                    .Where(filePath => File.Exists(filePath));
                foreach (var filePath in swaggerFiles)
                    options.IncludeXmlComments(filePath);

                // for ObjectId
                options.OperationFilter<ObjectIdOperationFilter>(swaggerFiles);
                options.SchemaFilter<ObjectIdSchemaFilter>();
                
                // other useful stuff
                options.OperationFilter<DateTimeOperationFilter>();
                options.SchemaFilter<DateTimeSchemaFilter>();

                //should be the last ones
                options.OperationFilter<LowercaseOperationFilter>();
                options.DocumentFilter<LowercaseDocumentFilter>();
            };
    }
}
