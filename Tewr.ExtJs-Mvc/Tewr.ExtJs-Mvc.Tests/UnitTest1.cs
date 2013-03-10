using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;
using System.IO;

using Moq;
using Tewr.ExtJsMvc;
using Tewr.ExtJsMvc.Examples.Models;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private IEnumerable<SelectListItem> BuildOccupationList(Occupation occupation) {
            return
                Enum.GetValues(typeof(Occupation))
                .Cast<Occupation>()
                .Select(x => new SelectListItem() { Text = x.ToString(), Selected = x == occupation, Value = OccupationId(x) }); 
        }

        private static string OccupationId(Occupation x)
        {
            return ((int)x).ToString();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var homeModel = new HomeModel();
            homeModel.Name = "The friends list";
            homeModel.Persons = new[] {
                new PersonModel {
                    Id = 1,
                    Name = "Anastasia",
                    OccupationId = OccupationId(Occupation.BeingRich),
                    Occupations = BuildOccupationList(Occupation.BeingRich)
                },
                new PersonModel {
                    Id = 2,
                    Name = "Nestor",
                    OccupationId = OccupationId(Occupation.Working),
                    Occupations = BuildOccupationList(Occupation.Working)
                },
                new PersonModel {
                    Id = 3,
                    Name = "Proletaerius",
                    OccupationId = OccupationId(Occupation.Unemployed),
                    Occupations = BuildOccupationList(Occupation.Unemployed)
                }
            };

            var htmlHelper = CreateHelper(homeModel);

            var test = htmlHelper.EditableGrid(
                "personsGrid", 
                x => x.Persons, 
                null,
                x => x
                    .Text(m => m.Name)
                    .Combo(m => m.Occupations, m => m.OccupationId));

            Debug.WriteLine(test);
        }

        private static HtmlHelper<TModel> CreateHelper<TModel>(TModel instance) { 
            ViewDataDictionary vd = new ViewDataDictionary(instance);

            var controllerContext = new ControllerContext(new Mock<HttpContextBase>().Object,
                                                          new RouteData(),
                                                          new Mock<ControllerBase>().Object);

            var viewContext = new ViewContext(controllerContext, new Mock<IView>().Object, vd, new TempDataDictionary(), new Mock<TextWriter>().Object);

            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData).Returns(vd);

            return new HtmlHelper<TModel>(viewContext, mockViewDataContainer.Object);

        }
    }
}
