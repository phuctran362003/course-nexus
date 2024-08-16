namespace Curus.Repository.ViewModels.Response;


public class ViewReviewRespone
{
    public List<UnHideReviewRespone> UnHideReviewRespones { get; set; }
    public List<HideReviewRespone> HideReviewRespones { get; set; }
}

public class UnHideReviewRespone
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public bool IsGoodFeedBack { get; set; }
}

public class HideReviewRespone
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public string ReasonHide { get; set; }
}