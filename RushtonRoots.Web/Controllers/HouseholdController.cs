using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Web.Controllers;

/// <summary>
/// Controller for Household management.
/// </summary>
[Authorize]
public class HouseholdController : Controller
{
    private readonly IHouseholdService _householdService;
    private readonly IPersonService _personService;

    public HouseholdController(IHouseholdService householdService, IPersonService personService)
    {
        _householdService = householdService;
        _personService = personService;
    }

    // GET: Household
    public async Task<IActionResult> Index()
    {
        var households = await _householdService.GetAllAsync();
        return View(households);
    }

    // GET: Household/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var household = await _householdService.GetByIdAsync(id);
        if (household == null)
        {
            return NotFound();
        }
        return View(household);
    }

    // GET: Household/Members/5
    public async Task<IActionResult> Members(int id)
    {
        var household = await _householdService.GetByIdAsync(id);
        if (household == null)
        {
            return NotFound();
        }

        var members = await _personService.GetByHouseholdIdAsync(id);
        ViewBag.Household = household;
        return View(members);
    }

    // GET: Household/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.People = await _personService.GetAllAsync();
        return View();
    }

    // POST: Household/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateHouseholdRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }

        try
        {
            await _householdService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }
    }

    // GET: Household/Edit/5
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Edit(int id)
    {
        var household = await _householdService.GetByIdAsync(id);
        if (household == null)
        {
            return NotFound();
        }

        var request = new UpdateHouseholdRequest
        {
            Id = household.Id,
            HouseholdName = household.HouseholdName,
            AnchorPersonId = household.AnchorPersonId
        };

        ViewBag.People = await _personService.GetAllAsync();
        return View(request);
    }

    // POST: Household/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> Edit(int id, UpdateHouseholdRequest request)
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
            await _householdService.UpdateAsync(request);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.People = await _personService.GetAllAsync();
            return View(request);
        }
    }

    // GET: Household/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var household = await _householdService.GetByIdAsync(id);
        if (household == null)
        {
            return NotFound();
        }
        
        // Get delete impact data
        var deleteImpact = await _householdService.GetDeleteImpactAsync(id);
        ViewBag.DeleteImpact = deleteImpact;
        
        return View(household);
    }

    // POST: Household/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _householdService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
