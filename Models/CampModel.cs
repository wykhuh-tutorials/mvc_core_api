using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp.Models
{
    public class CampModel
    {
        // use custom Url instead of Id
        public string Url { get; set; }
        public string Moniker { get; set; }
        public string Name { get; set; }
        // the entity has EventDate; we want StartDate and EndDate for the model
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Length { get; set; }
        public string Description { get; set; }

        // Camp entity has Location, which is a nested object.
        // in CampModel, we want flatten Location.
        // AutoMapper has autoflatten if you list the nested Entity
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }
    }
}
