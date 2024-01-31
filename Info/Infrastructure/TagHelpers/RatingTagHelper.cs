using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Info.Infrastructure.TagHelpers
{
    [HtmlTargetElement("star-rating")]
    public class RatingTagHelper : TagHelper
    {
        public int RatingCount { get; set; }
        public double? RatingAvg { get; set; }
        private string ocena = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "text-warning");
            var strSb = new StringBuilder();

            if (RatingAvg == 0)
            {
                ocena = EmptyStar(5);
                strSb.AppendFormat(ocena);
            }
            else if (RatingCount == 1)
            {
                int full = (int)(RatingAvg ?? 0);
                ocena = FullStar(full);
                ocena=EmptyStar(5-full);
                strSb.AppendFormat(ocena);
            }
            else
            {
                double value = Math.Round((RatingAvg ?? 0) * 2, 0, MidpointRounding.AwayFromZero);
                int full = (int)Math.Truncate(value / 2);
                ocena = FullStar(full);
                if (value % 2 != 0)
                {
                    ocena = HalfStar();
                    full = full + 1;
                }
                ocena = EmptyStar(5 - full);
                strSb.AppendFormat(ocena);
            }
            output.PreContent.SetHtmlContent(strSb.ToString());
        }

        private string EmptyStar(int n)
        {
            for (int i = 1; i <= n; i++)
            {
                ocena += "<i class=\"bi-star me-1\" ></i>";
            }
            return ocena;
        }
        private string HalfStar()
        {
            ocena += "<i class=\"bi-star-half me-1\"></i>";
            return ocena;
        }
        private string FullStar(int n)
        {
            for (int i = 1; i <= n; i++)
            {
                ocena += "<i class=\"bi-star-fill me-1\"></i>";
            }
            return ocena;
        }
    }
}
