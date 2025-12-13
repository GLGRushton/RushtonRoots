using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// Controller for ParentChild relationship management.
/// </summary>
[Authorize]
public class ParentChildController : Controller
{
    private readonly IParentChildService _parentChildService;
    private readonly IPersonService _personService;

    public ParentChildController(IParentChildService parentChildService, IPersonService personService)
    {
        _parentChildService = parentChildService;
        _personService = personService;
    }

    // GET: ParentChild
    public async Task<IActionResult> Index()
    {
        var relationships = await _parentChildService.GetAllAsync();
        return View(relationships);
    }

    // GET: ParentChild/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var relationship = await _parentChildService.GetByIdAsync(id);
        if (relationship == null)
        {
            return NotFound();
        }
        return View(relationship);
    }

    // GET: ParentChild/Create
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.People = await _personService.GetAllAsync();
        return View();
    }

    // POST: ParentChild/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Create(CreateParentChildRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }

        try
        {
            await _parentChildService.CreateAsync(request);
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

    // GET: ParentChild/Edit/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Edit(int id)
    {
        var relationship = await _parentChildService.GetByIdAsync(id);
        if (relationship == null)
        {
            return NotFound();
        }

        var request = new UpdateParentChildRequest
        {
            Id = relationship.Id,
            ParentPersonId = relationship.ParentPersonId,
            ChildPersonId = relationship.ChildPersonId,
            RelationshipType = relationship.RelationshipType
        };

        ViewBag.People = await _personService.GetAllAsync();
        return View(request);
    }

    // POST: ParentChild/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Edit(int id, UpdateParentChildRequest request)
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
            await _parentChildService.UpdateAsync(request);
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

    // GET: ParentChild/Delete/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var relationship = await _parentChildService.GetByIdAsync(id);
        if (relationship == null)
        {
            return NotFound();
        }
        return View(relationship);
    }

    // POST: ParentChild/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _parentChildService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
