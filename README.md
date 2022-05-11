# BlazingState
This library provides powerful, robust, fast and yet simple state management with almost 0 boilerplate code. 

You can use this library for any project, not just with Blazor.

## Setup
Download the latest release from [NuGet](https://www.nuget.org/packages/BlazingState).

## Usage
### Registering state observers
Start by registering your tracked type to your services.

**Note:** You can only register **1 state observer per type**.
```csharp
// Sample class for the examples
public class MyData
{
    public string SomeString { get; set; }
}

// Registers a state observer of this type with no initial value
builder.Services.AddStateObserver<MyData>();

// You can also initialize MyData with an initial value
builder.Services.AddStateObserver<MyData>(new MyData());

// And even with an implementation factory
builder.Services.AddStateObserver<MyData>(sp =>
{
    var service = sp.GetRequiredService<SomeOtherService>();
    return new MyData { SomeString = service.SomeText };
});
```

If you don't use DI or another DI framework you can also manually instantiate your state observers:
```csharp
var myStateObserver = new StateObserver<MyData>();

// And the same with an optional initial value
var myStateObserver = new StateObserver<MyData>(initialValue);
```

### Reading the value from state observers
Add the following property to your razor component (or if using a normal class, just use normal di):
```csharp
[Inject]
protected StateObserver<MyData> MyData { get; set; } = null!;
```

Subscribe using the ``OnParametersSet`` lifecycle event in Blazor (or use the ctor for normal classes):
```csharp
protected override void OnParametersSet()
{
    // Now if another component changed the value property of MyData, this callback gets executed
    MyData.Register(this, () =>
    {
        // Rerender the UI
        StateHasChanged();
    });

    // You can also use async callbacks (actually faster since the sync version gets wrapped)
    // Keep in mind that you can only register 1 event per instance, so the previous callback gets overriden by the async one
    MyData.Register(this, async () =>
    {
        await SomeMethodAsync();
    });
}
```

You don't have to unsubscribe from the event, as soon as the GC collects the instance, the event is removed. \
If you really need the memory you should implement ``IDisposable`` and remove the event by yourself in the dispose method to cleanup resources immediately but that's not needed. An alternative would be to call ``GC.Collect()``, as that forces the GC to perform a collection which removes all unused events but I wouldn't recommend this.
```csharp
public void Dispose()
{
    MyData.Unregister(this);
}
```

Display the value:
```xml
<p>Current value of MyData: @MyData.Value.SomeString</p>
```

### Updating the value of state observers
To update the current value of the observer you can just set the ``Value`` property of your observer.

```csharp
// Assuming you have a method which gets called when you want to perform an update (e.g. clicking on a button):
public void UpdateValue(string newValue)
{
    MyData.Value = new MyData { SomeString = newValue };
}
```
That's all! All subscribers are recieving your update automatically.

You can also just set some properties of the value instance and then manually notify all subscribers:
```csharp
public async Task UpdateValue(string newValue)
{
    MyData.Value.SomeString = newValue;
    await MyData.NotifyStateChangedAsync();
}
```
This way you don't have to reallocate the object every time which can build up a lot of pressure on GC with bigger objects.


## Sample projects
More samples can be found in the [Samples directory](/Samples).


## License
BlazingState is licensed under Apache License 2.0, see [LICENSE.txt](/LICENSE.txt) for more information.