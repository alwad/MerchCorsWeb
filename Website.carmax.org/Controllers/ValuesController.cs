using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Permissions;
using System.Web.Http;

namespace Website.carmax.org.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public object Get()
        {
            ClaimsPrincipal p = User as ClaimsPrincipal;

            if (p != null)
            {
                return p.Claims.Select(x => new { Type = x.Type, Value = x.Value });
            }

            return null;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
