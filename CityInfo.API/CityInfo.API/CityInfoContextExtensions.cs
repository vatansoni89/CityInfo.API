using CityInfo.API.Entities;
using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public static class CityInfoContextExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            if (context.Cities.Any())
            {
                return;
            }

            //init data seed

            var cities = new List<City>() {
                new City(){
                    Name="New York",
                    Description="Anamika",
                    PointsOfInterest = new List<PointOfInterest>(){
                        new PointOfInterest(){ Name="New", Description="New Descrip"},
                        new PointOfInterest(){ Name="New", Description="New Descrip"},
                        new PointOfInterest(){ Name="New", Description="New Descrip"}
                    }                                   
                },
                new City(){
                    Name="Old York",
                    Description="Anamika",
                    PointsOfInterest = new List<PointOfInterest>(){
                        new PointOfInterest(){ Name="Old", Description="Old Descrip"}
                    }
                },
                new City(){
                    Name="Mid York",
                    Description="Anamika",
                    PointsOfInterest = new List<PointOfInterest>(){
                        new PointOfInterest(){ Name="Mid", Description="Mid Descrip"}
                    }
                }
            };
            context.Cities.AddRange(cities);
            context.SaveChanges();
        }
    }
}
