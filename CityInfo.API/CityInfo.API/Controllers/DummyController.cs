using CityInfo.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    public class DummyController:Controller
    {
        private CityInfoContext _ctx;

        public DummyController(CityInfoContext cityInfoContext)
        {
            _ctx = cityInfoContext;
        }

        [HttpGet("api/testdb")]
        public IActionResult TestDb()
        {
            return Ok();
        }
    }
}
