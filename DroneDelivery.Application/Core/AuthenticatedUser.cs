using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DroneDelivery.Application.Core
{
    public class AuthenticatedCliente
    {
        private readonly IHttpContextAccessor _accessor;

        public AuthenticatedCliente(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
             
        public Guid ID =>  Guid.Parse( GetClaimsIdentity().FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Jti)?.Value);

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }
    }
}
