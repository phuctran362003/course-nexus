using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly Dictionary<int, EmailTemplateDto> _templates;

    public EmailTemplateService()
    {
        // Initialize predefined templates
        _templates = new Dictionary<int, EmailTemplateDto>
        {
            { 1, new EmailTemplateDto { Id = 1, Subject = "Email Verification", Body = EmailTemplates.EmailVerification } },
            { 2, new EmailTemplateDto { Id = 2, Subject = "Email Verification OTP", Body = EmailTemplates.OtpEmail } },
            { 3, new EmailTemplateDto { Id = 3, Subject = "Account Pending Approval", Body = EmailTemplates.PendingEmail } },
            { 4, new EmailTemplateDto { Id = 4, Subject = "Account Approval", Body = EmailTemplates.ApprovalEmail } },
            { 5, new EmailTemplateDto { Id = 5, Subject = "Instructor Rejection", Body = EmailTemplates.RejectionEmail } },
            { 6, new EmailTemplateDto { Id = 6, Subject = "Account Reactivation", Body = EmailTemplates.ActiveEmail } },
            { 7, new EmailTemplateDto { Id = 7, Subject = "Account Deactivation Notice", Body = EmailTemplates.DeactiveEmail } },
            { 8, new EmailTemplateDto { Id = 8, Subject = "Course Submission Approval Required", Body = EmailTemplates.SubmitCourseEmail } }
        };
    }

    public IEnumerable<EmailTemplateDto> GetTemplates()
    {
        return _templates.Values;
    }

    public EmailTemplateDto GetTemplateById(int id)
    {
        return _templates.ContainsKey(id) ? _templates[id] : null;
    }

    public void UpdateTemplate(int id, EmailTemplateDto dto)
    {
        if (_templates.ContainsKey(id))
        {
            var existingTemplate = _templates[id];
            existingTemplate.Subject = dto.Subject;
            existingTemplate.Body = dto.Body;
            _templates[id] = existingTemplate;
        }
    }


    public bool TemplateExists(int id)
    {
        return _templates.ContainsKey(id);
    }
}
