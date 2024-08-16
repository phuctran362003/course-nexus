﻿using Curus.Repository.ViewModels.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels
{
    public class InstructorRegistrationDTO : UserRegistrationDTO
    {
        public string TaxNumber { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public CardProviderEnum CardProvider { get; set; }
        public IFormFile Certification { get; set; }
    }
}
