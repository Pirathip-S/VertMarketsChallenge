
namespace VertMarketsServices.Interfaces
{
    public interface IVertMarketsService
    {
        /// <summary>
        /// IdentifySubscribersWhoSubscribedAllCategories
        /// </summary>
        /// <returns></returns>
        Task<string> IdentifySubscribersWhoSubscribedAllCategories();
    }
}
