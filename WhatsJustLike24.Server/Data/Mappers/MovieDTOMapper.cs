using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.DTOs;

namespace WhatsJustLike24.Server.Data.Mappers
{
    public class MovieDTOMapper
    {
        public MovieDTO MapToDTO(Movie movie)
        {
            return new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                MovieDetails = movie.MovieDetails != null ? new MovieDetailsDTO
                {
                    Id = movie.MovieDetails.Id,
                    Genre = movie.MovieDetails.Genre,
                    Director = movie.MovieDetails.Director,
                    Description = movie.MovieDetails.Description,
                    PosterPath = movie.MovieDetails.PosterPath
                } : null
            };
        }
    }
}
