$(document).ready(function () {

    function adjustIframes() {
        $('.row-overlay iframe').each(function () {
            var $this = $(this);
            var w = $this.attr('width');
            var actualW = $this.parent().width();
            var proportion = $this.attr('height') / w;

            if (actualW != w) {
                $this.css({
                    height: Math.round(actualW * proportion) + 'px',
                    width: (actualW - 10) + 'px'
                });
            }
        });
    }


    var windowW = $(window).width();

    function adjustContent() {
        $('#carousel, #carousel section').each(function () {
            var $this = $(this);
            	$this.css({
            		width: getCarouselWidth()
            });
        });
    }

    function getCarouselWidth() {
    	if (windowW < 767) {
				return windowW + 'px';
            } else {
                return '1000px';
            }
    }



    $(window).resize(function () {
        adjustIframes();
        adjustContent();
    });

    $('#carousel').cycle({
        fx: 'fade',
        fit: true,
        speed: 'fast',
        timeout: 5000,
        width: getCarouselWidth(),
        pager: '#nav'
    });    

    // Twitter feed
    var second = 1000, minute = second * 60, hour = minute * 60, day = hour * 24;
    var mNames = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");

    setInterval(function () {
        $.post("/Home/NewTweets", { id: tweetId }, function (result) {
            var jsonResult = JSON.parse(result);
            $.each(window.tweetArray, function (index) {
                $('#date_' + window.tweetArray[index].id).html(getDateDifference(window.tweetArray[index].createdDate));
            });
            $.each(jsonResult, function (index) {
                var retweet = jsonResult[index].Text.length > (133 - jsonResult[index].TwitterName.length) ? jsonResult[index].Text.substring(0, 133 - jsonResult[index].TwitterName.length) : jsonResult[index].Text;
                $('#twitterFeed').prepend("<article class='newArticle' style='display:none'><strong>" + jsonResult[index].UserName + " said:</strong> " + jsonResult[index].Text + "<br /><span id='date_" + jsonResult[index].TwitterId + "'>" + jsonResult[index].DateDifference + "</span> <a href='http://twitter.com/home?status=RT @@" + jsonResult[index].TwitterName + " – " + escape(retweet) + "' class='popup'>Retweet</a> <a href='http://twitter.com/home?status=@@" + jsonResult[index].TwitterName + "+' class='popup'>Reply</a></article>");
                window.tweetArray.push({ id: '' + jsonResult[index].TwitterId + '', createdDate: '' + jsonResult[index].CreatedDateString + '' });
                if (index + 1 == jsonResult.length)
                    window.tweetId = jsonResult[index].TwitterId;
            });

            $('.newArticle').fadeIn(1000);
        });
    }, 15000);

    function getDateDifference(date) {
        date = new Date(date);
        var timeDiff = (new Date()) - date;
        if (Math.floor(timeDiff / day) > 0)
            return date.getDate() + " " + mNames[date.getMonth()];
        else if (Math.floor(timeDiff / hour) > 0)
            return Math.floor(timeDiff / hour) + 'h';
        else if (Math.floor(timeDiff / minute) > 0)
            return Math.floor(timeDiff / minute) + 'm';
        else
            return Math.floor(timeDiff / second) + 's';
    }

});