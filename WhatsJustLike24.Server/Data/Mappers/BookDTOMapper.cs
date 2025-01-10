using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;

namespace WhatsJustLike24.Server.Data.Mappers
{
    public class BookDTOMapper
    {
        public BookDBDTO MapToDTO(Book book)
        {
            return new BookDBDTO
            {
                Title = book.Title,
                Description = book.BookDetails.Description,
                Cover = book.BookDetails.Cover,
                Author = book.BookDetails.Author,
                FirstRelease = book.BookDetails.FirstRelease.HasValue ? book.BookDetails.FirstRelease.Value.Year : (int?)null,
                Languages = book.BookDetails.Languages,
                Genre = book.BookDetails.Genre,
                Series = book.BookDetails.Series,
                Publisher = book.BookDetails.Publisher,
                Isbn = book.BookDetails.Isbn,
                Pages = book.BookDetails.Pages
            };
        }
    }
}
