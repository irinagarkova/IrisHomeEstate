using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeEstate.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        private readonly ApplicationUser applicationUser;

        //public AccountController(IAccountService accountService, IMapper mapper,ApplicationUser applicationUser)
        //{
        //    this.accountService = accountService;
        //    this.mapper = mapper;
        //    this.applicationUser = applicationUser;
        //}
        //public async Task<IActionResult> Index()
        //{
        //    return View();
        //}

       


    }
}
