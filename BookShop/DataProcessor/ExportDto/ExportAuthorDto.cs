using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ExportDto
{
    public class ExportAuthorDto
    {
        public string AuthorName { get; set; }

        public ExportBooksDto[] Books { get; set; }
    }
}
