using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;

namespace WhatsJustLike24.Server.Data.Mappers
{
    public class GameDTOMapper
    {
        public GameDBDTO MapToDTO(Game game)
        {
            return new GameDBDTO
            {
                Title = game.Title,
                Description = game.GameDetails.Description,
                Cover = game.GameDetails.Cover,
                Developer = game.GameDetails.Developer,
                FirstRelease = game.GameDetails.FirstRelease,
                Platforms = game.GameDetails.Platforms,
                Genre = game.GameDetails.Genre
            };
        }
    }
}
