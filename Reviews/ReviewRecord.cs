
namespace BotClient.Reviews
{
    public class ReviewRecord
    {
        public ReviewHeader Header { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string ReviewId { get; set; }
        public string ReviewerName { get; set; }
        public string Review { get; set; }
        public Ratings ReviewRatings { get; set; }
        public string Response { get; internal set; }
    }
}
