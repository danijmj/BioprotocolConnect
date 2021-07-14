using BioprotocolConnect.ROs.Protocols.Models;
using BioprotocolConnect.ROs.Protocols.AppCore;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace BioprotocolConnect.ROs.Protocols
{

    public interface ROProtocolInterface
    {

        /// <summary>
        /// Method to get all protocol,
        /// This method will be implemented by the function
        /// </summary>
        /// <returns></returns>
        List<Protocol> GetListProtocols();

    }
}
