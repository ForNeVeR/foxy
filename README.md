# Foxy

Experimental F# eDSL for OxyPlot charting library

Inspired by [Elmish.XamarinForms](https://github.com/fsprojects/Elmish.XamarinForms)

## Example

```fsharp
Foxy.PlotModel(
    title = "Hello World from F#",
    axes = [
        Foxy.LinearAxis(position = AxisPosition.Bottom)
        Foxy.LinearAxis(position = AxisPosition.Left)
    ],
    series = [
        Foxy.LineSeries(title = "LineSeries",
            points = [for x in 0.0 .. 0.1 .. 1.0 -> (x, x*x)])
        Foxy.LineSeries(title = "LineSeries2",
            points = [for x in 0.0 .. 0.1 .. 1.0 -> (x + 0.2, x*x)])
    ])
```

![Example](docs/example.png)

## Build

[.NET Core SDK](https://www.microsoft.com/net/download/windows#/sdk) is required to build the project.

```
$ dotnet build .\Foxy.Core\Foxy.Core.fsproj
```