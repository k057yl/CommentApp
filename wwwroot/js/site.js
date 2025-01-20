document.addEventListener("DOMContentLoaded", function () {
    const images = document.querySelectorAll('.comment-image');

    const modal = document.getElementById('imageModal');
    const modalImage = document.getElementById('modalImage');

    modalImage.style.transform = 'scale(0)';
    modalImage.style.transition = 'transform 0.5s ease';

    images.forEach(image => {
        image.addEventListener('click', (e) => {
            modalImage.src = e.target.src;

            modal.style.display = 'flex';

            setTimeout(() => {
                modalImage.style.transform = 'scale(1)';
            }, 10);
        });
    });

    modal.addEventListener('click', () => {
        modal.style.display = 'none';
        modalImage.style.transform = 'scale(0)';
    });

    const copyUserLinks = document.querySelectorAll(".copy-user");

    copyUserLinks.forEach(link => {
        link.addEventListener("click", function () {
            const username = `@${this.dataset.username}`;

            const textarea = this.closest(".post").querySelector("textarea");

            if (textarea) {
                const currentText = textarea.value.trim();

                if (!currentText.includes(username)) {
                    textarea.value = `${username} ${currentText}`.trim();
                    textarea.focus();
                }
            }
        });
    });
});