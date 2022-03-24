using RalphLaurenPortal.Models.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RalphLaurenPortal.Controllers
{
    public class GeneralAPIController : ApiController
    {

        [HttpPost]
        public IHttpActionResult FetchResources([FromBody] PassResourceFetch objFetch)
        {
            var resourceDefinition = new Resource().loadResourcesFromCms(
                objFetch.countryId,
                objFetch.languageId,
                objFetch.useDate);

            var result = new List<ResourceResult>();

            foreach (var r in objFetch.resources)
            {
                result.Add(new ResourceResult()
                {
                    Name = r,
                    Value = resourceDefinition.Result.Resources
                                                     .Where(p => p.Name == r)
                                                     .Select(p => p.Value)
                                                     .SingleOrDefault()
                });
            }

            return Ok(result);
        }
    }
}