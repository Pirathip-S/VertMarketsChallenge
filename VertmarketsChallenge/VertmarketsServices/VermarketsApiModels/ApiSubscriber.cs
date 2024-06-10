

namespace VertMarketsServices.VermarketsApiModels
{
    internal class ApiSubscriber
    {
        public string Id { get; set; }
        public string FirstName { get; }
        public string LastName { get; set; }
        public int[] MagazineIds { get; set; }
    }
}
