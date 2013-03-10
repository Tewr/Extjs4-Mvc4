using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tewr.ExtJsMvc.Examples.Models;

namespace Tewr.ExtJsMvc.Examples.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var homeModel = new HomeModel();
            homeModel.Name = "The friends list";
            homeModel.Persons = new[] {
                new PersonModel {
                    Id = 1,
                    Name = "Anastasia",
                    OccupationId = HomeModel.OccupationId(Occupation.BeingRich),
                    Occupations = BuildOccupationList(Occupation.Working),
                    BirthDate = new DateTime(2001, 06, 18),
                    Fortune = 5000000
                   
                },
                new PersonModel {
                    Id = 2,
                    Name = "Nestor",
                    OccupationId = HomeModel.OccupationId(Occupation.Working),
                    Occupations = BuildOccupationList(Occupation.Unemployed),
                    BirthDate = new DateTime(2003, 06, 15),
                    Fortune = 2231.12213m
                },
                new PersonModel {
                    Id = 3,
                    Name = "Proletaerius",
                    OccupationId = HomeModel.OccupationId(Occupation.Unemployed),
                    Occupations = BuildOccupationList(Occupation.BeingRich),
                    BirthDate = DateTime.Now,
                    Nationalities = 70
                }
            };

            return View("index", homeModel);
        }

        private IEnumerable<SelectListItem> BuildOccupationList(Occupation occupation) {
            return
                Enum.GetValues(typeof(Occupation))
                .Cast<Occupation>()
                .Select(x => new SelectListItem() { Text = x.ToString(), Selected = x == occupation, Value = HomeModel.OccupationId(x) }); 
        }
    }
}
