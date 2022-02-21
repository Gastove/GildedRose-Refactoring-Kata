namespace GildedRose

open System.Collections.Generic

type ItemName =
    | AgedBrie
    | BackstagePasses
    | Sulfuras
    | Cake
    | VestofDex
    | Elixir

    static member TryFrom(s: string) =
        match s with
        | "+5 Dexterity Vest" -> VestofDex |> Some
        | "Aged Brie" -> AgedBrie |> Some
        | "Elixir of the Mongoose" -> Elixir |> Some
        | "Sulfuras, Hand of Ragnaros" -> Sulfuras |> Some
        | "Backstage passes to a TAFKAL80ETC concert" -> BackstagePasses |> Some
        | "Conjured Mana Cake" -> Cake |> Some
        | _ -> None

    override this.ToString() =
        match this with
            | AgedBrie -> "Aged Brie"
            | BackstagePasses -> "Backstage passes to a TAFKAL80ETC concert"
            | Sulfuras -> "Sulfuras, Hand of Ragnaros"
            | Cake -> "Conjured Mana Cake"
            | VestofDex -> "+5 Dexterity Vest"
            | Elixir -> "Elixir of the Mongoose"

type ItemError = InvalidItem of string

type Item =
    { Name: ItemName
      SellIn: int
      Quality: int }

    static member Create name sellIn quality =
        { Name = name
          SellIn = sellIn
          Quality = quality }

    static member TryFrom (maybeName: string) sellIn quality =
        match maybeName |> ItemName.TryFrom with
        | Some n -> Item.Create n sellIn quality |> Ok
        | None -> sprintf "Invalid item: %s" maybeName |> Error


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

    let brie (item: Item) =
        match item.Quality, item.SellIn with
        | quality, sellIn when quality < 50 && sellIn <= 0 ->
            { item with
                Quality = quality + 2
                SellIn = sellIn - 1 }
        | quality, sellIn ->
            { item with
                Quality = min (quality + 1) 50
                SellIn = sellIn - 1 }

    let sulfuras (item: Item) = item

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
        match item.Name with
        | VestofDex
        | Elixir -> item |> Updaters.standard
        | Cake -> item |> Updaters.conjured
        | AgedBrie -> item |> Updaters.brie
        | Sulfuras -> item |> Updaters.sulfuras
        | BackstagePasses -> item |> Updaters.backstagePasses

type GildedRose(items: IList<Item>) =

    member val Items = items with get, set

    member this.UpdateQuality() =
        this.Items <-
            this.Items
            |> Seq.map Item.update
            |> Seq.toArray

module Program =
    [<EntryPoint>]
    let main argv =
        printfn "OMGHAI!"
        let Items = new List<Item>()

        Items.Add(
            { Name = ItemName.VestofDex
              SellIn = 10
              Quality = 20 }
        )

        Items.Add(
            { Name = ItemName.AgedBrie
              SellIn = 2
              Quality = 0 }
        )

        Items.Add(
            { Name = ItemName.Elixir
              SellIn = 5
              Quality = 7 }
        )

        Items.Add(
            { Name = ItemName.Sulfuras
              SellIn = 0
              Quality = 80 }
        )

        Items.Add(
            { Name = ItemName.Sulfuras
              SellIn = -1
              Quality = 80 }
        )

        Items.Add(
            { Name = ItemName.BackstagePasses
              SellIn = 15
              Quality = 20 }
        )

        Items.Add(
            { Name = ItemName.BackstagePasses
              SellIn = 10
              Quality = 49 }
        )

        Items.Add(
            { Name = ItemName.BackstagePasses
              SellIn = 5
              Quality = 49 }
        )

        Items.Add(
            { Name = ItemName.Cake
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
