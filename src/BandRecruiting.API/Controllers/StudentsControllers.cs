using Microsoft.AspNetCore.Mvc;

namespace BandRecruiting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents()
            => Ok(Array.Empty<object>());   // placeholder
    }
}
