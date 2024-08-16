using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Curus.Service.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
