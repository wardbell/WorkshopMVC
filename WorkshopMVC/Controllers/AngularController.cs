using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WorkshopMVC.Controllers
{
    public class AngularController : Controller
    {

        public ActionResult Index()
        {
            return View("Ng1");
        }
        public ActionResult Ng1()
        {
            return View("Ng1");
        }

        public ActionResult Ng2()
        {
            var model = new MyModel { Message = "All is well!" };
            return View("Ng2","", model);
        }

        public ActionResult Ng3()
        {
            var model = GetDummyFoodData();
            return View("Ng3", "", AsJson(model));
        }

        public ActionResult Ng4()
        {
            var model = GetDummyFoodData();
            model.Foods.Add(new Food{Name="Cavier", Rating=5, Notes="NOW you're talking"});
            return View("Ng4", "", AsJson(model));
        }


        public ActionResult Ng5()
        {
            var model = GetDummyFoodData();
            model.Message = "Doing it with templates";
            model.Foods.Add(new Food { Name = "Cavier", Rating = 5, Notes = "NOW you're talking" });
            model.Foods.Add(new Food { Name = "Cracked crab", Rating = 5, Notes = "'tis the season" }); 
            return View("Ng5", "", AsJson(model));
        }

        public ActionResult Ng6()
        {
            return View("Ng6");
        }

        [HttpGet]
        public ActionResult Ng6Data()
        {
            var model = GetDummyFoodData();
            model.Message = "Got data asynchronously from MVC controller";
            model.Foods.Add(new Food { Name = "Cavier", Rating = 5, Notes = "NOW you're talking" });
            model.Foods.Add(new Food { Name = "Cracked crab", Rating = 5, Notes = "'tis the season" });
            model.Foods.Add(new Food { Name = "Liver & onions", Rating = 2, Notes = "I actually like it" });

            return AsJsonResult(model);
        }

        private string AsJson(object model)
        {
            var settings = new JsonSerializerSettings
            {
                // per client-side JavaScript conventions
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(model, settings);
        }

        private ActionResult AsJsonResult(object model)
        {
            var result = new ContentResult
            {
                Content = AsJson(model),
                ContentType = "application/json"
            };
            return result; 
        }

        private MyModel GetDummyFoodData()
        {
            var model = new MyModel
            {
                Message = "These are the foods I like",

                Foods = new List<Food>
                {
                    new Food{Name = "Hamburger", Rating = 2, Notes = "Comfort food"},
                    new Food{Name = "French Fries", Rating = 4, Notes = "Ooooo!"},
                    new Food{Name = "Frog", Rating = 3, Notes = "Not as bad as you think"},
                }

            };
            return model;
        }
	}

    #region Model
    public class MyModel
    {
        public string Message { get; set; }
        public IList<Food> Foods { get; set; }
    }

    public class Food
    {
        private static int _nextId = 1;

        public Food()
        {
            Id = _nextId++;
        }
        public int Id { get; private set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Notes { get; set; }
    }
    #endregion
}