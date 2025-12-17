using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// Controller for Partnership management.
/// </summary>
[Authorize]
public class PartnershipController : Controller
{
    private readonly IPartnershipService _partnershipService;
    private readonly IPersonService _personService;
    private readonly ILogger<PartnershipController> _logger;

    public PartnershipController(IPartnershipService partnershipService, IPersonService personService, ILogger<PartnershipController> logger)
    {
        _partnershipService = partnershipService;
        _personService = personService;
        _logger = logger;
    }

    // GET: Partnership
    public async Task<IActionResult> Index()
    {
        var partnerships = await _partnershipService.GetAllAsync();
        return View(partnerships);
    }

    // GET: Partnership/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var partnership = await _partnershipService.GetByIdAsync(id);
        if (partnership == null)
        {
            return NotFound();
        }
        return View(partnership);
    }

    // GET: Partnership/Create
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.People = await _personService.GetAllAsync();
        return View();
    }

    // POST: Partnership/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [Obsolete("This MVC POST endpoint is deprecated. Use POST /api/partnership instead. This endpoint will be removed in a future version (planned: 3 sprints). Migration guide: https://github.com/GLGRushton/RushtonRoots/blob/main/docs/ApiEndpointsImplementationPlan.md#phase-43-deprecate-old-mvc-post-patterns")]
    public async Task<IActionResult> Create(CreatePartnershipRequest request)
    {
        _logger.LogWarning("DEPRECATED: POST /Partnership/Create was called. This endpoint is deprecated and will be removed in a future version. Please migrate to POST /api/partnership. User: {User}", User?.Identity?.Name ?? "Unknown");
        
        if (!ModelState.IsValid)
        {
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }

        try
        {
            await _partnershipService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }
        catch (NotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }
    }

    // GET: Partnership/Edit/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Edit(int id)
    {
        var partnership = await _partnershipService.GetByIdAsync(id);
        if (partnership == null)
        {
            return NotFound();
        }

        var request = new UpdatePartnershipRequest
        {
            Id = partnership.Id,
            PersonAId = partnership.PersonAId,
            PersonBId = partnership.PersonBId,
            PartnershipType = partnership.PartnershipType,
            StartDate = partnership.StartDate,
            EndDate = partnership.EndDate
        };

        ViewBag.People = await _personService.GetAllAsync();
        return View(request);
    }

    // POST: Partnership/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [Obsolete("This MVC POST endpoint is deprecated. Use PUT /api/partnership/{id} instead. This endpoint will be removed in a future version (planned: 3 sprints). Migration guide: https://github.com/GLGRushton/RushtonRoots/blob/main/docs/ApiEndpointsImplementationPlan.md#phase-43-deprecate-old-mvc-post-patterns")]
    public async Task<IActionResult> Edit(int id, UpdatePartnershipRequest request)
    {
        _logger.LogWarning("DEPRECATED: POST /Partnership/Edit was called. This endpoint is deprecated and will be removed in a future version. Please migrate to PUT /api/partnership. User: {User}, PartnershipId: {PartnershipId}", User?.Identity?.Name ?? "Unknown", id);
        
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }

        try
        {
            await _partnershipService.UpdateAsync(request);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }
    }

    // GET: Partnership/Delete/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var partnership = await _partnershipService.GetByIdAsync(id);
        if (partnership == null)
        {
            return NotFound();
        }
        return View(partnership);
    }

    // POST: Partnership/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    [Obsolete("This MVC POST endpoint is deprecated. Use DELETE /api/partnership/{id} instead. This endpoint will be removed in a future version (planned: 3 sprints). Migration guide: https://github.com/GLGRushton/RushtonRoots/blob/main/docs/ApiEndpointsImplementationPlan.md#phase-43-deprecate-old-mvc-post-patterns")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogWarning("DEPRECATED: POST /Partnership/Delete was called. This endpoint is deprecated and will be removed in a future version. Please migrate to DELETE /api/partnership. User: {User}, PartnershipId: {PartnershipId}", User?.Identity?.Name ?? "Unknown", id);
        
        try
        {
            await _partnershipService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
