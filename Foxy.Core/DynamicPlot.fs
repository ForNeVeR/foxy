namespace Foxy.Core

type Point = float * float

[<AutoOpen>]
module PlotElementExtensions =
    
    type PlotElement with

        member this.Title(value: string) =
            this.WithAttribute("Title", box value)

        member this.Position(value: OxyPlot.Axes.AxisPosition) =
            this.WithAttribute("Position", box value)

        member this.MarkerType(value: OxyPlot.MarkerType) =
            this.WithAttribute("MarkerType", box value)
           
        member this.Axes(value: PlotElement list) =
            this.WithAttribute("Axes", box value)

        member this.Points(value: Point list) =
            this.WithAttribute("Points", box value)
          
        member this.Series(value: PlotElement list) =
            this.WithAttribute("Series", box value)


type Foxy() =

    static member PlotModel(?title: string, ?axes: PlotElement list, ?series: PlotElement list) =
        let attribs =
            [| match title with None -> () | Some v -> yield ("Title", box v)
               match axes with None -> () | Some v -> yield ("Axes", box v)
               match series with None -> () | Some v -> yield ("Series", box v)
            |]

        let create() = box (new OxyPlot.PlotModel())

        let update (prevOpt: PlotElement option) (source: PlotElement) (targetObj: obj) =
            let target = (targetObj :?> OxyPlot.PlotModel)

            // update Title
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<string>("Title")
            let valueOpt = source.TryGetAttribute<string>("Title")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> target.Title <- v
            | Some _, None -> target.Title <- ""
            | None, None -> ()

            let setAxes (target: OxyPlot.PlotModel) (axes: PlotElement list) =
                axes |> List.iter (fun o -> 
                    let axis = (o.Create()) :?> OxyPlot.Axes.Axis
                    target.Axes.Add(axis))

            // update Axes
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<PlotElement list>("Axes")
            let valueOpt = source.TryGetAttribute<PlotElement list>("Axes")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> setAxes target v
            | Some _, None -> target.Axes.Clear()
            | None, None -> ()

            let setSeries (target: OxyPlot.PlotModel) (series: PlotElement list) =
                series |> List.iter (fun s ->
                    let ser = (s.Create()) :?> OxyPlot.Series.Series
                    target.Series.Add(ser))

            // update Series
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<PlotElement list>("Series")
            let valueOpt = source.TryGetAttribute<PlotElement list>("Series")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> setSeries target v
            | Some _, None -> target.Series.Clear()
            | None, None -> ()
        
        new PlotElement(typeof<OxyPlot.PlotModel>, create, update, attribs)

    static member LinearAxis(?position: OxyPlot.Axes.AxisPosition) =
        let attribs = 
            [| match position with None -> () | Some v -> yield ("Position", box v)
            |]

        let create() = box (new OxyPlot.Axes.LinearAxis())

        let update (prevOpt: PlotElement option) (source: PlotElement) (targetObj: obj) =
            let target = (targetObj :?> OxyPlot.Axes.LinearAxis)
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<OxyPlot.Axes.AxisPosition>("Position")
            let valueOpt = source.TryGetAttribute<OxyPlot.Axes.AxisPosition>("Position")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> target.Position <- v
            | Some _, None -> target.Position <- OxyPlot.Axes.AxisPosition.None
            | None, None -> ()
        
        new PlotElement(typeof<OxyPlot.Axes.LinearAxis>, create, update, attribs)


    static member LineSeries(points: Point list, ?title: string, ?markerType: OxyPlot.MarkerType) =
        let attribs =
            [| yield ("Points", box points)
               match title with None -> () | Some v -> yield ("Title", box v)
               match markerType with None -> () | Some v -> yield ("MarkerType", box v)
            |]

        let create() = box (new OxyPlot.Series.LineSeries())

        let setPoints (target: OxyPlot.Series.LineSeries) (points: Point list) =
            points |> List.iter (fun (x, y) -> target.Points.Add(new OxyPlot.DataPoint(x, y)))

        let update (prevOpt: PlotElement option) (source: PlotElement) (targetObj: obj) =
            let target = (targetObj :?> OxyPlot.Series.LineSeries)

            // update Points
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<Point list>("Points")
            let valueOpt = source.TryGetAttribute<Point list>("Points")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> setPoints target v
            | Some _, None -> target.Points.Clear()
            | None, None -> ()

            // update Title
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<string>("Title")
            let valueOpt = source.TryGetAttribute<string>("Title")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> target.Title <- v
            | Some _, None -> target.Title <- ""
            | None, None -> ()

            // update MarkerType
            let prevValueOpt =
                match prevOpt with
                | None -> None
                | Some prev -> prev.TryGetAttribute<OxyPlot.MarkerType>("MarkerType")
            let valueOpt = source.TryGetAttribute<OxyPlot.MarkerType>("MarkerType")
            match prevValueOpt, valueOpt with
            | Some pv, Some v when pv = v -> ()
            | pOpt, Some v -> target.MarkerType <- v
            | Some _, None -> target.MarkerType <- OxyPlot.MarkerType.None
            | None, None -> ()

        new PlotElement(typeof<OxyPlot.Series.LineSeries>, create, update, attribs)