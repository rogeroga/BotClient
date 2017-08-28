////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:

//  Review parser, parses an HTML review, extracts the information and stores it in a ReviewRecord.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

using BotClient.Reviews;
using HtmlAgilityPack;
using System;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using System.IO;

namespace BotClient
{
    public class ReviewParser
    {

        public static ReviewRecord ProcessReview(HtmlNode node)
        {
            ReviewRecord reviewRecord = new ReviewRecord();

            ReviewHeader headerdata = new ReviewHeader();

            HtmlAttributeCollection attrs = attrs = node.Attributes;

            GetData(attrs, headerdata);

            attrs = node.QuerySelector(".review-title a").Attributes;

            GetData(attrs, headerdata);

            reviewRecord.Header = headerdata;

            reviewRecord.Title = node.QuerySelector(".review-title a").InnerHtml;

            reviewRecord.Date = node.QuerySelector(".review-date").InnerHtml;
            reviewRecord.ReviewId = CleanReviewId(headerdata.DeepLinkPath);
            reviewRecord.ReviewerName = node.QuerySelector(".reviewer-name").InnerText;
            reviewRecord.Review = GetReview(node);
            
            reviewRecord.ReviewRatings = GetRatings(node);

            reviewRecord.Response = GetRespone(node);
           

            return reviewRecord;
        }

        public static Ratings GetRatings(HtmlNode jq)
        {
            Ratings results = new Ratings();

            var rows = jq.QuerySelectorAll(".ratings .row").ToArray();
            int offSet = 0;

            foreach(HtmlNode elm in rows)
            {

                var divs = elm.QuerySelectorAll("div").ToArray();
                if (divs.Count() != 4)
                {
                    continue;
                }

                string firstName = CleanRatingName(divs[0 + offSet].InnerHtml);

                double firstvalue = GetStarCount(divs[1 + offSet]);

                string secondName = CleanRatingName(divs[2 + offSet].InnerHtml);

                double secondsvalue = GetStarCount(divs[3 + offSet]);

                Set(results, firstName, firstvalue);
                Set(results, secondName, secondsvalue);

            };

            return results;

        }

        private static void Set(Ratings results, string propName, double propValue)
        {
            switch (propName)
            {
                case "overallexperience":
                    results.OverallExperience = propValue;
                    break;

                case "instructors":
                    results.Instructors = propValue;
                    break;

                case "jobassistance":
                    results.JobAssistance = propValue;
                    break;
                case "curriculum":
                    results.Curriculum = propValue;
                    break;

                default:
                    break;
            }
        }

        private static string CleanRatingName(string innerHtml)
        {
            return innerHtml.Trim().Replace(" ","").ToLower().Replace(":","");
        }

        private static double GetStarCount(HtmlNode jq)
        {
            if(jq == null)
            {
                return -1;

            }

            return jq.QuerySelectorAll(".icon-full_star").Count();
        }

        private static string GetReview(HtmlNode node)
        {
            var review = node.QuerySelector(".body");

            Remove(review.QuerySelector(".review-body"));

            Remove(review.QuerySelector("button"));

            Remove(review.QuerySelector(".inappropriate"));


            RemoveStyles(review);
            
            return review.InnerHtml;
        }

        private static string GetRespone(HtmlNode fullReview)
        {
            var reviewResponse = fullReview.QuerySelector(".response-container");

            if (reviewResponse == null)
            {
                return null;
            }

            Remove(reviewResponse.QuerySelector("button"));
            Remove(reviewResponse.QuerySelector(".inappropriate"));

            RemoveStyles(reviewResponse);

            return reviewResponse.InnerHtml;
        }

        private static void RemoveStyles(HtmlNode review)
        {
            if(review == null || review.ChildNodes == null)
            {
                return;
            }

            foreach (var child in review.ChildNodes)
            {
                if (child.Attributes.Contains("style"))
                {
                    child.Attributes["style"].Remove();
                }

            }
        }

        private static void Remove(HtmlNode nde)
        {
          
            if (nde != null)
            {
                nde.Remove();
            }
        }

        private static string CleanReviewId(string sIn)
        {
            if (!String.IsNullOrEmpty(sIn))
            {
                //"#review_5741"
                string id = Path.GetFileName(sIn.TrimEnd(Path.DirectorySeparatorChar));
                return id;
            }
            else
            {
                return "-1";
            }
        }


        private static void GetData(HtmlAttributeCollection col, ReviewHeader header)
        {
            HtmlAttribute[] attris = col.Where(ha => ha.Name.ToLower().StartsWith("data-")).ToArray();

            //Dictionary<string, string> data = new Dictionary<string, string>();

            foreach (var item in attris)
            {
                string name = item.Name.ToLower().Replace("data-", "");

                switch (name)
                {
                    case "deeplink-target":
                        header.DeepLinkTarget = item.Value;
                        break;
                    case "deeplink-path":
                        header.DeepLinkPath = item.Value;
                        break;

                    case "campus":
                        header.Campus = item.Value;
                        break;
                    case "course":
                        header.Course = item.Value;
                        break;

                    default:
                        break;
                }
            }


        }
    }
}
