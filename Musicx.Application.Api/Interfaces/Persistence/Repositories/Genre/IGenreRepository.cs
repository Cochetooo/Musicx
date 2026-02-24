using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;

public interface IGenreRepository : IRepository<InGenre, OutGenre>;