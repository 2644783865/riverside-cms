using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RiversideCms.Mvc.Models
{
    public class JsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException("Binding context must not be null");

            if (bindingContext.ModelType != typeof(String))
                throw new Exception("Model type is not System.String");

            string json = null;
            using (var sr = new StreamReader(bindingContext.HttpContext.Request.Body))
                json = sr.ReadToEnd();

            bindingContext.Result = ModelBindingResult.Success(json);

            return Task.CompletedTask;
        }
    }
}
