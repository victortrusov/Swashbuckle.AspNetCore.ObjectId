using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;
using Domain.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;
using Domain.JsonConverters;

namespace Middleware.Swagger
{
    public class DateTimeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var p in operation.Parameters.ToList())
                if (p.Name != null && p.Name.Length > 1)
                {
                    if (p.Schema.Format == "date-time")
                    {
                        p.Example = new OpenApiString(DateTime.Now.ToIsoDateTimeString());

                        // check if datetime for sure
                        var name = char.ToUpperInvariant(p.Name[0]) + p.Name.Substring(1);
                        var classProp = context.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType?.GetProperties().FirstOrDefault(x => x.Name == name);

                        if (classProp != null)
                        {
                            var typeAttr = (DataTypeAttribute)classProp.GetCustomAttribute<DataTypeAttribute>();
                            var converterAttr = (JsonConverterAttribute)classProp.GetCustomAttribute<JsonConverterAttribute>();
                            if (
                                (typeAttr == null || typeAttr.DataType != System.ComponentModel.DataAnnotations.DataType.DateTime) &&
                                (converterAttr == null || converterAttr.ConverterType != typeof(DateWithTimeConverter))
                            )
                                p.Schema.Format = "date";
                        }
                    }

                    if (p.Schema.Format == "date")
                        p.Example = new OpenApiString(DateTime.Now.ToIsoDateString());
                }
        }
    }
}
