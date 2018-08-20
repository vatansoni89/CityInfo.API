using CityInfo.API.Entities;
using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        bool CityExists(int cityId);
        IEnumerable<City> GetCities();
        City GetCity(int cityId, bool includePointOfInterest);
        IEnumerable<PointOfInterest> GetPointOfInterestForCity(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);
        
        bool SaveChanges();
        void CreatePointOfInterest(int cityId, PointOfInterest finalPointOfInterest);
        bool IsPointOfInterestForCityExist(int cityId, int pointOfInterestId);
        void UpdatePointOfInterestForCity(int cityId, int id, PointOfInterestForUpdateDto pointOfInterestForUpdateDto);
        void DeletePOI(PointOfInterest poiToDelete);
    }
}
