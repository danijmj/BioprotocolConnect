using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BioprotocolConnect.ROs.Protocols.Controllers;
using BioprotocolConnect.ROs.Protocols.Models;

namespace BioprotocolConnect.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("bioprotocol/[action]")]
    public class APIController : ControllerBase
    {


        private readonly ILogger<APIController> _logger;

        public APIController(ILogger<APIController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all protocols from a specified user account and RO
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /github/GetProtocols?user=1257920
        ///
        /// </remarks>
        /// <param name="userId">The user id in the application</param>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Invalid app</response> 
        /// <response code="500">Oops! Something went wrong</response> 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<Protocol> GetProtocols([FromQuery][Required] string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            // Get all BioProtocols from a user
            ROProtocolLogic bioProtocol = new ROProtocolLogic("https://bio-protocol.org/UserHome.aspx", userId);
            List<Protocol> protocols = bioProtocol.GetListProtocols();

            // Return the list of protocols
            return protocols;
        }

    }

}
