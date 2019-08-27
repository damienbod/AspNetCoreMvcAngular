using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreMvcAngular.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    public class LogoutController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("OpenIdConnect");

            return Ok();
            //return new SignOutResult(new[] { "Cookies", "OpenIdConnect" });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Add([FromBody] Thing thing)
        //{
        //    if (thing == null)
        //    {
        //        return BadRequest();
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Thing newThing = _thingsRepository.Add(thing);

        //    return CreatedAtRoute("GetSingleThing", new { id = newThing.Id }, newThing);
        //}
    }
}
