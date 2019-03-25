using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace BeltExam.Models
{
    public class LoginUser
    {
        public string Email {get; set;}

        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}