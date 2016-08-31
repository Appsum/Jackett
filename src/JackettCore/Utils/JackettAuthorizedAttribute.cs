using System.Net;

namespace JackettCore.Utils
{
    public class JackettAuthorizedAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Skip authorisation on blank passwords
            if (string.IsNullOrEmpty(Engine.Server.Config.AdminPassword))
            {
                return;
            }

            if (!Engine.SecurityService.CheckAuthorised(actionContext.Request))
            {
                if(actionContext.ControllerContext.ControllerDescriptor.ControllerType.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AllowAnonymousAttribute)).Any())
                {
                    return;
                }

                if (actionContext.ControllerContext.ControllerDescriptor.ControllerType.GetMethod(actionContext.ActionDescriptor.ActionName).GetCustomAttributes(true).Where(a => a.GetType() == typeof(AllowAnonymousAttribute)).Any())
                {
                    return;
                }


                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode
                                                                                  .Unauthorized);
            }
        }
    }
}
