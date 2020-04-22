using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deliver.Web
{
    public class ConsumesControllerConvention : IControllerModelConvention
    {
        private static readonly HashSet<string> _httpMethodsWithoutContent = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            HttpMethod.Delete.Method,
            HttpMethod.Get.Method,
            HttpMethod.Head.Method,
            HttpMethod.Options.Method,
            HttpMethod.Trace.Method,
        };

        public void Apply(ControllerModel controller)
        {
            var consumes = controller.Filters.OfType<ConsumesAttribute>().FirstOrDefault();
            if (consumes != null)
            {
                // Remove [Consumes] from the controller
                controller.Filters.Remove(consumes);
                foreach (var selector in controller.Selectors)
                {
                    selector.ActionConstraints.Remove(consumes);
                }

                // Search through controller actions, and place [Consumes] on any action without content
                foreach (var action in controller.Actions)
                {
                    if (action.Filters.OfType<ConsumesAttribute>().Any())
                    {
                        // Action already has a [Consumes]
                        break;
                    }

                    var httpMethodAttributes = action.Attributes.OfType<HttpMethodAttribute>().ToList();
                    if (!httpMethodAttributes.Any(httpMethodAttribute => httpMethodAttribute.HttpMethods.Any(method => _httpMethodsWithoutContent.Contains(method))))
                    {
                        // Action does not support content, so re-add [Consumes] constraint at the action level
                        action.Filters.Add(consumes);
                    }
                }
            }
        }
    }
}
