﻿using AspNetCoreMvcAngular.Models;
using AspNetCoreMvcAngular.Repositories.Things;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreMvcAngular.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    public class ThingsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IThingsRepository _thingsRepository;

        public ThingsController(IThingsRepository thingsRepository)
        {
            _thingsRepository = thingsRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_thingsRepository.GetAll());
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("all")]
        public IActionResult GetAll()
        {
            return Ok(_thingsRepository.GetAll());
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult Add([FromBody] Thing thing)
        {
            if (thing == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Thing newThing = _thingsRepository.Add(thing);

            return CreatedAtRoute("GetSingleThing", new { id = newThing.Id }, newThing);
        }

        [HttpPatch("{id:int}")]
        //[ValidateAntiForgeryToken]
        public IActionResult PartiallyUpdate(int id, [FromBody] JsonPatchDocument<Thing> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Thing existingEntity = _thingsRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            Thing thing = existingEntity;
            patchDoc.ApplyTo(thing, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Thing updatedThing = _thingsRepository.Update(id, thing);

            return Ok(updatedThing);
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetSingleThing")]
        public IActionResult Single(int id)
        {
            Thing thing = _thingsRepository.GetSingle(id);

            if (thing == null)
            {
                return NotFound();
            }

            return Ok(thing);
        }

        [HttpDelete]
        //[ValidateAntiForgeryToken]
        [Route("{id:int}")]
        public IActionResult Remove(int id)
        {
            Thing thing = _thingsRepository.GetSingle(id);

            if (thing == null)
            {
                return NotFound();
            }

            _thingsRepository.Delete(id);
            return NoContent();
        }

        [HttpPut]
        //[ValidateAntiForgeryToken]
        [Route("{id:int}")]
        public IActionResult Update(int id, [FromBody]Thing thing)
        {
            var thingToCheck = _thingsRepository.GetSingle(id);

            if (thingToCheck == null)
            {
                return NotFound();
            }

            if (id != thing.Id)
            {
                return BadRequest("Ids do not match");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Thing updatedThing = _thingsRepository.Update(id, thing);

            return Ok(updatedThing);
        }
    }
}
