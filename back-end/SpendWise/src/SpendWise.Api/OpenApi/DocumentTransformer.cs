using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SpendWise.Api.OpenApi
{
    // Document transformer: add a Bearer security scheme to components.securitySchemes
    // This uses the built-in Microsoft.OpenApi 2.x types (in the Microsoft.OpenApi namespace)
    public class DocumentTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(Microsoft.OpenApi.OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            try
            {
                if (document.Components == null)
                    document.Components = new Microsoft.OpenApi.OpenApiComponents();

                // Register Bearer security scheme
                var bearer = new Microsoft.OpenApi.OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
                    In = Microsoft.OpenApi.ParameterLocation.Header,
                    Name = "Authorization",
                };

                // Ensure SecuritySchemes dictionary exists. Use reflection to create an instance of the concrete
                // dictionary type expected by the runtime to avoid compile-time generic type mismatches.
                var compType = document.Components.GetType();
                var secProp = compType.GetProperty("SecuritySchemes");
                if (secProp != null)
                {
                    var current = secProp.GetValue(document.Components);
                    if (current == null)
                    {
                        var genericArgs = secProp.PropertyType.GetGenericArguments();
                        var dictType = typeof(System.Collections.Generic.Dictionary<,>).MakeGenericType(genericArgs);
                        var dictInstance = Activator.CreateInstance(dictType);
                        secProp.SetValue(document.Components, dictInstance);
                        current = dictInstance;
                    }

                    // Add the bearer scheme using reflection (value type expected may be an interface implemented by OpenApiSecurityScheme)
                    var add = current.GetType().GetMethod("Add", new[] { typeof(string), secProp.PropertyType.GetGenericArguments()[1] });
                    if (add != null)
                    {
                        add.Invoke(current, new object[] { "Bearer", bearer });
                    }
                }
            }
            catch
            {
                // If types are missing at compile/runtime, swallow and allow document to be generated without scheme.
            }

            return Task.CompletedTask;
        }
    }
}
