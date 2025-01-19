document.addEventListener("DOMContentLoaded", function () {
    // Модальное окно для просмотра изображений
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

    // Функционал для добавления имени пользователя в комментарий
    const copyUserLinks = document.querySelectorAll(".copy-user");

    copyUserLinks.forEach(link => {
        link.addEventListener("click", function () {
            const username = this.dataset.username;

            // Находим textarea в текущем посте
            const textarea = this.closest(".post").querySelector("textarea");

            if (textarea) {
                // Получаем текущий текст в textarea
                const currentText = textarea.value.trim();

                // Обновляем текст в textarea, добавляем имя пользователя
                textarea.value = `@${username} ${currentText}`;
                textarea.focus();
            }
        });
    });
});