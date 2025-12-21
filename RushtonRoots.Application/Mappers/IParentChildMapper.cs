using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper interface for ParentChild entity.
/// </summary>
public interface IParentChildMapper
{
    /// <summary>
    /// Maps a ParentChild entity to a ParentChildViewModel.
    /// </summary>
    ParentChildViewModel MapToViewModel(ParentChild parentChild);

    /// <summary>
    /// Maps a CreateParentChildRequest to a ParentChild entity.
    /// </summary>
    ParentChild MapToEntity(CreateParentChildRequest request);

    /// <summary>
    /// Updates an existing ParentChild entity with values from UpdateParentChildRequest.
    /// </summary>
    void UpdateEntity(ParentChild entity, UpdateParentChildRequest request);
}
