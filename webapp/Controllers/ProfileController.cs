using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FriendsAppNoORM.Data;
using FriendsAppNoORM.Models;
using FriendsAppNoORM.Utilities;
using FriendsAppNoORM.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FriendsAppNoORM.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDatabaseContext _db;

        private const int DEFAULT_PAGE_SIZE = 10;
        private const int MAX_PAGE_SIZE = 100;

        public ProfileController(ApplicationDatabaseContext context)
        {
            _db = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Age,Gender,City,Interests")] Profile model)
        {
            Guid accountId = HttpContext.User.GetAccountId();
            string accountNme = HttpContext.User.GetAccountName();

            Profile created = _db.Profile.Create(accountId, model);

            await HttpContext.SignInAsync(accountId, model.FirstName, created.ProfileId, false);

            return RedirectToAction(nameof(Details), new { id = created.ProfileId });
        }

        public async Task<IActionResult> FindFriend(string lnf, string fnf, int? pi, int? ps)
        {
            int pageSize = DEFAULT_PAGE_SIZE;
            if(ps.HasValue){
                pageSize = (ps.Value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : ps.Value;
            }
            int pageIndex = 0;
            if(pi.HasValue){
                pageIndex = pi.Value;
            }

            long profileId = HttpContext.User.GetMyProfileId();

            //read from replica
            long itemCount = await _db.Profile.RetieveNotRelatedCountAsync(profileId, lnf, fnf);
            List<Profile> profiles = await _db.Profile.RetrieveNotRelatedAsync(profileId, lnf, fnf, pageIndex, pageSize);

            ProfilePaginatedListViewModel model = new ProfilePaginatedListViewModel(profiles, itemCount, pageIndex, pageSize);

            if(!string.IsNullOrEmpty(fnf)){
                model.FirstNameFilter = fnf;
            }
            if(!string.IsNullOrEmpty(lnf)){
                model.LastNameFilter = lnf;
            }
            
            return View(model);
        }


        [HttpGet]
        public IActionResult My()
        {
            long? profileId = HttpContext.User.GetMyProfileId();
            return RedirectToAction(nameof(Details), new { id = profileId });
        }

        [HttpGet]
        public IActionResult Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile profile = _db.Profile.Retrieve(id.Value);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }


        [HttpGet]
        [ActionName("Edit")]
        public IActionResult Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            long myProfileId = HttpContext.User.GetMyProfileId();

            if (id.Value != myProfileId)
            {
                return Forbid();
            }

            Profile p = _db.Profile.Retrieve(id.Value);

            return View(p);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long? id, [Bind("ProfileId,FirstName,LastName,Age,Gender,City,Interests")] Profile model)
        {
            if (id != model.ProfileId)
            {
                return NotFound();
            }

            long myProfileId = HttpContext.User.GetMyProfileId();

            if (id.Value != myProfileId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Profile.Update(model);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Profile created = _db.Profile.Retrieve(id.Value);
                    if (created == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw ex;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = id.Value });

            }

            return View(model);
        }
    }
}
