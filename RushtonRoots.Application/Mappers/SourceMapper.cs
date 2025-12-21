using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper for Source entity to ViewModel.
/// </summary>
public class SourceMapper : ISourceMapper
{
    public SourceViewModel MapToViewModel(Source source)
    {
        return new SourceViewModel
        {
            Id = source.Id,
            Title = source.Title,
            Author = source.Author,
            Publisher = source.Publisher,
            PublicationDate = source.PublicationDate,
            RepositoryName = source.RepositoryName,
            RepositoryUrl = source.RepositoryUrl,
            CallNumber = source.CallNumber,
            SourceType = source.SourceType,
            Notes = source.Notes
        };
    }

    public List<SourceViewModel> MapToViewModels(List<Source> sources)
    {
        return sources.Select(MapToViewModel).ToList();
    }
}
