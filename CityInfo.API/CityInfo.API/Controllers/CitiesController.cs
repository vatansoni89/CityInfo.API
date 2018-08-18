using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;
        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            //return Ok(CitiesDataStore.Current.Cities);
            var cityEntities = _cityInfoRepository.GetCities();
            //var results = new List<CityWithoutPointOfInterestDto>();

            //foreach (var item in cityEntities)
            //{
            //    results.Add(new CityWithoutPointOfInterestDto() {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Description = item.Description
            //    });
            //}

            var results = Mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointOfInterest = false)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            var city = _cityInfoRepository.GetCity(id, includePointOfInterest);
            if (city == null)
            {
                return NotFound();
            }
            //else
            //{
            //    return Ok(city);
            //}

            if (includePointOfInterest)
            {
             var cityResult = Mapper.Map<CityDto>(city);

                return Ok(cityResult);
            }
            else
            {
                var cityWithoutPOI = Mapper.Map<CityWithoutPointOfInterestDto>(city);
                //new CityWithoutPointOfInterestDto() {
                //    Id = city.Id,
                //    Name = city.Name,
                //    Description = city.Description
                //};

                return Ok(cityWithoutPOI);
            }
        }
    }
}