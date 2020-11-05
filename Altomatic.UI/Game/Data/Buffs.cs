﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Data
{
	public class Buffs : IEnumerable<Tuple<string, string>>
	{
		public const short KO = 0;
		public const short Weakness = 1;
		public const short Sleep = 2;
		public const short Poison = 3;
		public const short Paralysis = 4;
		public const short Blindness = 5;
		public const short Silence = 6;
		public const short Petrification = 7;
		public const short Disease = 8;
		public const short Curse = 9;
		public const short Stun = 10;
		public const short Bind = 11;
		public const short Weight = 12;
		public const short Slow = 13;
		public const short Charm = 14;
		public const short Doom = 15;
		public const short Amnesia = 16;
		public const short Charm2 = 17;
		public const short GradualPetrification = 18;
		public const short Sleep2 = 19;
		public const short Curse2 = 20;
		public const short Addle = 21;
		public const short Intimidate = 22;
		public const short Kaustra = 23;
		public const short ST24 = 24;
		public const short ST25 = 25;
		public const short ST26 = 26;
		public const short ST27 = 27;
		public const short Terror = 28;
		public const short Mute = 29;
		public const short Bane = 30;
		public const short Plague = 31;
		public const short Flee = 32;
		public const short Haste = 33;
		public const short BlazeSpikes = 34;
		public const short IceSpikes = 35;
		public const short Blink = 36;
		public const short Stoneskin = 37;
		public const short ShockSpikes = 38;
		public const short Aquaveil = 39;
		public const short Protect = 40;
		public const short Shell = 41;
		public const short Regen = 42;
		public const short Refresh = 43;
		public const short MightyStrikes = 44;
		public const short Boost = 45;
		public const short HundredFists = 46;
		public const short Manafont = 47;
		public const short Chainspell = 48;
		public const short PerfectDodge = 49;
		public const short Invincible = 50;
		public const short BloodWeapon = 51;
		public const short SoulVoice = 52;
		public const short EagleEyeShot = 53;
		public const short MeikyoShisui = 54;
		public const short AstralFlow = 55;
		public const short Berserk = 56;
		public const short Defender = 57;
		public const short Aggressor = 58;
		public const short Focus = 59;
		public const short Dodge = 60;
		public const short Counterstance = 61;
		public const short Sentinel = 62;
		public const short Souleater = 63;
		public const short LastResort = 64;
		public const short SneakAttack = 65;
		public const short CopyImage = 66;
		public const short ThirdEye = 67;
		public const short Warcry = 68;
		public const short Invisible = 69;
		public const short Deodorize = 70;
		public const short Sneak = 71;
		public const short Sharpshot = 72;
		public const short Barrage = 73;
		public const short HolyCircle = 74;
		public const short ArcaneCircle = 75;
		public const short Hide = 76;
		public const short Camouflage = 77;
		public const short DivineSeal = 78;
		public const short ElementalSeal = 79;
		public const short STRBoost = 80;
		public const short DEXBoost = 81;
		public const short VITBoost = 82;
		public const short AGIBoost = 83;
		public const short INTBoost = 84;
		public const short MNDBoost = 85;
		public const short CHRBoost = 86;
		public const short TrickAttack = 87;
		public const short MaxHPBoost = 88;
		public const short MaxMPBoost = 89;
		public const short AccuracyBoost = 90;
		public const short AttackBoost = 91;
		public const short EvasionBoost = 92;
		public const short DefenseBoost = 93;
		public const short Enfire = 94;
		public const short Enblizzard = 95;
		public const short Enaero = 96;
		public const short Enstone = 97;
		public const short Enthunder = 98;
		public const short Enwater = 99;
		public const short Barfire = 100;
		public const short Barblizzard = 101;
		public const short Baraero = 102;
		public const short Barstone = 103;
		public const short Barthunder = 104;
		public const short Barwater = 105;
		public const short Barsleep = 106;
		public const short Barpoison = 107;
		public const short Barparalyze = 108;
		public const short Barblind = 109;
		public const short Barsilence = 110;
		public const short Barpetrify = 111;
		public const short Barvirus = 112;
		public const short Reraise = 113;
		public const short Cover = 114;
		public const short UnlimitedShot = 115;
		public const short Phalanx = 116;
		public const short WardingCircle = 117;
		public const short AncientCircle = 118;
		public const short STRBoost2 = 119;
		public const short DEXBoost2 = 120;
		public const short VITBoost2 = 121;
		public const short AGIBoost2 = 122;
		public const short INTBoost2 = 123;
		public const short MNDBoost2 = 124;
		public const short CHRBoost2 = 125;
		public const short SpiritSurge = 126;
		public const short Costume = 127;
		public const short Burn = 128;
		public const short Frost = 129;
		public const short Choke = 130;
		public const short Rasp = 131;
		public const short Shock = 132;
		public const short Drown = 133;
		public const short Dia = 134;
		public const short Bio = 135;
		public const short STRDown = 136;
		public const short DEXDown = 137;
		public const short VITDown = 138;
		public const short AGIDown = 139;
		public const short INTDown = 140;
		public const short MNDDown = 141;
		public const short CHRDown = 142;
		public const short LevelRestriction = 143;
		public const short MaxHPDown = 144;
		public const short MaxMPDown = 145;
		public const short AccuracyDown = 146;
		public const short AttackDown = 147;
		public const short EvasionDown = 148;
		public const short DefenseDown = 149;
		public const short PhysicalShield = 150;
		public const short ArrowShield = 151;
		public const short MagicShield = 152;
		public const short DamageSpikes = 153;
		public const short ShiningRuby = 154;
		public const short Medicine = 155;
		public const short Flash = 156;
		public const short SJRestriction = 157;
		public const short Provoke = 158;
		public const short Penalty = 159;
		public const short Preparations = 160;
		public const short Sprint = 161;
		public const short Enchantment = 162;
		public const short AzureLore = 163;
		public const short ChainAffinity = 164;
		public const short BurstAffinity = 165;
		public const short Overdrive = 166;
		public const short MagicDefDown = 167;
		public const short InhibitTP = 168;
		public const short Potency = 169;
		public const short Regain = 170;
		public const short Pax = 171;
		public const short Intension = 172;
		public const short DreadSpikes = 173;
		public const short MagicAccDown = 174;
		public const short MagicAtkDown = 175;
		public const short Quickening = 176;
		public const short Encumbrance = 177;
		public const short Firestorm = 178;
		public const short Hailstorm = 179;
		public const short Windstorm = 180;
		public const short Sandstorm = 181;
		public const short Thunderstorm = 182;
		public const short Rainstorm = 183;
		public const short Aurorastorm = 184;
		public const short Voidstorm = 185;
		public const short Helix = 186;
		public const short SublimationActivated = 187;
		public const short SublimationComplete = 188;
		public const short MaxTPDown = 189;
		public const short MagicAtkBoost = 190;
		public const short MagicDefBoost = 191;
		public const short Requiem = 192;
		public const short Lullaby = 193;
		public const short Elegy = 194;
		public const short Paeon = 195;
		public const short Ballad = 196;
		public const short Minne = 197;
		public const short Minuet = 198;
		public const short Madrigal = 199;
		public const short Prelude = 200;
		public const short Mambo = 201;
		public const short Aubade = 202;
		public const short Pastoral = 203;
		public const short Hum = 204;
		public const short Fantasia = 205;
		public const short Operetta = 206;
		public const short Capriccio = 207;
		public const short Serenade = 208;
		public const short Round = 209;
		public const short Gavotte = 210;
		public const short Fugue = 211;
		public const short Rhapsody = 212;
		public const short Aria = 213;
		public const short March = 214;
		public const short Etude = 215;
		public const short Carol = 216;
		public const short Threnody = 217;
		public const short Hymnus = 218;
		public const short Mazurka = 219;
		public const short Sirvente = 220;
		public const short Dirge = 221;
		public const short Scherzo = 222;
		public const short Nocturne = 223;
		public const short ST224 = 224;
		public const short ST225 = 225;
		public const short ST226 = 226;
		public const short StoreTP = 227;
		public const short Embrava = 228;
		public const short Manawell = 229;
		public const short Spontaneity = 230;
		public const short Marcato = 231;
		public const short AutoRegen = 233;
		public const short AutoRefresh = 234;
		public const short FishingImagery = 235;
		public const short WoodworkingImagery = 236;
		public const short SmithingImagery = 237;
		public const short GoldsmithingImagery = 238;
		public const short ClothcraftImagery = 239;
		public const short LeathercraftImagery = 240;
		public const short BonecraftImagery = 241;
		public const short AlchemyImagery = 242;
		public const short CookingImagery = 243;
		public const short Dedication = 249;
		public const short EFBadge = 250;
		public const short Food = 251;
		public const short Mounted = 252;
		public const short Signet = 253;
		public const short Battlefield = 254;
		public const short Sanction = 256;
		public const short Besieged = 257;
		public const short Illusion = 258;
		public const short Encumbrance2 = 259;
		public const short Obliviscence = 260;
		public const short Impairment = 261;
		public const short Omerta = 262;
		public const short Debilitation = 263;
		public const short Pathos = 264;
		public const short Flurry = 265;
		public const short Concentration = 266;
		public const short AlliedTags = 267;
		public const short Sigil = 268;
		public const short LevelSync = 269;
		public const short AftermathLv1 = 270;
		public const short AftermathLv2 = 271;
		public const short AftermathLv3 = 272;
		public const short Aftermath = 273;
		public const short Enlight = 274;
		public const short Auspice = 275;
		public const short Confrontation = 276;
		public const short EnfireII = 277;
		public const short EnblizzardII = 278;
		public const short EnaeroII = 279;
		public const short EnstoneII = 280;
		public const short EnthunderII = 281;
		public const short EnwaterII = 282;
		public const short PerfectDefense = 283;
		public const short Egg = 284;
		public const short Visitant = 285;
		public const short Baramnesia = 286;
		public const short Atma = 287;
		public const short Endark = 288;
		public const short EnmityBoost = 289;
		public const short SubtleBlowPlus = 290;
		public const short EnmityDown = 291;
		public const short Pennant = 292;
		public const short NegatePetrify = 293;
		public const short NegateTerror = 294;
		public const short NegateAmnesia = 295;
		public const short NegateDoom = 296;
		public const short NegatePoison = 297;
		public const short CriticalHitEvasionDown = 298;
		public const short Overload = 299;
		public const short FireManeuver = 300;
		public const short IceManeuver = 301;
		public const short WindManeuver = 302;
		public const short EarthManeuver = 303;
		public const short ThunderManeuver = 304;
		public const short WaterManeuver = 305;
		public const short LightManeuver = 306;
		public const short DarkManeuver = 307;
		public const short DoubleUpChance = 308;
		public const short Bust = 309;
		public const short FightersRoll = 310;
		public const short MonksRoll = 311;
		public const short HealersRoll = 312;
		public const short WizardsRoll = 313;
		public const short WarlocksRoll = 314;
		public const short RoguesRoll = 315;
		public const short GallantsRoll = 316;
		public const short ChaosRoll = 317;
		public const short BeastRoll = 318;
		public const short ChoralRoll = 319;
		public const short HuntersRoll = 320;
		public const short SamuraiRoll = 321;
		public const short NinjaRoll = 322;
		public const short DrachenRoll = 323;
		public const short EvokersRoll = 324;
		public const short MagussRoll = 325;
		public const short CorsairsRoll = 326;
		public const short PuppetRoll = 327;
		public const short DancersRoll = 328;
		public const short ScholarsRoll = 329;
		public const short BoltersRoll = 330;
		public const short CastersRoll = 331;
		public const short CoursersRoll = 332;
		public const short BlitzersRoll = 333;
		public const short TacticiansRoll = 334;
		public const short AlliesRoll = 335;
		public const short MisersRoll = 336;
		public const short CompanionsRoll = 337;
		public const short AvengersRoll = 338;
		public const short NaturalistsRoll = 339;
		public const short WarriorsCharge = 340;
		public const short FormlessStrikes = 341;
		public const short AssassinsCharge = 342;
		public const short Feint = 343;
		public const short Fealty = 344;
		public const short DarkSeal = 345;
		public const short DiabolicEye = 346;
		public const short Nightingale = 347;
		public const short Troubadour = 348;
		public const short KillerInstinct = 349;
		public const short StealthShot = 350;
		public const short FlashyShot = 351;
		public const short Sange = 352;
		public const short Hasso = 353;
		public const short Seigan = 354;
		public const short Convergence = 355;
		public const short Diffusion = 356;
		public const short SnakeEye = 357;
		public const short LightArts = 358;
		public const short DarkArts = 359;
		public const short Penury = 360;
		public const short Parsimony = 361;
		public const short Celerity = 362;
		public const short Alacrity = 363;
		public const short Rapture = 364;
		public const short Ebullience = 365;
		public const short Accession = 366;
		public const short Manifestation = 367;
		public const short DrainSamba = 368;
		public const short AspirSamba = 369;
		public const short HasteSamba = 370;
		public const short VelocityShot = 371;
		public const short BuildingFlourish = 375;
		public const short Trance = 376;
		public const short TabulaRasa = 377;
		public const short DrainDaze = 378;
		public const short AspirDaze = 379;
		public const short HasteDaze = 380;
		public const short FinishingMove = 381;
		public const short FinishingMove2 = 382;
		public const short FinishingMove3 = 383;
		public const short FinishingMove4 = 384;
		public const short FinishingMove5 = 385;
		public const short LethargicDaze = 386;
		public const short LethargicDaze2 = 387;
		public const short LethargicDaze3 = 388;
		public const short LethargicDaze4 = 389;
		public const short LethargicDaze5 = 390;
		public const short SluggishDaze = 391;
		public const short SluggishDaze2 = 392;
		public const short SluggishDaze3 = 393;
		public const short SluggishDaze4 = 394;
		public const short SluggishDaze5 = 395;
		public const short WeakenedDaze = 396;
		public const short WeakenedDaze2 = 397;
		public const short WeakenedDaze3 = 398;
		public const short WeakenedDaze4 = 399;
		public const short WeakenedDaze5 = 400;
		public const short AddendumWhite = 401;
		public const short AddendumBlack = 402;
		public const short Reprisal = 403;
		public const short MagicEvasionDown = 404;
		public const short Retaliation = 405;
		public const short Footwork = 406;
		public const short Klimaform = 407;
		public const short Sekkanoki = 408;
		public const short Pianissimo = 409;
		public const short SaberDance = 410;
		public const short FanDance = 411;
		public const short Altruism = 412;
		public const short Focalization = 413;
		public const short Tranquility = 414;
		public const short Equanimity = 415;
		public const short Enlightenment = 416;
		public const short AfflatusSolace = 417;
		public const short AfflatusMisery = 418;
		public const short Composure = 419;
		public const short Yonin = 420;
		public const short Innin = 421;
		public const short CarbunclesFavor = 422;
		public const short IfritsFavor = 423;
		public const short ShivasFavor = 424;
		public const short GarudasFavor = 425;
		public const short TitansFavor = 426;
		public const short RamuhsFavor = 427;
		public const short LeviathansFavor = 428;
		public const short FenrirsFavor = 429;
		public const short DiabolossFavor = 430;
		public const short AvatarsFavor = 431;
		public const short MultiStrikes = 432;
		public const short DoubleShot = 433;
		public const short Transcendency = 434;
		public const short Restraint = 435;
		public const short PerfectCounter = 436;
		public const short ManaWall = 437;
		public const short DivineEmblem = 438;
		public const short NetherVoid = 439;
		public const short Sengikori = 440;
		public const short Futae = 441;
		public const short Presto = 442;
		public const short ClimacticFlourish = 443;
		public const short CopyImage2 = 444;
		public const short CopyImage3 = 445;
		public const short CopyImage4 = 446;
		public const short MultiShots = 447;
		public const short BewilderedDaze = 448;
		public const short BewilderedDaze2 = 449;
		public const short BewilderedDaze3 = 450;
		public const short BewilderedDaze4 = 451;
		public const short BewilderedDaze5 = 452;
		public const short DivineCaress = 453;
		public const short Saboteur = 454;
		public const short Tenuto = 455;
		public const short Spur = 456;
		public const short Efflux = 457;
		public const short EarthenArmor = 458;
		public const short DivineCaress2 = 459;
		public const short BloodRage = 460;
		public const short Impetus = 461;
		public const short Conspirator = 462;
		public const short Sepulcher = 463;
		public const short ArcaneCrest = 464;
		public const short Hamanoha = 465;
		public const short DragonBreaker = 466;
		public const short TripleShot = 467;
		public const short StrikingFlourish = 468;
		public const short Perpetuance = 469;
		public const short Immanence = 470;
		public const short Migawari = 471;
		public const short TernaryFlourish = 472;
		public const short Muddle = 473;
		public const short Prowess = 474;
		public const short Voidwatcher = 475;
		public const short Ensphere = 476;
		public const short Sacrosanctity = 477;
		public const short Palisade = 478;
		public const short ScarletDelirium = 479;
		public const short ScarletDelirium2 = 480;
		public const short AbdhaljsSeal = 481;
		public const short DecoyShot = 482;
		public const short Hagakure = 483;
		public const short Issekigan = 484;
		public const short UnbridledLearning = 485;
		public const short CounterBoost = 486;
		public const short Endrain = 487;
		public const short Enaspir = 488;
		public const short Afterglow = 489;
		public const short BrazenRush = 490;
		public const short InnerStrength = 491;
		public const short Asylum = 492;
		public const short SubtleSorcery = 493;
		public const short Stymie = 494;
		public const short None = 495;
		public const short Intervene = 496;
		public const short SoulEnslavement = 497;
		public const short Unleash = 498;
		public const short ClarionCall = 499;
		public const short Overkill = 500;
		public const short Yaegasumi = 501;
		public const short Mikage = 502;
		public const short FlyHigh = 503;
		public const short AstralConduit = 504;
		public const short UnbridledWisdom = 505;
		public const short GrandPas = 507;
		public const short WidenedCompass = 508;
		public const short OdyllicSubterfuge = 509;
		public const short ErgonMight = 510;
		public const short ReiveMark = 511;
		public const short Ionis = 512;
		public const short Bolster = 513;
		public const short LastingEmanation = 515;
		public const short EclipticAttrition = 516;
		public const short CollimatedFervor = 517;
		public const short Dematerialize = 518;
		public const short TheurgicFocus = 519;
		public const short ElementalSforzo = 522;
		public const short Ignis = 523;
		public const short Gelus = 524;
		public const short Flabra = 525;
		public const short Tellus = 526;
		public const short Sulpor = 527;
		public const short Unda = 528;
		public const short Lux = 529;
		public const short Tenebrae = 530;
		public const short Vallation = 531;
		public const short Swordplay = 532;
		public const short Pflug = 533;
		public const short Embolden = 534;
		public const short Valiance = 535;
		public const short Gambit = 536;
		public const short Liement = 537;
		public const short OneforAll = 538;
		public const short Regen2 = 539;
		public const short Poison2 = 540;
		public const short Refresh2 = 541;
		public const short STRBoost3 = 542;
		public const short DEXBoost3 = 543;
		public const short VITBoost3 = 544;
		public const short AGIBoost3 = 545;
		public const short INTBoost3 = 546;
		public const short MNDBoost3 = 547;
		public const short CHRBoost3 = 548;
		public const short AttackBoost2 = 549;
		public const short DefenseBoost2 = 550;
		public const short MagicAtkBoost2 = 551;
		public const short MagicDefBoost2 = 552;
		public const short AccuracyBoost2 = 553;
		public const short EvasionBoost2 = 554;
		public const short MagicAccBoost = 555;
		public const short MagicEvasionBoost = 556;
		public const short AttackDown2 = 557;
		public const short DefenseDown2 = 558;
		public const short MagicAtkDown2 = 559;
		public const short MagicDefDown2 = 560;
		public const short AccuracyDown2 = 561;
		public const short EvasionDown2 = 562;
		public const short MagicAccDown2 = 563;
		public const short MagicEvasionDown2 = 564;
		public const short Slow2 = 565;
		public const short Paralysis2 = 566;
		public const short Weight2 = 567;
		public const short Foil = 568;
		public const short BlazeofGlory = 569;
		public const short Battuta = 570;
		public const short Rayke = 571;
		public const short AvoidanceDown = 572;
		public const short DelugeSpikes = 573;
		public const short FastCast = 574;
		public const short Gestation = 575;
		public const short Doubt = 576;
		public const short CaitSithsFavor = 577;
		public const short FishyIntuition = 578;
		public const short Commitment = 579;
		public const short Haste2 = 580;
		public const short Flurry2 = 581;
		public const short Contradance = 582;
		public const short Apogee = 583;
		public const short Entrust = 584;
		public const short Costume2 = 585;
		public const short CuringConduit = 586;
		public const short TPBonus = 587;
		public const short FinishingMove6 = 588;
		public const short Firestorm2 = 589;
		public const short Hailstorm2 = 590;
		public const short Windstorm2 = 591;
		public const short Sandstorm2 = 592;
		public const short Thunderstorm2 = 593;
		public const short Rainstorm2 = 594;
		public const short Aurorastorm2 = 595;
		public const short Voidstorm2 = 596;
		public const short Inundation = 597;
		public const short Cascade = 598;
		public const short ConsumeMana = 599;
		public const short RuneistsRoll = 600;
		public const short CrookedCards = 601;
		public const short Vorseal = 602;
		public const short Elvorseal = 603;
		public const short MightyGuard = 604;
		public const short GaleSpikes = 605;
		public const short ClodSpikes = 606;
		public const short GlintSpikes = 607;
		public const short NegateVirus = 608;
		public const short NegateCurse = 609;
		public const short NegateCharm = 610;
		public const short MagicEvasionBoost2 = 611;
		public const short ColureActive = 612;

		private readonly ConcurrentDictionary<string, List<BuffStatus>> partyBuffs = new ConcurrentDictionary<string, List<BuffStatus>>(StringComparer.InvariantCultureIgnoreCase);

		public AppViewModel App { get; }

		public Buffs(AppViewModel app)
		{
			App = app ?? throw new ArgumentNullException(nameof(app));
		}

		/// <summary>
		/// Clears all buffs
		/// </summary>
		public void Clear()
		{
			partyBuffs.Clear();
		}

		/// <summary>
		/// Updates all buffs for a player
		/// </summary>
		public void Update(string playerName, short[] buffs)
		{
			partyBuffs[playerName] = buffs.Select(b => new BuffStatus(b)).ToList();
		}

		/// <summary>
		/// Adds a buff to a player
		/// </summary>
		public void Add(string playerName, short buff)
		{
			if (!partyBuffs.TryGetValue(playerName, out var buffs))
			{
				partyBuffs[playerName] = buffs = new List<BuffStatus>();
			}

			Remove(playerName, buff);
			buffs.Add(new BuffStatus(buff));
		}

		/// <summary>
		/// Removes a buff from a player
		/// </summary>
		/// <param name="playerName"></param>
		/// <param name="buff"></param>
		public void Remove(string playerName, short buff)
		{
			if (partyBuffs.TryGetValue(playerName, out var buffs))
			{
				buffs.RemoveAll(b => b.Id == buff);
			}
		}

		/// <summary>
		/// Gets a player buff's age in seconds
		/// </summary>
		public int GetBuffAgeSeconds(string playerName, short buff)
		{
			if (App.Healer.Player.Name == playerName)
			{
				if (App.Healer.Player.Buffs.Contains(buff))
				{
					return -1;
				}
			}
			else if (App.Monitored.Player.Name == playerName)
			{
				if (App.Monitored.Player.Buffs.Contains(buff))
				{
					return -1;
				}
			}
			else if (partyBuffs.TryGetValue(playerName, out var buffs))
			{
				var status = buffs.SingleOrDefault(b => b.Id == buff);
				if (status != null) return status.AgeInSeconds;
			}

			return int.MaxValue;
		}

		/// <summary>
		/// Does the player have any of the specified buffs?
		/// </summary>
		public bool HasAny(string playerName, params short[] buffs)
		{
			var activeBuffs = new List<short>();

			if (App.Healer.Player.Name == playerName)
			{
				activeBuffs.AddRange(App.Healer.Player.Buffs);
			}
			else if (App.Monitored.Player.Name == playerName)
			{
				activeBuffs.AddRange(App.Monitored.Player.Buffs);
			}
			else if (partyBuffs.TryGetValue(playerName, out var playerBuffs))
			{
				activeBuffs.AddRange(playerBuffs.Select(b => b.Id).ToArray());
			}

			return buffs.Intersect(activeBuffs).Any();
		}

		/// <summary>
		/// Does the player have all of the specified buffs?
		/// </summary>
		public bool HasAll(string playerName, params short[] buffs)
		{
			var activeBuffs = new List<short>();

			if (App.Healer.Player.Name == playerName)
			{
				activeBuffs.AddRange(App.Healer.Player.Buffs);
			}
			else if (App.Monitored.Player.Name == playerName)
			{
				activeBuffs.AddRange(App.Monitored.Player.Buffs);
			}
			else if (partyBuffs.TryGetValue(playerName, out var playerBuffs))
			{
				activeBuffs.AddRange(playerBuffs.Select(b => b.Id).ToArray());
			}

			return buffs.Intersect(activeBuffs).Count() == buffs.Count();
		}

		public IEnumerator<Tuple<string, string>> GetEnumerator()
		{
			foreach (var buff in partyBuffs)
			{
				foreach (var status in buff.Value)
				{
					if (status.Id > 0)
					{
						yield return new Tuple<string, string>(buff.Key, status.Id.ToString());
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
