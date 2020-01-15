namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using DataProcessor.ImportDto;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var rootAttribute = new XmlRootAttribute("Books");
            var xmlSerializer = new XmlSerializer(typeof(ImportBooksDto[]), rootAttribute);

            var booksDtos = (ImportBooksDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validBooks = new List<Book>();

            StringBuilder sb = new StringBuilder();

            foreach (var bookDto in booksDtos)
            {
                if (!IsValid(bookDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var book = new Book
                {
                    Name = bookDto.Name,
                    Genre = Enum.Parse<Genre>(bookDto.Genre),
                    Price = bookDto.Price,
                    Pages = bookDto.Pages,
                    PublishedOn = DateTime.ParseExact(bookDto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture),
                };

                sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));

                validBooks.Add(book);
            }

            context.AddRange(validBooks);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authorDtos = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            var validAuthors = new List<Author>();
            var sb = new StringBuilder();

            foreach (var authorDto in authorDtos)
            {
                var emails = context.Authors.Select(a => a.Email).ToArray();
                var emailExists = emails.Any(e => e == authorDto.Email);

                if (!IsValid(authorDto) || emailExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var author = new Author
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Phone = authorDto.Phone,
                    Email = authorDto.Email
                };

                var validAuthorBooks = new List<AuthorBook>();

                foreach (var book in authorDto.Books)
                {
                    var bookExists = context.Books.Find(book);

                    if (bookExists == null)
                    {
                        continue;
                    }

                    if (!IsValid(book))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var newBook = new AuthorBook
                    {
                        BookId = book.Id,
                        AuthorId = author.Id

                    };

                    validAuthorBooks.Add(newBook);
                }

                if (!validAuthorBooks.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                author.AuthorsBooks = validAuthorBooks;
                validAuthors.Add(author);

                sb.AppendLine(string
                    .Format(SuccessfullyImportedAuthor, author.FirstName + " " + author.LastName, author.AuthorsBooks.Count));
            }

            context.AddRange(validAuthors);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}