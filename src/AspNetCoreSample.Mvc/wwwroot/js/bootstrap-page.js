// Bootstrap 5 のカスタムフォームバリデーションを有効化するための JavaScript
(function () {
  "use strict";
  // フォームとバリデーションを取得
  var forms = document.querySelectorAll(".needs-validation");

  // ループしてバリデーションを適用
  Array.prototype.slice.call(forms).forEach(function (form) {
    form.addEventListener(
      "submit",
      function (event) {
        if (!form.checkValidity()) {
          event.preventDefault();
          event.stopPropagation();
        }

        form.classList.add("was-validated");
      },
      false
    );
  });
})();
