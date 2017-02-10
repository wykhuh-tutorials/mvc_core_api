using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp.Filters
{
    // build an attribute that will check if model is valid.
    // ActionFilterAttribute if a filter that creates  an attribute that lets you do something as 
    // action is being executed.
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        // do something before action executes
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            if(!context.ModelState.IsValid)
            {
                // BadRequestObjectResult is a class representing a bad request being returned
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
