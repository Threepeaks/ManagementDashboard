(function ($) {
  $.fn.scrollIndicatorPlugin = function (options) {
    const settings = $.extend(
      {
        indicatorClass: "scroll-indicator",
        text: "Back to Top",
        pieColor: "#4caf50",
        backgroundColor: "#e0e0e0",
        textColor: "#0051ff",
      },
      options
    );

    return this.each(function () {
      const $element = $(this);

      // Create percentage text element
      const $percentText = $("<span>").css({
        zIndex: 2,
        position: "relative",
      });

      // Create scroll indicator container
      const $indicator = $("<div>").addClass(settings.indicatorClass).css({
        position: "fixed",
        bottom: "20px",
        right: "20px",
        width: "50px",
        height: "50px",
        borderRadius: "50%",
        display: "none",
        backgroundColor: settings.backgroundColor,
        color: "#fff",
        fontSize: "14px",
        fontWeight: "bold",
        textAlign: "center",
        zIndex: 9999,
        cursor: "pointer",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
      });

      // Create text bubble
      const $textBubble = $("<div>").text(settings.text).css({
        position: "absolute",
        right: "60px",
        top: "50%",
        transform: "translateY(-50%)",
        backgroundColor: "rgba(0, 0, 0, 0.7)",
        color: settings.textColor,
        padding: "6px 12px",
        borderRadius: "8px",
        fontSize: "13px",
        whiteSpace: "nowrap",
        zIndex: 10000,
        display: "none",
      });

      $indicator.append($percentText);
      $indicator.append($textBubble);
      $("body").append($indicator);

      // Scroll event
      $(window).on("scroll", function () {
        const scrollTop = $(window).scrollTop();
        const docHeight = $(document).height() - $(window).height();
        const scrollPercent = Math.round((scrollTop / docHeight) * 100);

        $percentText.text(scrollPercent + "%");

        if (scrollTop > 30) {
          if (scrollPercent > 0) {
            $indicator.css({
              background: `conic-gradient(${settings.pieColor} 0% ${scrollPercent}%, ${settings.backgroundColor} ${scrollPercent}% 100%)`,
            });
          } else {
            // No gradient at 0%
            $indicator.css({
              background: settings.backgroundColor,
            });
          }

          $indicator.fadeIn(200);
          $textBubble.fadeIn(200);
        } else {
          // Hide completely and remove any background
          $indicator.fadeOut(200, () => {
            $indicator.css("background", settings.backgroundColor);
          });
          $textBubble.fadeOut(200);
        }

        // Responsive tweaks
        if ($(window).width() < 500) {
          $indicator.css({ right: "10px", bottom: "10px" });
          $textBubble.css({ right: "60px", fontSize: "12px" });
        } else {
          $indicator.css({ right: "20px", bottom: "20px" });
          $textBubble.css({ right: "60px", fontSize: "13px" });
        }
      });

      // Scroll to top
      $indicator.on("click", function () {
        $("html, body").animate({ scrollTop: 0 }, 400);
        $textBubble.fadeOut(200);
      });

      // Initialize indicator with default background color
      $indicator.css({
        display: "none", // Ensure the indicator is hidden on page load
        background: settings.backgroundColor, // Set default background color
      });

      $percentText.css({
        display: "flex", // Use flexbox for alignment
        alignItems: "center", // Center text vertically
        justifyContent: "center", // Center text horizontally
        width: "100%", // Ensure it spans the full width of the pie
        height: "100%", // Ensure it spans the full height of the pie
        position: "absolute", // Position it within the pie
        top: 0, // Align to the top of the pie
        left: 0, // Align to the left of the pie
      });
    });
  };
})(jQuery);
