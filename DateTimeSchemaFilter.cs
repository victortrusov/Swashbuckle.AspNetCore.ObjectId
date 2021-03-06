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
using Newtonsoft.Json;
using Domain.JsonConverters;

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
                    p.Value.Example = new OpenApiString(DateTime.Now.ToIsoDateTimeString());

                    // check if datetime for sure
                    var name = char.ToUpperInvariant(p.Key[0]) + p.Key.Substring(1);
                    var classProp = context.Type.GetProperties()
                        .FirstOrDefault(x => x.Name == name);

                    if (classProp != null)
                    {
                        var typeAttr = (DataTypeAttribute)classProp.GetCustomAttribute<DataTypeAttribute>();
                        var converterAttr = (JsonConverterAttribute)classProp.GetCustomAttribute<JsonConverterAttribute>();
                        if (
                            (typeAttr == null || typeAttr.DataType != System.ComponentModel.DataAnnotations.DataType.DateTime) &&
                            (converterAttr == null || converterAttr.ConverterType != typeof(DateWithTimeConverter))
                        )
                            p.Value.Format = "date";
                    }
                }

                if (p.Value.Format == "date")
                    p.Value.Example = new OpenApiString(DateTime.Now.ToIsoDateString());
            }
        }
    }
}
