using System.ComponentModel.DataAnnotations;
using Curus.Repository.Validation;
using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;

namespace Curus.Repository.ViewModels;

public class InstructorDTO
{
    
    public string TaxNumber { get; set; }
    public string CardNumber { get; set; }
    public string CardName { get; set; }
    
    [EnumDataType(typeof(CardProviderEnum))]
    public string CardProvider { get; set; }

    public int RegistrationAttempts { get; set; }

    [Required]
    public string Certification { get; set; }

}