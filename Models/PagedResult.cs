/*
    PagedResult<T> Generic Model Class
    
    Purpose: Provides a generic container for paginated data results with navigation metadata
    and helper methods for implementing pagination in the EventEase application.
    
    Properties:
    - Items: The current page of data items (List<T>)
    - TotalCount: Total number of items across all pages
    - PageNumber: Current page number (1-based)
    - PageSize: Number of items per page
    - TotalPages: Calculated total number of pages (read-only)
    - HasNextPage: Boolean indicating if next page exists (read-only)
    - HasPreviousPage: Boolean indicating if previous page exists (read-only)
    
    Factory Methods:
    - CreateAsync<TKey>: Creates paginated result with optional ordering and cancellation support
    - CreatePagesAsync: Simplified version with string-based ordering
    - Both methods support async operations for consistency with data access patterns
    
    Pagination Logic:
    - Uses Skip/Take LINQ methods for efficient data slicing
    - Supports optional ordering via Func<T, TKey> delegates
    - Calculates navigation properties automatically
    - Handles edge cases (empty data, single page, etc.)
    
    Performance Features:
    - Deferred execution with LINQ for memory efficiency
    - Optional cancellation token support for long-running operations
    - Generic design allows reuse across different entity types
    
    Usage: Used throughout the application for paginating lists of events, registrations,
    and other collections in UI components like EventsPage and data grids.
*/

namespace BlazorFinalProject.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public static async Task<PagedResult<T>> CreateAsync<TKey>(
        IEnumerable<T> source,
        int pageNumber,
        int pageSize,
        Func<T, TKey>? orderBy = null)
    {
        await Task.CompletedTask;

        var totalCount = source.Count();

        var orderedSource = orderBy != null ? source.OrderBy(orderBy) : source;

        var items = orderedSource
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static async Task<PagedResult<T>> CreatePagesAsync(
        IEnumerable<T> source,
        int pageNumber,
        int pageSize,
        Func<T, string>? orderBy = null)
    {
        return await CreateAsync(source, pageNumber, pageSize, orderBy);
    }

    public static async Task<PagedResult<T>> CreateAsync<TKey>(
        IEnumerable<T> source,
        int pageNumber,
        int pageSize,
        Func<T, TKey>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();

        var totalCount = source.Count();

        cancellationToken.ThrowIfCancellationRequested();

        var orderedSource = orderBy != null ? source.OrderBy(orderBy) : source;

        cancellationToken.ThrowIfCancellationRequested();

        var items = orderedSource
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static async Task<PagedResult<T>> CreatePagesAsync(
        IEnumerable<T> source,
        int pageNumber,
        int pageSize,
        Func<T, string>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(source, pageNumber, pageSize, orderBy, cancellationToken);
    }

}