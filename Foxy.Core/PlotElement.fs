namespace Foxy.Core

open System

type PlotElement<'T> internal
    ( create: unit -> obj,
      update: PlotElement<'T> option -> PlotElement<'T> -> obj -> unit,
      attribsMap: Map<string, obj>,
      attribs: (string * obj)[] ) =

    let mutable attribsArray = attribs

    new (create, update, attribs) =
        PlotElement(create, update, Map.ofArray attribs, attribs)

    member this.TargetType = typeof<'T>

    member this.Attributes = attribsMap

    member this.TryGetAttribute<'Attr>(name: string) =
        match this.Attributes.TryFind(name) with
        | Some v -> Some (unbox<'Attr>(v))
        | None -> None

    member this.Update(target: obj) =
        update None this target

    member this.UpdateMethod = update

    member this.UpdateIncremental(prev: PlotElement<'T>, target: obj) =
        update (Some prev) this target

    member this.CreateMethod = create

    member this.Create() : obj =
        let target = this.CreateMethod()
        this.Update(target)
        target

    member this.WithAttribute(name: string, value: obj) = 
        PlotElement(create, update, attribsMap.Add(name, value), null)

    override this.ToString() = sprintf "%s(...)@%d" this.TargetType.Name (this.GetHashCode())