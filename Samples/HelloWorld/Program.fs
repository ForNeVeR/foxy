// Learn more about F# at http://fsharp.org

open System
open System.IO
open Foxy.Core

module NoDsl =
    open OxyPlot
    open OxyPlot.Axes
    open OxyPlot.Series

    let sample =
        let model = new PlotModel(Title = "Hello World from F#")
        model.Axes.Add(new LinearAxis(Position = AxisPosition.Bottom))
        model.Axes.Add(new LinearAxis(Position = AxisPosition.Left))

        let lineSeries = new LineSeries(Title = "LineSeries")
        for x in 0.0 .. 0.1 .. 1.0 do
            lineSeries.Points.Add(new DataPoint(x, x*x))

        let lineSeries2 = new LineSeries(Title = "LineSeries2")
        for x in 0.0 .. 0.1 .. 1.0 do
            lineSeries2.Points.Add(new DataPoint(x + 0.2, x*x))

        model.Series.Add(lineSeries)
        model.Series.Add(lineSeries2)

        model

module Dsl =
    open OxyPlot.Axes
    let sample = 
        Foxy.PlotModel(
            title = "Hello World from F#",
            axes = [
                Foxy.LinearAxis(position = AxisPosition.Bottom)
                Foxy.LinearAxis(position = AxisPosition.Left)
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
    pdfExporter.Export(Dsl.sample.Create() :?> OxyPlot.IPlotModel, stream)
    0 // return an integer exit code
