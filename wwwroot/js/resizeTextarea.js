window.resizeTextArea = function (id) {
    var textarea = document.getElementById(id);
    textarea.style.height = "auto";
    textarea.style.height = textarea.scrollHeight + "px";
}