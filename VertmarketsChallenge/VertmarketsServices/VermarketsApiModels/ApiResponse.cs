

namespace VertMarketsServices.VermarketsApiModels
{
    internal class ApiResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
    internal class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Token { get; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
