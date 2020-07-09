using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Middleware.Swagger
{
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Lowercase all routes, for Swagger, as discussed here:
            //   https://github.com/domaindrivendev/Swashbuckle/issues/834
            // Issue has a reference to the original gist, which can be found here:
            //   https://gist.github.com/smaglio81/e57a8bdf0541933d7004665a85a7b198

            var originalPaths = swaggerDoc.Paths;

            //	generate the new keys
            var newPaths = new Dictionary<string, OpenApiPathItem>();
            var removeKeys = new List<string>();
            foreach (var path in originalPaths)
            {
                var newKey = LowercaseEverythingButParameters(path.Key);
                if (newKey != path.Key)
                {
                    removeKeys.Add(path.Key);
                    newPaths.Add(newKey, path.Value);
                }
            }

            //	add the new keys
            foreach (var path in newPaths)
                swaggerDoc.Paths.Add(path.Key, path.Value);

            //	remove the old keys
            foreach (var key in removeKeys)
                swaggerDoc.Paths.Remove(key);
        }

        private static string LowercaseEverythingButParameters(string key)
        {
            return string.Join('/', key.Split('/')
                .Select(x => x.Contains("{") || x.Length < 2
                    ? x
                    : char.ToLowerInvariant(x[0]) + x.Substring(1)));
        }
    }
}
