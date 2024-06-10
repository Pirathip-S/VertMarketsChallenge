
using Microsoft.Extensions.Options;
using VertMarketsServices.ConfigModels;
using VertMarketsServices.Interfaces;
using VertMarketsServices.VermarketsApiModels;
using System.Text;
using Polly;
using Microsoft.Extensions.Caching.Memory;


namespace VertMarketsServices.Services
{
    public class VertMarketsService : IVertMarketsService
    {
        private readonly EndPointsOptions _endPointsValue;
        private readonly VertMarketsApiOptions _vertMarketsApiValue;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        private const string _token = "{token}";
        private const string _category = "{category}";

        private ApiResponse? token;
        private List<Tuple<string, string>>? _subscriberCategory;
        private string[]? _allCategorySubscribers;

        /// <summary>
        /// VertMarketsService
        /// </summary>
        /// <param name="endPointOptions"></param>
        /// <param name="vertMarketsApiOptions"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="cache"></param>

        public VertMarketsService(IOptionsMonitor<EndPointsOptions> endPointOptions, IOptionsMonitor<VertMarketsApiOptions> vertMarketsApiOptions, IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _endPointsValue = endPointOptions.CurrentValue;
            _vertMarketsApiValue = vertMarketsApiOptions.CurrentValue;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }
        /// <summary>
        /// IdentifySubscribersWhoSubscribedAllCatogories
        /// </summary>
        /// <returns></returns>
        public async Task<string> IdentifySubscribersWhoSubscribedAllCategories()
        {
            try
            {
                await SetToken();
                if (token?.Success ?? false)
                {
                    var categories = await GetApiResponse<ApiResponse<IEnumerable<string>>>(_endPointsValue.Categories);
                    var subscribers = await GetApiResponse<ApiResponse<IEnumerable<ApiSubscriber>>>(_endPointsValue.Subscribers);
                    _subscriberCategory = _subscriberCategory ?? [];
                    foreach (var category in categories.Data)
                    {                        
                        var res = await GetSubscriberCategory(category, subscribers.Data);
                        _subscriberCategory.AddRange(res);
                    }
                    _allCategorySubscribers = _subscriberCategory?.GroupBy(x => x.Item1).Where(y => y.Count() == categories.Data.Count()).Select(a => a.Key).ToArray();
                    return await GetAnswers(_allCategorySubscribers);
                }
                else
                    return token?.Message ?? "Token Generation Error try again after some time";

            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string> GetAnswers(string[]? subscribers)
        {
            var answer = new Answer { Subscribers = subscribers ??  [""] };
            string jsonData = JsonHelper.SerializeJsonRequest(answer);
            return await PostApiRequest(jsonData);
        }
        private async Task<List<Tuple<string, string>>> GetSubscriberCategory(string category, IEnumerable<ApiSubscriber> subscribers)
        {
            var magzineResponse = await GetApiResponse<ApiResponse<IEnumerable<Magazine>>>(_endPointsValue.CatogeriesMagazines.Replace(_category, category));
            
             var result = subscribers.Where(sub => sub.MagazineIds
                                    .Any(x => magzineResponse.Data.Select(a => a.Id).Contains(x)))
                            .Select(sub => new Tuple<string, string>(sub.Id, category)).ToList();
            return result;
        }
        private async Task SetToken()
        {
            var response = await GetApiResponse<ApiResponse>(_endPointsValue.Token);            
            if (response is null)
            {
                token = null;
                throw new Exception("Unable to retrive token");
            }
            token = response;
        }

        private async Task<T> GetApiResponse<T>(string endPoint)
        {
            var cacheKey = endPoint.Replace(_token, "").Replace("/", "_");
            var status = _cache.TryGetValue(cacheKey, out T cachedData);
            if (status && cachedData is not null )
            {
                return cachedData;
            }

            // Define a policy for retrying HTTP requests
            var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)            
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(200));

            var client = _httpClientFactory.CreateClient(_vertMarketsApiValue.Name);
            var response = await retryPolicy.ExecuteAsync(async () => await client.GetAsync($"{_vertMarketsApiValue.BaseUrl}{endPoint.Replace(_token, token?.Token)}"));
            var result = JsonHelper.DeSerializeResponse<T>(await response.Content.ReadAsStringAsync());
            var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(15));
            _cache.Set(cacheKey, result, cacheOptions);
            return result;
        }

        private async Task<string> PostApiRequest(string jsonData)
        {
            await SetToken();
            var client = _httpClientFactory.CreateClient(_vertMarketsApiValue.Name);
            // Create StringContent with JSON data
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_vertMarketsApiValue.BaseUrl}{_endPointsValue.Answer.Replace(_token, token?.Token)}", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
