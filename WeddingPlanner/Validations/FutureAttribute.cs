using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeddingPlanner.Valiidations
{
    public class FutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime) value < DateTime.Now){
                return new ValidationResult("Wedding must be in the future");
            }
            return ValidationResult.Success;
        }
    }
}
