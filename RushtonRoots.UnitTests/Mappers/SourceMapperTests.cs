using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.Database;
using Xunit;

namespace RushtonRoots.UnitTests.Mappers;

/// <summary>
/// Tests for SourceMapper (Phase 4.2).
/// </summary>
public class SourceMapperTests
{
    private readonly SourceMapper _mapper;

    public SourceMapperTests()
    {
        _mapper = new SourceMapper();
    }

    [Fact]
    public void MapToViewModel_MapsAllProperties()
    {
        // Arrange
        var source = new Source
        {
            Id = 1,
            Title = "Birth Certificate",
            Author = "County Clerk",
            Publisher = "County Records Office",
            PublicationDate = new DateTime(2000, 1, 1),
            RepositoryName = "State Archives",
            RepositoryUrl = "https://archives.state.gov",
            CallNumber = "BC-2000-001",
            SourceType = "Document",
            Notes = "Original document"
        };

        // Act
        var result = _mapper.MapToViewModel(source);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Birth Certificate", result.Title);
        Assert.Equal("County Clerk", result.Author);
        Assert.Equal("County Records Office", result.Publisher);
        Assert.Equal(new DateTime(2000, 1, 1), result.PublicationDate);
        Assert.Equal("State Archives", result.RepositoryName);
        Assert.Equal("https://archives.state.gov", result.RepositoryUrl);
        Assert.Equal("BC-2000-001", result.CallNumber);
        Assert.Equal("Document", result.SourceType);
        Assert.Equal("Original document", result.Notes);
    }

    [Fact]
    public void MapToViewModel_HandlesNullableFields()
    {
        // Arrange
        var source = new Source
        {
            Id = 1,
            Title = "Birth Certificate",
            SourceType = "Document",
            Author = null,
            Publisher = null,
            PublicationDate = null,
            RepositoryName = null,
            RepositoryUrl = null,
            CallNumber = null,
            Notes = null
        };

        // Act
        var result = _mapper.MapToViewModel(source);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Birth Certificate", result.Title);
        Assert.Null(result.Author);
        Assert.Null(result.Publisher);
        Assert.Null(result.PublicationDate);
        Assert.Null(result.RepositoryName);
        Assert.Null(result.RepositoryUrl);
        Assert.Null(result.CallNumber);
        Assert.Null(result.Notes);
    }

    [Fact]
    public void MapToViewModels_MapsMultipleSources()
    {
        // Arrange
        var sources = new List<Source>
        {
            new Source { Id = 1, Title = "Source 1", SourceType = "Document" },
            new Source { Id = 2, Title = "Source 2", SourceType = "Book" },
            new Source { Id = 3, Title = "Source 3", SourceType = "Website" }
        };

        // Act
        var result = _mapper.MapToViewModels(sources);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Source 1", result[0].Title);
        Assert.Equal("Source 2", result[1].Title);
        Assert.Equal("Source 3", result[2].Title);
    }

    [Fact]
    public void MapToViewModels_HandlesEmptyList()
    {
        // Arrange
        var sources = new List<Source>();

        // Act
        var result = _mapper.MapToViewModels(sources);

        // Assert
        Assert.Empty(result);
    }
}
