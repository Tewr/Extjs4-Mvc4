using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tewr.ExtJsMvc.Examples.Models
{
    public class PersonModel
    {
        public int Id { get; set; }

        [Display(Name="The Person name")]
        public string Name { get; set; }

        public string OccupationId { get; set; }

        public IEnumerable<SelectListItem> Occupations { get; set; }

        [Display(Name = "Date of birth")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Ammassed fortune")]
        [DisplayFormat(DataFormatString = "{0} $")]
        public decimal Fortune { get; set; }

        public int Nationalities { get; set; }
    }
}