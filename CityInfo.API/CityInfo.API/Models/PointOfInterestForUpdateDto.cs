using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointOfInterestForUpdateDto
    {
        [Required(ErrorMessage = "Its req vatan.")]
        [MaxLength(50, ErrorMessage = "Max length is 50 vatan")]
        public string Name { get; set; }


        [MaxLength(200)]
        public string Description { get; set; }
    }
}
