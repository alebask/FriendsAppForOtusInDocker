using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FriendsAppNoORM.Controllers
{
    public class ChatsController : Controller
    {     
        public async Task<IActionResult> Index(long? id)
        {
            return View();
        }
    }
}