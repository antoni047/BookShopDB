using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class ImportBooksDto
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string Genre { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(50, 5000)]
        public int Pages { get; set; }

        [Required]
        public string PublishedOn { get; set; }
    }
}
