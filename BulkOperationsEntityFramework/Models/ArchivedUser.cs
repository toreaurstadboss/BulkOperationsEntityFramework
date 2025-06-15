using BulkOperationsEntityFramework.Attributes;

namespace BulkOperationsEntityFramework.Models
{

    [Schema("Archive")]
    public class ArchivedUser
    {

        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

    }
}

