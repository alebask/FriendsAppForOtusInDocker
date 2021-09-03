using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FriendsAppNoORM.Data;
using FriendsAppNoORM.Models;
using FriendsAppNoORM.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FriendsAppNoORM.Controllers
{
    public class PagesController : Controller
    {
        private readonly ApplicationDatabaseContext _db;

        public PagesController(ApplicationDatabaseContext context)
        {
            _db = context;
        }

        // GET: Pages       
        public async Task<IActionResult> Index(long? id)
        {
            long profileId;
            if(id == null){
                profileId = HttpContext.User.GetMyProfileId();
            }
            else
            {
                profileId = id.Value;
            }

            List<Page> pages = _db.Page.RetrieveAllForProfile(profileId).OrderByDescending(p => p.ChangedOn).ToList();

            return View(pages);
        }

        [HttpGet("[controller]/[action]/{profileId}/{pageUrl}")]           
        public async Task<IActionResult> View(long? profileId, string pageUrl)
        {
            if (profileId == null || pageUrl == null)
            {
                return NotFound();
            }

            var decodedPageUrl = WebUtility.UrlDecode(pageUrl);

            var page = _db.Page.Retrieve(profileId.Value, decodedPageUrl);
            
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Pages/Create
        public IActionResult Create()
        {
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Profile,Url,Title,Content,ChangedOn")] Page page)
        {
            if (ModelState.IsValid)
            {            
                long myProfileId = HttpContext.User.GetMyProfileId();
                if(myProfileId != page.Profile.Id){
                    return Forbid();
                }

                try{

                _db.Page.Create(page);

                return RedirectToAction(nameof(Index));
                }catch(MySqlException ex){
                    if(ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry){
                        ModelState.AddModelError(String.Empty, $"Page with URL {page.Url} already exists. Please, choose another url.");
                    }
                    else{
                        ModelState.AddModelError(String.Empty, $"Unexpected error: {ex.Message}");
                    }
                }

            }
            return View(page);
        }

        [HttpGet]
        public IActionResult Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Page page = _db.Page.Retrieve(id.Value);

            if (page == null)
            {
                return NotFound();
            }

            long myProfileId = HttpContext.User.GetMyProfileId();
            if(page.Profile.Id != myProfileId){
                return Forbid();
            }

            
            return View(page);
        }

        // POST: Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long? id, [Bind("PageId,Profile,Url,Title,Content,ChangedOn")] Page page)
        {
            if (id != page.PageId)
            {
                return NotFound();
            }

            long myProfileId = HttpContext.User.GetMyProfileId();

            if(myProfileId != page.Profile.Id){
                return Forbid();
            }           

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Page.Update(page);

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_db.Page.Retrieve(page.PageId) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch(MySqlException ex){
                    if (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                    {
                        ModelState.AddModelError(String.Empty, $"Page with URL {page.Url} already exists. Please, choose another url.");
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, $"Unexpected error: {ex.Message}");
                    }
                }
            }
            return View(page);
        }

        // GET: Pages/Delete/5
        [HttpPost]
        public IActionResult Delete([FromForm] long? id)
        {
            if (id.HasValue)
            {
                Page p = _db.Page.Retrieve(id.Value);

                if(p == null){
                    return NotFound();
                }

                long myProfileId = HttpContext.User.GetMyProfileId();

                if(myProfileId != p.Profile.Id){
                    return Forbid();
                }
                
                _db.Page.Delete(p.PageId);

            }
            return RedirectToAction(nameof(Index));
        }     
    }
}
