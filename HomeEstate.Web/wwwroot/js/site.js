$(document).ready(function () {
    var $backToTop = $('#backToTopBtn');
    var hasShownPulse = false;

    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $backToTop.addClass('show');

            if (!hasShownPulse) {
                $backToTop.addClass('pulse');
                hasShownPulse = true;

                setTimeout(function () {
                    $backToTop.removeClass('pulse');
                }, 1000);
            }
        } else {
            $backToTop.removeClass('show');
        }
    });

    $backToTop.click(function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: 0
        }, 600);
    });
});
let currentImageIndex = 0;
const images = @Html.Raw(Json.Serialize(Model.Images?.Select(i => i.ImageUrl).ToArray() ?? new string[0]));

function selectImage(index) {
    currentImageIndex = index;
    updateGallery();
}

function previousImage() {
    currentImageIndex = currentImageIndex > 0 ? currentImageIndex - 1 : images.length - 1;
    updateGallery();
}

function nextImage() {
    currentImageIndex = currentImageIndex < images.length - 1 ? currentImageIndex + 1 : 0;
    updateGallery();
}

function updateGallery() {
    if (images.length > 0) {
        document.getElementById('mainImage').src = images[currentImageIndex];

        // Update thumbnails
        const thumbnails = document.querySelectorAll('.thumbnail');
        thumbnails.forEach((thumb, index) => {
            if (index === currentImageIndex) {
                thumb.classList.add('active');
            } else {
                thumb.classList.remove('active');
            }
        });
    }
}

// Viewing option toggle
document.querySelectorAll('.option-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        document.querySelectorAll('.option-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');
    });
});