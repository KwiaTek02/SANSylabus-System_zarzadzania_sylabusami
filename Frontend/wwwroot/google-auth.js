window.googleAuthInterop = {
    listenForToken: function (dotNetHelper) {
        window.addEventListener("message", function (event) {
            if (event.origin !== "https://localhost:7177") return;

            const token = event.data?.token;
            if (token) {
                dotNetHelper.invokeMethodAsync("OnGoogleLoginSuccess", token);
            }
        }, { once: true });
    }
};
