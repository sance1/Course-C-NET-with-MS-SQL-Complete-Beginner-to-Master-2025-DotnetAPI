namespace DotnetApi.Models;

public partial class PostToAddDto

{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string PostTitle { get; set; }
    public string PostContent { get; set; }
    public DateTime PostCreated { get; set; }
    public DateTime LastUpdated { get; set; }

    public PostToAddDto()
    {
        if(PostTitle == null)
        {
            PostTitle = "";
        }
        if(PostContent == null)
        {
            PostContent = "";
        }
    }
}