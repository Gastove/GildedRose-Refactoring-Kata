namespace GildedRose

open System.Collections.Generic

type ItemError = InvalidItemClass of string

type ItemClass =
    | Standard
    | Conjured
    | Legenday
    | Vintage
    | BackstagePasses

    static member TryFrom(s: string) =
        match s.ToLower() with
        | "standard" -> Standard |> Ok
        | "conjured" -> Conjured |> Ok
        | "legendary" -> Legenday |> Ok
        | "vintage" -> Vintage |> Ok
        | "backstagepasses" -> BackstagePasses |> Ok
        | wrong -> sprintf $"Invalid ItemClass: {wrong}" |> Error

type Item =
    { Name: string
      Class: ItemClass
      SellIn: int
      Quality: int }

    static member Create name clss sellIn quality =
        { Name = name
          Class = clss
          SellIn = sellIn
          Quality = quality }

    static member TryFrom name maybeClass sellIn quality =
        maybeClass
        |> ItemClass.TryFrom
        |> Result.map (fun c -> Item.Create name c sellIn quality)

module Updaters =
    let standard (item: Item) =
        match item.Quality, item.SellIn with
        | i, 0 ->
            { item with
                Quality = max (i - 2) 0
                SellIn = item.SellIn - 1 }
        | i, sellIn ->
            { item with
                Quality = i - 1
                SellIn = sellIn - 1 }

    let conjured (item: Item) =
        match item.Quality, item.SellIn with
        | i, 0 ->
            { item with
                Quality = max (i - 4) 0
                SellIn = item.SellIn - 1 }
        | i, sellIn ->
            { item with
                Quality = max (i - 2) 0
                SellIn = sellIn - 1 }

    let vintage (item: Item) =
        match item.Quality, item.SellIn with
        | quality, sellIn when quality < 50 && sellIn <= 0 ->
            { item with
                Quality = quality + 2
                SellIn = sellIn - 1 }
        | quality, sellIn ->
            { item with
                Quality = min (quality + 1) 50
                SellIn = sellIn - 1 }

    let legendary (item: Item) = item

    let backstagePasses (item: Item) =
        match item.Quality, item.SellIn with
        | _quality, sellIn when sellIn <= 0 ->
            { item with
                Quality = 0
                SellIn = sellIn - 1 }
        | quality, sellIn when sellIn <= 5 ->
            { item with
                Quality = min (quality + 3) 50
                SellIn = sellIn - 1 }
        | quality, sellIn when sellIn <= 10 ->
            { item with
                Quality = min (quality + 2) 50
                SellIn = sellIn - 1 }
        | quality, sellIn ->
            { item with
                Quality = min (quality + 1) 50
                SellIn = sellIn - 1 }

module Item =
    let update (item: Item) =
        item
        |> (match item.Class with
            | Standard -> Updaters.standard
            | Conjured -> Updaters.conjured
            | Legenday -> Updaters.legendary
            | Vintage -> Updaters.vintage
            | BackstagePasses -> Updaters.backstagePasses)

type GildedRose(items: IList<Item>) =

    member val Items = items with get, set

    member this.UpdateQuality() =
        this.Items <- this.Items |> Seq.map Item.update |> Seq.toArray

module Program =
    [<EntryPoint>]
    let main argv =
        printfn "OMGHAI!"
        let Items = new List<Item>()

        Items.Add(
            { Name = "+5 Dexterity Vest"
              Class = Standard
              SellIn = 10
              Quality = 20 }
        )

        Items.Add(
            { Name = "Aged Brie"
              Class = Vintage
              SellIn = 2
              Quality = 0 }
        )

        Items.Add(
            { Name = "Elixir of the Mongoose"
              Class = Standard
              SellIn = 5
              Quality = 7 }
        )

        Items.Add(
            { Name = "Sulfuras, Hand of Ragnaros"
              Class = Legenday
              SellIn = 0
              Quality = 80 }
        )

        Items.Add(
            { Name = "Sulfuras, Hand of Ragnaros"
              Class = Legenday
              SellIn = -1
              Quality = 80 }
        )

        Items.Add(
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              Class = BackstagePasses
              SellIn = 15
              Quality = 20 }
        )

        Items.Add(
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              Class = BackstagePasses
              SellIn = 10
              Quality = 49 }
        )

        Items.Add(
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              Class = BackstagePasses
              SellIn = 5
              Quality = 49 }
        )

        Items.Add(
            { Name = "Conjured Mana Cake"
              Class = Conjured
              SellIn = 3
              Quality = 6 }
        )

        let app = new GildedRose(Items)

        for i = 0 to 30 do
            printfn "-------- day %d --------" i
            printfn "name, sellIn, quality"

            for j = 0 to app.Items.Count - 1 do
                printfn "%O, %d, %d" app.Items.[j].Name app.Items.[j].SellIn app.Items.[j].Quality

            printfn ""
            app.UpdateQuality()

        0
