﻿using EddiDataDefinitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EddiEvents
{
    public class ShipSoldEvent : Event
    {
        public const string NAME = "Ship sold";
        public const string DESCRIPTION = "Triggered when you sell a ship";
        public const string SAMPLE = "{\"timestamp\":\"2016-06-10T14:32:03Z\",\"event\":\"ShipyardSell\",\"ShipType\":\"Adder\",\"SellShipID\":1,\"ShipPrice\":25000}";
        public static Dictionary<string, string> VARIABLES = new Dictionary<string, string>();

        static ShipSoldEvent()
        {
            VARIABLES.Add("shipid", "The ID of the ship that was sold");
            VARIABLES.Add("ship", "The ship that was sold");
            VARIABLES.Add("price", "The price for which the ship was sold");
        }

        [JsonProperty("shipid")]
        public int? shipid { get; private set; }

        [JsonProperty("ship")]
        public string ship { get; private set; }

        [JsonIgnore]
        public Ship Ship { get; private set; }

        [JsonProperty("price")]
        public long price { get; private set; }

        public ShipSoldEvent(DateTime timestamp, Ship ship, long price) : base(timestamp, NAME)
        {
            this.Ship = ship;
            this.ship = (ship == null ? null : ship.model);
            this.shipid = (ship == null ? (int?)null : ship.LocalId);
            this.price = price;
        }
    }
}
