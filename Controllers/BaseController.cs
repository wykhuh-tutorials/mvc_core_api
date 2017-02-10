using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeCamp.Controllers
{
    public  abstract class BaseController : Controller
    {
        public const string URLHELPER = "URLHELPER";

        // OnActionExecuting comes from MVC; do something as action is executing
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // make URLHELPER available to all controller actions
            context.HttpContext.Items[URLHELPER] = this.Url;
        }

    }
}
