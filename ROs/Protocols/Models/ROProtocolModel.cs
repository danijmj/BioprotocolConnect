using System;
using System.Collections.Generic;

namespace BioprotocolConnect.ROs.Protocols.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Protocol
    {
        
        public string id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public List<User> authors { get; set; }
        public string doi { get; set; }
        public DateTime published { get; set; }
        public string publicationInformation { get; set; }
        public string abstract_ { get; set; }
        public List<string> keywords { get; set; }
        public string background { get; set; }
        public List<Listado> materials { get; set; }
        public List<Listado> equipment { get; set; }
        public List<Listado> software { get; set; }
        public List<Listado> procedure { get; set; }
        public string dataAnalysis { get; set; }
        public string notes { get; set; }
        public string recipies { get; set; }
        public string acknowledgments { get; set; }
        public string competingInterest { get; set; }
        public string ethics { get; set; }
        public List<Listado> references { get; set; }
        public List<Listado> categories { get; set; }
    }


    public class Listado
    {
        public string text { get; set; }
        public List<Listado> listado { get; set; }
    }


    public class User
    {
        public string name { get; set; }
        public string email { get; set; }
        public List<string> emails { get; set; }
        public DateTime date { get; set; }
        public string login { get; set; }
        public List<string> institutions { get; set; }
        public List<string> studies { get; set; }
        public List<string> reserarchFocus { get; set; }
        public List<string> reserarchFields { get; set; }
        public string id { get; set; }
        public string bioProtocolId { get; set; }
        public string orcid { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }


}
