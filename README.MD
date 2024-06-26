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

### Release 0.5.1

> ### New Features
> - Added 20+ new genres
> - `Queue List` is back
> - `Track Number` now editable
> - `Clear Library` now working again.
> - `Album Page`
> - `Star Ratings & Tier` for albums
> ### Changes
> - `Now Playing` layout has been improved
> ### Fixes
> - Corrected a rare bug where an album without artwork would give an error.

### Release 0.5.0 *(2024-06-14)*

> ### New Features
> - `New Layout` for library
> - `Now Playing` tab
> - `Artist Editor` with delete feature
> - `Normalize Songs` but works only with WAV files for now.
> - `Structure Library`
> ### Changes
> - Edit is now directly in the table view
> - Filtering library is now asynchronous to avoid freezes
> - Reverted history icon
> - History and Profile tab are now disabled for the meantime.
> - `Queue List` is now disabled for the meantime
> - Font family for now playing track has been changed from *Space Grotesk* to *Orbitron*.
> ### Fixes
> - List View color fix for odd rows.
> - Background color for audio player is not stuck anymore after playing a song without artwork

### Release 0.4.1 *(2024-05-30)*

> ### Changes
> - `Language` is now loaded by config.json
> - `Local Folders` are now loaded by config.json
> - `Scene Tabs` Local Library is now on top
> - `Date` for track with no release year is now 1/1/1
> - `Song Play` now requires primary button double click on tracklist
> ### Fixes
> - Fixed wrong custom tag for loading song primary genres
> - Fixed Scan Folders buttons disabled if no songs but folders paths not empty
> - Songs name and genres now updates instantly when edited
> - Partially fixed transparency issue for autocompletion list for genres selection
> - Fixed Play Now button was not correctly working
> - When an album is edited from a song, now correctly updates all others song from the album

### Release 0.4.0 *(2024-05-25)*

> ### New Features
> - `Genre Edit` to add primary & secondary genres
> - `Albums Group` with list of albums and their songs if selected
> - `Local Library Group` with editable tabs
> - `Scenes` for each functionality
> - `Language: French` added
> - `Vertical Navigation bar` for scenes
> ### Changes
> - `Color Theme` has been improved
> - `Album Artwork Placeholder` is darker now
> - `Sliders` are now thiner
> - `Songs Group` List has been improved
> ### Fixes
> - Fixed a lot of visual issues
> - Songs better updates when edited
> - Fixed wrong custom tags for genres
> - Fixed null exceptions here and there
> - Fixed queue info not displaying until queue gets updated

### Release 0.3.3 *(2024-05-20)*

> #### New Features
> - Album artwork view in each song of the track list
> #### Fixes
> - Fixed audio player color transition
> - Fixed some library issues
> - Fixed player not working properly when song has no artist
> - Library now updates correctly when scanning new folders

### Release 0.3.2 *(2024-05-16)*

> #### Technical Changes
> - Documentation and specification for developers

### Release 0.3.1 *(2024-05-14)*

> #### New Features
> - `Edit` Songs & Albums
> - `Save Audio Files` metadata
> - `Console Tabs` for more clarity
> - `Console Level Filter`
> - `Restructure Folders` feature to make a fully organized music folder
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