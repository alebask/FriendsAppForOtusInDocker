using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FriendsAppNoORM.Data;
using FriendsAppNoORM.Models;
using FriendsAppNoORM.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FriendsAppNoORM.Controllers
{
    public class FriendshipController : Controller
    {
        private readonly ApplicationDatabaseContext _db;

        public FriendshipController(ApplicationDatabaseContext context)
        {
            _db = context;
        }

        [HttpPost]
        public IActionResult SendRequest([FromForm] long? to)
        {
            if (to.HasValue)
            {
                long myProfileId = HttpContext.User.GetMyProfileId();

                if (to.Value == myProfileId)
                {
                    return DisplayMessage("You cannot send friend request to youself");
                }

                Friendship fr1 = _db.Friendship.Retrieve(myProfileId, to.Value);

                if (fr1 != null)
                {
                    if (fr1.Status == FriendshipStatus.Pending)
                    {
                        _db.Friendship.UpdateStatus(fr1);
                        return RedirectToAction("Requests");
                    }
                    else
                    {
                        return DisplayMessage($"You are arlready friends with { fr1.RequestedTo.Name }");
                    }
                }
                else
                {
                    Friendship fr2 = _db.Friendship.Retrieve(to.Value, myProfileId);
                    if (fr2 != null)
                    {
                        if(fr2.Status == FriendshipStatus.Pending){
                            return DisplayMessage($"You arlready have pending requst from { fr2.RequestedBy.Name }");
                        }
                        else{
                            return DisplayMessage($"You are arlready friends with { fr1.RequestedTo.Name }");
                        }
                    }
                    else
                    {
                        Friendship fr = new Friendship();
                        fr.RequestedBy = new ProfileReference(myProfileId);
                        fr.RequestedTo = new ProfileReference(to.Value);
                        fr.Status = FriendshipStatus.Pending;

                        _db.Friendship.Create(fr);                        
                    }
                }
            }
            return RedirectToAction("FindFriend", "Profile");
        }

        [HttpPost]
        public IActionResult AcceptRequest([FromForm] long? by)
        {
            if (by.HasValue)
            {
                long myProfileId = HttpContext.User.GetMyProfileId();

                Friendship fr = _db.Friendship.Retrieve(by.Value, myProfileId);

                if(fr != null && fr.Status == FriendshipStatus.Pending){

                    fr.Status = FriendshipStatus.Established;

                    _db.Friendship.UpdateStatus(fr);
                }
            }

            return RedirectToAction("Requests");
        }

        [HttpPost]
        public IActionResult Delete([FromForm] long? id)
        {
            if (id.HasValue)
            {
                long myId = HttpContext.User.GetMyProfileId();
                _db.Friendship.Delete(id.Value, myId);
                _db.Friendship.Delete(myId, id.Value);
            }
            return RedirectToAction("Friends");
        }

        [HttpPost]
        public IActionResult RevokeRequest([FromForm] long? to)
        {
            if (to.HasValue)
            {
                long myId = HttpContext.User.GetMyProfileId();
                _db.Friendship.Delete(myId, to.Value);
            }
            return RedirectToAction("Requests");
        }

        [HttpPost]
        public IActionResult DeclineRequest(long? by)
        {
            if (by.HasValue)
            {
                long myId = HttpContext.User.GetMyProfileId();
                _db.Friendship.Delete(by.Value, myId);
                
            }
            return RedirectToAction("Requests");
        }

        [HttpGet]
        public IActionResult Friends()
        {
            long profileId = HttpContext.User.GetMyProfileId();

            List<Friendship> friendships = _db.Friendship.RetrieveAllForProfile(profileId, FriendshipStatus.Established);

            return View(friendships);
        }

        [HttpGet]
        public IActionResult Requests()
        {
            long profileId = HttpContext.User.GetMyProfileId();

            List<Friendship> pendingRequests = _db.Friendship.RetrieveAllForProfile(profileId, FriendshipStatus.Pending);

            return View(pendingRequests);
        }


        private RedirectToActionResult DisplayMessage(string message)
        {
            return RedirectToAction("Index", "Home", new { message = message });
        }
    }
}
