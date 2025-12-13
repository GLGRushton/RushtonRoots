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

    public PartnershipController(IPartnershipService partnershipService, IPersonService personService)
    {
        _partnershipService = partnershipService;
        _personService = personService;
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
    public async Task<IActionResult> Create(CreatePartnershipRequest request)
    {
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
    public async Task<IActionResult> Edit(int id, UpdatePartnershipRequest request)
    {
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
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
