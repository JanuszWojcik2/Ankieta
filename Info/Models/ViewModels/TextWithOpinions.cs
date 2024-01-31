namespace Info.Models.ViewModels
{
    public class TextWithOpinions
    {
        public Text SelectedText { get; set; }
        public int ReadingTime { get; set; }
        public int CommentsNumber { get; set; }
        public int OpinionsNumber { get; set; }
        public float AverageScore { get; set; }
        public string Description { get; set; }
        public Opinion NewOpinion { get; set; }
        

        public TextWithOpinions()
        {
            ReadingTime = 0; 
            CommentsNumber = 0;
            OpinionsNumber = 0;
            AverageScore = 0f;
        }
    }
}
