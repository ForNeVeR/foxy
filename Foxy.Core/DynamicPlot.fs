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

module Utils = 
    
    let updateAttribute<'T when 'T: equality> 
        (prevOpt: PlotElement option) 
        (source: PlotElement)
        (name: string)
        (set: 'T -> unit)
        (setDefault: unit -> unit) =

        let prevValueOpt =
            match prevOpt with
            | None -> None
            | Some prev -> prev.TryGetAttribute<'T>(name)
        let valueOpt = source.TryGetAttribute<'T>(name)
        match prevValueOpt, valueOpt with
        | Some prevValue, Some value when prevValue = value -> ()
        | _, Some value -> set value
        | Some _, None -> setDefault ()
        | None, None -> ()

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
            let inline updateAttribute name set setDefault = 
                Utils.updateAttribute prevOpt source name set setDefault

            let setAxes (target: OxyPlot.PlotModel) (axes: PlotElement list) =
                axes |> List.iter (fun o -> 
                    let axis = (o.Create()) :?> OxyPlot.Axes.Axis
                    target.Axes.Add(axis))

            let setSeries (target: OxyPlot.PlotModel) (series: PlotElement list) =
                series |> List.iter (fun s ->
                    let ser = (s.Create()) :?> OxyPlot.Series.Series
                    target.Series.Add(ser))

            updateAttribute 
                "Title" 
                (fun title -> target.Title <- title) 
                (fun ()    -> target.Title <- "") 

            updateAttribute
                "Axes"
                (fun axes -> setAxes target axes)
                (fun ()   -> target.Axes.Clear())

            updateAttribute
                "Series"
                (fun series -> setSeries target series)
                (fun ()     -> target.Series.Clear())                                   
        
        new PlotElement(typeof<OxyPlot.PlotModel>, create, update, attribs)

    static member LinearAxis(?position: OxyPlot.Axes.AxisPosition) =
        let attribs = 
            [| match position with None -> () | Some v -> yield ("Position", box v)
            |]

        let create() = box (new OxyPlot.Axes.LinearAxis())

        let update (prevOpt: PlotElement option) (source: PlotElement) (targetObj: obj) =
            let target = (targetObj :?> OxyPlot.Axes.LinearAxis)
            let inline updateAttribute name set setDefault = 
                Utils.updateAttribute prevOpt source name set setDefault
            
            updateAttribute 
                "Position"
                (fun pos -> target.Position <- pos)
                (fun ()  -> target.Position <- OxyPlot.Axes.AxisPosition.None)
        
        new PlotElement(typeof<OxyPlot.Axes.LinearAxis>, create, update, attribs)


    static member LineSeries(points: Point list, ?title: string, ?markerType: OxyPlot.MarkerType) =
        let attribs =
            [| yield ("Points", box points)
               match title with None -> () | Some v -> yield ("Title", box v)
               match markerType with None -> () | Some v -> yield ("MarkerType", box v)
            |]

        let create() = box (new OxyPlot.Series.LineSeries())

        let update (prevOpt: PlotElement option) (source: PlotElement) (targetObj: obj) =
            let target = (targetObj :?> OxyPlot.Series.LineSeries)
            let inline updateAttribute name set setDefault = 
                Utils.updateAttribute prevOpt source name set setDefault

            let setPoints (target: OxyPlot.Series.LineSeries) (points: Point list) =
                points |> 
                List.iter (fun (x, y) -> 
                    target.Points.Add(new OxyPlot.DataPoint(x, y)))

            updateAttribute
                "Points"
                (fun points -> setPoints target points)
                (fun ()     -> target.Points.Clear())

            updateAttribute
                "Title"
                (fun title -> target.Title <- title)
                (fun ()    -> target.Title <- "")

            updateAttribute
                "MarkerType"
                (fun markType -> target.MarkerType <- markType)
                (fun ()       -> target.MarkerType <- OxyPlot.MarkerType.None)

        new PlotElement(typeof<OxyPlot.Series.LineSeries>, create, update, attribs)