window.blazorCulture = {
    get: () => window.localStorage['BlazorCulture'],
    set: (value) => window.localStorage['BlazorCulture'] = value
};

window.isDarkModePreferred = () => {
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        return true;
    }
    return false;
};

function splashscreen() {
    const userPreferences = JSON.parse(window.localStorage["userPreferences"] ?? "null");
    const preferredColorScheme = userPreferences?.DarkLightTheme;
    const colorScheme = preferredColorScheme ?? (window.isDarkModePreferred() ? 2 : 1);

    if (colorScheme == 2) {
        const elem = document.getElementById("splashscreen");
        elem.classList.toggle("dark");
    }
}

splashscreen();