using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// Controller for Person management.
/// </summary>
[Authorize]
public class PersonController : Controller
{
    private readonly IPersonService _personService;
    private readonly IHouseholdService _householdService;
    private readonly ILogger<PersonController> _logger;

    public PersonController(IPersonService personService, IHouseholdService householdService, ILogger<PersonController> logger)
    {
        _personService = personService;
        _householdService = householdService;
        _logger = logger;
    }

    // GET: Person
    public async Task<IActionResult> Index(SearchPersonRequest? request)
    {
        IEnumerable<RushtonRoots.Domain.UI.Models.PersonViewModel> people;
        
        if (request != null && (!string.IsNullOrEmpty(request.SearchTerm) || request.HouseholdId.HasValue || request.IsDeceased.HasValue))
        {
            people = await _personService.SearchAsync(request);
        }
        else
        {
            people = await _personService.GetAllAsync();
        }

        ViewBag.Households = await _householdService.GetAllAsync();
        ViewBag.SearchRequest = request ?? new SearchPersonRequest();
        return View(people);
    }

    // GET: Person/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null)
        {
            return NotFound();
        }
        return View(person);
    }

    // GET: Person/Create
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Person/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [Obsolete("This MVC POST endpoint is deprecated. Use POST /api/person instead. This endpoint will be removed in a future version (planned: 3 sprints). Migration guide: https://github.com/GLGRushton/RushtonRoots/blob/main/docs/ApiEndpointsImplementationPlan.md#phase-43-deprecate-old-mvc-post-patterns")]
    public async Task<IActionResult> Create(CreatePersonRequest request)
    {
        _logger.LogWarning("DEPRECATED: POST /Person/Create was called. This endpoint is deprecated and will be removed in a future version. Please migrate to POST /api/person. User: {User}", User?.Identity?.Name ?? "Unknown");
        
        if (!ModelState.IsValid)
        {
            ViewBag.Households = await _householdService.GetAllAsync();
            return View(request);
        }

        try
        {
            await _personService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Households = await _householdService.GetAllAsync();
            return View(request);
        }
    }

    // GET: Person/Edit/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Edit(int id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null)
        {
            return NotFound();
        }

        return View(person);
    }

    // POST: Person/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [Obsolete("This MVC POST endpoint is deprecated. Use PUT /api/person/{id} instead. This endpoint will be removed in a future version (planned: 3 sprints). Migration guide: https://github.com/GLGRushton/RushtonRoots/blob/main/docs/ApiEndpointsImplementationPlan.md#phase-43-deprecate-old-mvc-post-patterns")]
    public async Task<IActionResult> Edit(int id, UpdatePersonRequest request)
    {
        _logger.LogWarning("DEPRECATED: POST /Person/Edit was called. This endpoint is deprecated and will be removed in a future version. Please migrate to PUT /api/person. User: {User}, PersonId: {PersonId}", User?.Identity?.Name ?? "Unknown", id);
        
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Households = await _householdService.GetAllAsync();
            return View(request);
        }

        try
        {
            await _personService.UpdateAsync(request);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Households = await _householdService.GetAllAsync();
            return View(request);
        }
    }

    // GET: Person/Delete/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null)
        {
            return NotFound();
        }
        return View(person);
    }

    // POST: Person/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [Obsolete("This MVC POST endpoint is deprecated. Use DELETE /api/person/{id} instead. This endpoint will be removed in a future version (planned: 3 sprints). Migration guide: https://github.com/GLGRushton/RushtonRoots/blob/main/docs/ApiEndpointsImplementationPlan.md#phase-43-deprecate-old-mvc-post-patterns")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogWarning("DEPRECATED: POST /Person/Delete was called. This endpoint is deprecated and will be removed in a future version. Please migrate to DELETE /api/person. User: {User}, PersonId: {PersonId}", User?.Identity?.Name ?? "Unknown", id);
        
        try
        {
            await _personService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
