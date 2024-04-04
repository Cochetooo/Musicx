# Desktop Module

This module contains the desktop output application of the program.<br>
It currently uses **JavaFX** ([API Documentation](https://openjfx.io/javadoc/21/)).<br>

***

### Package : Application

> This package contains the main JavaFX Application and its data :<br>
> All loaders and main stage / scene are created here.<br>
> It's not supposed to handle any of the rendered components though 
> and only the core logic of the window.

### Package : Helper

> This package contains static utility classes that serves as shortcut
> for data rendering for our specific desktop framework renderer.<br>
> For example, we have `DurationHelper` which helps for formatting
> a duration as a clean `String`.

- Utility classes **should** have a `-Helper` suffix in their class name.
- Utility methods **should** be static to reduce unnecessary instantiation, unless
there is enough to reason to do so.
- Remember

### Package : Model

> This package contains DTO objects derived from domain models but appropriate for
> JavaFX.

- Model **should** have a `-ViewModel` suffix in their class name.
- Model **should** use Property classes from JavaFX such as `LongProperty` or `StringProperty` for its members.
- Model **should** contain a method to hydrate an instance from a domain object.

### Package : Views

> In relation with `resources/views/`, this package handles FXML views.<br>
> It is important to note that it doesn't necessarily needs a FXML view in case
> of dynamic view loading (dialog for example).<br>

- **Controller**: Class that controls FXML views **should** have a `-ViewController` suffix.<br>
These classes should only serve as dialog between Java code and FXML views, therefore
only controlled elements will be added here, as well as the parent ViewController and
the children.<br>
It should implement `ViewControllerInterface`. Children's parent as well as props property will be initialized on the `initialize()` method, make sure to make your setup logic just after setting the parent controller and the props.<br>
Children properties must be `private` and **read-only**.

*By Alexis Cochet*