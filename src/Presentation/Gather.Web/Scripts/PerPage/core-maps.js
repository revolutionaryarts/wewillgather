var clusterer, map, mapIcon, mapLocation, mapTiles;

mapTiles = new L.TileLayer('http://{s}.tile.cloudmade.com/3a41dec5500e444c81b6c1c42a7a6ac9/997/256/{z}/{x}/{y}.png', {
    attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery &copy; <a href="http://cloudmade.com">CloudMade</a>'
});

mapIcon = L.icon({
    iconUrl: '/content/images/maps/marker.png',
    iconAnchor: [12, 41],
    popupAnchor: [1, -35],
    shadowUrl: '/content/images/maps/marker-shadow.png'
});

function createMarker(project) {

    var marker = new L.Marker(new L.LatLng(project.Latitude, project.Longitude), { icon: window.mapIcon });
    marker.bindPopup('<h3 class="row h4"><a href="' + project.URL + '">' + project.Name + '</a></h3><div class="row bumped-down">' +
        '<strong>Date:</strong> ' + project.StartDate + '<br />' +
        '<strong>Time:</strong> ' + project.StartTime + '<br />' +
        '<strong>Location:</strong> ' + project.Location + '<br />' +
        '<strong>Category:</strong> ' + project.Categories +
        '</div>' +
        '<div class="row">' +
        '<a href="' + project.URL + '" class="button button-arrow">See all the info</a>' +
        '</div>', { maxWidth: '230' });

    clusterer.addMarker(marker);

}

$(function() {

    $('#map').click(function() {
        if (!window.map.scrollWheelZoom.enabled()) {
            window.map.scrollWheelZoom.enable();
        }
    });

    $('#map').mouseleave(function() {
        if (window.map.scrollWheelZoom.enabled()) {
            window.map.scrollWheelZoom.disable();
        }
    });

});