using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Curus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateService _service;

        public EmailTemplateController(IEmailTemplateService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EmailTemplateDto>> GetTemplates()
        {
            try
            {
                var templates = _service.GetTemplates();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching templates", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<EmailTemplateDto> GetTemplateById(int id)
        {
            try
            {
                var template = _service.GetTemplateById(id);
                if (template == null)
                {
                    return NotFound(new { message = "Template not found" });
                }
                return Ok(template);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the template", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTemplate(int id, [FromBody] EmailTemplateDto dto)
        {
            try
            {
                if (!_service.TemplateExists(id))
                {
                    return NotFound(new { message = "Template not found" });
                }

                _service.UpdateTemplate(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the template", details = ex.Message });
            }
        }
    }
}
