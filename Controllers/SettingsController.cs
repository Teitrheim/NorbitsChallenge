using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NorbitsChallenge.Dal;
using NorbitsChallenge.Models;

namespace NorbitsChallenge.Controllers
{
    public class SettingsController: Controller
    {
        private readonly IConfiguration _config;

        public SettingsController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(int companyId)
        {
            var model = new SettingsViewModel();
            var settings = new SettingsDb(_config).GetSettings(companyId);
            model.Settings = settings;
            

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(SettingsInputModel input)
        {
            new SettingsDb(_config).UpdateSetting(input.Setting, input.CompanyId);

            return RedirectToAction("Index", new {companyId = input.CompanyId});
        }
    }
}
