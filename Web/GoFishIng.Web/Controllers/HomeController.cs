namespace GoFishIng.Web.Controllers
{
    using GoFishIng.Services;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ICartsService cartsServices;

        public HomeController(ICartsService cartsServices)
        {
            this.cartsServices = cartsServices;
        }
        public IActionResult Index()
        {
           
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}
