(function ($) {
  $.overlayPlugin = function (options) {
    // Default settings
    var settings = $.extend(
      {
        spinnerClass: "spinner",
        overlayClass: "overlay",
        spinnerHTML: '<div class="spinner"></div>',
      },
      options
    );

    var methods = {
      show: function () {
        // Create overlay and spinner dynamically
        var $overlay = $("<div>").addClass(settings.overlayClass).css({
          position: "fixed",
          top: 0,
          left: 0,
          width: "100%",
          height: "100%",
          backgroundColor: "rgba(0, 0, 0, 0.5)",
          zIndex: 9999,
        });

        var $spinner = $(settings.spinnerHTML).addClass(settings.spinnerClass).css({
          position: "absolute",
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
          zIndex: 10000,
        });

        $overlay.append($spinner);
        $("body").append($overlay);
      },

      hide: function () {
        $("body")
          .find("." + settings.overlayClass)
          .remove();
      },
    };

    return methods;
  };
})(jQuery);
