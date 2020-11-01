using System;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Game.Data
{
	public class CurePotency
	{
		public PlayerTools Player { get; }
		public double Potency { get; }

		public double Cure
		{
			get
			{
				var mnd = Math.Floor(Player.Stats.Mind / 2.0);
				var vit = Math.Floor(Player.Stats.Vitality / 4.0);
				var skill = Player.CombatSkills.Healing.Skill;
				var power = mnd + vit + skill;

				var amount = power switch
				{
					var x when x >= 600 => 65,
					var x when x >= 200 => Math.Floor((((0 + power) - 200) / 20) + 45),
					var x when x >= 125 => Math.Floor((((0 + power) - 125) / 8.5) + 40),
					var x when x >= 40 => Math.Floor((((0 + power) - 40) / 8.5) + 30),
					var x when x >= 20 => Math.Floor((((0 + power) - 20) / 1.33) + 15),
					var x when x >= 0 => Math.Floor((((0 + power) - 0) / 1) + 10),
					_ => 0,
				};

				var bonus = Potency * (amount * 0.01);
				return Math.Round(amount + bonus) - (amount * 0.1);
			}
			set { /* ignore */ }
		}

		public double Cure2
		{
			get
			{
				var mnd = Math.Floor(Player.Stats.Mind / 2D);
				var vit = Math.Floor(Player.Stats.Vitality / 2D);
				var skill = Player.CombatSkills.Healing.Skill;
				var power = mnd + vit + skill;

				var amount = power switch
				{
					var x when x >= 700 => 145,
					var x when x >= 400 => Math.Floor((((0 + power) - 400) / 20.0) + 130),
					var x when x >= 200 => Math.Floor((((0 + power) - 200) / 10.0) + 110),
					var x when x >= 125 => Math.Floor((((0 + power) - 125) / 7.5) + 100),
					var x when x >= 70 => Math.Floor((((0 + power) - 70) / 5.5) + 90),
					_ => Math.Floor((((0 + power) - 40) / 1.0) + 60),
				};

				var bonus = Potency * (amount * 0.01);
				return Math.Round(amount + bonus) - (amount * 0.1);
			}
			set { /* ignore */ }
		}

		public double Cure3
		{
			get
			{
				var mnd = Math.Floor(Player.Stats.Mind / 2D);
				var vit = Math.Floor(Player.Stats.Vitality / 2D);
				var skill = Player.CombatSkills.Healing.Skill;
				var power = mnd + vit + skill;

				var amount = power switch
				{
					var x when x >= 700 => 340,
					var x when x >= 300 => Math.Floor((((0 + power) - 300) / 5.0) + 260),
					var x when x >= 200 => Math.Floor((((0 + power) - 200) / 2.5) + 220),
					var x when x >= 125 => Math.Floor((((0 + power) - 125) / 1.15) + 155),
					_ => Math.Floor((((0 + power) - 70) / 2.2) + 130),
				};

				var bonus = Potency * (amount * 0.01);
				return Math.Round(amount + bonus) - (amount * 0.1);
			}
			set { /* ignore */ }
		}

		public double Cure4
		{
			get
			{
				var mnd = Math.Floor(Player.Stats.Mind / 2D);
				var vit = Math.Floor(Player.Stats.Vitality / 2D);
				var skill = Player.CombatSkills.Healing.Skill;
				var power = mnd + vit + skill;

				var amount = power switch
				{
					var x when x >= 700 => 640,
					var x when x >= 400 => Math.Floor((((0 + power) - 400) / 2.5) + 520),
					var x when x >= 300 => Math.Floor((((0 + power) - 300) / 1.43) + 450),
					var x when x >= 200 => Math.Floor((((0 + power) - 200) / 2.0) + 400),
					_ => Math.Floor((((0 + power) - 70) / 1.0) + 270),
				};

				var bonus = Potency * (amount * 0.01);
				return Math.Round(amount + bonus) - (amount * 0.1);
			}
			set { /* ignore */ }
		}

		public double Cure5
		{
			get
			{
				var mnd = Math.Floor(Player.Stats.Mind / 2D);
				var vit = Math.Floor(Player.Stats.Vitality / 2D);
				var skill = Player.CombatSkills.Healing.Skill;
				var power = mnd + vit + skill;

				var amount = power switch
				{
					var x when x >= 700 => 780,
					var x when x >= 500 => Math.Floor((((0 + power) - 500) / 3.33) + 720),
					var x when x >= 300 => Math.Floor((((0 + power) - 300) / 2.5) + 640),
					var x when x >= 260 => Math.Floor((((0 + power) - 260) / 2.0) + 620),
					var x when x >= 190 => Math.Floor((((0 + power) - 190) / 1.84) + 582),
					var x when x >= 150 => Math.Floor((((0 + power) - 150) / 1.25) + 550),
					_ => Math.Floor((((0 + power) - 80) / 0.7) + 450),
				};

				var bonus = Potency * (amount * 0.01);
				return Math.Round(amount + bonus) - (amount * 0.1);
			}
			set { /* ignore */ }
		}

		public double Cure6
		{
			get
			{
				var mnd = Math.Floor(Player.Stats.Mind / 2D);
				var vit = Math.Floor(Player.Stats.Vitality / 2D);
				var skill = Player.CombatSkills.Healing.Skill;
				var power = mnd + vit + skill;

				var amount = power switch
				{
					var x when x >= 700 => 1010,
					var x when x >= 500 => Math.Floor((((0 + power) - 500) / 1.67) + 890),
					var x when x >= 400 => Math.Floor((((0 + power) - 300) / 2.5) + 850),
					var x when x >= 300 => Math.Floor((((0 + power) - 300) / 1.43) + 780),
					var x when x >= 210 => Math.Floor((((0 + power) - 210) / 0.9) + 680),
					_ => Math.Floor((((0 + power) - 90) / 1.5) + 600),
				};

				var bonus = Potency * (amount * 0.01);
				return Math.Round(amount + bonus) - (amount * 0.1);
			}
			set { /* ignore */ }
		}

		public double Curaga
		{
			get { return Cure2; }
			set { /* ignore */}
		}

		public double Curaga2
		{
			get { return Cure3; }
			set { /* ignore */}
		}

		public double Curaga3
		{
			get { return Cure4; }
			set { /* ignore */}
		}

		public double Curaga4
		{
			get { return Cure5; }
			set { /* ignore */}
		}

		public double Curaga5
		{
			get { return Cure6; }
			set { /* ignore */}
		}

		public CurePotency(PlayerTools player, double potency)
		{
			Player = player;
			Potency = potency;
		}
	}
}
