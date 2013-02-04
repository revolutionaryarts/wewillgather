using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Gather.Core.Domain.Tweets;
using Gather.Core.Infrastructure;
using Gather.Services.Logging;
using Gather.Services.MessageQueues;
using Gather.Services.Settings;
using Gather.Services.Tasks;
using Gather.Core.Domain.Common;
using Gather.Services.Tweets;

namespace Gather.Services.Projects.Tools
{
    public class TwitterMonitorTask : ITask
    {

        public const string POSTCODE_REGEX = "(GIR 0AA)|(((A[BL]|B[ABDHLNRSTX]?|C[ABFHMORTVW]|D[ADEGHLNTY]|E[HNX]?|F[KY]|G[LUY]?|H[ADGPRSUX]|I[GMPV]|JE|K[ATWY]|L[ADELNSU]?|M[EKL]?|N[EGNPRW]?|O[LX]|P[AEHLOR]|R[GHM]|S[AEGKLMNOPRSTY]?|T[ADFNQRSW]|UB|W[ADFNRSV]|YO|ZE)[1-9]?[0-9]|((E|N|NW|SE|SW|W)1|EC[1-4]|WC[12])[A-HJKMNPR-Y]|(SW|W)([2-9]|[1-9][0-9])|EC[1-9][0-9])( {0,1}[0-9][ABD-HJLNP-UW-Z]{2})?)";
        public const int TWEET_PER_PAGE = 15;

        public void Execute()
        {
            // Initialize engine
            EngineContext.Initialize(false);

            // Resolve required services
            var projectService = EngineContext.Current.Resolve<IProjectService>();
            var messageQueueService = EngineContext.Current.Resolve<IMessageQueueService>();

            try
            {
                List<Result> fullResultList = GetAllResults();
                for (int i = fullResultList.Count - 1; i >= 0; i--)
                {
                    var projectResult = fullResultList[i];

                    // Insert the project object
                    var projectId = projectService.InsertTwitterProject(projectResult.Postcode, projectResult.Text, projectResult.FromUserIdStr);

                    // Queue a response tweet to the user who tweeted
                    messageQueueService.ProjectTemporaryMessage(projectResult.FromUser, projectResult.FromUserIdStr, projectId);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private List<Result> GetAllResults()
        {
            var resultList = new List<Result>();

            try
            {
                using (var client = new WebClient())
                {
                    var coreSettings = EngineContext.Current.Resolve<CoreSettings>();
                    var siteSettings = EngineContext.Current.Resolve<SiteSettings>();

                    string downloadString = string.Format("http://search.twitter.com/search.json?q={0}-filter:retweets&rpp={1}&include_entities=true&with_twitter_user_id=true&result_type=recent&since_id={2}", HttpUtility.UrlEncode(siteSettings.TwitterQuery), TWEET_PER_PAGE, coreSettings.LastTweetId);
                    bool hasResults = true;
                    int page = 1;
                    var postcodeRegex = new Regex(POSTCODE_REGEX);

                    while (hasResults)
                    {
                        string jsonData = client.DownloadString(downloadString + "&page=" + page);
                        var serializer = new DataContractJsonSerializer(typeof (RootObject));
                        using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonData)))
                        {
                            var dataObject = (RootObject)serializer.ReadObject(ms);
                            if (dataObject != null && dataObject.Results.Count > 0)
                            {
                                if (page == 1)
                                    UpdateTweets(dataObject.Results);

                                foreach (var result in dataObject.Results)
                                {
                                    string upperText = result.Text.ToUpper();
                                    if (upperText.Contains("HELP"))
                                    {
                                        var match = postcodeRegex.Match(upperText);
                                        while (match.Success && (match.Index != 0 && result.Text.Substring(match.Index - 1, 1) != " "))
                                        {
                                            match = postcodeRegex.Match(upperText, match.Index + 1);
                                        }

                                        if (match.Success)
                                        {
                                            int length = (result.Text.IndexOf(" ", match.Index) == -1 ? result.Text.Length : result.Text.IndexOf(" ", match.Index)) - match.Index;
                                            result.Postcode = match.Value;
                                            if (length > match.Length)
                                            {
                                                result.Postcode = result.Text.Substring(match.Index, length);
                                                result.Text = result.Text.Remove(match.Index, length).Replace(siteSettings.TwitterQuery, "").Trim();
                                            }
                                            else
                                            {
                                                result.Text = result.Text.Remove(match.Index, match.Length).Replace(siteSettings.TwitterQuery, "").Trim();
                                            }
                                            resultList.Add(result);
                                        }
                                        else
                                        {
                                            result.Text = result.Text.Replace(siteSettings.TwitterQuery, "").Trim();
                                            resultList.Add(result);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                hasResults = false;
                            }
                        }
                        page++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return resultList;
        }

        private void UpdateTweets(List<Result> results)
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var tweetService = EngineContext.Current.Resolve<ITweetService>();

            for (int i = results.Count - 1; i >= 0; i--)
            {
                tweetService.InsertTweet(new Tweet
                {
                    CreatedDate = DateTime.Parse(results[i].CreatedAt),
                    Text = results[i].Text,
                    TwitterId = results[i].Id,
                    TwitterProfile = results[i].FromUserIdStr,
                    TwitterName = results[i].FromUser,
                    UserName = results[i].FromUserName
                });

                var setting = settingService.GetSettingByName<CoreSettings>("LastTweetId");
                setting.Value = results[i].IdStr;
                settingService.UpdateSetting(setting, true);
            }

            tweetService.DeleteOldTweets();
        }

        private void LogException(Exception exc)
        {
            if (exc == null)
                return;

            try
            {
                var logger = EngineContext.Current.Resolve<ILogService>();
                logger.Error(exc.Message, exc);
            }
            catch { }
        }
    }

    [DataContract]
    public class Hashtag
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "indices")]
        public List<int> Indices { get; set; }
    }

    [DataContract]
    public class Url
    {
        [DataMember(Name = "url")]
        public string UrlName { get; set; }

        [DataMember(Name = "expanded_url")]
        public string ExpandedUrl { get; set; }

        [DataMember(Name = "display_url")]
        public string DisplayUrl { get; set; }

        [DataMember(Name = "indices")]
        public List<int> Indices { get; set; }
    }

    [DataContract]
    public class UserMention
    {
        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "indices")]
        public List<int> Indices { get; set; }
    }

