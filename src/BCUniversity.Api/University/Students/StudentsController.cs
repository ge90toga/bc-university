using System.Threading.Tasks;
using BCUniversity.Service;
using BCUniversity.Service.Dtos.Requests;
using BCUniversity.Service.Students;
using Microsoft.AspNetCore.Mvc;

namespace BCUniversity.Api.University.Students
{
    [Route("/api/students")]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _studentService.GetStudents();

            return Ok(result);
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            var result = await _studentService.GetStudent(id);

            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]StudentRequestDto requestDto)
        {
            var result = await _studentService.CreateStudent(requestDto);
            return Ok(new { Id = result });
        }
        
        [HttpPost]
        [Route("{id}/enrolments")]
        public async Task<IActionResult> Enrol([FromRoute]string id, [FromBody]EnrolmentRequestDto requestDto)
        {
            await _studentService.EnrolStudentToSubject(id, requestDto);
            return Ok();
        }
        
        [HttpGet]
        [Route("{id}/enrolments")]
        public async Task<IActionResult> Enrol([FromRoute]string id)
        {
            var result = await _studentService.GetEnrolments(id);
            return Ok(result);
        }
    }
}