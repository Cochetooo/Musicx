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
> - **Known Bugs**: 

*By Alexis Cochet.*