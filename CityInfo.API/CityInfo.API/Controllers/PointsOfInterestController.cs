using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CityInfo.API.Services;
using CityInfo.API.Entities;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService localMailService,
            ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = localMailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointofinterest")]
        public IActionResult GetPointOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City not found with id {cityId}");
                    return NotFound();
                }
                //throw new Exception("I m exception msz");
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} not found");
                //    return NotFound();
                //}

                var pointOfInterestForCity = _cityInfoRepository.GetPointOfInterestForCity(cityId);

                //var POIList = new List<PointOfIntrestDto>();
                //foreach (var item in pointOfInterestForCity)
                //{
                //    POIList.Add(new PointOfIntrestDto()
                //    {
                //        Id = item.Id,
                //        Name = item.Name,
                //        Description = item.Description
                //    });
                //}

                var POIList = AutoMapper.Mapper.Map<IEnumerable<PointOfIntrestDto>>(pointOfInterestForCity);

                return Ok(POIList);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while getting info for city id {cityId}", ex);
                return StatusCode(500, "Problem while handling req.");
            }

        }

        [HttpGet("{cityId}/pointofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
            else
            {
                var poi = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
                if (poi == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(
                    /*new PointOfIntrestDto()
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                }*/

                    AutoMapper.Mapper.Map<PointOfIntrestDto>(poi)
                    );
                }
            }
        }

        [HttpPost("{cityId}/pointofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterestForCreationDto)
        {
            if (pointOfInterestForCreationDto == null)
            {
                return BadRequest();
            }

            if (pointOfInterestForCreationDto.Name == pointOfInterestForCreationDto.Description)
            {
                ModelState.AddModelError("Description", "Name and description can not be same.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var IsCityExist = _cityInfoRepository.CityExists(cityId);

            if (!IsCityExist)
            {
                return NotFound("City with given id not exist");
            }
            var finalPointOfInterest = AutoMapper.Mapper.Map<PointOfInterest>(pointOfInterestForCreationDto);
            _cityInfoRepository.CreatePointOfInterest(cityId, finalPointOfInterest);

            if (!_cityInfoRepository.SaveChanges())
            {
                return StatusCode(500, "Could not save POI");
            }

            var poi = AutoMapper.Mapper.Map<Models.PointOfIntrestDto>(finalPointOfInterest);

            //var xxx = CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = poi.Id }, finalPointOfInterest); <= It fails Why???

            var yyy = CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = poi.Id }, poi);

            return yyy;
        }

        [HttpPut("{cityId}/pointofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterestForUpdateDto)
        {

            if (pointOfInterestForUpdateDto == null)
            {
                return BadRequest();
            }

            if (pointOfInterestForUpdateDto.Name == pointOfInterestForUpdateDto.Description)
            {
                ModelState.AddModelError("Description", "Name and description can not be same.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsPOIExist = _cityInfoRepository.IsPointOfInterestForCityExist(cityId, id); //CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Any(x => x.Id == id);
            if (!IsPOIExist)
            {
                return NotFound("POI with given id not exist");
            }

            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, id, pointOfInterestForUpdateDto);

            if (!_cityInfoRepository.SaveChanges())
            {
                return StatusCode(500, "POI not updated.");
            }

            return NoContent();
        }

        [HttpPatch("{cityId}/pointofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            /*
             1. Get the doc to be updated from store. (let's say its docFromStore)
             2. Create an object from docFromStore which have specific properties we have scope from path (i.e. except id usually) (lets say its POIToPatch)
             3. Apply JSONPatchDocument (patchDoc) to POIToPatch and get model validation error in ModelState.
             4. Validate ModelState.
             5. Replace values.
             6. Return NoContent.
             */

            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NoContent();
            }

            var poiEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if (poiEntity  == null)
            {
                return NotFound();
            }

            var PoiToPatch = AutoMapper.Mapper.Map<PointOfInterestForUpdateDto>(poiEntity);

            patchDoc.ApplyTo(PoiToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (PoiToPatch.Description == PoiToPatch.Name)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(PoiToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AutoMapper.Mapper.Map(PoiToPatch, poiEntity);

            if (!_cityInfoRepository.SaveChanges())
            {
                return StatusCode(500, "Could not patch update POI");
            }

            return NoContent();
        }

        [HttpDelete("{cityId}/pointofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsCityExist = _cityInfoRepository.CityExists(cityId); //CitiesDataStore.Current.Cities.Any(x => x.Id == cityId);

            if (!IsCityExist)
            {
                return NotFound("City with given id not exist");
            }

            var IsPOIExist = _cityInfoRepository.IsPointOfInterestForCityExist(cityId, id); //CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Any(x => x.Id == id);
            if (!IsPOIExist)
            {
                return NotFound("POI with given id not exist");
            }

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            //var POI = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId).PointsOfInterest.FirstOrDefault(x => x.Id == id);

            //var v = city.PointsOfInterest.Remove(POI);

            var poiToDelete = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            _cityInfoRepository.DeletePOI(poiToDelete);

            if (!_cityInfoRepository.SaveChanges())
            {
                return StatusCode(500, "POI not updated.");
            }

            // _mailService.Send("Point of interest was deleted", $"POI: {POI.Name} was deleted.");

            return NoContent();
        }
    }
}