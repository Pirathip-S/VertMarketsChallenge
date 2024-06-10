

using System.Text.Json;


namespace VertMarketsServices
{
    internal static class JsonHelper
    {
        static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
      
        /// <summary>
        /// DeSerializeResponse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static T DeSerializeResponse<T>(string response ) => JsonSerializer.Deserialize<T>(response, options);
       
        /// <summary>
        /// SerializeJsonRequest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SerializeJsonRequest<T>(T content) => JsonSerializer.Serialize(content);
       
    }
}
