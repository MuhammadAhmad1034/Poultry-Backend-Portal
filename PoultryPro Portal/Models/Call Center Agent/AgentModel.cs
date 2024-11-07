using System.ComponentModel.DataAnnotations;

namespace PoultryPro_Portal.Models.Call_Center_Agent
{
    public class AgentModel
    {

        [Required]
        public string callAgentEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
