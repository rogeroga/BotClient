////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description:
//  SQL Server CLR Functions used in loading and extracting data for the analytics of the bootcamp school reviews. 
//
//  The GetReviews() function is used to parse the HTML content and return a table with a review in each row
//
//  The GetFiles() function retrieves the filenames and their information from a file directory
//
////////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using BotClient.Reviews;


namespace BotClient
{

    public class TableFunctions
    {

        [SqlFunction(FillRowMethodName = "FillReview")]
        public static IEnumerable GetReviews(string reviewsHtml)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(reviewsHtml);

            var reviews = doc.DocumentNode.QuerySelectorAll(".review").Select(node => ReviewParser.ProcessReview(node)).ToList();

            return reviews;
        }

        public static void FillReview(Object obj,
            out SqlInt32 ReviewId,
            out SqlDateTime ReviewDate,
            out SqlString ReviewTitle,
            out SqlString ReviewerName,
            out SqlString Review,
            out SqlString Response,

            out SqlString Campus,
            out SqlString Course,
            out SqlString DeepLinkPath,
            out SqlString DeepLinkTarget,

            out SqlDouble RateCurriculum,
            out SqlDouble RateInstructors,
            out SqlDouble RateJobAssistance,
            out SqlDouble RateOverallExperience
            )
        {
            ReviewRecord review = (ReviewRecord)obj;

            Campus = new SqlString(review.Header.Campus);
            Course = new SqlString(review.Header.Course);
            DeepLinkPath = new SqlString(review.Header.DeepLinkPath);
            DeepLinkTarget = new SqlString(review.Header.DeepLinkTarget);

            ReviewId = SqlInt32.Parse(review.ReviewId);
            ReviewDate = SqlDateTime.Parse(review.Date);
            ReviewTitle = new SqlString(review.Title);
            ReviewerName = new SqlString(review.ReviewerName);
            Review = new SqlString(review.Review);
            Response = new SqlString(review.Response);

            RateCurriculum = new SqlDouble(review.ReviewRatings.Curriculum);
            RateInstructors = new SqlDouble(review.ReviewRatings.Instructors);
            RateJobAssistance = new SqlDouble(review.ReviewRatings.JobAssistance);
            RateOverallExperience = new SqlDouble(review.ReviewRatings.OverallExperience);
        }



        [SqlFunction(FillRowMethodName = "FillFile")]
        public static IEnumerable GetFiles(string dirname)
        {
            DirectoryInfo info = new DirectoryInfo(dirname);

            return info.GetFiles();
        }

        public static void FillFile(Object obj, out SqlString name, out SqlDateTime creationTime, out SqlDateTime lastWriteTime)
        {
            FileInfo info = (FileInfo)obj;
            name = new SqlString(info.FullName);
            creationTime = new SqlDateTime(info.CreationTime);
            lastWriteTime = new SqlDateTime(info.LastWriteTime);
        }

    }

}
