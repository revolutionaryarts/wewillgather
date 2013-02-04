using System.Collections.Generic;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Messages;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;

namespace Gather.Services.MessageQueues
{
    public interface IMessageQueueService
    {

        #region Methods

        /// <summary>
        /// Deletes a item in the message queue
        /// </summary>
        /// <param name="message">Item in the message queue</param>
        void DeleteQueuedMessage(MessageQueue message);

        /// <summary>
        /// Get a message by id
        /// </summary>
        /// <param name="id">Id of the message to retrieve</param>
        /// <returns>MessageQueue</returns>
        MessageQueue GetMessageQueueById(int id);

        /// <summary>
        ///  Retrieves the message queue for processing
        /// </summary>
        /// <returns>MessageQueue</returns>
        IList<MessageQueue> GetMessageQueueForMessageSend();

        /// <summary>
        ///  Retrieves the message queue for twitting messages
        /// </summary>
        /// <returns>MessageQueue</returns>
        IList<MessageQueue> GetMessageQueueForTwitterSend();

        /// <summary>
        /// Inserts a message queue item
        /// </summary>
        /// <param name="message">Message to send</param>        
        void InsertMessageQueue(MessageQueue message);

        /// <summary>
        /// Updates a item in the message queue
        /// </summary>
        /// <param name="message">Item in the message queue</param>
        void UpdateQueuedMessage(MessageQueue message);

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
        void CommentMessage(Comment comment, MessageType messageType, int createdBy, string authorMessage = "", string userMessage = "");

        /// <summary>
        /// Inserts a message for project management
        /// </summary>
        /// <param name="project">The project to send a message about</param>
        /// <param name="messageType">The message type to send</param>
        /// <param name="authorMessage">Moderator comments to author</param>
        /// <param name="volunteerMessage">Moderator comments to volunteers</param>
        /// <param name="notifyVolunteers">Flag to denote if volunteers need to be notified about the project message</param>
        /// <param name="overrideSendTo">Override message sender</param>
        void ProjectMessage(Project project, MessageType messageType, string authorMessage = "", string volunteerMessage = "", bool notifyVolunteers = true, int overrideSendTo = 0);

        /// <summary>
        /// Project starting message
        /// </summary>
        /// <param name="project">the project to update</param>
        void ProjectStartingMessage(Project project);

        /// <summary>
        /// Insert the Project temporary message
        /// </summary>
        /// <param name="user">The twitter username</param>
        /// <param name="profile">The twitter profile</param>
        /// <param name="projectId">The project Id</param>
        void ProjectTemporaryMessage(string user, string profile, int projectId);

        /// <summary>
        /// Messaging service for tweeting about projects
        /// </summary>
        /// <param name="project">The project to send out</param>
        /// <param name="messageType">The type of message to send</param>
        /// <returns></returns>
        bool ProjectTweet(Project project, MessageType messageType);

        /// <summary>
        /// Sends a single message to a user
        /// </summary>
        /// <param name="project">The project to send a message about</param>
        /// <param name="messageType">he message type to send</param>
        /// <param name="user">User to send the message to</param>
        /// <param name="authorMessage">Moderator comments to author</param>
        void ProjectUserMessage(Project project, MessageType messageType, User user, string authorMessage = "");

        /// <summary>
        /// Tweets a message from the site
        /// </summary>
        /// <param name="tweet">content of the tweet</param>
        /// <param name="twitterProfile">Override Twitter Profile</param>
        /// <param name="twitterUsername">Override Twitter Name</param>
        bool TweetMessage(string tweet = "", string twitterProfile = "", string twitterUsername = "");

        #endregion

    }
}