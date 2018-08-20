using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(x => x.Id == cityId);
        }

        public void CreatePointOfInterest(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
            
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return _context.Cities.Include(x => x.PointsOfInterest)
                    .Where(x => x.Id == cityId).FirstOrDefault();

            }
            return _context.Cities
                    .Where(x => x.Id == cityId).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest//.Include(x=>x.CityId)
                    .Where(x => x.CityId == cityId && x.Id == pointOfInterestId).FirstOrDefault();
        }

        public bool IsPointOfInterestForCityExist(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest
                    .Any(x => x.CityId == cityId && x.Id == pointOfInterestId);
        }

        public IEnumerable<PointOfInterest> GetPointOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest
                    .Where(x => x.CityId == cityId).ToList();
        }

        public bool SaveChanges()
        {
            var v = _context.SaveChanges();
            return (v > 0);
        }

        public void UpdatePointOfInterestForCity(int cityId, int id, PointOfInterestForUpdateDto pointOfInterestForUpdateDto)
        {
            var poi = GetPointOfInterestForCity(cityId, id);
            AutoMapper.Mapper.Map(pointOfInterestForUpdateDto, poi);
        }

        public void DeletePOI(PointOfInterest poiToDelete)
        {
            _context.PointsOfInterest.Remove(poiToDelete);
        }
    }
}
