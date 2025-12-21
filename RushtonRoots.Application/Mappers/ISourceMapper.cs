using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper interface for Source entity.
/// </summary>
public interface ISourceMapper
{
    SourceViewModel MapToViewModel(Source source);
    List<SourceViewModel> MapToViewModels(List<Source> sources);
}
