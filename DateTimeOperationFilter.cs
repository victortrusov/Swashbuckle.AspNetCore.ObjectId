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
                        var classProp = context.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType?
                            .GetProperties().FirstOrDefault(x => x.Name == p.Name);
                        var typeAttr = classProp != null
                        ? (DataTypeAttribute)classProp.GetCustomAttribute<DataTypeAttribute>()
                        : null;
                        if (typeAttr != null && typeAttr.DataType == System.ComponentModel.DataAnnotations.DataType.DateTime)
                            p.Example = new OpenApiString(DateTime.Now.ToIsoDateTimeString());
                        else
                            p.Schema.Format = "date";
                    }

                    if (p.Schema.Format == "date")
                        p.Example = new OpenApiString(DateTime.Now.ToIsoDateString());
                }
        }
    }
}
