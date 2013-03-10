using System.Collections.Generic;

namespace Tewr.ExtJsMvc.Examples.Models
{
    public class HomeModel
    {

        public string Name { get; set; }

        public IEnumerable<PersonModel> Persons { get; set; }

        public static string OccupationId(Occupation x)
        {
            return ((int)x).ToString();
        }
    }


}