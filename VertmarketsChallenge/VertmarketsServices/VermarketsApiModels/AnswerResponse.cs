
namespace VertMarketsServices.VermarketsApiModels
{
    internal class AnswerResponse
    {
        public string TotalTime { get; set; }
        public bool AnswerCorrect { get; set; }
        public string[] ShouldBe { get; set; }
    }
}
