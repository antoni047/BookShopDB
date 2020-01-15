using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorDto
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3), MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression("^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Email { get; set; }

        public ImportBookIdsDto[] Books { get; set; }
    }
}
