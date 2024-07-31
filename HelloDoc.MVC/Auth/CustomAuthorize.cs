
using HelloDoc.BAL.Interface;
using HelloDoc.DAL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace HelloDoc.MVC.Auth
{
    //[AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorize : Attribute,IAuthorizationFilter
    {

        //private readonly string _role;

        //public CustomAuthorize(string role = "")
        //{
        //    _role = role;
        //}
        //private readonly string _menu;
        private readonly string[] _role;

        public CustomAuthorize(params string[] role)
        {
            //_menu = menu;
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtservice = context.HttpContext.RequestServices.GetService<IJwtServiceRepo>();

            if (jwtservice == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "PatientSide", Action = "Login" }));
                return;
            }

            var session=context.HttpContext.Session;
            string token = session.GetString("token");

            if(token == null || !jwtservice.ValidateToken(token,out JwtSecurityToken jwtSecurityToken)) 
            {
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new { error = "Failed to Authenticate User" })
                    {
                        StatusCode = 401
                    };
                    return;
                }
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "PatientSide", Action = "Login" }));
                return;
            }

            var roleclaims = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (roleclaims == null) 
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "PatientSide", Action = "Login" }));
                return;
            }
             
            //for one value
            //if (string.IsNullOrWhiteSpace(_role) || roleclaims.Value != _role)
            //{
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "PatientSide", Action = "AccessDenied" }));
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(roleclaims.Value) || (_role.Any() && !_role.Contains(roleclaims.Value)))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "PatientSide", Action = "AccessDenied" }));
                return;
            }
          
        }
    }
}
