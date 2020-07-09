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
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.OperationFilter<AuthenticationRequirementsOperationFilter>();

                //This action return true/false after selected SwaggerDoc section for add/decline Request
                options.DocInclusionPredicate((docName, apiDescription) => docName == apiDescription.GroupName);
            
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
