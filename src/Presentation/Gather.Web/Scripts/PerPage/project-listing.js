//var radius, query, start, childFriendly, locationId, sortType, sortDirection, targetPage, firstZoom = true;
var urlParams = {};

$(function () {

    $('.category-filters a').remove();

    prepareQuerystringParams();

    if ($.cookie("currentListingView") != null && $.cookie("currentListingView").length > 0) {
        $('#listTabs a').removeClass('active');
        var $activeTab = $('#listTabs a[href="' + $.cookie("currentListingView") + '"]');
        $activeTab.addClass('active');
        window.activateTab($activeTab);
    }

    $('input[name="categories"], input[name="categoriesMobile"]').live('change', function () {
        redirectToResults();
    });

    $('#sortType').live('change', function () {
        redirectToResults('sortType', $(this).val());
    });

    $('#sortDirection').live('change', function () {
        redirectToResults('sortDirection', $(this).val());
    });

    $('#loadNextResults').live('click', function () {
        var locationId = window.locationId;
        var categories = "";
        $('input[name="categories"]:checked').each(function () {
            categories += $(this).val() + ",";
        });
        var targetPage = $(this).data("page");
        var sortType = $('#sortType').val();
        var sortDirection = $('#sortDirection').val();

        $.get(window.loadNextResultsUrl, { locationId: locationId, categories: categories, page: targetPage, sortType: sortType, sortDirection: sortDirection }, function (data) {
            $('.project-listing').append(data["Projects"]);
            if (data["NextResultCount"] > 0) {
                $('#loadNextResults').data("page", targetPage + 1);
                $('#loadNextResults').html("Load next " + data["NextResultCount"] + " results");
            } else {
                $('#loadNextResults').remove();
            }
        });

        return false;
    });

    $('#listTabs a').click(function () {
        L.Util.requestAnimFrame(window.map.invalidateSize, window.map, !1, window.map._container);
        $.cookie("currentListingView", $(this).attr('href'), { expires: 3650 });
    });

});

function redirectToResults(thisParam, value) {
    var url = window.location.protocol + "//" + window.location.host + window.location.pathname;
    var querystring = "";

    if (thisParam && thisParam.length > 0)
        urlParams[thisParam] = value;

    var categories = "";
    $('input[name="categories"]:checked').each(function () {
        categories += $(this).val() + ",";
    });

    if (categories.length > 0) {
        categories = categories.substring(0, categories.length - 1);

        if ("page" in urlParams && ("categories" in urlParams || urlParams["categories"] != categories))
            delete urlParams["page"];

        urlParams["categories"] = categories;
    } else if ("categories" in urlParams) {
        delete urlParams["categories"];
    }

    for (var param in urlParams)
        querystring += param + "=" + urlParams[param] + "&";

    if (querystring.length > 0) {
        querystring = querystring.substring(0, querystring.length - 1);
        url += "?" + querystring;
    }

    window.location = url;
}

function prepareQuerystringParams() {
    var match,
        pl = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query = window.location.search.substring(1);

    while (match = search.exec(query))
        urlParams[decode(match[1])] = decode(match[2]);
}

function adjustMap() {
    var $map = $('#map');
    var windowW = $(window).width();
    var windowH = $(window).height();
    if (windowW < 767) {
        if (!$map.hasClass('leaflet-fullscreen')) {
            $map.css({ width: (windowW - 40) + 'px', height: (parseInt(windowH) - 20) + 'px' });
        }
        window.map.setView(window.mapLocation, 4);
        window.map.invalidateSize();
    }
}

function initMap() {

    window.map = new L.Map('map', {
        maxZoom: 18,
        minZoom: 4,
        scrollWheelZoom: false,
        zoomControl: false
    });

    window.mapLocation = new L.LatLng(window.mapLatitude, window.mapLongitude);
    window.map.setView(window.mapLocation, window.mapZoomLevel).addLayer(window.mapTiles);

    var zoomFs = new L.Control.ZoomFS();
    window.map.addControl(zoomFs);

    window.clusterer = new LeafClusterer(window.map);

    if (window.mapProjects.length > 0) {
        var projects = window.mapProjects;
        for (var i = 0; i < projects.length; i++) {
            createMarker(projects[i]);
        }
    }

    $(window).bind('orientationchange', function () {
        adjustMap();
    });

    $(window).resize(function() {
        adjustMap();
    });

    adjustMap();

    window.map.on('enterFullscreen', function () {
        $('html, body').scrollTop($(window).scrollTop() + 1);
        $('html, body').scrollTop($(window).scrollTop() - 1);
        if (!window.map.scrollWheelZoom.enabled()) {
            window.map.scrollWheelZoom.enable();
        }
    });

}