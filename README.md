## ExploreDeezer

A demo UWP application showing the use of my .NET Deezer API wrapper: [E.Deezer](https://github.com/projectgoav/E.Deezer) 

### Features
- Browse new releases for each genre
- Browse charts for each genre
- Detailed tracklists for albums and playlists
- Explore information about artists and discover related music
- Search Deezer's catalog
- Login to view and manage your favourites (Requires additional configuration. See section below)

***Please Note: Playback of tracks or track previews is not supported.*** 

## Build & Run

Project has been developed using VS2019 and targets a minimum version of Windows 10 v1803.

You should simply be able to clone this repository, open the solution and run it from within Visual Studio. 

### Enable OAuth Login

To enable login you must first register as a Deezer developer on their website. Once you have an account you can create an OAuth application which will provide you with an *AppId*, *AppSecret* and a *Redirect URI*. Instructions on how to do this can be found on the Deezer developer site. 

You must add these values to the `Secrets.cs` file in the root of `E.ExploreDeezer.UWP` project. Once preset, they will be used to provide the OAuth login. 

**Please Note:** *This application will request the `OfflineAccess` permission from accounts that login in order to receive a token that never expires. This is simply to avoid having to deal with token expiry. Any real application should only request permissions it actually requires and handle token expiry.*


## Design & Implementation

Project is split into 2 parts: Core and UI, the hope being that should it target another platform then it can reuse the functionality in 'Core'.

The MVVM pattern has been used with 'DataControllers' handling the interaction between this and the E.Deezer library, 'ViewModels' exposing information to display and 'Views' (present in the UI) which display that data in appropriate controls. 

The goal of this application was to be as minimal as possible in order to serve as a demo for the E.Deezer library and so that is the only dependency. 

### Core
This library is roughly organised by feature with each one comprising of one or more 'DataControllers' and a 'ViewModel'. 

### UI
To avoid polluting the 'Core' with navigation and other features many MVVM frameworks provide this is entirely handled and managed by the platform layer. The application is structured around a UWP NavigationView with a number of pages. Each page, when navigated to, will construct a 'ViewModel' and set it as the root DataContext. XAML bindings are then used to populate controls with data. Views will create event handlers on controls it is interested in and can react to interaction from the user. 

Where possible, I have tried to stick with the guidelines specified in the UWP developer documentation. 

A UWP implementation of the 'IPlatformServices' interface provides a helper to execute code on the UI thread and provide a secure store for a user accounts OAuth token. 


## Future Work & Goals

Below is a short list of things I'd like to work on at somepoint in time. A more details list can be found in the `.todo` file in the root of the repo.

- Proper app icons
- Support for radios & podcasts
- Track preview playback



## License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


## Screengrabs

### Whats New

![WhatsNewPage](https://github.com/projectgoav/E.ExploreDeezer/blob/master/Screengrabs/WhatsNew.png)


### Charts

![ChartsPage](https://github.com/projectgoav/E.ExploreDeezer/blob/master/Screengrabs/Charts.png)

### Search Results

![SearchPage](https://github.com/projectgoav/E.ExploreDeezer/blob/master/Screengrabs/Search.png)

### Artist Details

![ArtistDetailsPage](https://github.com/projectgoav/E.ExploreDeezer/blob/master/Screengrabs/ArtistOverview.png)
