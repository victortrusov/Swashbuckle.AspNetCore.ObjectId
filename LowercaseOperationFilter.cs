using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Middleware.Swagger
{
    public class LowercaseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var p in operation.Parameters.ToList())
                if (p.Name != null && p.Name.Length > 1)
                    p.Name = char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1);
        }
    }
}
