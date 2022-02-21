module StartingBehaviorTests

open System.Collections.Generic

open Expecto

open GildedRose

let dexVest = "+5 Dexterity Vest"
let brie = "Aged Brie"
let elixir = "Elixir of the Mongoose"
let sulfuras = "Sulfuras, Hand of Ragnaros"

let backstagePasses =
    "Backstage passes to a TAFKAL80ETC concert"

let cake = "Conjured Mana Cake"

let makeItem name sellIn quality =
    let item =
        { Name = name
          SellIn = sellIn
          Quality = quality }

    let list = List<Item>()
    list.Add(item)
    list

[<Tests>]
let standardItemsTests =
    testList
        "Testing UpdateQuality for Standard Items"
        [ testCase "Dexterity Vest Quality Should Decrease"
          <| fun _ ->
              let items = makeItem VestofDex 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should decrease by 1"

          testCase "Dexterity Vest SellIn Should Decrease"
          <| fun _ ->
              let items = makeItem VestofDex 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].SellIn 0 "SellIn should decrease by 1"

          testCase "Dexterity Vest Quality Should Stop at Zero"
          <| fun _ ->
              let items = makeItem VestofDex 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should not decrease"

          testCase "Dexterity Vest Quality Should Degrade Twice as Fast with 0 days left to Sell"
          <| fun _ ->
              let items = makeItem VestofDex 0 2

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should not decrease"

          testCase "Elixir of the Mongoose Quality Should Decrease"
          <| fun _ ->
              let items = makeItem Elixir 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should decrease by 1"

          testCase "Elixir of the Mongoose SellIn Should Decrease"
          <| fun _ ->
              let items = makeItem Elixir 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].SellIn 0 "SellIn should decrease by 1"

          testCase "Elixir of the Mongoose  Quality Should Stop at Zero"
          <| fun _ ->
              let items = makeItem Elixir 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should not decrease"

          testCase "Elixir of the Mongoose Quality Should Degrade Twice as Fast with 0 days left to Sell"
          <| fun _ ->
              let items = makeItem Elixir 0 2

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should not decrease" ]

[<Tests>]
let conjuredItemTests =
    testList
        "Testing Conjured Items"
        [ testCase "Mana Cake Quality Should Decrease Twice as Fast as Normal Items"
          <| fun _ ->
              let items = makeItem Cake 1 2

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should decrease by 1"

          testCase "Mana Cake SellIn Should Decrease"
          <| fun _ ->
              let items = makeItem Cake 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].SellIn 0 "SellIn should decrease by 1"

          testCase "Mana Cake Quality Should Stop at Zero"
          <| fun _ ->
              let items = makeItem Cake 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should not decrease"

          testCase "Mana Cake Quality Should Degrade Twice as Fast with 0 days left to Sell"
          <| fun _ ->
              let items = makeItem Cake 0 4

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should not decrease"

          ]


[<Tests>]
let brieTests =
    testList
        "Testing Aged Brie"
        [ testCase "Aged Brie Quality Should Increase"
          <| fun _ ->
              let items = makeItem AgedBrie 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 2 "Quality should increase by 1"

          testCase "Aged Brie SellIn Should Decrease"
          <| fun _ ->
              let items = makeItem AgedBrie 1 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].SellIn 0 "SellIn should decrease by 1"

          testCase "Aged Brie Quality Should Not Increase Past a Maximum"
          <| fun _ ->
              let items = makeItem AgedBrie 1 50

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 50 "Quality should not increase past 50"

          testCase "Aged Brie Quality Should Increase Twice as Fast with 0 Days Left"
          <| fun _ ->
              let items = makeItem AgedBrie 0 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 3 "Quality should increase by 2" ]

[<Tests>]
let backstagePassTests =
    testList
        "Testing Backstage Pass Quality Updates"
        [ testCase "Backstage Pass Quality increases by 2 between 5 and 10 days left"
          <| fun _ ->
              let items = makeItem BackstagePasses 7 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 3 "Quality should increase by 2"

          testCase "Backstage Pass SellIn should decrease"
          <| fun _ ->
              let items = makeItem BackstagePasses 7 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].SellIn 6 "SellIn should decrease by 1"

          testCase "Backstage Pass Quality increases by 3 between 1 and 5 days left"
          <| fun _ ->
              let items = makeItem BackstagePasses 3 1

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 4 "Quality should increase by 3"

          testCase "Backstage Pass Quality goes to 0 with 0 or fewer days left"
          <| fun _ ->
              let items = makeItem BackstagePasses 0 10

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 0 "Quality should go to 0"

          testCase "Backstage Pass Quality should not increase past 50"
          <| fun _ ->
              let items = makeItem BackstagePasses 3 49

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 50 "Quality should not exceed 50" ]

[<Tests>]
let sulfurasTests =
    testList
        "Testing Sulfuras Quality"
        [ testCase "Should always be 80"
          <| fun _ ->
              let items = makeItem Sulfuras 3 80

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].Quality 80 "Quality should always be 80"

          testCase "SellIn Should Not Change"
          <| fun _ ->
              let items = makeItem Sulfuras 3 80

              let gr = GildedRose items

              gr.UpdateQuality()

              Expect.equal gr.Items.[0].SellIn 3 "SellIn should not change" ]
