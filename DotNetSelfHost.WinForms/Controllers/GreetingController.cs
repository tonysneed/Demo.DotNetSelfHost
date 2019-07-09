using Microsoft.AspNetCore.Mvc;

namespace DotNetSelfHost.WinForms.Controllers
{
    [Route("/hello")]
    public class GreetingController
    {
        [HttpGet]
        public IActionResult Get()
        {
            var name = Program.Form.NameText;
            var greeting = "Hello " + name;
            return new JsonResult(greeting);
        }

        [HttpPost]
        public IActionResult Post([FromBody] string name)
        {
            Program.Form.NameText = name;
            return new NoContentResult();
        }
    }
}
