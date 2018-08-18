using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        //Run Add-Migration when u change entity structure to include that in migrate scripts.
        //Add-Migration CityInfoDbAddPOIDescription
        [MaxLength(200)]
        public string Description { get; set; }

        //Navigation Prop..
        [ForeignKey("CityId")]
        public City City { get; set; }
        public int CityId { get; set; }

        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
    }
}