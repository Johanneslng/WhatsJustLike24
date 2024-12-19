using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.DTOs;

namespace WhatsJustLike24.Server.Data.Mappers
{
    public class ShowDTOMapper
    {
        public ShowDTO MapToDTO(Show show)
        {
            return new ShowDTO
            {
                Id = show.Id,
                Title = show.Title,
                ShowDetails = show.ShowDetails != null ? new ShowDetailsDTO
                {
                    Id = show.ShowDetails.Id,
                    Genre = show.ShowDetails.Genre,
                    Director = show.ShowDetails.Director,
                    Description = show.ShowDetails.Description,
                    PosterPath = show.ShowDetails.PosterPath,
                    FirstAirDate = show.ShowDetails.FirstAirDate ?? Convert.ToDateTime(01-01-1970)
                } : null
            };
        }
    }
}
