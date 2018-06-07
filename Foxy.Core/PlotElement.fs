namespace Foxy.Core

open System

type PlotElement internal
    ( targetType: Type,
      create: unit -> obj,
      update: PlotElement option -> PlotElement -> obj -> unit,
      attribsMap: Map<string, obj>,
      attribs: (string * obj)[] ) =

    let mutable attribsArray = attribs

    new (targetType, create, update, attribs) =
        PlotElement(targetType, create, update, Map.ofArray attribs, attribs)

    member this.TargetType = targetType

    member this.Attributes = attribsMap

    member this.TryGetAttribute<'T>(name: string) =
        match this.Attributes.TryFind(name) with
        | Some v -> Some (unbox<'T>(v))
        | None -> None

    member this.Update(target: obj) =
        update None this target

    member this.UpdateMethod = update

    member this.UpdateIncremental(prev: PlotElement, target: obj) =
        update (Some prev) this target

    member this.CreateMethod = create

    member this.Create() : obj =
        let target = this.CreateMethod()
        this.Update(target)
        target

    member x.WithAttribute(name: string, value: obj) = PlotElement(targetType, create, update, attribsMap.Add(name, value), null)

    override x.ToString() = sprintf "%s(...)@%d" x.TargetType.Name (x.GetHashCode())