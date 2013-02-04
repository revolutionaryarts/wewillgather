using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Messages;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;
using Gather.Services.Users;

namespace Gather.Services.MessageQueues
{
    public class MessageQueueService : IMessageQueueService
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly HttpContextBase _httpContext;
        private readonly IRepository<MessageQueue> _messageQueueRepository;
        private readonly SiteSettings _siteSettings;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public MessageQueueService(CoreSettings coreSettings, HttpContextBase httpContext, IRepository<MessageQueue> messageQueueRepository, SiteSettings siteSettings, IUserService userService, IWebHelper webHelper)
        {
            _coreSettings = coreSettings;
            _httpContext = httpContext;
            _messageQueueRepository = messageQueueRepository;
            _siteSettings = siteSettings;
            _userService = userService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a item in the message queue
        /// </summary>
        /// <param name="message">Item in the message queue</param>
        public void DeleteQueuedMessage(MessageQueue message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            message.Deleted = true;
            _messageQueueRepository.Update(message);
        }

        /// <summary>
        /// Get a message by id
        /// </summary>
        /// <param name="id">Id of the message to retrieve</param>
        /// <returns>MessageQueue</returns>
        public MessageQueue GetMessageQueueById(int id)
        {
            if (id == 0)
                return null;

            var success = _messageQueueRepository.GetById(id);
            return success;
        }

        /// <summary>
        ///  Retrieves the message queue for processing
        /// </summary>
        /// <returns>MessageQueue</returns>
        public IList<MessageQueue> GetMessageQueueForMessageSend()
        {
            var query = _messageQueueRepository.Table;
            query = query.Where(s => !s.Deleted);
            query = query.Where(qe => !qe.SentOn.HasValue);
            query = query.Where(qe => qe.SentTries < 20);
            query = query.Where(qe => qe.User != null);
            query = query.OrderBy(qe => qe.Priority);

            var queue = query.ToList();
            return queue;
        }

        /// <summary>
        ///  Retrieves the message queue for twitting messages
        /// </summary>
        /// <returns>MessageQueue</returns>
        public IList<MessageQueue> GetMessageQueueForTwitterSend()
        {
            var query = _messageQueueRepository.Table;
            query = query.Where(s => !s.Deleted);
            query = query.Where(qe => !qe.SentOn.HasValue);
            query = query.Where(qe => qe.SentTries < 20);
            query = query.Where(qe => qe.User == null);
            query = query.OrderBy(qe => qe.Priority);

            var queue = query.ToList();
            return queue;
        }

        /// <summary>
        /// Inserts a message queue item
        /// </summary>
        /// <param name="message">Message to send</param>        
        public void InsertMessageQueue(MessageQueue message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            message.SentTries = 0;
            message.CreatedOn = DateTime.Now;

            // A last minute truncate if the message is too long to avoid a DB error
            // This shouldn't really ever happen as the lengths should be controlled when the message is created
            if (!string.IsNullOrEmpty(message.ShortBody))
                if (message.ShortBody.Length > 140)
                    message.ShortBody = message.ShortBody.Substring(0, 140);

            _messageQueueRepository.Insert(message);

            // Quick fix for short messages sent to twitter users, add the Id of the message into the message.
            if (!string.IsNullOrEmpty(message.ShortBody))
                message.ShortBody = string.Format(message.ShortBody, message.Id);

            _messageQueueRepository.Update(message);
        }

        /// <summary>
        /// Updates a item in the message queue
        /// </summary>
        /// <param name="message">Item in the message queue</param>
        public void UpdateQueuedMessage(MessageQueue message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _messageQueueRepository.Update(message);
        }

        #endregion

        #region Custom Messages

        /// <summary>
        /// Inserts a message for comment management
        /// </summary>
        /// <param name="comment">The comment to send a message about</param>
        /// <param name="messageType">The message type to send</param>
        /// <param name="createdBy">The user who requested moderation of the comment</param>
        /// <param name="authorMessage">A message to the author of the comment</param>
        /// <param name="userMessage">A message to the user who flagged the comment</param>        
        public void CommentMessage(Comment comment, MessageType messageType, int createdBy, string authorMessage = "", string userMessage = "")
        {
            if (comment == null)
                throw new ArgumentNullException("comment");

            string messageUrl = MessageUrl();

            switch (messageType)
            {
                case MessageType.CommentRemovalRejected:

                    var message = new MessageQueue
                    {
                        Priority = 30,
                        User = _userService.GetUserById(createdBy),
                        Subject = _siteSettings.TwitterHashTag + " - Comment Moderation",
                        ShortBody = "The comment you flagged has been reviewed " + messageUrl,
                        Body = WrapStringInHtml("The comment you flagged has been mulled over by a moderator and deemed acceptable.") + WrapStringInHtml(userMessage)
                    };
                    InsertMessageQueue(message);

                    break;
                case MessageType.CommentRemovalApproved:

                    var message2 = new MessageQueue
                    {
                        Priority = 30,
                        User = _userService.GetUserById(createdBy),
                        Subject = _siteSettings.TwitterHashTag + " - Comment Moderation",
                        ShortBody = "The comment you flagged has been removed " + messageUrl,
                        Body = WrapStringInHtml("The comment you flagged has been removed.") + WrapStringInHtml(userMessage)
                    };
                    InsertMessageQueue(message2);

                    //var message3 = new MessageQueue
                    //{
                    //    Priority = 30,
                    //    User = comment.Author,
                    //    Subject = message2.Subject = _hashTag + " - Comment Removal",
                    //    ShortBody = "Your comment has been removed from the site. " + messageUrl,
                    //    Body = WrapStringInHtml("Your comment '" + comment.UserComment + "' has been removed from the site.") + WrapStringInHtml("We have received complaints from the site users.") + WrapStringInHtml(authorMessage)
                    //};
                    //InsertMessageQueue(message3);

                    break;
            }
        }

        /// <summary>
        /// Inserts a message for project management
        /// </summary>
        /// <param name="project">The project to send a message about</param>
        /// <param name="messageType">The message type to send</param>
        /// <param name="authorMessage">Moderator comments to author</param>
        /// <param name="volunteerMessage">Moderator comments to volunteers</param>
        /// <param name="notifyVolunteers">Flag to denote if volunteers need to be notified about the project message</param>
        /// <param name="overrideSendTo">Override message sender</param>
        public void ProjectMessage(Project project, MessageType messageType, string authorMessage = "", string volunteerMessage = "", bool notifyVolunteers = true, int overrideSendTo = 0)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            string messageUrl = MessageUrl();
            string projectUrl = ProjectUrl(project.Id);

            foreach (var message in project.Owners.Select(owner => new MessageQueue { Priority = 10, User = owner }))
            {
                switch (messageType)
                {
                    case MessageType.ProjectApproved:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action Approval";
                        message.ShortBody = "Your good thing has been approved " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your good thing. " + project.Name + " has been approved.") + WrapStringInHtml(BuildLink(projectUrl)) + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectRejected:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action Rejection";
                        message.ShortBody = "Your good thing has been denied :-( " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your good thing - " + project.Name + " - has been denied :-(") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectChangeApproved:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action change approval";
                        message.ShortBody = "You wanted to change your good thing. We said yes " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("You wanted to change your good thing - " + project.Name + ". We said yes.") + WrapStringInHtml(BuildLink(projectUrl)) + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectChangeRejected:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action change rejected";
                        message.ShortBody = "You wanted to change your good thing, but we can't change it right now - sorry " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("You wanted to change your good thing - " + project.Name + ", but we can't change it right now - sorry.") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectModerationApproved:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action removed";
                        message.ShortBody = "Your good thing has been withdrawn because it was reported as a bad thing " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your good thing - " + project.Name + " - has been withdrawn because it was reported as a bad thing.") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectWithdrawalApproved:

                        message.Subject = _siteSettings.TwitterHashTag + " - Your good thing has been withdrawn";
                        message.ShortBody = "Your good thing has been withdrawn. Thank you, come again soon " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your good thing - " + project.Name + " - has been withdrawn. Thank you, come again soon.") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectWithdrawalRejected:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action Withdrawal Rejected";
                        message.ShortBody = "Your action withdrawal request has been rejected " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your action withdrawal request - " + project.Name + ", has been rejected. We're keeping your good thing live on the site.") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectDisputeRejected:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action Dispute Rejected";
                        message.ShortBody = "Your action dispute request has been rejected " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your action dispute request - " + project.Name + ", has been rejected, you are still not an owner of this project.") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectDisputeApproved:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action Dispute Approved";
                        message.ShortBody = "Your action dispute request has been approved " + (!string.IsNullOrEmpty(authorMessage) ? messageUrl : projectUrl);
                        message.Body = WrapStringInHtml("Your action dispute request - " + project.Name + ", has been approved. We have reinstated you as an owner.") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectDisputeOwnerRemoved:

                        message.Subject = _siteSettings.TwitterHashTag + " - Action Dispute Owner Removed";
                        message.ShortBody = "You have been removed from \"" + project.Name + "\" due to a dispute " + messageUrl;
                        message.Body = WrapStringInHtml("You have been removed from - " + project.Name + ".") + WrapStringInHtml(authorMessage);

                        break;
                    case MessageType.ProjectRecurrenceScheduled:

                        message.Subject = _siteSettings.TwitterHashTag + " - Project Recurrence Scheduled";
                        message.ShortBody = "A good thing you organised is a recurring action, can you make the next one? " + projectUrl;
                        message.Body = WrapStringInHtml("A good thing you have helped to organised, \"" + project.Name + "\", is a recurring event. The good thing has now finished, can you make the next occurrence? You can check the new start time at: " + projectUrl);

                        break;
                }

                // Do we have a message to send.
                if (!string.IsNullOrEmpty(message.Body))
                    InsertMessageQueue(message);
            }

            // In some circumstances, we need to send a message to the user who reported the project as well.
            if (overrideSendTo > 0)
            {
                var message2 = new MessageQueue
                {
                    Priority = 10,
                    User = _userService.GetUserById(overrideSendTo)
                };

                switch (messageType)
                {
                    case MessageType.ProjectModerationApproved:

                        message2.Subject = _siteSettings.TwitterHashTag + " - Project Removed";
                        message2.ShortBody = "The action you reported has been removed " + messageUrl;
                        message2.Body = WrapStringInHtml("Reported action - " + project.Name + ". This project has been removed, thank you for reporting the project.");

                        break;
                    case MessageType.ProjectModerationRejected:

                        message2.Subject = _siteSettings.TwitterHashTag + " - Project Moderation";
                        message2.ShortBody = "We have reviewed the action you reported " + messageUrl;
                        message2.Body = WrapStringInHtml("Reported good thing - " + project.Name + ". We have reviewed the action and we are happy with the content.");

                        break;
                }

                if (!string.IsNullOrEmpty(message2.Body))
                    InsertMessageQueue(message2);
            }

            // By default we notify volunteers, we send a message to any volunteer isn't already a product owner.
            if (notifyVolunteers)
            {
                var volunteers = project.Volunteers.Where(volunteer => project.Owners.All(x => x.Id != volunteer.Id)).ToList();
                if (volunteers.Count > 0)
                {
                    foreach (var message in volunteers.Select(user => new MessageQueue { Priority = 20, User = user }))
                    {
                        switch (messageType)
                        {
                            case MessageType.ProjectChangeApproved:

                                message.Subject = _siteSettings.TwitterHashTag + " - " + project.Name + " : Action Update";
                                message.ShortBody = "Changes have been made to a good thing you signed up for " + messageUrl;
                                message.Body = WrapStringInHtml(project.Name + ". Changes have been made to a good thing you signed up for.") + WrapStringInHtml(BuildLink(projectUrl)) + WrapStringInHtml(volunteerMessage);

                                break;
                            case MessageType.ProjectModerationApproved:

                                message.Subject = _siteSettings.TwitterHashTag + " - " + project.Name + " : Action Withdrawn";
                                message.ShortBody = "A good thing you signed up for has sadly had to be withdrawn by its creator " + messageUrl;
                                message.Body = WrapStringInHtml(project.Name + ". A good thing you signed up for has sadly had to be withdrawn by its creator.") + WrapStringInHtml(volunteerMessage);

                                break;
                            case MessageType.ProjectWithdrawalApproved:

                                message.Subject = _siteSettings.TwitterHashTag + " - " + project.Name + " : Action Withdrawn";
                                message.ShortBody = "A good thing you signed up for has been cancelled " + messageUrl;
                                message.Body = WrapStringInHtml(project.Name + ". A good thing you signed up for has been cancelled.") + WrapStringInHtml(volunteerMessage);

                                break;
                            case MessageType.ProjectRecurrenceScheduled:

                                message.Subject = _siteSettings.TwitterHashTag + " - Project Recurrence Scheduled";
                                message.ShortBody = "A good thing you volunteered for is a recurring event, can you make the next one? " + projectUrl;
                                message.Body = WrapStringInHtml("A good thing you have signed up for, \"" + project.Name + "\", is a recurring action. The good thing has now finished, can you make the next occurrence? You can check the new start time at: " + projectUrl);

                                break;
                        }

                        if (!string.IsNullOrEmpty(message.Body))
                            InsertMessageQueue(message);
                    }
                }
            }
        }

        /// <summary>
        /// Project starting message (Called from scheduled task running on a thread)
        /// </summary>
        /// <param name="project">the project to update</param>
        public void ProjectStartingMessage(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            string messageUrl = MessageUrl();
            string projectUrl = ProjectUrl(project.Id);

            // Send a message to the project owners
            foreach (var message in project.Owners.Select(owner => new MessageQueue { Priority = 10, User = owner }))
            {
                message.Subject = _siteSettings.TwitterHashTag + " - " + project.Name + " : The action you setup is tomorrow";
                message.ShortBody = "The action you setup is tomorrow " + projectUrl;
                message.Body = WrapStringInHtml("The good thing you are helping to organise is just around the corner, \"" + project.Name + "\" is starting on " + project.StartDate) + WrapStringInHtml(BuildLink(projectUrl));

                InsertMessageQueue(message);
            }

            // We send a different message to any volunteer who isn't already a product owner.
            var volunteers = project.Volunteers.Where(volunteer => project.Owners.All(x => x.Id != volunteer.Id)).ToList();
            if (volunteers.Count > 0)
            {
                foreach (var message in volunteers.Select(user => new MessageQueue { Priority = 20, User = user }))
                {
                    message.Subject = _siteSettings.TwitterHashTag + " - " + project.Name + " : The action you joined is tomorrow";
                    message.ShortBody = "The action you joined is tomorrow " + messageUrl;
                    message.Body = WrapStringInHtml("The good thing you signed up for is just around the corner, \"" + project.Name + "\" is starting on " + project.StartDate) + WrapStringInHtml(BuildLink(projectUrl));

                    InsertMessageQueue(message);
                }
            }
        }

        /// <summary>
        /// Insert the Project temporary message
        /// </summary>
        /// <param name="user">The twitter username</param>
        /// <param name="profile">The twitter profile</param>
        /// <param name="projectId">The project Id</param>
        public void ProjectTemporaryMessage(string user, string profile, int projectId)
        {
            // Send out response tweet
            TweetMessage(string.Format("We've started a page for your action, please tell us more so we can share it {0}", ProjectTemporaryUrl(projectId)), profile, user);
        }

        /// <summary>
        /// Messaging service for tweeting about projects
        /// </summary>
        /// <param name="project">The project to send out</param>
        /// <param name="messageType">The type of message to send</param>
        /// <returns></returns>
        public bool ProjectTweet(Project project, MessageType messageType)
        {
            string projectUrl = ProjectUrl(project.Id);
            string startDate = project.StartDate != null ? " on " + _webHelper.DateTimeFormat("dd~ MMMM", project.StartDate.Value) : "";

            string message;
            switch (messageType)
            {
                case MessageType.TweetProjectApproved:
                    message = string.Format("An event has been created in {0}{1} {2} {3}", project.Locations.First(x => x.Primary).Location.Name, startDate, projectUrl, _siteSettings.TwitterHashTag);
                    if (project.Categories.Count > 0)
                    {
                        string catName = project.Categories.First().Name.ToLower();
                        string catPrefix = catName == "meeting" ? "Join a" : "Do some";
                        string catMessage = string.Format("{0} {1} in {2}{3} {4} {5}", catPrefix, catName, project.Locations.First(x => x.Primary).Location.Name, startDate, projectUrl, _siteSettings.TwitterHashTag);
                        message = catMessage.Length > 140 ? message : catMessage;
                    }
                    InsertMessageQueue(new MessageQueue { ShortBody = message, Priority = 10 });
                    return true;
                case MessageType.TweetProjectMoreVolunteers:
                    int remainingNumberOfVolunteers = project.NumberOfVolunteers - project.Volunteers.Count;
                    message = string.Format("We need {0} more {1} for this good thing in {2} {3}", remainingNumberOfVolunteers, (remainingNumberOfVolunteers > 1 ? "people" : "person"), project.Locations.First(x => x.Primary).Location.Name, projectUrl);
                    InsertMessageQueue(new MessageQueue { ShortBody = message, Priority = 1 });
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sends a single message to a user
        /// </summary>
        /// <param name="project">The project to send a message about</param>
        /// <param name="messageType">he message type to send</param>
        /// <param name="user">User to send the message to</param>
        /// <param name="authorMessage">Moderator comments to author</param>
        public void ProjectUserMessage(Project project, MessageType messageType, User user, string authorMessage = "")
        {
            string messageUrl = MessageUrl();
            var message = new MessageQueue
            {
                Priority = 10,
                User = user
            };

            switch (messageType)
            {
                case MessageType.ProjectDisputeRejected:

                    message.Subject = _siteSettings.TwitterHashTag + " - Action Dispute Rejected";
                    message.ShortBody = "Your action dispute request has been rejected. " + messageUrl;
                    message.Body = WrapStringInHtml("Your action dispute request - " + project.Name + ", has been rejected, you are still not an owner of this project.") + WrapStringInHtml(authorMessage);

                    break;
                case MessageType.ProjectDisputeApproved:

                    message.Subject = _siteSettings.TwitterHashTag + " - Action Dispute Approved";
                    message.ShortBody = "Your action dispute request has been approved. " + messageUrl;
                    message.Body = WrapStringInHtml("Your action dispute request - " + project.Name + ", has been approved. We have reinstated you as an owner.") + WrapStringInHtml(authorMessage);

                    break;
                case MessageType.ProjectDisputeOwnerRemoved:

                    message.Subject = _siteSettings.TwitterHashTag + " - Action Dispute Owner Removed";
                    message.ShortBody = "You have been removed from - " + project.Name + " due to a dispute." + messageUrl;
                    message.Body = WrapStringInHtml("You have been removed from - " + project.Name + ".") + WrapStringInHtml(authorMessage);

                    break;
            }

            if (!string.IsNullOrEmpty(message.Body))
                InsertMessageQueue(message);
        }

        /// <summary>
        /// Tweets a message from the site
        /// </summary>
        /// <param name="tweet">content of the tweet</param>
        /// <param name="twitterProfile">Override Twitter Profile</param>
        /// <param name="twitterUsername">Override Twitter Name</param>
        public bool TweetMessage(string tweet = "", string twitterProfile = "", string twitterUsername = "")
        {
            if (tweet.Length > 140)
                return false;

            InsertMessageQueue(new MessageQueue
            {
                Priority = 20,
                ShortBody = tweet,
                TwitterProfile = twitterProfile,
                TwitterUsername = twitterUsername
            });
            return true;
        }

        #endregion

        #region Message Helpers

        private static string WrapStringInHtml(string text)
        {
            if (!string.IsNullOrEmpty(text))
                return "<p>" + text + "</p>";
            return "";
        }

        #endregion

        #region Url Helpers

        private static string BuildLink(string url, string linkText = "Link to Action")
        {
            return "<a href=\"" + url + "\">" + linkText + "</a>";
        }

        private string MessageUrl()
        {
            var urlHelper = new UrlHelper(_httpContext.Request.RequestContext);
            return _coreSettings.Domain.TrimEnd('/') + urlHelper.RouteUrl("Messages", new { Id = "0" }).Replace("0", "{0}");
        }

        private string ProjectTemporaryUrl(int id)
        {
            var urlHelper = new UrlHelper(_httpContext.Request.RequestContext);
            return _coreSettings.Domain.TrimEnd('/') + urlHelper.RouteUrl("AddTemporaryProject", new { Id = id });
        }

        private string ProjectUrl(int projectId)
        {
            var urlHelper = new UrlHelper(_httpContext.Request.RequestContext);
            return _coreSettings.Domain.TrimEnd('/') + urlHelper.RouteUrl("ProjectDetailTiny", new { Id = projectId });
        }

        #endregion

    }
}