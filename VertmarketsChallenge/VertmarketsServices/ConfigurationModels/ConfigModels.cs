namespace VertMarketsServices.ConfigModels
{
    public class EndPointsOptions
    {
        public const string configSection = "EndPointsPaths";
        public string Token { get; set; }
        public string Categories { get; set; }
        public string CatogeriesMagazines { get; set; }
        public string Subscribers { get; set; }
        public string Answer { get; set; }
    }
    public class VertMarketsApiOptions
    {
        public const string configSection = "VertmarketsApi";
        public string Name { get; set; }
        public string BaseUrl { get; set; }
    }
}
