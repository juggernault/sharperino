﻿#region

using System;
using LeagueSharp;
using System.Linq;
using LeagueSharp.Common;
using LX_Orbwalker;

#endregion

namespace D_Shyvana
{
    class Program
    {

        private const string ChampionName = "Shyvana";

        private static Spell _q, _w, _e, _r;

        private static Menu _config;

        private static Obj_AI_Hero _player;

        private static Int32 _lastSkin;

        private static Items.Item _Tiamat, _Hydra, _Blade, _Bilge, _Rand, _lotis;

        private static SpellSlot _igniteSlot;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 0);
            _w = new Spell(SpellSlot.W, 350f);
            _e = new Spell(SpellSlot.E, 925f);
            _r = new Spell(SpellSlot.R, 1000f);

            _e.SetSkillshot(0.25f, 60f, 1700, false, SkillshotType.SkillshotLine);
            _r.SetSkillshot(0.25f, 150f, 1500, false, SkillshotType.SkillshotLine);

            _Bilge = new Items.Item(3144, 475f);
            _Blade = new Items.Item(3153, 425f);
            _Hydra = new Items.Item(3074, 375f);
            _Tiamat = new Items.Item(3077, 375f);
            _Rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);

            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            //D Shyvana
            _config = new Menu("D-Shyvana", "D-Shyvana", true);

            //TargetSelector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);


            var orbwalkerMenu = new Menu("LX-Orbwalker", "LX-Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            _config.AddSubMenu(orbwalkerMenu);

            //Combo
            _config.AddSubMenu(new Menu("Combo", "Combo"));
            _config.SubMenu("Combo").AddItem(new MenuItem("UseQC", "Use Q")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWC", "Use W")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseEC", "Use E")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRC", "Use R")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("UseRE", "AutoR Min Targ")).SetValue(true);
            _config.SubMenu("Combo").AddItem(new MenuItem("MinTargets", "Ult when>=min enemy(COMBO)").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Items public static Int32 Tiamat = 3077, Hydra = 3074, Blade = 3153, Bilge = 3144, Rand = 3143, lotis = 3190;
            _config.AddSubMenu(new Menu("items", "items"));
            _config.SubMenu("items").AddSubMenu(new Menu("Offensive", "Offensive"));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Tiamat", "Use Tiamat")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Hydra", "Use Hydra")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "Use Bilge")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("BilgeEnemyhp", "If Enemy Hp < ").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilgemyhp", "Or your Hp < ").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "Use Blade")).SetValue(true);
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("BladeEnemyhp", "If Enemy Hp < ").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blademyhp", "Or Your  Hp < ").SetValue(new Slider(85, 1, 100)));
            _config.SubMenu("items").AddSubMenu(new Menu("Deffensive", "Deffensive"));
            _config.SubMenu("items").SubMenu("Deffensive").AddItem(new MenuItem("Omen", "Use Randuin Omen")).SetValue(true);
            _config.SubMenu("items").SubMenu("Deffensive").AddItem(new MenuItem("Omenenemys", "Randuin if enemys>").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("items").SubMenu("Deffensive").AddItem(new MenuItem("lotis", "Use Iron Solari")).SetValue(true);
            _config.SubMenu("items").SubMenu("Deffensive").AddItem(new MenuItem("lotisminhp", "Solari if Ally Hp<  ").SetValue(new Slider(35, 1, 100)));
            /*_config.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotion", "Use Hp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotionuse", "Use Hp potion if HP<").SetValue(new Slider(35, 1, 100)));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotion", "Use Mp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotionuse", "Use Mp potion if HP<").SetValue(new Slider(35, 1, 100)));
            */
            //Harass
            _config.AddSubMenu(new Menu("Harass", "Harass"));
            _config.SubMenu("Harass").AddItem(new MenuItem("UseQH", "Use Q")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseWH", "Use W")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseEH", "Use E")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("UseItemsharass", "Use Tiamat/Hydra")).SetValue(true);
            _config.SubMenu("Harass").AddItem(new MenuItem("harasstoggle", "AutoHarass (toggle)").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));
            _config.SubMenu("Harass").AddItem(new MenuItem("ActiveHarass", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //LaneClear
            _config.AddSubMenu(new Menu("Farm", "Farm"));
            _config.SubMenu("Farm").AddItem(new MenuItem("UseItemslane", "Use Items in LaneClear")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseQL", "Q LaneClear")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseWL", "W LaneClear")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseEL", "E LaneClear")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseQLH", "Q LastHit")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseWLH", "W LastHit")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseELH", "E LastHit")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseQJ", "Q Jungle")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseWJ", "W Jungle")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("UseEJ", "E Jungle")).SetValue(true);
            _config.SubMenu("Farm").AddItem(new MenuItem("ActiveLast", "LastHit!").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Farm").AddItem(new MenuItem("ActiveLane", "LaneClear/Jungle!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Forest
            _config.AddSubMenu(new Menu("Forest Gump", "Forest Gump"));
            _config.SubMenu("Forest Gump").AddItem(new MenuItem("UseWF", "Use W ")).SetValue(true);
            _config.SubMenu("Forest Gump").AddItem(new MenuItem("UseEF", "Use E ")).SetValue(true);
            _config.SubMenu("Forest Gump").AddItem(new MenuItem("UseRF", "Use R ")).SetValue(true);
            _config.SubMenu("Forest Gump").AddItem(new MenuItem("Forest", "Active Forest Gump!").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));


            //Misc
            _config.AddSubMenu(new Menu("Misc", "Misc"));
            _config.SubMenu("Misc").AddItem(new MenuItem("UseEM", "Use E KillSteal")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseRM", "Use R KillSteal")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("Gap_E", "R GapClosers")).SetValue(true);
            _config.SubMenu("Misc").AddItem(new MenuItem("UseRInt", "R to Interrupt")).SetValue(true);
            // _config.SubMenu("Misc").AddItem(new MenuItem("MinTargetsgap", "min enemy >=(GapClosers)").SetValue(new Slider(2, 1, 5)));
            _config.SubMenu("Misc").AddItem(new MenuItem("skinshy", "Use Custom Skin").SetValue(false));
            _config.SubMenu("Misc").AddItem(new MenuItem("skinshyvana", "Skin Changer").SetValue(new Slider(4, 1, 6)));
            _config.SubMenu("Misc").AddItem(new MenuItem("usePackets", "Usepackes")).SetValue(true);

            //Misc
            _config.AddSubMenu(new Menu("HitChance", "HitChance"));
            _config.SubMenu("HitChance")
                               .AddItem(new MenuItem("Echange", "E Hit").SetValue(
                                new StringList(new[] { "Low", "Medium", "High", "Very High" })));
            _config.SubMenu("HitChance")
                               .AddItem(new MenuItem("Rchange", "R Hit").SetValue(
                                new StringList(new[] { "Low", "Medium", "High", "Very High" })));


            //Drawings
            _config.AddSubMenu(new Menu("Drawings", "Drawings"));
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            _config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            _config.AddToMainMenu();
            Game.PrintChat("<font color='#881df2'>D-Shyvana by Diabaths</font> Loaded.");
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            if (_config.Item("skinshy").GetValue<bool>())
            {
                GenModelPacket(_player.ChampionName, _config.Item("skinshyvana").GetValue<Slider>().Value);
                _lastSkin = _config.Item("skinshyvana").GetValue<Slider>().Value;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_config.Item("skinshy").GetValue<bool>() && SkinChanged())
            {
                GenModelPacket(_player.ChampionName, _config.Item("skinshyvana").GetValue<Slider>().Value);
                _lastSkin = _config.Item("skinshyvana").GetValue<Slider>().Value;
            }
            if (_config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (_config.Item("ActiveHarass").GetValue<KeyBind>().Active || _config.Item("harasstoggle").GetValue<KeyBind>().Active)
            {
                Harass();

            }
            if (_config.Item("ActiveLane").GetValue<KeyBind>().Active)
            {
                Laneclear();
                JungleClear();

            }
            if (_config.Item("ActiveLast").GetValue<KeyBind>().Active)
            {
                LastHit();
            }
            if (_config.Item("Forest").GetValue<KeyBind>().Active)
            {
                Forest();
            }

            _player = ObjectManager.Player;


            LXOrbwalker.SetAttack(true);

            KillSteal();
        }
        static void GenModelPacket(string champ, int skinId)
        {
            Packet.S2C.UpdateModel.Encoded(new Packet.S2C.UpdateModel.Struct(_player.NetworkId, skinId, champ)).Process();
        }

        static bool SkinChanged()
        {
            return (_config.Item("skinshyvana").GetValue<Slider>().Value != _lastSkin);
        }
        private static float ComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown &&
               _player.SummonerSpellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += _player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q) * 1.2;
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q) * 3;
            if (_e.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);

            damage += _player.GetAutoAttackDamage(enemy, true) * 1.1;
            damage += _player.GetAutoAttackDamage(enemy, true);
            return (float)damage;
        }
        private static void Combo()
        {
            var useQ = _config.Item("UseQC").GetValue<bool>();
            var useW = _config.Item("UseWC").GetValue<bool>();
            var useE = _config.Item("UseEC").GetValue<bool>();
            var useR = _config.Item("UseRC").GetValue<bool>();
            var autoR = _config.Item("UseRE").GetValue<bool>();

            var t = SimpleTs.GetTarget(_r.Range, SimpleTs.DamageType.Magical);

            if (t.Distance(_player.Position) < _w.Range + 250)
            {
                if (useW && _w.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) < _e.Range)
                        _w.Cast();

                }
                if (useE && _e.IsReady())
                {

                    if (t != null && t.Distance(_player.Position) < _e.Range &&
                        _e.GetPrediction(t).Hitchance >= Echange())
                        _e.Cast(t, Packets(), true);
                }
                if (useQ && _q.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) < _w.Range)
                        _q.Cast();
                }
                if (useR && _r.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) > _w.Range + 150)
                        if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") &&
                            _r.GetDamage(t) > t.Health
                            && _r.GetPrediction(t).Hitchance >= Rchange())
                            _r.Cast(t, Packets(), true);
                }
            }
            else if (t.Distance(_player.Position) > _w.Range + 250)
            {

                if (useR && _r.IsReady() && !_w.IsReady())
                {
                    if (t != null && _r.GetPrediction(t).Hitchance >= Rchange())
                        _r.Cast(t, Packets(), true);
                }
                if (useW && _w.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) < _e.Range)
                        _w.Cast();
                }
                if (useE && _e.IsReady())
                {

                    if (t != null && t.Distance(_player.Position) < _e.Range &&
                        _e.GetPrediction(t).Hitchance >= Echange())
                        _e.Cast(t, Packets(), true);
                }
                if (useQ && _q.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) < _w.Range)
                        _q.Cast();
                }
            }
            else if (t.Distance(_player.Position) < _r.Range)
            {

                if (_r.IsReady() && autoR)
                {
                    if (ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(_r.Range)) >=
                        _config.Item("MinTargets").GetValue<Slider>().Value
                        && _r.GetPrediction(t).Hitchance >= Rchange())
                        _r.Cast(t, Packets(), true);
                }
                if (useW && _w.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) < _e.Range)
                        _w.Cast();
                }
                if (useE && _e.IsReady())
                {

                    if (t != null && t.Distance(_player.Position) < _e.Range &&
                        _e.GetPrediction(t).Hitchance >= Echange())
                        _e.Cast(t, Packets(), true);
                }
                if (useQ && _q.IsReady())
                {
                    if (t != null && t.Distance(_player.Position) < _w.Range)
                        _q.Cast();
                }
            }
            UseItemes(t);
        }

        private static void Harass()
        {
            var target = SimpleTs.GetTarget(_r.Range, SimpleTs.DamageType.Magical);
            var useQ = _config.Item("UseQH").GetValue<bool>();
            var useW = _config.Item("UseWH").GetValue<bool>();
            var useE = _config.Item("UseEH").GetValue<bool>();
            var useItemsH = _config.Item("UseItemsharass").GetValue<bool>();
            if (useQ && _q.IsReady())
            {
                var t = SimpleTs.GetTarget(_w.Range, SimpleTs.DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _w.Range)
                    _q.Cast();
            }
            if (useW && _w.IsReady())
            {
                var t = SimpleTs.GetTarget(_w.Range, SimpleTs.DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _w.Range)
                    _w.Cast();
            }
            if (useE && _e.IsReady())
            {
                var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _e.Range && _e.GetPrediction(t).Hitchance >= Echange())
                    _e.Cast(t, Packets(), true);
            }
            if (useItemsH && _Tiamat.IsReady() && target.Distance(_player.Position) < _Tiamat.Range)
            {
                _Tiamat.Cast();
            }
            if (useItemsH && _Hydra.IsReady() && target.Distance(_player.Position) < _Hydra.Range)
            {
                _Hydra.Cast();
            }
        }

        private static void Laneclear()
        {
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _w.Range, MinionTypes.All);
            var rangedMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range + _e.Width,
                MinionTypes.Ranged);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range + _e.Width,
                MinionTypes.All);
            var useItemsl = _config.Item("UseItemslane").GetValue<bool>();
            var useQl = _config.Item("UseQL").GetValue<bool>();
            var useWl = _config.Item("UseWL").GetValue<bool>();
            var useEl = _config.Item("UseEL").GetValue<bool>();
            if (_q.IsReady() && useQl && allMinionsW.Count > 0)
            {
                _q.Cast();
            }

            if (_w.IsReady() && useWl)
            {

                if (allMinionsW.Count >= 2)
                {
                    _w.Cast();
                }
                else
                    foreach (var minion in allMinionsW)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W))
                            _w.Cast();
            }
            if (_e.IsReady() && useEl)
            {
                var fl1 = _e.GetLineFarmLocation(rangedMinionsE, _e.Width);
                var fl2 = _e.GetLineFarmLocation(allMinionsE, _e.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _e.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsE.Count == 1)
                {
                    _e.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                            _e.Cast(minion);
            }

            if (useItemsl && _Tiamat.IsReady() && allMinionsW.Count > 0)
            {
                _Tiamat.Cast();
            }
            if (useItemsl && _Hydra.IsReady() && allMinionsW.Count > 0)
            {
                _Hydra.Cast();
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range, MinionTypes.All);
            var useQ = _config.Item("UseQLH").GetValue<bool>();
            var useW = _config.Item("UseWLH").GetValue<bool>();
            var useE = _config.Item("UseELH").GetValue<bool>();
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && _player.Distance(minion) < 200 && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useW && _player.Distance(minion) < _w.Range && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W))
                {
                    _w.Cast();
                }
                if (_e.IsReady() && useE && _player.Distance(minion) < _e.Range && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast(minion);
                }
            }
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _w.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var UseItemsJ = _config.Item("UseItemslane").GetValue<bool>();
            var useQ = _config.Item("UseQJ").GetValue<bool>();
            var useW = _config.Item("UseWJ").GetValue<bool>();
            var useE = _config.Item("UseEJ").GetValue<bool>();
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && _q.IsReady())
                {
                    _q.Cast();
                }
                if (_w.IsReady() && useW)
                {
                    _w.Cast(mob, Packets());
                }
                if (_e.IsReady() && useE)
                {
                    _e.Cast(mob, Packets());
                }
            }
            if (UseItemsJ && _Tiamat.IsReady() && mobs.Count > 0)
            {
                _Tiamat.Cast();
            }
            if (UseItemsJ && _Hydra.IsReady() && mobs.Count > 0)
            {
                _Hydra.Cast();
            }

        }

        private static HitChance Echange()
        {
            switch (_config.Item("Echange").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }
        private static HitChance Rchange()
        {
            switch (_config.Item("Rchange").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private static bool Packets()
        {
            return _config.Item("usePackets").GetValue<bool>();
        }

        private static void KillSteal()
        {

            if (_e.IsReady() && _config.Item("UseEM").GetValue<bool>())
            {
                var t = SimpleTs.GetTarget(_e.Range, SimpleTs.DamageType.Magical);
                if (_e.GetDamage(t) > t.Health && _player.Distance(t) <= _e.Range)
                {
                    _e.CastIfHitchanceEquals(t, Echange(), Packets());
                }
            }
            if (_r.IsReady() && _config.Item("UseRM").GetValue<bool>())
            {
                var t = SimpleTs.GetTarget(_r.Range, SimpleTs.DamageType.Magical);
                if (t != null)
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") &&
                        _r.GetDamage(t) > t.Health && _r.GetPrediction(t).Hitchance >= Rchange())
                        _r.Cast(t, Packets(), true);
            }
        }
        private static void UseItemes(Obj_AI_Hero target)
        {
            var iBilge = _config.Item("Bilge").GetValue<bool>();
            var iBilgeEnemyhp = target.Health <= (target.MaxHealth * (_config.Item("BilgeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBilgemyhp = _player.Health <= (_player.MaxHealth * (_config.Item("Bilgemyhp").GetValue<Slider>().Value) / 100);
            var iBlade = _config.Item("Blade").GetValue<bool>();
            var iBladeEnemyhp = target.Health <= (target.MaxHealth * (_config.Item("BladeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBlademyhp = _player.Health <= (_player.MaxHealth * (_config.Item("Blademyhp").GetValue<Slider>().Value) / 100);
            var iOmen = _config.Item("Omen").GetValue<bool>();
            var iOmenenemys = ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(450)) >=
                              _config.Item("Omenenemys").GetValue<Slider>().Value;
            var iTiamat = _config.Item("Tiamat").GetValue<bool>();
            var iHydra = _config.Item("Hydra").GetValue<bool>();
            var ilotis = _config.Item("lotis").GetValue<bool>();
            //var ihp = _config.Item("Hppotion").GetValue<bool>();
            // var ihpuse = _player.Health <= (_player.MaxHealth * (_config.Item("Hppotionuse").GetValue<Slider>().Value) / 100);
            //var imp = _config.Item("Mppotion").GetValue<bool>();
            //var impuse = _player.Health <= (_player.MaxHealth * (_config.Item("Mppotionuse").GetValue<Slider>().Value) / 100);

            if (_player.Distance(target) <= 450 && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _Bilge.IsReady())
            {
                _Bilge.Cast(target);

            }
            if (_player.Distance(target) <= 450 && iBlade && (iBladeEnemyhp || iBlademyhp) && _Blade.IsReady())
            {
                _Blade.Cast(target);

            }
            if (Utility.CountEnemysInRange(350) >= 1 && iTiamat && _Tiamat.IsReady())
            {
                _Tiamat.Cast(target);

            }
            if (Utility.CountEnemysInRange(350) >= 1 && iHydra && _Hydra.IsReady())
            {
                _Hydra.Cast(target);

            }
            if (iOmenenemys && iOmen && _Rand.IsReady())
            {
                _Rand.Cast();

            }
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (_config.Item("lotisminhp").GetValue<Slider>().Value) / 100) &&
                        hero.Distance(_player.ServerPosition) <= _lotis.Range && _lotis.IsReady())
                        _lotis.Cast();
                }
            }

        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_r.IsReady() && gapcloser.Sender.IsValidTarget(_r.Range) && _config.Item("Gap_E").GetValue<bool>())
            {
                _r.Cast(Game.CursorPos, Packets());
            }
        }
        private static void Interrupter_OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (!_config.Item("UseRInt").GetValue<bool>()) return;
            if (_player.Distance(target) < _r.Range && target != null && _r.GetPrediction(target).Hitchance >= HitChance.Low)
            {
                _r.Cast(target, Packets());
            }
        }

        private static void Forest()
        {

            var target = SimpleTs.GetTarget(_r.Range, SimpleTs.DamageType.Magical);

            if (_config.Item("UseRF").GetValue<bool>() && _r.IsReady() && target != null)
            {
                _r.Cast(Game.CursorPos, Packets());
            }
            if (_config.Item("UseWF").GetValue<bool>() && _w.IsReady() && target != null)
            {
                _w.Cast();
            }
            if (_config.Item("UseEF").GetValue<bool>() && _e.IsReady() && _player.Distance(target) < _e.Range)
            {
                _e.Cast(target, Packets());
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_config.Item("CircleLag").GetValue<bool>())
            {
                if (_config.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.Gray,
                        _config.Item("CircleThickness").GetValue<Slider>().Value,
                        _config.Item("CircleQuality").GetValue<Slider>().Value);
                }
            }
            else
            {
                if (_config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawW").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.White);
                }
                if (_config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White);
                }

                if (_config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.White);
                }

            }
        }
    }
}