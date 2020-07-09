using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Middleware.Swagger
{
    public class DateTimeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema swaggerSchema, SchemaFilterContext context)
        {
            foreach (var p in swaggerSchema.Properties)
            {
                if (p.Value.Format == "date-time")
                {
                    var name = char.ToUpperInvariant(p.Key[0]) + p.Key.Substring(1);
                    // check if datetime for sure
                    var classProp = context.Type.GetProperties()
                        .FirstOrDefault(x => x.Name == name);
                    var typeAttr = classProp != null
                        ? (DataTypeAttribute)classProp.GetCustomAttribute<DataTypeAttribute>()
                        : null;
                    if (typeAttr != null && typeAttr.DataType == System.ComponentModel.DataAnnotations.DataType.DateTime)
                        p.Value.Example = new OpenApiString(DateTime.Now.ToIsoDateTimeString());
                    else
                        p.Value.Format = "date";
                }
                if (p.Value.Format == "date")
                    p.Value.Example = new OpenApiString(DateTime.Now.ToIsoDateString());
            }
        }
    }
}
