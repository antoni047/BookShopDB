namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                 .Select(a => new ExportAuthorDto
                 {
                     AuthorName = a.FirstName + " " + a.LastName,
                     Books = a.AuthorsBooks
                     .OrderByDescending(ab => ab.Book.Price)
                     .Select(ab => new ExportBooksDto
                     {
                         BookName = ab.Book.Name,
                         BookPrice = ab.Book.Price.ToString("0.00")
                     })
                     .ToArray()
                 })
                 .ToArray()
                 .OrderByDescending(a => a.Books.Count())
                 .ThenByDescending(a => a.AuthorName)
                 .ToArray();

            string jsonExport = JsonConvert.SerializeObject(authors, Formatting.Indented);

            return jsonExport;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .Where(b => b.PublishedOn.Date < date && b.Genre.ToString() == "Science")
                .Select(b => new ExportTopbooks
                {
                    Pages = b.Pages.ToString(),
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture)
                })
                .ToArray()
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.Date)
                .Take(10)
                .ToArray();


            var rootAttribute = new XmlRootAttribute("Books");
            var xmlSerializer = new XmlSerializer(typeof(ExportTopbooks[]), rootAttribute);

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), books, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}