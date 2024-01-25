using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Native;
using GTA;
using GTA.Math;
using System.Drawing;

namespace Health_Bars
{
    public class Main : Script
    {
        public static (PointF, float)[] HealthBars(PointF MiddleScreen)
        {
            return new (PointF, float)[8] {
            (new PointF(MiddleScreen.X+0.75f,MiddleScreen.Y-17), 20f),
            (new PointF(MiddleScreen.X+9.5f,MiddleScreen.Y-8), 65f),
            (new PointF(MiddleScreen.X+9.5f,MiddleScreen.Y+4), 115f),
            (new PointF(MiddleScreen.X+0.75f,MiddleScreen.Y+13), 160f),

            (new PointF(MiddleScreen.X-12.25f,MiddleScreen.Y+13), 360f-160f),
            (new PointF(MiddleScreen.X-21.5f,MiddleScreen.Y+4), 360f-115f),
            (new PointF(MiddleScreen.X-21.5f,MiddleScreen.Y-8), 360f-65f),
            (new PointF(MiddleScreen.X-12.25f,MiddleScreen.Y-17), 360f-20f),
            };
        }

        public static (PointF, float)[] ArmorBars(PointF MiddleScreen)
        {
            return new (PointF, float)[6] {
            (new PointF(MiddleScreen.X+6,MiddleScreen.Y-10), 35f),
            (new PointF(MiddleScreen.X+12,MiddleScreen.Y), 90f),
            (new PointF(MiddleScreen.X+6,MiddleScreen.Y+10), 145f),

            (new PointF(MiddleScreen.X-6,MiddleScreen.Y+10), 360f-145f),
            (new PointF(MiddleScreen.X-12,MiddleScreen.Y), 360f-90f),
            (new PointF(MiddleScreen.X-6,MiddleScreen.Y-10), 360f-35f),
            };
        }

        public static float CamFov;
        public static int DelayTime;
        public Main()
        {
            Interval = 0;
            Tick += MainTick;
        }

        public static Ped LastTargetPed;

        public static void MainTick(object sender, EventArgs e)
        {
            if(!Game.Player.Character.IsInVehicle() && Game.Player.Character.Weapons.Current.Group != WeaponGroup.Sniper)GTA.UI.Hud.HideComponentThisFrame(GTA.UI.HudComponent.Reticle);
            bool ReadyToShoot = false;
           
            if ((Game.Player.Character.IsAiming || Game.Player.Character.IsInMeleeCombat || (Game.Player.Character.IsReloading && CamFov<48)) && DelayTime < Game.GameTime)
            {
                ReadyToShoot = true;
            }
            CamFov = GTA.GameplayCamera.FieldOfView;

            if (ReadyToShoot && Game.Player.Character.Weapons.Current.Group != WeaponGroup.Sniper)
            {
                PointF MiddleScreen = new Point((int)(GTA.UI.Screen.Width / 2), (int)(GTA.UI.Screen.Height / 2));
                Ped TargetPed = Game.Player.Character;
                if (Game.Player.TargetedEntity != null && Game.Player.TargetedEntity.EntityType == EntityType.Ped)
                {
                    TargetPed = (Ped)Game.Player.TargetedEntity;
                    if(!TargetPed.IsInVehicle())LastTargetPed = TargetPed;
                }
                bool MeeleCombat = Game.Player.Character.IsInMeleeCombat;
                if (MeeleCombat)
                {
                    if (LastTargetPed != null && LastTargetPed.IsAlive)TargetPed= LastTargetPed;
                    MiddleScreen = GTA.UI.Screen.WorldToScreen(TargetPed.Bones[Bone.SkelSpine3].Position);
                }
                if (TargetPed == Game.Player.Character && Game.Player.Character.IsInMeleeCombat) return;
                else if(Game.Player.IsTargetingAnything || Game.Player.Character.IsAiming)
                {
                    new GTA.UI.CustomSprite(@".\scripts\Okoniewitz\HealthBars\Circle.png", new SizeF(40, 40), MiddleScreen, Color.White, 0f, true).Draw();
                    new GTA.UI.TextElement(".", new PointF(MiddleScreen.X, MiddleScreen.Y - 18), 0.5f, Color.White, GTA.UI.Font.ChaletComprimeCologne, GTA.UI.Alignment.Center).Draw();
                }
                if (Game.Player.Character == TargetPed) return;

                if ((Game.Player.Character.IsAiming || Game.Player.IsTargetingAnything) && TargetPed.IsAlive)
                {
                    if (!TargetPed.IsInVehicle())
                    {

                        float hp = ((TargetPed.HealthFloat - 100) / (TargetPed.MaxHealthFloat - 100)) * 100;
                        float ap = TargetPed.ArmorFloat;
                        if (hp < 0) hp = 0;
                        bool Enemy = false;
                        Enemy = (TargetPed.IsInCombatAgainst(Game.Player.Character));
                        Color enemyColor = Color.FromArgb(200, 63, 120, 69);
                        if (Enemy) enemyColor = Color.FromArgb(200, 151, 69, 64);
                            for (int i = 0; i < Math.Ceiling((hp * 10) / 125); i++)
                            {
                                new GTA.UI.CustomSprite(@".\scripts\Okoniewitz\HealthBars\Dot.png", new SizeF(12, 3.5f), HealthBars(MiddleScreen)[i].Item1, enemyColor, HealthBars(MiddleScreen)[i].Item2).Draw();
                            }
                            for (int i = 0; i < Math.Ceiling(ap / 17f); i++)
                            {
                                new GTA.UI.CustomSprite(@".\scripts\Okoniewitz\HealthBars\Dot.png", new SizeF(12, 3f), ArmorBars(MiddleScreen)[i].Item1, Color.FromArgb(220, 23, 110, 146), ArmorBars(MiddleScreen)[i].Item2, true).Draw();
                            }
                    }
                }
            }
            if(!Game.Player.IsTargetingAnything && !Game.Player.Character.IsAiming && !Game.Player.Character.IsInMeleeCombat && (!Game.Player.Character.IsReloading || CamFov > 48))
            {
                DelayTime = Game.GameTime + 300;
            }
        }
    }
}
