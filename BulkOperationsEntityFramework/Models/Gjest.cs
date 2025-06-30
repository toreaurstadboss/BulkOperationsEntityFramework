using BulkOperationsEntityFramework.Attributes;

namespace BulkOperationsEntityFramework.Models
{

    [Schema("Arkiv")]
    public class Gjest
    {

        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}

