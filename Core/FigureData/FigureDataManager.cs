using System;
using System.Linq;
using System.Collections.Generic;
using log4net;
using Cloud.Core.FigureData.Types;
using System.Xml;
using Cloud.HabboHotel.Catalog.Clothing;
using Cloud.HabboHotel.Users.Clothing.Parts;
using Cloud.Core.FigureDataManager;

namespace Cloud.Core.FigureData
{
    public class FigureDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.Core.FigureData");
        private bool _legacyEnabled;
        private readonly LegacyFigureMutant _legacy;
        private readonly List<string> _requirements;
        private readonly Dictionary<int, Palette> _palettes; //pallet id, Pallet
        private readonly Dictionary<string, FigureSet> _setTypes; //type (hr, ch, etc), Set
        public FigureDataManager(bool legacyEnabled)
        {
            this._legacyEnabled = legacyEnabled;
            this._legacy = new LegacyFigureMutant();
            this._palettes = new Dictionary<int, Palette>();
            this._setTypes = new Dictionary<string, FigureSet>();
            this._requirements = new List<string>();
            this._requirements.Add("hd");
            this._requirements.Add("ch");
            this._requirements.Add("lg");
        }
        public void Init()
        {
            if (this._legacyEnabled == true)
            {
                this._legacy.Init();
                log.Info("» Figure Manager -> CARGADO.");
                return;
            }
            if (this._palettes.Count > 0)
                this._palettes.Clear();
            if (this._setTypes.Count > 0)
                this._setTypes.Clear();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"extra/figuredata.xml");
            XmlNodeList Colors = xDoc.GetElementsByTagName("colors");
            foreach (XmlNode Node in Colors)
            {
                foreach (XmlNode Child in Node.ChildNodes)
                {
                    this._palettes.Add(Convert.ToInt32(Child.Attributes["id"].Value), new Palette(Convert.ToInt32(Child.Attributes["id"].Value)));
                    foreach (XmlNode Sub in Child.ChildNodes)
                    {
                        this._palettes[Convert.ToInt32(Child.Attributes["id"].Value)].Colors.Add(Convert.ToInt32(Sub.Attributes["id"].Value), new Color(Convert.ToInt32(Sub.Attributes["id"].Value), Convert.ToInt32(Sub.Attributes["index"].Value), Convert.ToInt32(Sub.Attributes["club"].Value), Convert.ToInt32(Sub.Attributes["selectable"].Value) == 1, Convert.ToString(Sub.InnerText)));
                    }
                }
            }
            XmlNodeList sets = xDoc.GetElementsByTagName("sets");
            foreach (XmlNode node in sets)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    this._setTypes.Add(childNode.Attributes["type"].Value, new FigureSet(SetTypeUtility.GetSetType(childNode.Attributes["type"].Value), Convert.ToInt32(childNode.Attributes["paletteid"].Value)));
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        int id = 0;
                        if (int.TryParse(subChild.Attributes["id"].Value, out id))
                        {
                            if (!this._setTypes[childNode.Attributes["type"].Value].Sets.ContainsKey(id))
                            {
                                if (subChild.Attributes["selectable"] == null || subChild.Attributes["preselectable"] == null)
                                {
                                    continue;
                                }
                                this._setTypes[childNode.Attributes["type"].Value].Sets.Add(Convert.ToInt32(subChild.Attributes["id"].Value), new Set(Convert.ToInt32(subChild.Attributes["id"].Value), Convert.ToString(subChild.Attributes["gender"].Value), Convert.ToInt32(subChild.Attributes["club"].Value), Convert.ToInt32(subChild.Attributes["colorable"].Value) == 1, Convert.ToInt32(subChild.Attributes["selectable"].Value) == 1, Convert.ToInt32(subChild.Attributes["preselectable"].Value) == 1));
                                foreach (XmlNode grandChild in subChild.ChildNodes)
                                {
                                    if (grandChild.Attributes["type"] != null)
                                    {
                                        if (!this._setTypes[childNode.Attributes["type"].Value].Sets[id].Parts.ContainsKey(Convert.ToInt32(grandChild.Attributes["id"].Value) + "-" + grandChild.Attributes["type"].Value))
                                        {
                                            this._setTypes[childNode.Attributes["type"].Value].Sets[id].Parts.Add(Convert.ToInt32(grandChild.Attributes["id"].Value) + "-" + grandChild.Attributes["type"].Value,
                                      new Part(Convert.ToInt32(grandChild.Attributes["id"].Value), SetTypeUtility.GetSetType(childNode.Attributes["type"].Value), Convert.ToInt32(grandChild.Attributes["colorable"].Value) == 1, Convert.ToInt32(grandChild.Attributes["index"].Value), Convert.ToInt32(grandChild.Attributes["colorindex"].Value)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Faceless.
            this._setTypes["hd"].Sets.Add(99999, new Set(99999, "U", 0, true, false, false));
            log.Info("Loaded " + this._palettes.Count + " Color Palettes");
            log.Info("Loaded " + this._setTypes.Count + " Set Types");
        }
        public string ProcessFigure(string figure, string gender, ICollection<ClothingParts> clothingParts, bool hasHabboClub)
        {
            if (this._legacyEnabled)
                return this._legacy.RunLook(figure);
            figure = figure.ToLower();
            gender = gender.ToUpper();
            string rebuildFigure = string.Empty;
            #region Check clothing, colors & Habbo Club
            string[] figureParts = figure.Split('.');
            foreach (string part in figureParts.ToList())
            {
                string type = part.Split('-')[0];
                FigureSet figureSet = null;
                if (this._setTypes.TryGetValue(type, out figureSet))
                {
                    int partId = Convert.ToInt32(part.Split('-')[1]);
                    int colorId = 0;
                    int secondColorId = 0;
                    Set set = null;
                    if (figureSet.Sets.TryGetValue(partId, out set))
                    {
                        #region Gender Check
                        if (set.Gender != gender && set.Gender != "U")
                        {
                            if (figureSet.Sets.Count(x => x.Value.Gender == gender || x.Value.Gender == "U") > 0)
                            {
                                partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                                //Fetch the new set.
                                figureSet.Sets.TryGetValue(partId, out set);
                                colorId = GetRandomColor(figureSet.PalletId);
                            }
                            else
                            {
                                //No replacable?
                            }
                        }
                        #endregion
                        #region Colors
                        if (set.Colorable)
                        {
                            //Couldn't think of a better way to split the colors, if I looped the parts I still have to remove Type-PartId, then loop color 1 & color 2. Meh
                            int splitterCounter = part.Count(x => x == '-');
                            if (splitterCounter == 2 || splitterCounter == 3)
                            {
                                #region First Color
                                if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                {
                                    if (int.TryParse(part.Split('-')[2], out colorId))
                                    {
                                        colorId = Convert.ToInt32(part.Split('-')[2]);
                                        Palette palette = GetPalette(colorId);
                                        if (palette != null && colorId != 0)
                                        {
                                            if (figureSet.PalletId != palette.Id)
                                            {
                                                colorId = GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else if (palette == null && colorId != 0)
                                        {
                                            colorId = GetRandomColor(figureSet.PalletId);
                                        }
                                    }
                                    else
                                        colorId = 0;
                                }
                                else
                                    colorId = 0;
                                #endregion
                            }
                            if (splitterCounter == 3)
                            {
                                #region Second Color
                                if (!string.IsNullOrEmpty(part.Split('-')[3]))
                                {
                                    if (int.TryParse(part.Split('-')[3], out secondColorId))
                                    {
                                        secondColorId = Convert.ToInt32(part.Split('-')[3]);
                                        Palette palette = GetPalette(secondColorId);
                                        if (palette != null && secondColorId != 0)
                                        {
                                            if (figureSet.PalletId != palette.Id)
                                            {
                                                secondColorId = GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else if (palette == null && secondColorId != 0)
                                        {
                                            secondColorId = GetRandomColor(figureSet.PalletId);
                                        }
                                    }
                                    else
                                        secondColorId = 0;
                                }
                                else
                                    secondColorId = 0;
                                #endregion
                            }
                        }
                        else
                        {
                            string[] ignore = new string[] { "ca", "wa" };
                            if (ignore.Contains(type))
                            {
                                if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                {
                                    colorId = Convert.ToInt32(part.Split('-')[2]);
                                }
                            }
                        }
                        #endregion
                        if (set.ClubLevel > 0 && !hasHabboClub)
                        {
                            partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U" && x.Value.ClubLevel == 0).Value.Id;
                            figureSet.Sets.TryGetValue(partId, out set);
                            colorId = GetRandomColor(figureSet.PalletId);
                        }
                        if (secondColorId == 0)
                            rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + ".";
                        else
                            rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + "-" + secondColorId + ".";
                    }
                }
            }
            #endregion
            #region Check Required Clothing
            foreach (string requirement in this._requirements)
            {
                if (!rebuildFigure.Contains(requirement))
                {
                    if (requirement == "ch" && gender == "M")
                        continue;
                    FigureSet figureSet = null;
                    if (this._setTypes.TryGetValue(requirement, out figureSet))
                    {
                        Set set = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value;
                        if (set != null)
                        {
                            int partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                            int colorId = GetRandomColor(figureSet.PalletId);
                            rebuildFigure = rebuildFigure + requirement + "-" + partId + "-" + colorId + ".";
                        }
                    }
                }
            }
            #endregion
            #region Check Purcashable Clothing
            if (clothingParts != null)
            {
                ICollection<ClothingItem> purchasableParts = CloudServer.GetGame().GetCatalog().GetClothingManager().GetClothingAllParts;
                figureParts = rebuildFigure.TrimEnd('.').Split('.');
                foreach (string part in figureParts.ToList())
                {
                    int partId = Convert.ToInt32(part.Split('-')[1]);
                    if (purchasableParts.Count(x => x.PartIds.Contains(partId)) > 0)
                    {
                        if (clothingParts.Count(x => x.PartId == partId) == 0)
                        {
                            string type = part.Split('-')[0];
                            FigureSet figureSet = null;
                            if (this._setTypes.TryGetValue(type, out figureSet))
                            {
                                Set set = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value;
                                if (set != null)
                                {
                                    partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                                    int colorId = GetRandomColor(figureSet.PalletId);
                                    rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + ".";
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            return rebuildFigure;
        }
        public Palette GetPalette(int colorId)
        {
            return this._palettes.FirstOrDefault(x => x.Value.Colors.ContainsKey(colorId)).Value;
        }
        public bool TryGetPalette(int palletId, out Palette palette)
        {
            return this._palettes.TryGetValue(palletId, out palette);
        }
        public int GetRandomColor(int palletId)
        {
            return this._palettes[palletId].Colors.FirstOrDefault().Value.Id;
        }
    }
}