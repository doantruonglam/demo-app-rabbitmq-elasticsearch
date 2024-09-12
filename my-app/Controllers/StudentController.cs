using Microsoft.AspNetCore.Mvc;
using my_app.Models;
using my_app.Services;

namespace my_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAll(Paging pagesize)
        {

            var result = await _studentService.GetAllAsync(pagesize);

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetElasticAll(Paging pagesize)
        {

            var result = await _studentService.GetElasticAllAsync(pagesize);

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] StudentCreate student)
        {

            var result = await _studentService.CreateAsync(student);

            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update([FromBody] StudentUpdate student)
        {

            var result = await _studentService.UpdateAsync(student);

            return Ok(result);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(int studentID)
        {

            var result = await _studentService.DeleteAsync(studentID);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Get([FromQuery]int studentID)
        {

            var result = await _studentService.GetAsync(studentID);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ElasticGet([FromQuery] int studentID)
        {

            var result = await _studentService.ElasticGetAsync(studentID);

            return Ok(result);
        }
    }
}