    [DataContract]
    public class Entities
    {
        [DataMember(Name = "hashtags")]
        public List<Hashtag> Hashtags { get; set; }

        [DataMember(Name = "urls")]
        public List<Url> Urls { get; set; }

        [DataMember(Name = "user_mentions")]
        public List<UserMention> UserMentions { get; set; }
    }

    [DataContract]
    public class Metadata
    {
        [DataMember(Name = "result_type")]
        public string ResultType { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; set; }

        [DataMember(Name = "from_user")]
        public string FromUser { get; set; }

        [DataMember(Name = "from_user_id")]
        public long FromUserId { get; set; }

        [DataMember(Name = "from_user_id_str")]
        public string FromUserIdStr { get; set; }

        [DataMember(Name = "from_user_name")]
        public string FromUserName { get; set; }

        [DataMember(Name = "geo")]
        public object Geo { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "iso_language_code")]
        public string IsoLanguageCode { get; set; }

        [DataMember(Name = "metadata")]
        public Metadata Metadata { get; set; }

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "to_user")]
        public string ToUser { get; set; }

        [DataMember(Name = "to_user_id")]
        public long ToUserId { get; set; }

        [DataMember(Name = "to_user_id_str")]
        public string ToUserIdStr { get; set; }

        [DataMember(Name = "to_user_name")]
        public string ToUserName { get; set; }

        [DataMember(Name = "in_reply_to_status_id")]
        public long InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_status_id_str")]
        public string InReplyToStatusIdStr { get; set; }

        public string Postcode { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember(Name = "completed_in")]
        public double CompletedIn { get; set; }

        [DataMember(Name = "max_id")]
        public string MaxId { get; set; }

        [DataMember(Name = "max_id_str")]
        public string MaxIdStr { get; set; }

        [DataMember(Name = "next_page")]
        public string NextPage { get; set; }

        [DataMember(Name = "page")]
        public int Page { get; set; }

        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "refresh_url")]
        public string RefreshUrl { get; set; }

        [DataMember(Name = "results")]
        public List<Result> Results { get; set; }

        [DataMember(Name = "results_per_page")]
        public int ResultsPerPage { get; set; }

        [DataMember(Name = "since_id")]
        public string SinceId { get; set; }

        [DataMember(Name = "since_id_str")]
        public string SinceIdStr { get; set; }
    }

}