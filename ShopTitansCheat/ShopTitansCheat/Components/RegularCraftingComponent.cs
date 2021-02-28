﻿using System;
using System.Collections.Generic;
using System.Linq;
using Riposte;
using Riposte.Sim;
using ShopTitansCheat.Data;
using ShopTitansCheat.Utils;

namespace ShopTitansCheat.Components
{
    class RegularCraftingComponent
    {
        internal void Craft()
        {
            if (Settings.RegularCrafting.CraftRandomItems)
            {
                CraftRandomStuffOverValue(Settings.RegularCrafting.CraftRandomStuffValue);
            }
            else if (Settings.RegularCrafting.CraftBookmarked)
            {
                CraftBookMarked();
            }
            else
            {
                foreach (Equipment item in Settings.RegularCrafting.CraftingEquipmentsList)
                {
                    if (item.Done)
                        continue;

                    if (!StartCraft(item.ShortName))
                    {
                        Log.Instance.PrintConsoleMessage($"Not enough for {item.FullName}, continuing", ConsoleColor.Red);
                        return;
                    }

                    Log.Instance.PrintConsoleMessage($"Sucesfully crafted {item.FullName}, {item.ItemQuality}", ConsoleColor.Green);
                    item.Done = true;
                    item.FullName = $"{item.FullName}, {item.Done}";

                    if (Settings.RegularCrafting.CraftingEquipmentsList.All(i => i.Done))
                    {
                        Log.Instance.PrintConsoleMessage($"Crafted everything in list. \tRestarting.", ConsoleColor.Green);

                        foreach (Equipment equipment in Settings.RegularCrafting.CraftingEquipmentsList)
                        {
                            equipment.FullName = equipment.FullName.Replace(", True", "");
                            equipment.Done = false;
                        }

                        return;
                    }
                }
            }
        }

        private bool CraftBookMarked()
        {
            foreach (var blueprint in Game.User.zt.Values.ToList())
            {
                if (Settings.RegularCrafting.CraftBookmarked)
                {
                    string fullName = Game.Texts.GetText(blueprint.ai8());


                    if (!blueprint.ck)
                        continue;

                    if (StartCraft(blueprint.b0))
                    {
                        Log.Instance.PrintConsoleMessage($"started crafting: {blueprint.b0}", ConsoleColor.Green);
                        return true;
                    }

                    Log.Instance.PrintConsoleMessage($"not enough materials too craft: {fullName}", ConsoleColor.Red);

                }
            }

            return false;
        }

        private bool CraftRandomStuffOverValue(int value)
        {
            var tmpList = Game.User.zt.Values.ToList();
            tmpList.Shuffle();
            foreach (var blueprint in tmpList)
            {
                ItemData itemData = Game.Data.ep(blueprint.b0);
                string fullName = Game.Texts.GetText(blueprint.ai8());

                if (!Settings.RegularCrafting.IncludeElements || !Settings.RegularCrafting.IncludeRune)
                    if (itemData.Type == ItemData.ItemType.Tag || itemData.Type == ItemData.ItemType.Rune)
                        continue;

                if (itemData.Value < value)
                    continue;

                if (StartCraft(blueprint.b0))
                {
                    Log.Instance.PrintConsoleMessage($"started crafting: {blueprint.b0}", ConsoleColor.Green);
                    return true;
                }

                Log.Instance.PrintConsoleMessage($"not enough materials too craft: {fullName}", ConsoleColor.Red);

                return false;
            }
            return false;
        }

        private bool StartCraft(string itemName)
        {
            if (dg.a0(Game.User.aof(), itemName).ar())
            {
                Game.SimManager.SendUserAction("CraftItem", new Dictionary<string, object>
                {
                    {
                        "item",
                        itemName
                    }

                });
                Log.Instance.PrintMessageInGame(string.Format(Game.Texts.GetText("craft_started"), Game.Texts.GetText(itemName)), OverlayMessageControl.MessageType.Neutral);
                return true;
            }
            return false;
        }
    }
}