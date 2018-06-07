// Learn more about F# at http://fsharp.org

open System
open System.IO
open Foxy.Core

let sample = 
    Foxy.PlotModel(
        title = "Hello World from F#",
        axes = [
            Foxy.LinearAxis(position = OxyPlot.Axes.AxisPosition.Bottom)
            Foxy.LinearAxis(position = OxyPlot.Axes.AxisPosition.Left)
        ],
        series = [
            Foxy.LineSeries(
                title = "LineSeries",
                points = [for x in 0.0 .. 0.1 .. 1.0 -> (x, x*x)])
            Foxy.LineSeries(
                title = "LineSeries2",
                points = [for x in 0.0 .. 0.1 .. 1.0 -> (x + 0.2, x*x)]
            )
        ])

[<EntryPoint>]
let main argv =
    use stream = File.Create("sample.pdf")  
    let pdfExporter = new OxyPlot.PdfExporter(Width = 600., Height = 400.)
    pdfExporter.Export(sample.Create() :?> OxyPlot.IPlotModel, stream)
    0 // return an integer exit code
