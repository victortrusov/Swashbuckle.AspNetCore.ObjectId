using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.XPath;

namespace Middleware.Swagger
{
    public class ObjectIdOperationFilter : IOperationFilter
    {
        private readonly IEnumerable<string> objectIdIgnoreParameters = new[]
        {
            "Timestamp",
            "Machine",
            "Pid",
            "Increment",
            "CreationTime"
        };
        private readonly IEnumerable<XPathNavigator> xmlNavigators;
        public ObjectIdOperationFilter(IEnumerable<string> filePaths)
        {
            xmlNavigators = filePaths.Select(x => new XPathDocument(x).CreateNavigator());
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var p in operation.Parameters.ToList())
                if (objectIdIgnoreParameters.Any(x => p.Name.EndsWith(x)))
                {
                    var parameterIndex = operation.Parameters.IndexOf(p);
                    operation.Parameters.Remove(p);
                    var dotIndex = p.Name.LastIndexOf(".");
                    if (dotIndex > -1)
                    {
                        var idName = p.Name.Substring(0, dotIndex);
                        if (!operation.Parameters.Any(x => x.Name == idName))
                        {
                            operation.Parameters.Insert(parameterIndex, new OpenApiParameter()
                            {
                                Name = idName,
                                Schema = new OpenApiSchema()
                                {
                                    Type = "string",
                                    Format = "24-digit hex string"
                                },
                                Description = GetFieldDescription(idName, context),
                                Example = new OpenApiString(ObjectId.Empty.ToString()),
                                In = p.In,
                            });
                        }
                    }
                }
        }

        private string GetFieldDescription(string idName, OperationFilterContext context)
        {
            var description = $"Идентификатор {idName.Substring(0, idName.Length - 2)} в системе Pooling";

            var classProp = context.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType?.GetProperties().FirstOrDefault(x => x.Name == idName);
            var typeAttr = classProp != null
                ? (DescriptionAttribute)classProp.GetCustomAttribute<DescriptionAttribute>()
                : null;
            if (typeAttr != null)
                description = typeAttr?.Description;

            if (classProp != null)
                foreach (var xmlNavigator in xmlNavigators)
                {
                    var propertyMemberName = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(classProp);
                    var propertySummaryNode = xmlNavigator.SelectSingleNode($"/doc/members/member[@name='{propertyMemberName}']/summary");
                    if (propertySummaryNode != null)
                        description = XmlCommentsTextHelper.Humanize(propertySummaryNode.InnerXml);
                }

            return description;
        }
    }
}
