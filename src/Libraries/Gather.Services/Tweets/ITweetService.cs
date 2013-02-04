using System.Collections.Generic;
using Gather.Core.Domain.Tweets;

namespace Gather.Services.Tweets
{
    public interface ITweetService
    {
        /// <summary>
        /// Delete all but a certain amount of tweets
        /// </summary>
        /// <param name="amount">Amount to reduce the table to</param>
        void DeleteOldTweets(int amount = 15);

        /// <summary>
        /// Gets all of the tweets above a certain id or all of them
        /// </summary>
        /// <param name="ascending">order of the results </param>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Tweet> GetAllTweetsAboveId(bool ascending = true, long id = 0);
        
        /// <summary>
        /// Inserts a tweet
        /// </summary>
        /// <param name="tweet"></param>
        void InsertTweet(Tweet tweet);
    }
}