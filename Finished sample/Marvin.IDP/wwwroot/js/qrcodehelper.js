window.addEventListener("load", function () {
    var keyUri = document.getElementById("qrCodeKeyUri");
    new QRCode(document.getElementById("qrCode"),
        {
            text: keyUri.innerText,
            width: 150,
            height: 150
        });
});