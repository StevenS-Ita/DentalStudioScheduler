using DentalStudioScheduler.Services;
using Microsoft.AspNetCore.Mvc;
using DentalStudioScheduler.Models;
using DentalStudioScheduler.Data.ViewModels.Filters;

namespace DentalStudioScheduler.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        /* ------------------------------ G E T ------------------------------ */

        [HttpGet()]
        public async Task<IActionResult> GetAllAppointment()
        {
            var result = await _appointmentService.GetAllAppointmentAsync();

            return Ok(result);
        }

        [HttpGet("id/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] Guid appointmentId)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

            return Ok(result);
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetAvailableSlots([FromRoute] DateTime date)
        {
            var result = await _appointmentService.GetAvailableTimeSlotsAsync(date);

            return Ok(result);
        }
        /* ------------------------------ P O S T ------------------------------ */

        [HttpPost("find")]
        public async Task<IActionResult> FindAppointments([FromBody] FilterAppointmentViewModel filter, [FromQuery] int page, [FromQuery] int size)
        {
            var result = await _appointmentService.FindAppointmentsAsync(filter, page, size);

            return Ok(result);
        }

        /* ------------------------------ P U T ------------------------------ */

        [HttpPut]
        public async Task<IActionResult> CreateOrUpdateSite([FromBody] Appointment model)
        {
            var userRef = "ControllerTest";
            await _appointmentService.CreateOrUpdateAppointmentAsync(model, userRef);

            return Ok();
        }

        /* ------------------------------ D E L E T E ------------------------------ */

        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid appointmentId)
        {
            await _appointmentService.DeleteAppointmentAsync(appointmentId);

            return Ok();
        }
    }
}
