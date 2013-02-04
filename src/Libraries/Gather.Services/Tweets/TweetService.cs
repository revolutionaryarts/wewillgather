using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core.Data;
using Gather.Core.Domain.Tweets;

namespace Gather.Services.Tweets
{
    public class TweetService : ITweetService
    {

        #region Fields

        private readonly IRepository<Tweet> _tweetRepository;

        #endregion

        #region Constructors

        public TweetService(IRepository<Tweet> tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete all but a certain amount of tweets
        /// </summary>
        /// <param name="amount">amount</param>
        public void DeleteOldTweets(int amount = 15)
        {
            List<Tweet> tweets = GetAllTweetsAboveId();
            if (tweets.Count() > amount)
            {
                int deleteAmount = tweets.Count() - amount;
                for(int i = 0; i < deleteAmount; i++)
                    _tweetRepository.Delete(tweets[i], true);
            }
        }

        /// <summary>
        /// Gets all of the tweets above a certain id or all of them
        /// </summary>   
        /// <returns>returns all of the tweets above a certain id or all of them</returns>
        public List<Tweet> GetAllTweetsAboveId(bool ascending = true, long id = 0)
        {
            var query = _tweetRepository.Table;

            if (id > 0)
                query = query.Where(s => s.TwitterId > id);

            query = ascending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id);

            var stories = query.ToList();
            return stories;
        }

        /// <summary>
        /// Insert a tweet
        /// </summary>
        /// <param name="tweet">tweet</param>
        public void InsertTweet(Tweet tweet)
        {
            if (tweet == null)
                throw new ArgumentNullException("tweet");

            _tweetRepository.Insert(tweet);
        }

        #endregion

    }
}