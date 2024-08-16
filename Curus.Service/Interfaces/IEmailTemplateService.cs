using Curus.Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Service.Interfaces
{
    public interface IEmailTemplateService
    {
        IEnumerable<EmailTemplateDto> GetTemplates();
        EmailTemplateDto GetTemplateById(int id);
        void UpdateTemplate(int id, EmailTemplateDto dto);
        bool TemplateExists(int id);
    }
}
