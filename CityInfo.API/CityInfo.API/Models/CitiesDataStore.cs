using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore()
        {
            this.Cities = new List<CityDto>() {

                new CityDto(){

                    Id=1,
                    Name="New York",
                    Description="Anamika",
                    PointOfIntrest = new List<PointOfIntrestDto>(){
                        new PointOfIntrestDto(){ Id=101,Name="New", Description="New Descrip"},
                        new PointOfIntrestDto(){ Id=201,Name="New", Description="New Descrip"},
                        new PointOfIntrestDto(){ Id=301,Name="New", Description="New Descrip"}
                    }

                },
                new CityDto(){

                    Id=2,
                    Name="Old York",
                    Description="Anamika",
                    PointOfIntrest = new List<PointOfIntrestDto>(){
                        new PointOfIntrestDto(){ Id=102,Name="Old", Description="Old Descrip"}
                    }

                },
                new CityDto(){

                    Id=3,
                    Name="Mid York",
                    Description="Anamika",
                    PointOfIntrest = new List<PointOfIntrestDto>(){
                        new PointOfIntrestDto(){ Id=103,Name="Mid", Description="Mid Descrip"}
                    }

                }
            };
        }
    }
}
