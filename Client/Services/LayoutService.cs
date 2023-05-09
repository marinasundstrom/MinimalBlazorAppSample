// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BlazorApp1.Client.Services.UserPreferences;

using MudBlazor;

namespace BlazorApp1.Client.Services;

public class LayoutService
{
    private readonly IUserPreferencesService _userPreferencesService;
    private UserPreferences.UserPreferences _userPreferences = default!;
    private bool _systemPreferences;

    public bool IsRTL { get; private set; } = false;
    public DarkLightMode DarkModeToggle = DarkLightMode.System;

    public bool IsDarkMode { get; private set; }

    public MudTheme CurrentTheme { get; private set; } = default!;


    public LayoutService(IUserPreferencesService userPreferencesService)
    {
        _userPreferencesService = userPreferencesService;
    }

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _systemPreferences = isDarkModeDefaultTheme;
        _userPreferences = await _userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.DarkLightTheme switch
            {
                DarkLightMode.Dark => true,
                DarkLightMode.Light => false,
                DarkLightMode.System => isDarkModeDefaultTheme,
                _ => IsDarkMode
            };
            IsRTL = _userPreferences.RightToLeft;
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences.UserPreferences { DarkLightTheme = DarkLightMode.System };
            await _userPreferencesService.SaveUserPreferences(_userPreferences);
        }
    }

    public Task OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccured();
        }

        return Task.CompletedTask;
    }

    public event EventHandler? MajorUpdateOccured;

    private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

    public async Task ToggleDarkMode()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                IsDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode = _systemPreferences;
                break;
        }

        _userPreferences.DarkLightTheme = DarkModeToggle;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public async Task ToggleRightToLeft()
    {
        IsRTL = !IsRTL;
        _userPreferences.RightToLeft = IsRTL;
        await _userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccured();
    }
}