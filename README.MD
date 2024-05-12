# Musicx Documentation

Musicx is a music library application that serves several purposes for dedicated
music lovers:<br>
- Local Computer Music Library with smart import that uses a local database for efficient
  loading and music editing.
- A tag editor system to edit songs efficiently.
- An online review and rating system to share and seek people's opinion.

***

## Technical Specifications

> Musicx currently uses **Java 21** and **Gradle 8.5**.<br>
> It uses several external libraries:
> - **H2** as local database.
> - **Hibernate 6.4.4** to interact between domain objects and databases.
> - **JSON** to parse translations.
> - **JAudioTagger 3.0.1** for audio import and editing.
> - **JavaFX**, **ControlsFX**, **FormsFX**, **Ikonli** and **AtlantaFX** for desktop application.

***

## Patch Log

### Release 0.3.1

> #### New Features
> - `Edit` Songs & Albums
> - `Save Audio Files` metadata
> - `Console Tabs` for more clarity
> #### Changes
> - Changed pause fade-out from 0.25s to 0.35s

### Release 0.3.0 *(2024-05-12)*

> #### Changes
> - Rewritten database and models entirely
> - Cleaned up code

### Release 0.2.4 *(2024-04-25)*

> #### New Features
> - `Artist List` Double click now plays artist discography
> - `Artist List` Play Now & Play shuffled context menu
> #### Changes
> - `Player` background now changes according to album artwork, can be changed in settings
> #### Fixes
> - `Artwork` Loading caused player to freeze
> - `Settings` resetting everytime settings modal was opened, and not binding correctly

### Release 0.2.3 *(2024-04-20)*

> #### New Features
> - `Album Artwork` now displayed bottom left, using LastFm & iTunes API
> - `Album Artwork Shadow` displayed, can be toggled in settings
> - `Queue` Now displays number of tracks and total duration of the queue
> - `Repeat Mode` with queue repeat and song repeat
> - `Shuffle` not available as a mode right now but shuffle queue
> - `Track Genres` with primary/secondary
> #### Changes
> - `Track List` is now sorted by year and albums
> - `Track List` color changes to fit app color theme
> #### Fixes
> - `Player` now changes correctly when a song of the same position as the current is requested
> - `Support for M4A` custom tags now works correctly
> - `Scan Formats` now properly selects on start-up

### Release 0.2.2 *(2024-04-17)*

> #### New Features
> - `Formats` selection for library scan
> - `Settings` modal
> - `Settings` Player : Smooth Fade when pausing/stopping song
> - `Console Output` modal
> - `Queue` navigation list
> - `Queue : Remove Track`
> #### Changes
> - `Track List` has been changed, list header has been hidden and columns now take 100% width.
> - `Track Time Slider` is now disabled when no song is playing.
> - `Sliders` fill colors are now more intuitive.
> - `Stop Button` removed
> - `Volume Slider` change volume icon state according to volume
> #### Fixes
> - `Library Clear` Track list, queue list and artist list now get cleared as well.
> - `Track Time Slider` is now clickable everywhere in addition to drag click.
> - `Scan Folders` Menu Item now properly enables if library not empty at start.
> - `Artist List` "All Artists" is no longer duplicated for each empty rows.
> - `Artist List` No Longer duplicates artists with different letter cases.
> - `Track List` Fixed warning of a redundant property for the track column

### Release 0.2.1 *(2024-04-11)*

> #### Fixes
> - `Audio Player: Backward 30s & Forward 30s` now working properly

### Release 0.2.0 *(2024-04-09)*

> #### New Features
> - `Home screen` when library is empty
> - `Library: Multiple folders source`
> - `Library: Clear & Refresh`
> - `Track List: Search bar`
> - `Track List: Play Now, Queue Next, Queue Last` button
> - `Song Queue`
> - `Audio Player: Previous & Next` button
> - `Logger` for more tracing
> - `Controls Menu` with basic audio player controls
> - `Shortcut` for a lot of actions
> #### Changes
> - `Font Family` is now Space Grotesk.
> - `Track List` now displays Artist, Album, Title, Year & Duration
> - `Translations` resources has been reworked
> - `Modules` for the whole project
> - `Top Menu Bar` is now packed in one button
> #### Others
> - Increased performance for library import
> - Rewritten code to make it cleaner
> - Fixed a lot of bugs

### Release 0.1.0 *(2024-03-29)*

> - **New Features**: Library Folder Import & Scan, Track List, Audio Player, 
> Local Database Persistence, Song title, duration & format display

*By Alexis Cochet.*