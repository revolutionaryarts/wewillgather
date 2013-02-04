$(function () {

    window.map = new L.Map('map', {
        maxZoom: 18,
        minZoom: 5,
        scrollWheelZoom: false,
        zoomControl: false
    });

    window.mapLocation = new L.LatLng(window.projectLatitude, window.projectLongitude);
    window.map.setView(window.mapLocation, 15).addLayer(window.mapTiles);

    var zoomFs = new L.Control.ZoomFS();
    window.map.addControl(zoomFs);

    var markerLocation = new L.LatLng(window.projectLatitude, window.projectLongitude);
    L.marker(markerLocation, { icon: window.mapIcon }).addTo(window.map);

    $(window).resize(function () {
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

    $('.comment-reply').click(function () {
        $('#cancelReply').show();
        var formHtml = $('#commentForm').clone().addClass("row").wrap('<p>').parent().html();

        $('#commentForm').remove();
        $(this).closest('.row').after(formHtml);
        $('#commentForm h2').replaceWith("<h5>Add a reply</h5>");
        $('#commentInResponseTo').val($(this).data('id'));
        return false;
    });

    $('#cancelReply').live('click', function () {
        $('#cancelReply').hide();
        var formHtml = $('#commentForm').clone().wrap('<p>').parent().html();
        $('#commentForm').remove();
        $('.panel-comments').before(formHtml);
        $('#commentInResponseTo').val('0');
        return false;
    });

    $('a[data-comment-id]').live('click', function () {
        var commentId = $(this).data('comment-id');
        $('#flagCommentId').val(commentId);
        $('#flagComment').reveal($(this).data());
        return false;
    });

    $('a[data-delete-comment-id]').live('click', function () {
        var commentId = $(this).data('delete-comment-id');
        $('#deleteCommentId').val(commentId);
        $('#deleteComment').reveal($(this).data());
        return false;
    });

});

function adjustMap() {
    var $map = $('#map');
    if ($map.closest('.project-map').width() < 767) {
        window.map.setView(window.mapLocation, 15);
        window.map.invalidateSize();
    }
}