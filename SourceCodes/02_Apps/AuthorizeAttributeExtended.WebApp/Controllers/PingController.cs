using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aliencube.AuthorizeAttribute.Extended.WebApp.Controllers
{
    [Web.Http.Extended.Authorize]
    [RoutePrefix("api")]
    public class PingController : ApiController
    {
        [Web.Http.Extended.Authorize(Roles = "Admin")]
        [Route("ping/{name}")]
        public virtual async Task<HttpResponseMessage> Get(string name)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { message = string.Format("Hello, {0}", name) });
        }
    }
}