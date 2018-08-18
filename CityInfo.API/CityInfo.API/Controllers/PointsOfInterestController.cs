using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CityInfo.API.Services;

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
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreation pointOfInterestForCreation)
        {
            if (pointOfInterestForCreation == null)
            {
                return BadRequest();
            }

            if (pointOfInterestForCreation.Name == pointOfInterestForCreation.Description)
            {
                ModelState.AddModelError("Description", "Name and description can not be same.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsCityExist = CitiesDataStore.Current.Cities.Any(x => x.Id == cityId);

            if (!IsCityExist)
            {
                return NotFound("City with given id not exist");
            }

            var pointOfInterestCollection = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest);
            var maxOfPointOfInterestId = pointOfInterestCollection.Max(p => p.Id);

            var finalPointOfInterest = new PointOfIntrestDto()
            {
                Id = maxOfPointOfInterestId,
                Description = pointOfInterestForCreation.Description,
                Name = pointOfInterestForCreation.Name
            };

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
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

            var IsCityExist = CitiesDataStore.Current.Cities.Any(x => x.Id == cityId);

            if (!IsCityExist)
            {
                return NotFound("City with given id not exist");
            }

            var IsPOIExist = CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Any(x => x.Id == id);
            if (!IsPOIExist)
            {
                return NotFound("POI with given id not exist");
            }

            var POI = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId).PointsOfInterest.FirstOrDefault(x => x.Id == id);

            POI.Name = pointOfInterestForUpdateDto.Name;
            POI.Description = pointOfInterestForUpdateDto.Description;

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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsCityExist = CitiesDataStore.Current.Cities.Any(x => x.Id == cityId);

            if (!IsCityExist)
            {
                return NotFound("City with given id not exist");
            }

            var IsPOIExist = CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Any(x => x.Id == id);
            if (!IsPOIExist)
            {
                return NotFound("POI with given id not exist");
            }

            var POIFromStore = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId).PointsOfInterest.FirstOrDefault(x => x.Id == id);

            var POIToPatch = new PointOfInterestForUpdateDto()
            {
                Name = POIFromStore.Name,
                Description = POIFromStore.Description
            };

            patchDoc.ApplyTo(POIToPatch, ModelState);

            TryValidateModel(POIToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            POIFromStore.Name = POIToPatch.Name;
            POIFromStore.Description = POIToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IsCityExist = CitiesDataStore.Current.Cities.Any(x => x.Id == cityId);

            if (!IsCityExist)
            {
                return NotFound("City with given id not exist");
            }

            var IsPOIExist = CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Any(x => x.Id == id);
            if (!IsPOIExist)
            {
                return NotFound("POI with given id not exist");
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            var POI = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId).PointsOfInterest.FirstOrDefault(x => x.Id == id);

            var v = city.PointsOfInterest.Remove(POI);

            _mailService.Send("Point of interest was deleted", $"POI: {POI.Name} was deleted.");

            return NoContent();
        }
    }
}