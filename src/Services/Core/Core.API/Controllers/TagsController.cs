﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Core.Domain;

namespace Core.API.Controllers
{
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/tags/{tagId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Tag), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadTag(long tenantId, long tagId)
        {
            Tag tag = await _tagService.ReadTagAsync(tenantId, tagId);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }

        [HttpGet]
        [Route("api/v1/core/tenants/{tenantId:int}/tags")]
        [ProducesResponseType(typeof(IEnumerable<Tag>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListTags(long tenantId, [FromQuery]string tagIds)
        {
            IEnumerable<long> tagIdCollection = !string.IsNullOrWhiteSpace(tagIds) ? tagIds.Split(",").Select(long.Parse) : null;
            IEnumerable<Tag> tags = await _tagService.ListTagsAsync(tenantId, tagIdCollection);
            return Ok(tags);
        }
    }
}
