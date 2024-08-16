using Curus.Repository.ViewModels.Response;
using Curus.Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Service.Interfaces
{
    public interface IAdminService
    {
        Task<LoginUserResponse> LoginAsync(AdminLoginDTO adminLoginDTO);
    }
}
