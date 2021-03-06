﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;

namespace Actio.Api.Controllers
{
    [Route("[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IBusClient _busClient;

        public ActivitiesController(IBusClient busClient) {
            _busClient = busClient;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Content("Content");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody] CreateActivity command) {

            command.Id = Guid.NewGuid();
            command.CreatedAt = DateTime.UtcNow;
            await _busClient.PublishAsync(command);

            return Accepted($"activities/{command.Id}");
        }
    }
}