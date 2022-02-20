namespace GildedRose

open System.Collections.Generic

type Item =
    { Name: string
      SellIn: int
      Quality: int }

type ShopItem =
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

    member this.Legendary =
        match this with
        | Sulfuras -> true
        | _ -> false

    member this.Conjured =
        match this with
        | Cake -> true
        | _ -> false

module Updaters =
    let standard (item: Item) =
        match item.Quality, item.SellIn with
        | 0, sellIn -> { item with SellIn = sellIn - 1 }
        | i, 0 ->
            { item with
                Quality = i - 2
                SellIn = item.SellIn - 1 }
        | i, sellIn ->
            { item with
                Quality = i - 1
                SellIn = sellIn - 1 }

    let brie (item: Item) =
        match item.Quality, item.SellIn with
        | quality, sellIn when quality < 50 && sellIn <= 0 ->
            { item with
                Quality = quality + 2
                SellIn = sellIn - 1 }
        | quality, sellIn when quality = 50 ->
            { item with
                Quality = 50
                SellIn = sellIn - 1 }
        | quality, sellIn ->
            { item with
                Quality = quality + 1
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


type GildedRose(items: IList<Item>) =
    let mutable Items = items

    member __.UpdateQuality() =

        for i = 0 to Items.Count - 1 do

            let oldItem = Items.[i]
            let newItem =
                match oldItem.Name |> ShopItem.TryFrom with
                | Some VestofDex
                | Some Elixir
                | Some Cake -> oldItem |> Updaters.standard
                | Some AgedBrie -> oldItem |> Updaters.brie
                | Some Sulfuras -> oldItem |> Updaters.sulfuras
                | Some BackstagePasses ->  oldItem |> Updaters.backstagePasses
                | _ -> failwith "we shouldn't be able to get here"

            Items.[i] <- newItem
        ()


module Program =
    [<EntryPoint>]
    let main argv =
        printfn "OMGHAI!"
        let Items = new List<Item>()

        Items.Add(
            { Name = "+5 Dexterity Vest"
              SellIn = 10
              Quality = 20 }
        )

        Items.Add(
            { Name = "Aged Brie"
              SellIn = 2
              Quality = 0 }
        )

        Items.Add(
            { Name = "Elixir of the Mongoose"
              SellIn = 5
              Quality = 7 }
        )

        Items.Add(
            { Name = "Sulfuras, Hand of Ragnaros"
              SellIn = 0
              Quality = 80 }
        )

        Items.Add(
            { Name = "Sulfuras, Hand of Ragnaros"
              SellIn = -1
              Quality = 80 }
        )

        Items.Add(
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              SellIn = 15
              Quality = 20 }
        )

        Items.Add(
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              SellIn = 10
              Quality = 49 }
        )

        Items.Add(
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              SellIn = 5
              Quality = 49 }
        )

        Items.Add(
            { Name = "Conjured Mana Cake"
              SellIn = 3
              Quality = 6 }
        )

        let app = new GildedRose(Items)

        for i = 0 to 30 do
            printfn "-------- day %d --------" i
            printfn "name, sellIn, quality"

            for j = 0 to Items.Count - 1 do
                printfn "%s, %d, %d" Items.[j].Name Items.[j].SellIn Items.[j].Quality

            printfn ""
            app.UpdateQuality()

        0
