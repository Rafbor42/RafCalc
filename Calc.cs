/*  Copyright (c) Raphael Borrelli (@Rafbor)
    Ce fichier fait partie de RafCalc.

    RafCalc est un logiciel libre: vous pouvez le redistribuer ou le modifier
    suivant les termes de la GNU General Public License telle que publiée par
    la Free Software Foundation, soit la version 3 de la licence, soit
    (à votre gré) toute version ultérieure.

    RafCalc est distribué dans l'espoir qu'il sera utile,
    mais SANS AUCUNE GARANTIE; sans même la garantie tacite de
    QUALITÉ MARCHANDE ou d'ADÉQUATION à UN BUT PARTICULIER. Consultez la
    GNU General Public License pour plus de détails.

    Vous devez avoir reçu une copie de la GNU General Public License
    en même temps que RafCalc. Si ce n'est pas le cas, consultez <https://www.gnu.org/licenses>.

    -----------------------------------------------------------------

    This file is part of RafCalc.

    RafCalc is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    RafCalc is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with RafCalc.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components;

namespace RafCalc
{
    public class Calc
    {
	//	[Inject]
	//	private IStringLocalizer<CalcStr> LocStr { get; }
		public IStringLocalizer LocStr { get; set; }

        public enum eQuantites
		{
			Choix1,
			Choix2
		}
		public enum eTypeTempis
		{
			TA,
			Frigo
		}
		public enum eAcqua
		{
			Acqua1,
			Acqua2
		}
        public enum eModeCalcul
		{
			ImpastoTotale,
			FarinaSola
		}
		public enum eLievito
		{
			LdbF,
			LdbS
		}
        public enum eTypeCalcul
		{
			Acqua,
			Farina
		}
        private enum eTypeLigne
		{
			Normal,
			EnTete,
			Titre,
			PreImpasto
		}
        public enum eTypePreimpasto
		{
			Autolyse,
			Biga,
			Poolish
		}
		public enum eFarina
		{
			Farina1,
			Farina2,
			Farina3
		}
		public enum eTypeMethode
		{
			Italienne,
			Française
		}

		[StringLength(256, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
        public string NomeRicetta { get; set; }
        [Range(1, int.MaxValue, ErrorMessageResourceName = "ValeurPosSup0Requis", ErrorMessageResourceType = typeof(CalcStr))]
        public int NumeroPanielli1 { get; set; }
        [Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
        public int NumeroPanielli2 { get; set; }
        [Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
        public int NumeroPanielli3 { get; set; }
        //
        [Range(1, int.MaxValue, ErrorMessageResourceName = "ValeurPosSup0Requis", ErrorMessageResourceType = typeof(CalcStr))]
        public double PesoPaniello1 { get; set; }
        [Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double PesoPaniello2 { get; set; }
        [Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double PesoPaniello3 { get; set; }
        //
    	// dans ValNonValidPlage, les paramètres {1} et {2} correspondent aux limites
		[Range(50.0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
        public double Idro { get; set; }
		//
		private double dblSaleLitro;
        [Range(0.0, 60, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double SaleLitro { get => Math.Round(dblSaleLitro, GetSGDec()); set => dblSaleLitro = value; }
		//
		private double dblGrassiLitro;
        [Range(0.0, 60, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double GrassiLitro { get => Math.Round(dblGrassiLitro, GetSGDec()); set => dblGrassiLitro = value; }
		//
        [Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double Farina { get; set; }
        [Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double Acqua { get; set; }
        [Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double Sale { get; set; }
        [Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double Grassi { get; set; }
        //
        [Range(0.0, 99, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
        public double Impastamento { get; set; }
        //
		[Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
        public double PuntataOre { get; set; }
		[Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double ApprettoOre { get; set; }
        [Range(15, 35, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Temperatura { get; set; }
		[Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double FrigoOre { get; set; }
		[Range(15.0, 35.0, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double ApprettoTemp { get; set; }
		[Range(15.0, 35.0, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double PuntataTemp { get; set; }
		//
		[Range(10, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
        public double CoefCalcolo { get; set; }
		[Range(100, 6000, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double CoefCalcoloAT { get; set; }
		//
		public DateTime CotturaDataOra { get; set; }
		public DateTime CotturaOra { get; set; }
		//
		[StringLength(32000, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
		public string Note { get; set; }
		//
		public double TotalImpasto { get; set; }
		public double Lievito { get; set; }
		//
		public double LievLitro { get; set; }
		public double LievKilo { get; set; }
		// membres utilisés pour la liaison avec l'IHM, nécessaire car ils peuvent correspondre
		// à 2 membres différents en fonction du TypeCalcul 
		public double LievKiloLitroPc
		{
			get
			{
				if (TypeCalcul == Convert.ToInt16(eTypeCalcul.Acqua))
					return GetLievitoAcquaPercent();
				else
					return GetLievitoFarinaPercent();
			}
			set
			{
				if (TypeCalcul == Convert.ToInt16(eTypeCalcul.Acqua))
					LievLitro = value * 10;
				else
				{
					LievKilo = value * 10;
					ConvertLievFarinaToAcqua();
				}
			}
		}
		public double LievKiloLitro
		{
			get
			{
				if (TypeCalcul == Convert.ToInt16(eTypeCalcul.Acqua))
					return GetLievitoAcquaGrammi();
				else
					return GetLievitoFarinaGrammi();
			}
			set
			{
				if (TypeCalcul == Convert.ToInt16(eTypeCalcul.Acqua))
					LievLitro = value;
				else
				{
					LievKilo = value;
					ConvertLievFarinaToAcqua();
				}
			}
		}
		public double SaleKiloLitroPc
		{
			get	{ return GetSaleFarinaPercent(); }
			set	{ ConvertSaleFarinaToAcqua(value * 10);	}
		}
		public double SaleKiloLitro
		{
			get	{ return GetSaleFarinaGrammi(); }
			set	{ ConvertSaleFarinaToAcqua(value); }
		}
		public double GrassiKiloLitroPc
		{
			get	{ return GetGrassiFarinaPercent(); }
			set	{ ConvertGrassiFarinaToAcqua(value * 10);	}
		}
		public double GrassiKiloLitro
		{
			get	{ return GetGrassiFarinaGrammi(); }
			set	{ ConvertGrassiFarinaToAcqua(value); }
		}
		//
		private double dblFarina2Pc;
		[Range(0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
        public double Farina2Pc
		{
			get { return dblFarina2Pc; }
			set
			{
				dblFarina2Pc = value;
				if (dblFarina2Pc < 0)
					dblFarina2Pc = 0;
				if (100 - Farina2Pc - Farina3Pc < 0)
					Farina3Pc--;
			}
		}
		private double dblFarina3Pc;
		[Range(0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Farina3Pc
		{
			get { return dblFarina3Pc; }
			set
			{
				dblFarina3Pc = value;
				if (dblFarina3Pc < 0)
					dblFarina3Pc = 0;
				if (100 - Farina2Pc - Farina3Pc < 0)
					Farina2Pc--;
			}
		}
		[Range(0, 450, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Farina1W { get; set; }
		[Range(0, 450, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Farina2W { get; set; }
		[Range(0, 450, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Farina3W { get; set; }
		//
		public double FarinaCalc;// pour contourner le bug sur Farina qui est remis à 0 dans l'IHM.
		public double AcquaCalc;// idem
		public double SaleCalc;// idem
		public double GrassiCalc;// idem
		public int NumeroPanielliCalc1;// idem
		public double PesoPanielloCalc1;// idem
		public int NumeroPanielliCalc2;// idem
		public double PesoPanielloCalc2;// idem
		public int NumeroPanielliCalc3;// idem
		public double PesoPanielloCalc3;// idem
		//
		public bool PresetLievito { get; set; }// calcul forcé de la levure
		public bool CalculChaleur { get; set; }// calcul avec la nouvelle formule HT
		public short TypeCalcul { get; set; }
		//
        public DataTable TblResult { get; set; }
		// DataTable plus vraiment utiles, mais conservés car utilisé dans RafCalc PC
		// dans lequel le nombre de phases est illimité...
		public DataTable TblTempisPuntata { get; set; }
		public DataTable TblTempisAppretto { get; set; }
		//
		[Range(0, 1000, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Acqua1Ca { get; set; }
		[Range(0, 1000, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Acqua1Mg { get; set; }
		[Range(0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Acqua2Pc { get; set; }
		[Range(0, 1000, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Acqua2Ca { get; set; }
		[Range(0, 1000, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Acqua2Mg { get; set; }
		[StringLength(16, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
		public string Acqua1Nome { get; set; }
		[StringLength(16, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
		public string Acqua2Nome { get; set; }
		//
		[Range(0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double PdrPc { get; set; }
		public double PdrPcMax { get; set; } // non stocké
		public short PdrCalc { get; set; }
		private double Pdr { get; set; } // non stocké
		public double RapportoLievFS { get; set; }
		//
		private double dblTLunghezza1;
		[Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double TLunghezza1 { get => dblTLunghezza1; set { dblTLunghezza1 = value; SetPesoTeglia1(); } }
		private double dblTLarghezza1;
		[Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double TLarghezza1 { get => dblTLarghezza1; set { dblTLarghezza1 = value; SetPesoTeglia1(); } }
		private double dblTMajorPc1;
		[Range(-50, 50, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double TMajorPc1 { get => dblTMajorPc1; set { dblTMajorPc1 = value; SetPesoTeglia1(); } }
		//
		private double dblTLunghezza2;
		[Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double TLunghezza2 { get => dblTLunghezza2; set { dblTLunghezza2 = value; SetPesoTeglia2(); } }
		private double dblTLarghezza2;
		[Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double TLarghezza2 { get => dblTLarghezza2; set { dblTLarghezza2 = value; SetPesoTeglia2(); } }
		private double dblTMajorPc2;
		[Range(-50, 50, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double TMajorPc2 { get => dblTMajorPc2; set { dblTMajorPc2 = value; SetPesoTeglia2(); } }
		//
		private double dblTLunghezza3;
		[Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double TLunghezza3 { get => dblTLunghezza3; set { dblTLunghezza3 = value; SetPesoTeglia3(); } }
		private double dblTLarghezza3;
		[Range(0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double TLarghezza3 { get => dblTLarghezza3; set { dblTLarghezza3 = value; SetPesoTeglia3(); } }
		private double dblTMajorPc3;
		[Range(-50, 50, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double TMajorPc3 { get => dblTMajorPc3; set { dblTMajorPc3 = value; SetPesoTeglia3(); } }
		//
		private double dblPreimpOre;
		[Range(0.0, int.MaxValue, ErrorMessageResourceName = "ValeurPosRequis", ErrorMessageResourceType = typeof(CalcStr))]
		public double PreimpOre {
			get => dblPreimpOre;
			set
			{
				dblPreimpOre = value;
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
				{
					if (dblPreimpOre < 2)
						dblPreimpOre = 2;
					if (dblPreimpOre > 18)
						dblPreimpOre = 18;
					CalcolaLievitoPoolish();
				}
			}
		}
		[Range(0.0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double PreimpFarina { get; set; }
		[Range(0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double PreimpAcqua { get; set; }
		[Range(0, 100, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double PreimpSale { get; set; }
		[Range(4, 35, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double PreimpTemperatura { get; set; }
		public short TypePreimpasto { get; set; }
		public double LievitoPreImp { get; set; }
		public double PreImpLievKiloPc { get; set; }
		public short TypeMethode { get; set; }// non stocké
		//
		public bool CalculIsOk { get; set; } // non stocké
		//
        public Microsoft.AspNetCore.Components.RenderFragment ChildContent { get; set; }
		//
		private double dblPointagePhase1Ore;
		public double PointagePhase1Ore { get => dblPointagePhase1Ore; set { dblPointagePhase1Ore = value; UpdateTempistichePuntata(); } }
		private double dblPointagePhase2Ore;
		public double PointagePhase2Ore { get => dblPointagePhase2Ore; set { dblPointagePhase2Ore = value; UpdateTempistichePuntata(); } }
		private double dblPointagePhase3Ore;
		public double PointagePhase3Ore { get => dblPointagePhase3Ore; set { dblPointagePhase3Ore = value; UpdateTempistichePuntata(); } }
		private string strPointagePhase1;
		public string PointagePhase1 { get => strPointagePhase1; set { strPointagePhase1 = value; UpdateTempistichePuntata(); } }
		private string strPointagePhase2;
		public string PointagePhase2 { get => strPointagePhase2; set { strPointagePhase2 = value; UpdateTempistichePuntata(); } }
		private string strPointagePhase3;
		public string PointagePhase3 { get => strPointagePhase3; set { strPointagePhase3 = value; UpdateTempistichePuntata(); } }
		//
		private double dblAppretPhase1Ore;
		public double AppretPhase1Ore { get => dblAppretPhase1Ore; set { dblAppretPhase1Ore = value; UpdateTempisticheAppretto(); } }
		private double dblAppretPhase2Ore;
		public double AppretPhase2Ore { get => dblAppretPhase2Ore; set { dblAppretPhase2Ore = value; UpdateTempisticheAppretto(); } }
		private double dblAppretPhase3Ore;
		public double AppretPhase3Ore { get => dblAppretPhase3Ore; set { dblAppretPhase3Ore = value; UpdateTempisticheAppretto(); } }
		private string strAppretPhase1;
		public string AppretPhase1 { get => strAppretPhase1; set { strAppretPhase1 = value; UpdateTempisticheAppretto(); } }
		private string strAppretPhase2;
		public string AppretPhase2 { get => strAppretPhase2; set { strAppretPhase2 = value; UpdateTempisticheAppretto(); } }
		private string strAppretPhase3;
		public string AppretPhase3 { get => strAppretPhase3; set { strAppretPhase3 = value; UpdateTempisticheAppretto(); } }
		//
		[Range(0.0, 30.0, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Additivo1Pc { get; set; }
		[StringLength(24, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
		public string Additivo1Nome { get; set; }
		[Range(0.0, 30.0, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Additivo2Pc { get; set; }
		[StringLength(24, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
		public string Additivo2Nome { get; set; }
		[Range(0.0, 30.0, ErrorMessageResourceName = "ValNonValidPlage", ErrorMessageResourceType = typeof(CalcStr))]
		public double Additivo3Pc { get; set; }
		[StringLength(24, ErrorMessageResourceName="TexteTropLong", ErrorMessageResourceType = typeof(CalcStr))]
		public string Additivo3Nome { get; set; }
		// config
		public eQuantites ChoixQuantites { get; set; }
		public bool SaleGrassiDecimali { get; set; }
		public eLievito LievitoFS { get; set; }

        // Constructeur.
        public Calc()
        {
            // init des DataTable
			TblResult = new DataTable("Result");
			TblResult.Columns.Add("Libelle", typeof(string));
			TblResult.Columns.Add("Valeur", typeof(string));
			TblResult.Columns.Add("Type", typeof(eTypeLigne));
            //
            TblTempisPuntata = new DataTable("TempisPuntata");
			TblTempisPuntata.Columns.Add("Type", typeof(short));
			TblTempisPuntata.Columns.Add("Libelle", typeof(string));
			TblTempisPuntata.Columns.Add("Valeur", typeof(double));
            //
			TblTempisAppretto = new DataTable("TempisAppretto");
			TblTempisAppretto.Columns.Add("Type", typeof(short));
			TblTempisAppretto.Columns.Add("Libelle", typeof(string));
			TblTempisAppretto.Columns.Add("Valeur", typeof(double));
            //
            Init();
        }
		
		// Initialisations.
        public void Init()
        {
            // quantità
            NomeRicetta = "New";
            NumeroPanielli1 = 1;
            NumeroPanielli2 = 0;
            NumeroPanielli3 = 0;
            PesoPaniello1 = 200;
            PesoPaniello2 = 0;
            PesoPaniello3 = 0;
            Idro = 65;
			SaleLitro = 40;
			GrassiLitro = 0;
			Farina = 0;
			Acqua = 0;
			Sale = 0;
			Grassi = 0;
			// impastamento
			Impastamento = 10;
            // lievitazione
			PuntataOre = 16;
			ApprettoOre = 8;
			Temperatura = 20;
			FrigoOre = 0;
			ApprettoTemp = 20;
			PuntataTemp = 20;
			// coef
			CoefCalcolo = 57.5;
			CoefCalcoloAT = 2250;
			// cottura
			CotturaDataOra = DateTime.Now;
			CotturaOra = DateTime.Now;
			// note
			Note = String.Empty;
			//
            TotalImpasto = 0;
			Lievito = 0;
			LievLitro = 0;
			LievKilo = 0;
			//
			Farina2Pc = 0;
			Farina3Pc = 0;
			Farina1W = 0;
			Farina2W = 0;
			Farina3W = 0;
			//
			Acqua1Mg = 0;
			Acqua1Ca = 0;
			Acqua2Pc = 0;
			Acqua2Mg = 0;
			Acqua2Ca = 0;
			Acqua1Nome = string.Empty;
			Acqua2Nome = string.Empty;
			//
			FarinaCalc = 0;
			AcquaCalc = 0;
			SaleCalc = 0;
			GrassiCalc = 0;
			NumeroPanielliCalc1 = 0;
			PesoPanielloCalc1 = 0;
			NumeroPanielliCalc2 = 0;
			PesoPanielloCalc2 = 0;
			NumeroPanielliCalc3 = 0;
			PesoPanielloCalc3 = 0;
			//
			PresetLievito = false;
			CalculChaleur = true;
			TypeCalcul = Convert.ToInt16(eTypeCalcul.Acqua);
			//
			PdrPc = 0;
			PdrPcMax = 100;// non modifiable
			PdrCalc = Convert.ToInt16(eModeCalcul.ImpastoTotale);
			Pdr = 0;
			RapportoLievFS = 3;
			//
			dblTLunghezza1 = 0;
			dblTLarghezza1 = 0;
			dblTMajorPc1 = 0;
			dblTLunghezza2 = 0;
			dblTLarghezza2 = 0;
			dblTMajorPc2 = 0;
			dblTLunghezza3 = 0;
			dblTLarghezza3 = 0;
			dblTMajorPc3 = 0;
			//
			dblPreimpOre = 0;
			PreimpAcqua = 0;
			PreimpFarina = 0;
			PreimpSale = 0;
			PreimpTemperatura = 18;
			TypePreimpasto = Convert.ToInt16(eTypePreimpasto.Autolyse);
			LievitoPreImp = 0;
			PreImpLievKiloPc = 0;
			TypeMethode = Convert.ToInt16(eTypeMethode.Italienne);
			//
			Additivo1Pc = 0;
			Additivo2Pc = 0;
			Additivo3Pc = 0;
			Additivo1Nome = string.Empty;
			Additivo2Nome = string.Empty;
			Additivo3Nome = string.Empty;
			//
			InitTblTempistiche();
			//
			CalculIsOk = false;
			//
			// config
			ChoixQuantites = eQuantites.Choix1;
			SaleGrassiDecimali = true;
			LievitoFS = eLievito.LdbF;
			//
			ClearChildContent();
			TblResult.Clear();
        }
		//
		private void ClearChildContent()
		{
			ChildContent = builder =>
            {
                builder.OpenElement(1, "div");
				builder.CloseElement();
			};
		}
		// 
		public void InitTblTempistiche()
		{
			TblTempisPuntata.Clear();
			TblTempisPuntata.AcceptChanges();
			TblTempisAppretto.Clear();
			TblTempisAppretto.AcceptChanges();
			// /!\ ne pas modifier les accesseurs sinon le 'Set' va modifier d'autres valeurs.
			dblPointagePhase1Ore = 0;
			dblPointagePhase2Ore = 0;
			dblPointagePhase3Ore = 0;
			strPointagePhase1 = "TA";
			strPointagePhase2 = "TA";
			strPointagePhase3 = "TA";
			dblAppretPhase1Ore = 0;
			dblAppretPhase2Ore = 0;
			dblAppretPhase3Ore = 0;
			strAppretPhase1 = "TA";
			strAppretPhase2 = "TA";
			strAppretPhase3 = "TA";
		}

		// Controle des valeurs.
		// Inclus les tests non gérés par le processus de validation automatique du EditForm.
		// Les tests sont donc redondants mais je les conserve en cas d'appel de la classe de calcul par un autre assembly.
		private bool CheckValues(ref string strMsg)
		{
			bool bResult = true;
			double dblTmp;

			// valeurs négatives
			if (	NumeroPanielli1 < 0
			    ||	PesoPaniello1 < 0
			    ||	NumeroPanielli2 < 0
			    ||	PesoPaniello2 <0
			    ||	NumeroPanielli3 < 0
			    ||	PesoPaniello3 <0
			    ||	Idro < 0
			    ||	SaleLitro < 0
			    ||	Sale < 0
			    ||	GrassiLitro < 0
			    ||	Grassi < 0
			    ||	Farina < 0
			    ||	Acqua < 0
			    ||	Impastamento < 0
			    ||	PuntataOre < 0
			    ||	ApprettoOre < 0
			    ||	Temperatura < 0
			    ||	PuntataTemp < 0
			    ||	ApprettoTemp < 0
			    ||	FrigoOre < 0
			    ||	CoefCalcolo < 0
			    ||	CoefCalcoloAT < 0//
				||	LievLitro < 0
				||	LievKilo< 0
				||	Farina1W < 0
				||	Farina2W < 0
				||	Farina3W < 0
				||	Acqua1Mg < 0
				||	Acqua1Ca < 0
				||	Acqua2Mg < 0
				||	Acqua2Ca < 0
				||	PreimpFarina < 0
				||	PreimpAcqua < 0
				||	PreimpSale < 0
				||	PreimpOre < 0
				||	Additivo1Pc < 0
				||	Additivo2Pc < 0
				||	Additivo3Pc < 0
			   )
			{
				strMsg = LocStr["ValPositivesRequises"];
				return false;
			}

			// si proportions choix1
			if (ChoixQuantites == eQuantites.Choix1)
			{
				// au moins 1 pâton
				if (NumeroPanielli1 + NumeroPanielli2 + NumeroPanielli3 < 1)
				{
					strMsg = LocStr["NbePatonsPositifRequis"];
					return false;
				}
				
				// peso panielli
				if (PesoPaniello1 + PesoPaniello2 + PesoPaniello3 == 0)
				{
					strMsg = LocStr["PoidsPatonPositifRequis"];
					return false;
				}
				
				// idro tra 50 e 100
				if (Idro < 50 || Idro > 100)
				{
					strMsg = LocStr["PcHydratEntre50_100"];
					return false;
				}
				
				// sale tra 0 e 60
				if (SaleLitro < 0 || SaleLitro > 60)
				{
					strMsg = LocStr["TauxSellitreEntre0_60"];
					return false;
				}
				
				// grassi tra 0 e 60
				if (GrassiLitro < 0 || GrassiLitro > 60)
				{
					strMsg = LocStr["TauxHuilelitreEntre0_60"];
					return false;
				}
			}
			else
			{
				// farine > 0
				if (Farina <= 0)
				{
					strMsg = LocStr["PoidsFarinePositifRequis"];
					return false;
				}
				
				// eau > 0
				if (Acqua <= 0)
				{
					strMsg = LocStr["PoidsEauPositifRequis"];
					return false;
				}

				// % eau entre 50 et 100
				if (Acqua / Farina < 0.5 || Acqua / Farina > 1)
				{
					strMsg = LocStr["PcHydratEntre50_100"];
					return false;
				}

				// % sale tra 0 e 60
				if (Sale * 1000 / Acqua > 60)
				{
					strMsg = LocStr["TauxSellitreEntre0_60"];
					return false;
				}
				
				// % grassi tra 0 e 60
				if (Grassi * 1000 / Acqua > 60)
				{
					strMsg = LocStr["TauxHuilelitreEntre0_60"];
					return false;
				}
			}

			// impastamento tra 0 e 99 min
			if (Impastamento < 0 || Impastamento > 99)
			{
				strMsg = LocStr["DureePetrissEntre0_99"];
				return false;
			}
			
			// temperature tra 15°C e 35°C
			if (	Temperatura < 15 || Temperatura > 35
				||	PuntataTemp < 15 || PuntataTemp > 35
				||	ApprettoTemp < 15 || ApprettoTemp > 35)
			{
				strMsg = LocStr["TempEntre15_35"];
				return false;
			}

			// ore tra 3 e 96
			if (PuntataOre + ApprettoOre < 3 || PuntataOre + ApprettoOre > 96)
			{
				strMsg = LocStr["DureeTotMaturEntre3_96"];
				return false;
			}

			// ore frigo < ore totale - 1
			if (PuntataOre + ApprettoOre - FrigoOre < 1)
			{
				strMsg = LocStr.GetString("HeureFriNoSupTotalHeure");
				return false;
			}

			// coefficiente tra 10 e 100
			if (CoefCalcolo < 10 || CoefCalcolo > 100)
			{
				strMsg = LocStr["CoefCalcEntre10_100"];
				return false;
			}
			
			// nuovo coefficiente tra 100 e 6000
			if (CoefCalcoloAT < 100 || CoefCalcoloAT > 6000)
			{
				strMsg = LocStr["CoefCalcEntre100_6000"];
				return false;
			}

			// W farines entre 0 et 450
			if (	Farina1W < 0 || Farina1W > 450
				||	Farina2W < 0 || Farina2W > 450
				||	Farina3W < 0 || Farina3W > 450)
			{
				strMsg = LocStr["WFarineEntre90_450"];
				return false;
			}

			// dureté eaux entre 0 et 1000
			if (	Acqua1Ca < 0 || Acqua1Ca > 1000
				||	Acqua2Ca < 0 || Acqua2Ca > 1000
				||	Acqua1Mg < 0 || Acqua1Mg > 1000
				||	Acqua2Mg < 0 || Acqua2Mg > 1000)
			{
				strMsg = LocStr["QteCaMgEntre0_1000"];
				return false;
			}
			
			// autolyse, biga, poolish
			if (PreimpOre < 0 || PreimpOre > 48)
			{
				strMsg = LocStr["DureeAutoBigaPoolishEntre0_48"];
				return false;
			}
			if (	PreimpFarina < 0 || PreimpFarina > 100
				||	PreimpAcqua < 0 || PreimpAcqua > 100
				||	PreimpSale < 0 || PreimpSale > 100)
			{
				strMsg = LocStr["PcIngredAutoBigaPoolEntre0_100"];
				return false;
			}

			if (PreimpOre > 0)
			{
				// farine poolish
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
				{
					dblTmp = AcquaCalc * PreimpAcqua * PreimpFarina / 10000.0;
					// farine restante dans recette
					if (FarinaCalc - dblTmp < 0)
					{
						strMsg = LocStr["DiminuerPcFarineEauPool"];
						return false;
					}
				}
				// eau biga
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
				{
					dblTmp = FarinaCalc * PreimpFarina * PreimpAcqua / 10000.0;
					// eau restante dans la recette
					if (AcquaCalc - dblTmp < 0)
					{
						strMsg = LocStr["DiminuerPcFarineEauBiga"];
						return false;
					}
				}
				// sel sur farine du preimpasto
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse)
				||	TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
				{
					dblTmp = FarinaCalc * PreimpFarina * PreimpSale / 10000.0;
					// sel restant dans la recette
					if (SaleCalc - dblTmp < 0)
					{
						strMsg = LocStr["DiminuerPcFarineSel"];
						if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
							strMsg += " (" + LocStr["Autolyse"] + ")";
						else// Biga
						    strMsg += " (Biga)";
						return false;
					}
				}
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
				{
					dblTmp = AcquaCalc * PreimpAcqua * PreimpFarina * PreimpSale / 1000000.0;
					// sel restant dans la recette
					if (SaleCalc - dblTmp < 0)
					{
						strMsg = LocStr["DiminuerPcFarineSelPool"];
						return false;
					}
				}
			}
			return bResult;
		}

		// Retourne la moyenne des températures.
		public double GetTemperaturaMedia()
		{
			double dblTemp = 0;
			double dblDurataTotale = PuntataOre + ApprettoOre;
			
			dblTemp = PuntataTemp * PuntataOre / dblDurataTotale;
			dblTemp += ApprettoTemp * ApprettoOre / dblDurataTotale;
			
			return Math.Round(dblTemp, 1);
		}

		// Calcul.
		// ancien: LB= f*57,5*(1+s/200)*(1+o/300)/((-80+4,2*i-0,0305*i^2)*(g*t)^1,2)
		// nouveau: LB= f*2820*(1+s/200)*(1+o/300)/((-80+4,2*i-0,0305*i^2)*(g^2,5)*(t^1,2))
		// LB = grammi lievito di birra fresco
		// f = grammi di farina
		// i = idratazione “alla pizzaiola” ovvero acqua/farina*100
		// g = gradi °C a cui avviene la lievitazione
		// t = tempo in ore
		// s = sale in grammi per litro
		// o = olio/strutto/burro in grammi per litro
		// c = coefficiente
		public void Calcola(ref string strMsg)
		{
			TotalImpasto = 0;
			Pdr = 0;

			CalculIsOk = false;
			//
			Temperatura = GetTemperaturaMedia();
			//
			// au 1er passage dans GetIngredienti le Lievito n'est pas encore défini.
			// on exécute donc 2 fois le calcul.
			for (int n = 0; n < 2; n++)
			{
				// check + calcul ingrédients
				if (GetIngredienti(ref strMsg) == false)
					return;
				//
				// calcul levure
				Lievito = GetLievito();
				// pour 1 L
				if (PresetLievito == false)
					LievLitro = Lievito * 1000 / AcquaCalc;
			}
			//
			if (CheckValueTempistiche(ref strMsg) == false)
				return;
			//
			CalculIsOk = true;
		}

		// Calcul des ingrédients avec check préliminaire.
		// NB: lors du 1er appel, Lievito n'est pas encore défini. Il le sera lors du 2ème appel dans la méthode Calcola.
		bool GetIngredienti(ref string strMsg, bool bIgnorePdr = false)
		{
			double dblTotalImpasto, dblFarina;
			double dblTmpBiga=0, dblTmpPoolish=0;
			
			if (CheckValues(ref strMsg) == false)
				return false;
			//
			// ingrédients
			if (ChoixQuantites == eQuantites.Choix1)
			{
				NumeroPanielliCalc1 = NumeroPanielli1;
				PesoPanielloCalc1 = PesoPaniello1;
				NumeroPanielliCalc2 = NumeroPanielli2;
				PesoPanielloCalc2 = PesoPaniello2;
				NumeroPanielliCalc3 = NumeroPanielli3;
				PesoPanielloCalc3 = PesoPaniello3;
				TotalImpasto = NumeroPanielli1 * PesoPaniello1;
				TotalImpasto += NumeroPanielli2 * PesoPaniello2;
				TotalImpasto += NumeroPanielli3 * PesoPaniello3;
				//
				if (bIgnorePdr == false)
				{
					if (PdrCalc == Convert.ToInt16(eModeCalcul.ImpastoTotale))
						Pdr = TotalImpasto * PdrPc / 100;
					else
					{
						// recalcul qté de farine pour calculer la PDR sur la farine
						dblTotalImpasto = TotalImpasto - SaleCalc - GrassiCalc - Lievito - LievitoPreImp;
						dblFarina = dblTotalImpasto / (1 + ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100) + Idro / 100);
						Pdr = dblFarina * PdrPc / 100;
					}
				}
				//
				TotalImpasto -= Pdr;
                //
                // TotalImpasto = Farina + Acqua + Sale + Grassi + Lievito + LievitoPreImp
                // avec Acqua = Farine * Idro / 100
                // Sale = Acqua * SaleLitro / 1000
                // Grassi = Acqua * GrassiLitro / 1000
				// Lievito = Acqua * LievLitro / 1000
				// LievitoPreImp: si Biga ou poolish
				if (PreimpOre > 0)
				{
					if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
						dblTmpBiga = PreImpLievKiloPc * PreimpFarina / 10000;
					else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
						dblTmpPoolish = PreImpLievKiloPc * PreimpAcqua * PreimpFarina / 1000000;
				}
				FarinaCalc = TotalImpasto / (1 + dblTmpBiga + ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100) + (Idro / 100) * (1 + (SaleLitro / 1000) + (GrassiLitro / 1000) + (LievLitro / 1000) + dblTmpPoolish) );
				//
				// si on prend les valeurs de levure calculées => bug: nécessite un 2e appui sur Calcul
		//		Console.WriteLine("Levure:" + Lievito.ToString());
		//		TotalImpasto -= Lievito + LievitoPreImp;
		//		FarinaCalc = TotalImpasto / (1 + ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100) + (Idro / 100) * (1 + (SaleLitro / 1000) + (GrassiLitro / 1000)) );
				//
                AcquaCalc = FarinaCalc * Idro / 100;
                SaleCalc = Math.Round((SaleLitro * AcquaCalc / 1000), GetSGDec());
                GrassiCalc = Math.Round((GrassiLitro * AcquaCalc / 1000), GetSGDec());
			}
			else
			{
				FarinaCalc = Farina;
				AcquaCalc = Acqua;
				SaleCalc = Sale;
				GrassiCalc = Grassi;
				TotalImpasto = FarinaCalc + AcquaCalc + SaleCalc + GrassiCalc + Lievito + LievitoPreImp + FarinaCalc * ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100);
				Idro = 100 * AcquaCalc / FarinaCalc;
				SaleLitro = SaleCalc * 1000 / AcquaCalc;
				GrassiLitro = GrassiCalc * 1000 / AcquaCalc;
				NumeroPanielliCalc1 = 1;
				//
				if (bIgnorePdr == false)
				{
					if (PdrCalc == Convert.ToInt16(eModeCalcul.ImpastoTotale))
						Pdr = TotalImpasto * PdrPc / 100;
					else
						Pdr = FarinaCalc * PdrPc / 100;
				}
				//
				PesoPanielloCalc1 = TotalImpasto + Pdr;
			}
			// total
			TotalImpasto = FarinaCalc + AcquaCalc + SaleCalc + GrassiCalc + Pdr + Lievito + LievitoPreImp + FarinaCalc * ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100);
			//
			return true;
		}

		// Retourne le nombre de décimales à appliquer au sel et huile.
		private int GetSGDec()
		{
			if (SaleGrassiDecimali == true)
				return 1;
			return 0;
		}

		// Calcul de la levure.
		double GetLievito()
		{
			double dblLievito = 0;
			double dblDurata = GetDurata();
			double dblFarina = 0;
			LievitoPreImp = 0;

			// si préfermentation
			if (PreimpOre > 0)
			{
				// si Biga ou poolish
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga)
				||	TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
				{
					if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
						dblFarina = FarinaCalc * PreimpFarina / 100;
					else// poolish
						dblFarina = AcquaCalc * PreimpAcqua * PreimpFarina / 10000.0;
					LievitoPreImp = PreImpLievKiloPc * dblFarina / 100;
				}
			}

			// levure imposée
			if (PresetLievito == true)
			{
				if (TypeCalcul == Convert.ToInt16(eTypeCalcul.Acqua))
					dblLievito = LievLitro * AcquaCalc / 1000;
				else
				{
					dblLievito =  LievKilo * FarinaCalc / 1000;
					ConvertLievFarinaToAcqua();
				}
			}
			else
			{
				// calcul spécial pour haute température
				if (CalculChaleur == true)
				{
					dblLievito = FarinaCalc * CoefCalcoloAT * (1 + SaleLitro / 200) * (1 + GrassiLitro / 300);
					dblLievito /= (-80 + 4.2 * Idro - 0.0305 * Math.Pow(Idro, 2)) * Math.Pow((Temperatura), 2.5) * Math.Pow((dblDurata), 1.2);
				}
				else
				{
					dblLievito = FarinaCalc * CoefCalcolo * (1 + SaleLitro / 200) * (1 + GrassiLitro / 300);
					dblLievito /= (-80 + 4.2 * Idro - 0.0305 * Math.Pow(Idro, 2)) * Math.Pow((Temperatura * dblDurata), 1.2);
				}
			}
			return dblLievito;
		}

        // Retourne une estimation des heures de matûration.
		private double GetDurata()
		{
			double dblResult = ApprettoOre + PuntataOre;
			// on considère une efficacité de /10 pour les heures frigo
			if (FrigoOre > 0)
			{
				dblResult -= FrigoOre;
				dblResult += FrigoOre / 10;
			}
			return dblResult;
		}

		// Retourne la valeur passée en paramètre divisée par le rapport F/S.
		double GetLievitoValueFS(double dblVal)
		{
			if (LievitoFS == eLievito.LdbS)
				return dblVal / RapportoLievFS;
			else
				return dblVal;
		}

		// Retourn F ou S suivant le type de levure.
		char GetLievitoChar()
		{
			if (LievitoFS == eLievito.LdbF)
				return 'F';
			else
				return 'S';
		}

        // Génération du datatable de résultats.
        // Appelé lors de la validation du formulaire.
        private void GenererTableResult()
        {
            DataRow row;
            string strMsg = string.Empty;
			string strPreImpasto;
			double dblTmp;
			string strTmp;
            
			ClearChildContent();
			TblResult.Clear();
			if (CalculIsOk == false)
				return;
            //
            row = TblResult.NewRow();
			row["Libelle"] = LocStr["Patons"];
			row["Valeur"] = Math.Round((double)NumeroPanielliCalc1, 0).ToString() + " x " + Math.Round((double)PesoPanielloCalc1, 0).ToString() + " g";
			row["Type"] = eTypeLigne.EnTete;
			TblResult.Rows.Add(row);
			//
			if (NumeroPanielliCalc2 > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = "";
				row["Valeur"] = Math.Round((double)NumeroPanielliCalc2, 0).ToString() + " x " + Math.Round((double)PesoPanielloCalc2, 0).ToString() + " g";
				row["Type"] = eTypeLigne.EnTete;
				TblResult.Rows.Add(row);
			}
			if (NumeroPanielliCalc3 > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = "";
				row["Valeur"] = Math.Round((double)NumeroPanielliCalc3, 0).ToString() + " x " + Math.Round((double)PesoPanielloCalc3, 0).ToString() + " g";
				row["Type"] = eTypeLigne.EnTete;
				TblResult.Rows.Add(row);
			}
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["PoidsTotal"];
			row["Valeur"] = Math.Round(TotalImpasto, 0).ToString() + " g";
			row["Type"] = eTypeLigne.EnTete;
			TblResult.Rows.Add(row);
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Ingredients"];
			row["Valeur"] = string.Empty;
			row["Type"] = eTypeLigne.Titre;
			TblResult.Rows.Add(row);
			//
			if (PreimpOre > 0)
			{
				// farine
				row = TblResult.NewRow();
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
				{
					row["Libelle"] = LocStr["Farine"] + " " + LocStr["Autolyse"];
					// % sur la farine de la recette
					dblTmp = FarinaCalc * PreimpFarina / 100.0;
					strTmp = LocStr["Farine"];
				}
				else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
				{
					row["Libelle"] = LocStr["Farine"] + " Biga";
					// % sur la farine de la recette
					dblTmp = FarinaCalc * PreimpFarina / 100.0;
					strTmp = LocStr["Farine"];
				}
				else// poolish
				{
					row["Libelle"] = LocStr["Farine"] + " Poolish";
					// % sur l'eau du poolish
					dblTmp = AcquaCalc * PreimpAcqua * PreimpFarina / 10000.0;
					strTmp = LocStr["Eau"] + " Poolish";
				}
				row["Valeur"] = Math.Round(dblTmp, 0).ToString() + " g (" + Math.Round(PreimpFarina, 0).ToString() + " % " + strTmp + ")";
				row["Type"] = eTypeLigne.PreImpasto;
				TblResult.Rows.Add(row);
				//
				// eau
				row = TblResult.NewRow();
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
				{
					row["Libelle"] = LocStr["Eau"] + " " + LocStr["Autolyse"];
					// % sur l'eau de la recette
					dblTmp = AcquaCalc * PreimpAcqua / 100.0;
					strTmp = LocStr["Eau"];
				}
				else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
				{
					row["Libelle"] = LocStr["Eau"] + " Biga";
					// % sur la farine de la biga
					dblTmp = FarinaCalc * PreimpFarina * PreimpAcqua / 10000.0;
					strTmp = LocStr["Farine"] + " Biga";
				}
				else// Poolish
				{
					row["Libelle"] = LocStr["Eau"] + " Poolish";
					// % sur l'eau de la recette
					dblTmp = AcquaCalc * PreimpAcqua / 100.0;
					strTmp = LocStr["Eau"];
				}				
				row["Valeur"] = Math.Round(dblTmp, 0).ToString() + " g (" + Math.Round(PreimpAcqua, 0).ToString() + " % " + strTmp + ")";
				row["Type"] = eTypeLigne.PreImpasto;
				TblResult.Rows.Add(row);
				//
				// sel
				if (PreimpSale > 0)
				{
					row = TblResult.NewRow();
					if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
					{
						row["Libelle"] = LocStr["Sel"] + " " + LocStr["Autolyse"];
						// % sur farine de l'autolyse
						dblTmp = FarinaCalc * PreimpFarina * PreimpSale / 10000.0;
						strTmp = LocStr["Farine"] + " " + LocStr["Autolyse"];
					}
					else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
					{
						row["Libelle"] = LocStr["Sel"] + " Biga";
						// % sur farine de la biga
						dblTmp = FarinaCalc * PreimpFarina * PreimpSale / 10000.0;
						strTmp = LocStr["Farine"] + " Biga";
					}
					else// Poolish
					{
						row["Libelle"] = LocStr["Sel"] + " Poolish";
						// % sur farine du poolish
						dblTmp = AcquaCalc * PreimpAcqua * PreimpFarina * PreimpSale / 1000000.0;
						strTmp = LocStr["Farine"] + " Poolish";
					}
					row["Valeur"] = Math.Round(dblTmp, GetSGDec()).ToString() + " g (" + Math.Round(PreimpSale, GetSGDec()).ToString() + " % " + strTmp + ")";
					row["Type"] = eTypeLigne.PreImpasto;
					TblResult.Rows.Add(row);
				}
				//
				// levure pour Biga ou Poolish
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga) || TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
				{
					row = TblResult.NewRow();
					if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
					{
						row["Libelle"] = LocStr["Levure"] + " Biga (" + GetLievitoChar() + ")";
						strTmp = LocStr["Farine"] + " Biga";
					}
					else
					{
						row["Libelle"] = LocStr["Levure"] + " Poolish (" + GetLievitoChar() + ")";
						strTmp = LocStr["Farine"] + " Poolish";
					}
					row["Valeur"] = Math.Round(GetLievitoValueFS(LievitoPreImp), 2).ToString() + " g (";
					row["Valeur"] += Math.Round(GetLievitoValueFS(PreImpLievKiloPc), 2).ToString() + " % " + strTmp + ")";
					row["Type"] = eTypeLigne.PreImpasto;
					TblResult.Rows.Add(row);
				}
			}
			//
			// farine
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Farine"];
			if (PreimpOre > 0)
			{
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse)
				||	TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
					// % restant sur farine de la recette
					dblTmp = FarinaCalc * (100 - PreimpFarina) / 100.0;
				else// poolish
					dblTmp = FarinaCalc - AcquaCalc * PreimpAcqua * PreimpFarina / 10000.0;
				row["Valeur"] = Math.Round(dblTmp, 0).ToString() + " g";
			}
			else
				row["Valeur"] = Math.Round(FarinaCalc, 0).ToString() + " g";
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			// eau
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Eau"];
			if (PreimpOre > 0)
			{
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse)
				||	TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Poolish))
					// % restant sur eau de la recette
					dblTmp = AcquaCalc * (100 - PreimpAcqua) / 100.0;
				else// biga
					dblTmp = AcquaCalc - FarinaCalc * PreimpFarina * PreimpAcqua / 10000.0;
				row["Valeur"] = Math.Round(dblTmp, 0).ToString() + " g (" + Math.Round(Idro, 0).ToString() + " %)";
			}
			else
				row["Valeur"] = Math.Round(AcquaCalc, 0).ToString() + " g (" + Math.Round(Idro, 0).ToString() + " %)";
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			// sel
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Sel"];
			if (PreimpOre > 0)
			{
				// sel sur farine du preimpasto
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse)
				||	TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
					dblTmp = FarinaCalc * PreimpFarina * PreimpSale / 10000.0;
				else// poolish
					dblTmp = AcquaCalc * PreimpAcqua * PreimpFarina * PreimpSale / 1000000.0;
				row["Valeur"] = Math.Round(SaleCalc - dblTmp, GetSGDec()).ToString() + " g (" + Math.Round(SaleLitro, GetSGDec()).ToString() + " g/lit)";
			}
			else
				row["Valeur"] = Math.Round(SaleCalc, GetSGDec()).ToString() + " g (" + Math.Round(SaleLitro, GetSGDec()).ToString() + " g/lit)";
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			if (GrassiLitro > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["Huile"];
				row["Valeur"] = Math.Round(GrassiCalc, GetSGDec()).ToString() + " g (" + Math.Round(GrassiLitro, GetSGDec()).ToString() + " g/lit)";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			//
			// levure
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Levure"] + " (" + GetLievitoChar() + ")";
			row["Type"] = eTypeLigne.Normal;
			row["Valeur"] = Math.Round(GetLievitoValueFS(Lievito), 2).ToString() + " g (";
			if (TypeCalcul == Convert.ToInt16(eTypeCalcul.Acqua) || PresetLievito == false)
				row["Valeur"] += Math.Round(GetLievitoValueFS(LievLitro), 2).ToString() + " g/lit)";
			else// farine
				row["Valeur"] += Math.Round(GetLievitoValueFS(LievKilo / 10), 3).ToString() + " % " + LocStr["Farine"] + ")";
			TblResult.Rows.Add(row);
			//
			if (PdrPc > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = "PDR";
				row["Valeur"] = Math.Round(Pdr).ToString() + " g";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			//
			// additifs
			if (Additivo1Pc > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = Additivo1Nome;
				row["Valeur"] = Math.Round(FarinaCalc * Additivo1Pc / 100, GetSGDec()).ToString() + " g (" + Additivo1Pc.ToString() + " %)";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			if (Additivo2Pc > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = Additivo2Nome;
				row["Valeur"] = Math.Round(FarinaCalc * Additivo2Pc / 100, GetSGDec()).ToString() + " g (" + Additivo2Pc.ToString() + " %)";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			if (Additivo3Pc > 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = Additivo3Nome;
				row["Valeur"] = Math.Round(FarinaCalc * Additivo3Pc / 100, GetSGDec()).ToString() + " g (" + Additivo3Pc.ToString() + " %)";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Horaires"];
			row["Valeur"] = string.Empty;
			row["Type"] = eTypeLigne.Titre;
			TblResult.Rows.Add(row);
			//
			DateTime dtDebut;
			CotturaDataOra = new DateTime(CotturaDataOra.Year, CotturaDataOra.Month, CotturaDataOra.Day, CotturaOra.Hour, CotturaOra.Minute, 0);
			dtDebut = CotturaDataOra.AddHours(-(PuntataOre + ApprettoOre));
			dtDebut = dtDebut.AddMinutes(-Impastamento);
			//
			if (PreimpOre > 0)
			{
				row = TblResult.NewRow();
				dtDebut = dtDebut.AddHours(-PreimpOre);
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
				{
					row["Libelle"] = LocStr["Autolyse"];
				}
				else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
				{
					row["Libelle"] = "Biga";
				}
				else
				{
					row["Libelle"] = "Poolish";
				}
				row["Valeur"] = dtDebut.Day.ToString() + "/" + dtDebut.Month.ToString() + "/" + dtDebut.Year.ToString().Substring(2);
				row["Valeur"] = row["Valeur"] + " - " + dtDebut.ToShortTimeString() + " (" + PreimpOre.ToString() + " h)";
				row["Type"] = eTypeLigne.PreImpasto;
				TblResult.Rows.Add(row);
			}
			//
			dtDebut = dtDebut.AddHours(PreimpOre);
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Petrissage"];
			row["Valeur"] = dtDebut.Day.ToString() + "/" + dtDebut.Month.ToString() + "/" + dtDebut.Year.ToString().Substring(2);
			row["Valeur"] = row["Valeur"] + " - " + dtDebut.ToShortTimeString() + " (" + Impastamento.ToString() + " min)";
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			dtDebut = dtDebut.AddMinutes(Impastamento);
			if (TblTempisPuntata.Rows.Count == 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["Pointage"];
				row["Valeur"] = dtDebut.Day.ToString() + "/" + dtDebut.Month.ToString() + "/" + dtDebut.Year.ToString().Substring(2);
				row["Valeur"] = row["Valeur"] + " - " + dtDebut.ToShortTimeString() + " (" + PuntataOre.ToString() + " h)";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
				
				dtDebut = dtDebut.AddHours(PuntataOre);
			}
			else
			{
				// horaires détaillés pointage
				foreach (DataRow rowT in TblTempisPuntata.Rows)
				{
					if (rowT.RowState == DataRowState.Deleted)
						continue;
					row = TblResult.NewRow();
					row["Libelle"] = LocStr["Pointage"] + " " + rowT["Libelle"].ToString();
					row["Valeur"] = dtDebut.Day.ToString() + "/" + dtDebut.Month.ToString() + "/" + dtDebut.Year.ToString().Substring(2);
					row["Valeur"] = row["Valeur"] + " - " + dtDebut.ToShortTimeString() + " (" + Convert.ToDouble(rowT["Valeur"]).ToString() + " h)";
					row["Type"] = eTypeLigne.Normal;
					TblResult.Rows.Add(row);
					
					dtDebut = dtDebut.AddHours(Convert.ToDouble(rowT["Valeur"]));
				}
			}
			//
			if (TblTempisAppretto.Rows.Count == 0)
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["Appret"];
				row["Valeur"] = dtDebut.Day.ToString() + "/" + dtDebut.Month.ToString() + "/" + dtDebut.Year.ToString().Substring(2);
				row["Valeur"] = row["Valeur"] + " - " + dtDebut.ToShortTimeString() + " (" + ApprettoOre.ToString() + " h)";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			else
			{
				// horaires détaillés apprêt
				foreach (DataRow rowT in TblTempisAppretto.Rows)
				{
					if (rowT.RowState == DataRowState.Deleted)
						continue;
					row = TblResult.NewRow();
					row["Libelle"] = LocStr["Appret"] + " " + rowT["Libelle"].ToString();
					row["Valeur"] = dtDebut.Day.ToString() + "/" + dtDebut.Month.ToString() + "/" + dtDebut.Year.ToString().Substring(2);
					row["Valeur"] = row["Valeur"] + " - " + dtDebut.ToShortTimeString() + " (" + Convert.ToDouble(rowT["Valeur"]).ToString() + " h)";
					row["Type"] = eTypeLigne.Normal;
					TblResult.Rows.Add(row);
					
					dtDebut = dtDebut.AddHours(Convert.ToDouble(rowT["Valeur"]));
				}
			}
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Cuisson"];
			row["Valeur"] = CotturaDataOra.Day.ToString() + "/" + CotturaDataOra.Month.ToString() + "/" + CotturaDataOra.Year.ToString().Substring(2);
			row["Valeur"] = row["Valeur"] + " - " + CotturaDataOra.ToShortTimeString();
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Infos"];
			row["Valeur"] = string.Empty;
			row["Type"] = eTypeLigne.Titre;
			TblResult.Rows.Add(row);
			//
			if (PreimpOre > 0)
			{
				row = TblResult.NewRow();
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
				{
					row["Libelle"] = "Temp. " + LocStr["Autolyse"];
				}
				else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
				{
					row["Libelle"] = "Temp. Biga";
				}
				else
				{
					row["Libelle"] = "Temp. Poolish";
				}
				row["Valeur"] = Math.Round(PreimpTemperatura, 1).ToString() + " °C";
				row["Type"] = eTypeLigne.PreImpasto;
				TblResult.Rows.Add(row);
			}
			//
			if (PuntataTemp != Temperatura || ApprettoTemp != Temperatura)
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["Pointage"];
				row["Valeur"] = Math.Round(PuntataTemp, 1).ToString() + " °C";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
				//
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["Appret"];
				row["Valeur"] = Math.Round(ApprettoTemp, 1).ToString() + " °C";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			else
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["Temperature"];
				row["Valeur"] = Math.Round(Temperatura, 1).ToString() + " °C";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["DureeTotale"];
			if (PreimpOre > 0)
			{
				if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
					strPreImpasto = LocStr["Autolyse"] + ":";
				else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
					strPreImpasto = "Biga:";
				else
					strPreImpasto = "Poolish:";
				row["Valeur"] = (PreimpOre + PuntataOre + ApprettoOre).ToString() + " h (" + strPreImpasto + PreimpOre.ToString() + " h : Frigo:" + FrigoOre.ToString() + " h)";
			}
			else
				row["Valeur"] = (PuntataOre + ApprettoOre).ToString() + " h (Frigo: " + FrigoOre.ToString() + " h)";
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			double dblFarina=0;
			GetMixFarineValue(eFarina.Farina1, ref dblFarina);
			if (dblFarina != Math.Round(FarinaCalc))
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["MixFarines"];
				row["Valeur"] = "(W" + Farina1W.ToString() + ") " + dblFarina.ToString();
				GetMixFarineValue(eFarina.Farina2, ref dblFarina);
				if (dblFarina > 0)
					row["Valeur"] = row["Valeur"] + " : (W" + Farina2W.ToString() + ") " + dblFarina.ToString();
				GetMixFarineValue(eFarina.Farina3, ref dblFarina);
				if (dblFarina > 0)
					row["Valeur"] = row["Valeur"] + " : (W" + Farina3W.ToString() + ") " + dblFarina.ToString();
				row["Valeur"] = row["Valeur"] + " g";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			//
			double dblAcqua = 0;
			GetMixAcqueValue(eAcqua.Acqua1, ref dblAcqua);
			if (dblAcqua != Math.Round(AcquaCalc))
			{
				row = TblResult.NewRow();
				row["Libelle"] = LocStr["MixEaux"];
				row["Valeur"] = "(" + Acqua1Nome + ") " + dblAcqua.ToString();
				GetMixAcqueValue(eAcqua.Acqua2, ref dblAcqua);
				row["Valeur"] = row["Valeur"] + " : (" + Acqua2Nome + ") " + dblAcqua.ToString();
				row["Valeur"] = row["Valeur"] + " g";
				row["Type"] = eTypeLigne.Normal;
				TblResult.Rows.Add(row);
			}
			//
			row = TblResult.NewRow();
			row["Libelle"] = LocStr["Calcul"];
			// levure imposée
			if (PresetLievito == true)
				row["Valeur"] = "Preset Ldb";
			else
			{
				if (CalculChaleur == true)
					row["Valeur"] = "Japi2 (Coef: " + CoefCalcoloAT.ToString() + ")";
				else
					row["Valeur"] = "Japi1 (Coef: " + CoefCalcolo.ToString() + ")";
			}
			row["Type"] = eTypeLigne.Normal;
			TblResult.Rows.Add(row);
			//
			TblResult.AcceptChanges();
        }

		// Retourne les valeurs des mix de farines.
		public double GetMixFarineValue(eFarina eFarina, ref double dblFarina)
		{
			double dblValuePc = 0;
			
			switch (eFarina)
			{
				case eFarina.Farina1:
					dblValuePc = 100 - Farina2Pc - Farina3Pc;
					dblFarina = Math.Round(FarinaCalc * dblValuePc / 100);
					break;

				case eFarina.Farina2:
					dblFarina = Math.Round(FarinaCalc * Farina2Pc / 100);
					break;
					
				case eFarina.Farina3:
					dblFarina = Math.Round(FarinaCalc * Farina3Pc / 100);
					break;
			}
			return dblValuePc;
		}

		// Calcul de la dureté moyenne du mix d'eaux (°f).
		public double GetMixAcque()
		{
			double dblfMix = 0;
			double dblAcqua1Pc = GetMixAcqueValue(eAcqua.Acqua1, ref dblfMix);// dblfMix pas utilisé ici
			
			dblfMix = (Acqua1Ca * 2.5 + Acqua1Mg * 4.1) * dblAcqua1Pc;
			dblfMix += (Acqua2Ca * 2.5 + Acqua2Mg * 4.1) * Acqua2Pc;
			dblfMix = Math.Round(dblfMix / 100 / 10, 1);
			
			return dblfMix;
		}

		// Retourne les valeurs des mix des eaux.
		public double GetMixAcqueValue(eAcqua eAcqua, ref double dblAcqua)
		{
			double dblValuePc = 0;
			
			switch (eAcqua)
			{
				case eAcqua.Acqua1:
					dblValuePc = 100 - Acqua2Pc;
					dblAcqua = Math.Round(AcquaCalc * dblValuePc / 100);
					break;

				case eAcqua.Acqua2:
					dblAcqua = Math.Round(AcquaCalc * Acqua2Pc / 100);
					break;
			}
			return dblValuePc;
		}

        // Génération du tableau Html de résultats.
        public void GenererHtmlResult(ref string strMsg)
        {
			Calcola(ref strMsg);
            GenererTableResult();
        
            DataRow row;
			int i = 0;
			int nCount = TblResult.Rows.Count;

            // génération dans le RenderFragment
            ChildContent = builder =>
            {
                builder.OpenElement(1, "table");
                builder.AddAttribute(1, "style", "border: 0");
                builder.OpenElement(2, "tbody");
                for (i = 0; i < nCount; i++)
			    {
                    row = TblResult.Rows[i];
                    //
                    if ((eTypeLigne)Convert.ToInt16(row["Type"]) == eTypeLigne.EnTete)
				    {
                        builder.OpenElement(3, "tr");
                        builder.AddAttribute(3, "style", "background-color: #cec9cc;");
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "width: 40%; border: 1px solid #000000;");
                            builder.AddContent(4, row["Libelle"].ToString());
                            builder.CloseElement();
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "width: 60%; border: 1px solid #000000;");
                            builder.AddContent(4, row["Valeur"].ToString());
                            builder.CloseElement();
                        builder.CloseElement();
                    }
                    else if ((eTypeLigne)Convert.ToInt16(row["Type"]) == eTypeLigne.Titre)
                    {
                        builder.OpenElement(3, "tr");
                        builder.AddAttribute(3, "style", "background-color: #e3cb1c;");
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "border: 1px solid #000000; text-align: center;");
                            builder.AddAttribute(4, "colspan", "2");
                                builder.OpenElement(4, "strong");
                                builder.AddContent(4, row["Libelle"].ToString());
                                builder.CloseElement();
                            builder.CloseElement();
                        builder.CloseElement();
                    }
                    else if ((eTypeLigne)Convert.ToInt16(row["Type"]) == eTypeLigne.PreImpasto)
                    {
                        builder.OpenElement(3, "tr");
                        builder.AddAttribute(3, "style", "background-color: #f9f4d0");
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "border: 1px solid #000000;");
                            builder.AddContent(4, row["Libelle"].ToString());
                            builder.CloseElement();
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "border: 1px solid #000000;");
                            builder.AddContent(4, row["Valeur"].ToString());
                            builder.CloseElement();
                        builder.CloseElement();
                    }
                    else// if ((eTypeLigne)Convert.ToInt16(row["Type"]) == eTypeLigne.Normal)
                    {
                        builder.OpenElement(3, "tr");
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "border: 1px solid #000000;");
                            builder.AddContent(4, row["Libelle"].ToString());
                            builder.CloseElement();
                            builder.OpenElement(4, "td");
                            builder.AddAttribute(4, "style", "border: 1px solid #000000;");
                            builder.AddContent(4, row["Valeur"].ToString());
                            builder.CloseElement();
                        builder.CloseElement();
                    }
                }
                builder.CloseElement();
                builder.CloseElement();
            };
        }

		// Retourne la qté de farine 1 en %.
		public double GetFarina1Pc()
		{
			double dblResult;
			dblResult = 100 - Farina2Pc - Farina3Pc;
			if (dblResult < 0)
				dblResult = 0;

			return dblResult;
		}

		// Calcul du W moyen du mix de farines.
		public double GetWMixFarine()
		{
			double dblWMix;
			
			dblWMix = Farina1W * GetFarina1Pc();		
			dblWMix += Farina2W * Farina2Pc;
			dblWMix += Farina3W * Farina3Pc;
			dblWMix = Math.Round(dblWMix / 100);
			
			return dblWMix;
		}
		
		// Retourne la qté de eau 1 en %.
		public double GetAcqua1Pc()
		{
			double dblResult;
			dblResult = 100 - Acqua2Pc;
			if (dblResult < 0)
				dblResult = 0;

			return dblResult;
		}

		// Importation d'une string au format Xml.
		public Int16 ImportaRicetta(string strRicetta, ref string strMsg)
		{
			XmlReader XMLReader = null;
			Int16 nNbParam = 0;
			string strCotturaData = DateTime.Now.ToShortDateString();
			string strCotturaOra = DateTime.Now.ToShortTimeString();
			
			try
			{
				XMLReader = XmlReader.Create(new StringReader(strRicetta));
				//
				nNbParam = LectureXml(ref XMLReader, ref strCotturaData, ref strCotturaOra);
				//
			    // on reconstitue la date de cuisson
				CotturaDataOra = Convert.ToDateTime(strCotturaData + " " + strCotturaOra);
				CotturaOra = CotturaDataOra;
			}
			catch (Exception ex)
			{
				strMsg += ex.Message;
			}
			finally
			{
				if (XMLReader != null)
					XMLReader.Close();
			}
			return nNbParam;
		}

		// Lecture du fichier Xml.
		Int16 LectureXml(ref XmlReader XMLReader, ref string strCotturaData, ref string strCotturaOra)
		{
			Int16 nNbParam = 0;
			string strTmp;
			string[] strSplit;

			while (XMLReader.Read())
			{
				if (XMLReader.IsStartElement())
				{
					switch (XMLReader.Name)
					{
						case "Ricetta":
							NomeRicetta = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
						// quantità
						case "Panielli":
							NumeroPanielli1 = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Panielli2":
							NumeroPanielli2 = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Panielli3":
							NumeroPanielli3 = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PesoPaniello":
							PesoPaniello1 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PesoPaniello2":
							PesoPaniello2 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PesoPaniello3":
							PesoPaniello3 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Idro":
							Idro = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "SaleLitro":
							SaleLitro = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "GrassiLitro":
							GrassiLitro = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Farina":
							Farina = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Acqua":
							Acqua = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Sale":
							Sale = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Grassi":
							Grassi = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// lievito
						case "LievitoLitro":
							LievLitro = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "LievitoKilo":
							LievKilo = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// lievito preset
						case "PresetLievito":
							PresetLievito = Convert.ToBoolean(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// type calcul lievito
						case "TipoCalcolo":
							TypeCalcul = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// calcul chaleur
						case "CalcoloPerCaldo":
							CalculChaleur = Convert.ToBoolean(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// impastamento
						case "ImpastamentoDurata":
							Impastamento = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// lievitazione
						case "PuntataDurata":
						PuntataOre = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "ApprettoDurata":
							ApprettoOre = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Temperatura":
							Temperatura = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "FrigoDurata":
							FrigoOre = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PuntataTemperatura":
							PuntataTemp = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "ApprettoTemperatura":
							ApprettoTemp = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "LievitazioneCx":
							CoefCalcolo = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "LievitazioneCxAT":
							CoefCalcoloAT = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// cottura
						case "CotturaData":
							strCotturaData = Convert.ToDateTime(XMLReader.GetAttribute("value")).ToShortDateString();
							nNbParam++;
							break;
						case "CotturaOra":
							strCotturaOra = Convert.ToDateTime(XMLReader.GetAttribute("value")).ToShortTimeString();//.Replace('.',':');
							nNbParam++;
							break;
						// note
						case "Note":
							Note = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
						// mix
						case "Farina2Pc":
							Farina2Pc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Farina3Pc":
							Farina3Pc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Farina1W":
							Farina1W = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Farina2W":
							Farina2W = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Farina3W":
							Farina3W = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// eau
						case "Acqua1Ca":
							Acqua1Ca = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Acqua1Mg":
							Acqua1Mg = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Acqua1Nome":
							Acqua1Nome = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
						case "Acqua2Pc":
							Acqua2Pc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Acqua2Ca":
							Acqua2Ca = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Acqua2Mg":
							Acqua2Mg = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Acqua2Nome":
							Acqua2Nome = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
						// horaires détaillés
						case "PuntataTempistiche":
							strTmp = XMLReader.GetAttribute("value");
							nNbParam++;
							strSplit = strTmp.Split(new Char [] {';'});
							for (int i = 0; i < strSplit.Length - 1; i += 2)
							{
								DataRow row = TblTempisPuntata.NewRow();
								if (strSplit[i] == eTypeTempis.TA.ToString())
									row["Type"] = eTypeTempis.TA;
								else if (strSplit[i] == eTypeTempis.Frigo.ToString())
									row["Type"] = eTypeTempis.Frigo;
								row["Libelle"] = strSplit[i];
								row["Valeur"] = Convert.ToDouble(strSplit[i+1]);
								TblTempisPuntata.Rows.Add(row);
								// Ajout pour alimenter les controles de l'IHM (3 max)
								if (i == 0)
								{
									PointagePhase1 = strSplit[i];
									PointagePhase1Ore = Convert.ToDouble(strSplit[i+1]);
								}
								else if (i == 2)
								{
									PointagePhase2 = strSplit[i];
									PointagePhase2Ore = Convert.ToDouble(strSplit[i+1]);
								}
								else if (i == 4)
								{
									PointagePhase3 = strSplit[i];
									PointagePhase3Ore = Convert.ToDouble(strSplit[i+1]);
								}
							}
							TblTempisPuntata.AcceptChanges();
							break;
						case "ApprettoTempistiche":
							strTmp = XMLReader.GetAttribute("value");
							nNbParam++;
							strSplit = strTmp.Split(new Char [] {';'});
							for (int i = 0; i < strSplit.Length - 1; i += 2)
							{
								DataRow row = TblTempisAppretto.NewRow();
								if (strSplit[i] == eTypeTempis.TA.ToString())
									row["Type"] = eTypeTempis.TA;
								else if (strSplit[i] == eTypeTempis.Frigo.ToString())
									row["Type"] = eTypeTempis.Frigo;
								row["Libelle"] = strSplit[i];
								row["Valeur"] = Convert.ToDouble(strSplit[i+1]);
								TblTempisAppretto.Rows.Add(row);
								// Ajout pour alimenter les controles de l'IHM (3 max)
								if (i == 0)
								{
									AppretPhase1 = strSplit[i];
									AppretPhase1Ore = Convert.ToDouble(strSplit[i+1]);
								}
								else if (i == 2)
								{
									AppretPhase2 = strSplit[i];
									AppretPhase2Ore = Convert.ToDouble(strSplit[i+1]);
								}
								else if (i == 4)
								{
									AppretPhase3 = strSplit[i];
									AppretPhase3Ore = Convert.ToDouble(strSplit[i+1]);
								}
							}
							TblTempisAppretto.AcceptChanges();
							break;
						// Pdr
						case "PdrPc":
							PdrPc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PdrCalcolo":
							PdrCalc = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "RapportoLievFS":
							RapportoLievFS = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// Teglia: /!\ ne pas modifier les accesseurs sinon le 'Set' va modifier d'autres valeurs.
						case "TLunghezza":
							dblTLunghezza1 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TLarghezza":
							dblTLarghezza1 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TMajorPc":
							dblTMajorPc1 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TLunghezza2":
							dblTLunghezza2 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TLarghezza2":
							dblTLarghezza2 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TMajorPc2":
							dblTMajorPc2 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TLunghezza3":
							dblTLunghezza3 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TLarghezza3":
							dblTLarghezza3 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TMajorPc3":
							dblTMajorPc3 = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// Autolyse
						case "PreImpastoDurata":
							PreimpOre = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PreImpastoFarina":
							PreimpFarina = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PreImpastoAcqua":
							PreimpAcqua = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PreImpastoSale":
							PreimpSale = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "TipoPreImpasto":
							TypePreimpasto = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PreimpTemperatura":
							PreimpTemperatura = Convert.ToInt16(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "PreImpLievKiloPc":
							PreImpLievKiloPc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						// additifs
						case "Additivo1Pc":
							Additivo1Pc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Additivo1Nome":
							Additivo1Nome = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
						case "Additivo2Pc":
							Additivo2Pc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Additivo2Nome":
							Additivo2Nome = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
						case "Additivo3Pc":
							Additivo3Pc = Convert.ToDouble(XMLReader.GetAttribute("value"));
							nNbParam++;
							break;
						case "Additivo3Nome":
							Additivo3Nome = XMLReader.GetAttribute("value");
							nNbParam++;
							break;
					}
				}
		    }
			return nNbParam;
		}

		// Définit le mode de calcul de la levure. 
		public void SetCalculLdb(string strValue)
		{
			if (strValue == "Acqua")
				TypeCalcul = Convert.ToInt16(eTypeCalcul.Acqua);
			else
				TypeCalcul = Convert.ToInt16(eTypeCalcul.Farina);
		}

		// Définit le type de calcul de la levure.
		public void SetTypeCalcul(string strValue)
		{
			if (strValue == "PresetLdb")
				PresetLievito = true;
			else if (strValue == "Japi1")
			{
				CalculChaleur = false;
				PresetLievito = false;
			}
			else if (strValue == "Japi2")
			{
				CalculChaleur = true;
				PresetLievito = false;
			}
		}

		// Retourne la quantité de farine pour 1 litre eau.
		double GetFarinaPerLitro()
		{
			double dblFarine;
			
			dblFarine = 1000 / (Idro / 100);
			
			return dblFarine;
		}
		
		// Convertit la quantité de levure / farine en levure / eau.
		public void ConvertLievFarinaToAcqua()
		{
			LievLitro = LievKilo * 100 / Idro;
		}

		// Retourne la quantité de levure / eau en grammes.
		public double GetLievitoAcquaGrammi()
		{
			return Math.Round(LievLitro, 2);
		}

		// Retourne la quantité de levure / eau en pourcent.
		public double GetLievitoAcquaPercent()
		{
			double dblRes = 0;
			
			dblRes = LievLitro / 10;
			
			return Math.Round(dblRes, 3);
		}
		
		// Retourne la quantité de levure / farine en grammes.
		public double GetLievitoFarinaGrammi()
		{
			double dblRes = 0, dblFarine;
			
			// farine pour 1 litre eau
			dblFarine = GetFarinaPerLitro();
			// levure sur 1kg farine
			dblRes = LievLitro * 1000 / dblFarine;
			//
			// si levure imposée et calcul sur farine
			if (PresetLievito == true && TypeCalcul == Convert.ToInt16(eTypeCalcul.Farina))
				dblRes = LievKilo;
			
			return Math.Round(dblRes, 2);
		}
		
		
		// Retourne la quantité de levure / farine en pourcent.
		public double GetLievitoFarinaPercent()
		{
			double dblRes;
			
			// levure sur 1kg farine
			dblRes = GetLievitoFarinaGrammi();
			dblRes /= 10;
			
			return Math.Round(dblRes, 3);
		}
		
		// <summary>
		public double GetSaleFarinaGrammi()
		{
			double dblRes = 0, dblFarine;
			
			// farine pour 1 litre eau
			dblFarine = GetFarinaPerLitro();
			// sel sur 1kg farine
			dblRes = SaleLitro * 1000 / dblFarine;
			
			return Math.Round(dblRes, 1);
		}
		
		
		// Retourne la quantité de sel / farine en pourcent.
		public double GetSaleFarinaPercent()
		{
			double dblRes;
			
			// sel sur 1kg farine
			dblRes = GetSaleFarinaGrammi();
			dblRes /= 10;
			
			return Math.Round(dblRes, 2);
		}

		// Converti la quantité de sel / farine en sel / eau.
		public void ConvertSaleFarinaToAcqua(double dblSaleFarina)
		{
			SaleLitro = dblSaleFarina * 100 / Idro;
		}

		// Retourne la quantité de grassi / farine en grammes.
		public double GetGrassiFarinaGrammi()
		{
			double dblRes = 0, dblFarine;
			
			// farine pour 1 litre eau
			dblFarine = GetFarinaPerLitro();
			// grassi sur 1kg farine
			dblRes = GrassiLitro * 1000 / dblFarine;
			
			return Math.Round(dblRes, 1);
		}
		
		// Retourne la quantité de grassi / farine en pourcent.
		public double GetGrassiFarinaPercent()
		{
			double dblRes;
			
			// grassi sur 1kg farine
			dblRes = GetGrassiFarinaGrammi();
			dblRes /= 10;
			
			return Math.Round(dblRes, 2);
		}

		/// Converti la quantité de grassi / farine en grassi / eau.
		public void ConvertGrassiFarinaToAcqua(double dblGrassiFarina)
		{
			GrassiLitro = dblGrassiFarina * 100 / Idro;
		}

		// Définit le type de levure (F ou S).
		public void SetTypeLdb(string strTypeLiev)
		{
			if (strTypeLiev == "LdbF")
				LievitoFS = eLievito.LdbF;
			else
				LievitoFS = eLievito.LdbS;

		}

		// Définit le mode de calcul de la PDR.
		public void SetCalculPDR(string strCalcul)
		{
			if (strCalcul == "PdrImpasto")
				PdrCalc = Convert.ToInt16(eModeCalcul.ImpastoTotale);
			else
				PdrCalc = Convert.ToInt16(eModeCalcul.FarinaSola);
		}

		// Retourne un chaine contenant la force conseillée de la farine.
		// D'après CalcolaPizza.
		public string GetWConsigliata()
		{
			double dblDurata;
			string strResult;
			
			//if (ForzaConOreFrigo == true)
			//	dblDurata = GetDurata();
			//else
			dblDurata = PuntataOre + ApprettoOre;
			double dblForzatot = 81.4206918743428 + 78.3939060802556 * Math.Log(dblDurata);
			double dblForza = Math.Round(dblForzatot/10) * 10;
			
			// protéines
			double dblProt = Math.Round((dblForza + 414.9) / 54.5, 1);
			
			strResult = string.Format(LocStr["ForceFarineConseillee"], dblForza, dblProt);
			
			return strResult;
		}

		public void SetVolvicVittel()
		{
			Acqua1Nome = "Volvic";
			Acqua1Ca = 12;
			Acqua1Mg = 8;

			Acqua2Nome = "Vittel";
			Acqua2Ca = 240;
			Acqua2Mg = 42;
			Acqua2Pc = 22;
		}

		public void SetVolvicEvian()
		{
			Acqua1Nome = "Volvic";
			Acqua1Ca = 12;
			Acqua1Mg = 8;

			Acqua2Nome = "Evian";
			Acqua2Ca = 78;
			Acqua2Mg = 24;
			Acqua2Pc = 68;
		}

		// Chargement des données.
		public Int16 ChargerDonnees(MemoryStream ms, ref string strMsg, in CultureInfo CurrentCulture)
		{
			XmlReader XMLReader = null;
			Int16 nNbParam = 0;
			string strCotturaData = DateTime.Now.ToShortDateString();
			string strCotturaOra = DateTime.Now.ToShortTimeString();//.Replace('.',':');
			//
			Thread.CurrentThread.CurrentCulture = CurrentCulture;
			//
			try
			{
				ms.Position = 0;
				XMLReader = XmlReader.Create(ms);
				//
				nNbParam = LectureXml(ref XMLReader, ref strCotturaData, ref strCotturaOra);
				//
				// on reconstitue la date de cuisson
				CotturaDataOra = Convert.ToDateTime(strCotturaData + " " + strCotturaOra);
				CotturaOra = CotturaDataOra;
			}
			catch (Exception ex)
			{
				strMsg += ex.Message;
			}
			finally
			{
				if (XMLReader != null)
					XMLReader.Close();
			}
			return nNbParam;
		}

		// Calcul le poids de la Pdr.
		public double GetPdrGrammi()
		{
			double dblTotalImpasto, dblFarina;
			Pdr = 0;

			if (ChoixQuantites == eQuantites.Choix1)
			{
				if (PdrCalc == Convert.ToInt16(eModeCalcul.ImpastoTotale))
					Pdr = Math.Round(TotalImpasto * PdrPc / 100);
				else
				{
					// recalcul qté de farine pour calculer la PDR sur la farine
					dblTotalImpasto = NumeroPanielliCalc1 * PesoPanielloCalc1;
					dblTotalImpasto += NumeroPanielliCalc2 * PesoPanielloCalc2;
					dblTotalImpasto += NumeroPanielliCalc3 * PesoPanielloCalc3;
					dblTotalImpasto -=  SaleCalc + GrassiCalc + Lievito;
					dblFarina = dblTotalImpasto / (1 + ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100) + Idro / 100);
					//
					Pdr = Math.Round(dblFarina * PdrPc / 100);
				}
			}
			else
			{
				dblTotalImpasto = FarinaCalc + AcquaCalc + SaleCalc + GrassiCalc + Lievito + LievitoPreImp + FarinaCalc * ((Additivo1Pc + Additivo2Pc + Additivo3Pc) / 100);
				//
				if (PdrCalc == Convert.ToInt16(eModeCalcul.ImpastoTotale))
					Pdr = Math.Round(dblTotalImpasto * PdrPc / 100);
				else
					Pdr = Math.Round(FarinaCalc * PdrPc / 100);
			}
			
			return Pdr;
		}

		// Définit le type de préimpasto.
		public void SetTypePreImpasto(string strValue)
		{
			if (strValue == "Autolyse")
				TypePreimpasto = Convert.ToInt16(eTypePreimpasto.Autolyse);
			else if (strValue == "Biga")
				TypePreimpasto = Convert.ToInt16(eTypePreimpasto.Biga);
			else// "Poolish"
				TypePreimpasto = Convert.ToInt16(eTypePreimpasto.Poolish);
		}
		// Définit les valeurs par défaut du préferment.
		public void SetValuesPreImpasto()
		{
			if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Autolyse))
			{
				TypeCalcul = Convert.ToInt16(eTypeCalcul.Acqua);
				PresetLievito = false;
				CalculChaleur = true;// Japi2
				//
				PreImpLievKiloPc = 0;
				if (TypeMethode == Convert.ToInt16(eTypeMethode.Italienne))
				{
					PreimpTemperatura = 18;
					PreimpFarina = 100;	// 100% farine recette
					PreimpAcqua = 55;	// 55% eau recette
					PreimpOre = 6;
				}
				else// Française
				{
					PreimpTemperatura = 20;
					PreimpFarina = 50;	// 50% farine recette
					PreimpAcqua = 100;	// 100% eau recette
					PreimpOre = 16;
				}
				PreimpSale = 0;		// % farine autolyse
			}
			else if (TypePreimpasto == Convert.ToInt16(eTypePreimpasto.Biga))
			{
				// preset Ldb recette à 0% sur farine
				TypeCalcul = Convert.ToInt16(eTypeCalcul.Farina);
				PresetLievito = true;
				LievKilo = 0;
				// biga
				PreImpLievKiloPc = 1;// 1% / farine
				PreimpTemperatura = 18;
				PreimpFarina = 100;	// 100% farine recette
				PreimpAcqua = 45;	// 45% farine biga
				PreimpSale = 0;		// % farine biga
				PreimpOre = 19;
			}
			else // Poolish
			{
				// preset Ldb recette à 0% sur farine
				TypeCalcul = Convert.ToInt16(eTypeCalcul.Farina);
				PresetLievito = true;
				LievKilo = 0;
				PreimpTemperatura = 21;
				PreimpFarina = 100;	// 100% eau poolish
				PreimpAcqua = 50;	// 50% eau recette
				PreimpSale = 0;		// % farine poolish
				PreimpOre = 16;
				//
				CalcolaLievitoPoolish();				
			}
		}
		// Définit la méthode d'autolyse.
		public void SetMethodeAutolyse(string strValue)
		{
			if (strValue == "IT")
				TypeMethode = Convert.ToInt16(eTypeMethode.Italienne);
			else// "FR"
				TypeMethode = Convert.ToInt16(eTypeMethode.Française);
		}

		// TODO: à appeler lors du changement durée poolish
		public void CalcolaLievitoPoolish()
		{
			double dblTmp;

			if (PreimpOre <= 3)
				dblTmp = -0.01*PreimpOre+0.045;
			else
				dblTmp = 0.00000001964*Math.Pow(PreimpOre,6)-0.000001373841*Math.Pow(PreimpOre,5)+0.00003856769*Math.Pow(PreimpOre,4)-0.00055632627*Math.Pow(PreimpOre,3)+0.004428364621*Math.Pow(PreimpOre,2)-0.01982851654*PreimpOre+0.046814567303;
			PreImpLievKiloPc = Math.Round(100 * dblTmp, 2);
		}

		// Application des pourcentages définis.
		public bool UsaPercentuale(ref string strMsg)
		{
			if (CheckValuesForUsePC(ref strMsg) == false)
				return false;
			
			Acqua = Math.Round(Farina * Idro / 100, 0);
			Sale = Math.Round(SaleLitro * Acqua / 1000, GetSGDec());
			Grassi = Math.Round(GrassiLitro * Acqua / 1000, GetSGDec());
			
			return true;
		}

		// Contrôle des valeurs pour l'utilisation des pourcentages définis.
		private bool CheckValuesForUsePC(ref string strMsg)
		{
			bool bResult = true;
			
			// valeurs négatives
			if (	Idro < 0
			    ||	SaleLitro < 0
			    ||	GrassiLitro < 0
			   )
			{
				strMsg = LocStr["ValPositivesRequises"];
				return false;
			}
			
			// idro tra 50 e 100
			if (Idro < 50 || Idro > 100)
			{
				strMsg = LocStr["PcHydratEntre50_100"];
				return false;
			}
			
			// sale tra 0 e 60
			if (SaleLitro < 0 || SaleLitro > 60)
			{
				strMsg = LocStr["TauxSellitreEntre0_60"];
				return false;
			}
			
			// grassi tra 0 e 60
			if (GrassiLitro < 0 || GrassiLitro > 60)
			{
				strMsg = LocStr["TauxHuilelitreEntre0_60"];
				return false;
			}
			
			return bResult;
		}

		// Contrôle des valeurs pour les horaires détaillés.
		public bool CheckValueTempistiche(ref string strMsg)
		{
			bool bResult = true;
			
			double dblTA1 = 0, dblFrigo1 = 0;
			double dblTA2 = 0, dblFrigo2 = 0;
			
			if (TblTempisPuntata.Rows.Count + TblTempisAppretto.Rows.Count == 0)
				return true;
			
			// pointage
			GetOreTempistiche(ref dblTA1, ref dblFrigo1, true);
			// apprêt
			GetOreTempistiche(ref dblTA2, ref dblFrigo2, false);
			
			if (dblTA1 + dblFrigo1 != PuntataOre)
			{
				strMsg = LocStr["ErreurHeuresPointageTAFrigo"];
				return false;
			}
			
			if (dblTA2 + dblFrigo2 != ApprettoOre)
			{
				strMsg = LocStr["ErreurHeuresAppretTAFrigo"];
				return false;
			}
			
			if (dblFrigo1 + dblFrigo2 != FrigoOre)
			{
				strMsg = LocStr["ErreurHeuresFrigoTAFrigo"];
				return false;
			}
			
			return bResult;
		}
		// Retourne le total des heures TA et le total des heures Frigo pour une phase.
		void GetOreTempistiche(ref double dblTA, ref double dblFrigo, bool bPuntata)
		{
			if (bPuntata == true)
		    {
				foreach (DataRow row in TblTempisPuntata.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					if (Convert.ToInt16(row["Type"]) == Convert.ToInt16(eTypeTempis.TA))
						dblTA += Convert.ToDouble(row["Valeur"]);
					if (Convert.ToInt16(row["Type"]) == Convert.ToInt16(eTypeTempis.Frigo))
						dblFrigo += Convert.ToDouble(row["Valeur"]);
				}
			}
			else
		    {
				foreach (DataRow row in TblTempisAppretto.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					if (Convert.ToInt16(row["Type"]) == Convert.ToInt16(eTypeTempis.TA))
						dblTA += Convert.ToDouble(row["Valeur"]);
					if (Convert.ToInt16(row["Type"]) == Convert.ToInt16(eTypeTempis.Frigo))
						dblFrigo += Convert.ToDouble(row["Valeur"]);
				}
			}
		}
		public double GetTotalPointage()
		{
			double dblTA=0;
			double dblFrigo=0;
			GetOreTempistiche(ref dblTA, ref dblFrigo, true);

			return dblTA + dblFrigo;
		}
		public double GetTotalPointageFrigo()
		{
			double dblTA=0;
			double dblFrigo=0;
			GetOreTempistiche(ref dblTA, ref dblFrigo, true);

			return dblFrigo;
		}
		public double GetTotalAppret()
		{
			double dblTA=0;
			double dblFrigo=0;
			GetOreTempistiche(ref dblTA, ref dblFrigo, false);

			return dblTA + dblFrigo;
		}
		public double GetTotalAppretFrigo()
		{
			double dblTA=0;
			double dblFrigo=0;
			GetOreTempistiche(ref dblTA, ref dblFrigo, false);

			return dblFrigo;
		}

		private void UpdateTempistichePuntata()
		{
			TblTempisPuntata.Clear();
			if (PointagePhase1Ore > 0)
				DtTableTempisAddRow(true, PointagePhase1, PointagePhase1Ore);
			if (PointagePhase2Ore > 0)
				DtTableTempisAddRow(true, PointagePhase2, PointagePhase2Ore);
			if (PointagePhase3Ore > 0)
				DtTableTempisAddRow(true, PointagePhase3, PointagePhase3Ore);
		}
		private void UpdateTempisticheAppretto()
		{
			TblTempisAppretto.Clear();
			if (AppretPhase1Ore > 0)
				DtTableTempisAddRow(false, AppretPhase1, AppretPhase1Ore);
			if (AppretPhase2Ore > 0)
				DtTableTempisAddRow(false, AppretPhase2, AppretPhase2Ore);
			if (AppretPhase3Ore > 0)
				DtTableTempisAddRow(false, AppretPhase3, AppretPhase3Ore);
		}

		private void DtTableTempisAddRow(bool bPuntata, string strType, double dblOre)
		{
			DataTable dtTable;
			if (bPuntata == true)
				dtTable = TblTempisPuntata;
			else
				dtTable = TblTempisAppretto;
			//
			DataRow newrow = dtTable.NewRow();
			if (strType == "TA")
				newrow["Type"] = eTypeTempis.TA;
			else
				newrow["Type"] = eTypeTempis.Frigo;
			//
			newrow["Libelle"] = strType;
			newrow["Valeur"] = dblOre;
			//
			dtTable.Rows.Add(newrow);
		}

		// Définit le poids du pâton 1 par rapport aux dimensions de la teglia.
		public void SetPesoTeglia1()
		{
			double dblTmp;

			dblTmp = TLunghezza1 * TLarghezza1 / 2;
			dblTmp *= 1 + TMajorPc1 * 0.01;
			PesoPaniello1 = Math.Round(dblTmp, 0);
		}

		// Définit le poids du pâton 2 par rapport aux dimensions de la teglia.
		public void SetPesoTeglia2()
		{
			double dblTmp;

			dblTmp = TLunghezza2 * TLarghezza2 / 2;
			dblTmp *= 1 + TMajorPc2 * 0.01;
			PesoPaniello2 = Math.Round(dblTmp, 0);
		}

		// Définit le poids du pâton 3 par rapport aux dimensions de la teglia.
		public void SetPesoTeglia3()
		{
			double dblTmp;

			dblTmp = TLunghezza3 * TLarghezza3 / 2;
			dblTmp *= 1 + TMajorPc3 * 0.01;
			PesoPaniello3 = Math.Round(dblTmp, 0);
		}

		// Ecriture des données dans un XmlWriter.
		// 22/02/2021: bugfix .Net5, redéfinition du CurrentCulture pour le thread.
		public Int16 ExporterDonneesXml(ref XmlWriter xmlWriter, ref string strMsg, in CultureInfo CurrentCulture)
		{
			Int16 nNbParam = 0;

			Thread.CurrentThread.CurrentCulture = CurrentCulture;
			
			try
			{
				GenererDonneesXml(ref xmlWriter, ref nNbParam, false);// bExport = false pour sauver les dates de cuisson
			}
			catch (Exception ex)
			{
				strMsg += ex.Message;
			}
			finally
			{
				if (xmlWriter != null)
				{
					xmlWriter.Flush();
					xmlWriter.Close();
				}
			}
			return nNbParam;
		}

		// Génération des données.
		private void GenererDonneesXml(ref XmlWriter XMLWriter, ref short nNbParam, bool bExport)
		{
			string strTmp;

			XMLWriter.WriteStartDocument(true);
			XMLWriter.WriteStartElement("RafCalc_Ricetta");
			if (bExport == false)
				XMLWriter.WriteComment("Dati per RafCalc");
			//
			XMLWriter.WriteStartElement("Ricetta");
			XMLWriter.WriteAttributeString("value", NomeRicetta.Trim());
			XMLWriter.WriteEndElement();
			nNbParam++;
			// quantità
			XMLWriter.WriteStartElement("Panielli");
			XMLWriter.WriteAttributeString("value", NumeroPanielli1.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Panielli2");
			XMLWriter.WriteAttributeString("value", NumeroPanielli2.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Panielli3");
			XMLWriter.WriteAttributeString("value", NumeroPanielli3.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PesoPaniello");
			XMLWriter.WriteAttributeString("value", PesoPaniello1.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PesoPaniello2");
			XMLWriter.WriteAttributeString("value", PesoPaniello2.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PesoPaniello3");
			XMLWriter.WriteAttributeString("value", PesoPaniello3.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Idro");
			XMLWriter.WriteAttributeString("value", Idro.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("SaleLitro");
			XMLWriter.WriteAttributeString("value", SaleLitro.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("GrassiLitro");
			XMLWriter.WriteAttributeString("value", GrassiLitro.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Farina");
			XMLWriter.WriteAttributeString("value", Farina.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua");
			XMLWriter.WriteAttributeString("value", Acqua.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Sale");
			XMLWriter.WriteAttributeString("value", Sale.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Grassi");
			XMLWriter.WriteAttributeString("value", Grassi.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// impastamento
			XMLWriter.WriteStartElement("ImpastamentoDurata");
			XMLWriter.WriteAttributeString("value", Impastamento.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// lievitazione
			XMLWriter.WriteStartElement("PuntataDurata");
			XMLWriter.WriteAttributeString("value", PuntataOre.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("ApprettoDurata");
			XMLWriter.WriteAttributeString("value", ApprettoOre.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Temperatura");
			XMLWriter.WriteAttributeString("value", Temperatura.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("FrigoDurata");
			XMLWriter.WriteAttributeString("value", FrigoOre.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("LievitazioneCx");
			XMLWriter.WriteAttributeString("value", CoefCalcolo.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("LievitazioneCxAT");
			XMLWriter.WriteAttributeString("value", CoefCalcoloAT.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PuntataTemperatura");
			XMLWriter.WriteAttributeString("value", PuntataTemp.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("ApprettoTemperatura");
			XMLWriter.WriteAttributeString("value", ApprettoTemp.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			if (bExport == false)
			{
				// cottura
				XMLWriter.WriteStartElement("CotturaData");
				XMLWriter.WriteAttributeString("value", CotturaDataOra.ToShortDateString());
				XMLWriter.WriteEndElement();
				nNbParam++;
				//
				XMLWriter.WriteStartElement("CotturaOra");
				XMLWriter.WriteAttributeString("value", CotturaDataOra.ToShortTimeString());//.Replace('.',':'));
				XMLWriter.WriteEndElement();
				nNbParam++;
			}
			//
			// lievito
			XMLWriter.WriteStartElement("LievitoLitro");
			XMLWriter.WriteAttributeString("value", Math.Round(LievLitro, 4).ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("LievitoKilo");
			XMLWriter.WriteAttributeString("value", Math.Round(LievKilo, 4).ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// lievito preset
			XMLWriter.WriteStartElement("PresetLievito");
			XMLWriter.WriteAttributeString("value", PresetLievito.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// type calcul lievito
			XMLWriter.WriteStartElement("TipoCalcolo");
			XMLWriter.WriteAttributeString("value", TypeCalcul.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// calcul chaleur
			XMLWriter.WriteStartElement("CalcoloPerCaldo");
			XMLWriter.WriteAttributeString("value", CalculChaleur.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// note
			XMLWriter.WriteStartElement("Note");
			XMLWriter.WriteAttributeString("value", Note.Trim());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// mix
			XMLWriter.WriteStartElement("Farina2Pc");
			XMLWriter.WriteAttributeString("value", Farina2Pc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Farina3Pc");
			XMLWriter.WriteAttributeString("value", Farina3Pc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Farina1W");
			XMLWriter.WriteAttributeString("value", Farina1W.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;//
			XMLWriter.WriteStartElement("Farina2W");
			XMLWriter.WriteAttributeString("value", Farina2W.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;//
			XMLWriter.WriteStartElement("Farina3W");
			XMLWriter.WriteAttributeString("value", Farina3W.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// eaux
			XMLWriter.WriteStartElement("Acqua1Ca");
			XMLWriter.WriteAttributeString("value", Acqua1Ca.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua1Mg");
			XMLWriter.WriteAttributeString("value", Acqua1Mg.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua1Nome");
			XMLWriter.WriteAttributeString("value", Acqua1Nome);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua2Pc");
			XMLWriter.WriteAttributeString("value", Acqua2Pc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua2Ca");
			XMLWriter.WriteAttributeString("value", Acqua2Ca.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua2Mg");
			XMLWriter.WriteAttributeString("value", Acqua2Mg.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Acqua2Nome");
			XMLWriter.WriteAttributeString("value", Acqua2Nome);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// horaires détaillés
			strTmp = string.Empty;
			foreach (DataRow row in TblTempisPuntata.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				strTmp += ((eTypeTempis)Convert.ToInt16(row["Type"])).ToString() + ";";
				strTmp += Convert.ToDouble(row["Valeur"]).ToString() + ";";
			}
			XMLWriter.WriteStartElement("PuntataTempistiche");
			XMLWriter.WriteAttributeString("value", strTmp);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			strTmp = string.Empty;
			foreach (DataRow row in TblTempisAppretto.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				strTmp += ((eTypeTempis)Convert.ToInt16(row["Type"])).ToString() + ";";
				strTmp += Convert.ToDouble(row["Valeur"]).ToString() + ";";
			}
			XMLWriter.WriteStartElement("ApprettoTempistiche");
			XMLWriter.WriteAttributeString("value", strTmp);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// Pdr
			XMLWriter.WriteStartElement("PdrPc");
			XMLWriter.WriteAttributeString("value", PdrPc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PdrCalcolo");
			XMLWriter.WriteAttributeString("value", PdrCalc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("RapportoLievFS");
			XMLWriter.WriteAttributeString("value", RapportoLievFS.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// Teglia
			XMLWriter.WriteStartElement("TLunghezza");
			XMLWriter.WriteAttributeString("value", TLunghezza1.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TLarghezza");
			XMLWriter.WriteAttributeString("value", TLarghezza1.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TMajorPc");
			XMLWriter.WriteAttributeString("value", TMajorPc1.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TLunghezza2");
			XMLWriter.WriteAttributeString("value", TLunghezza2.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TLarghezza2");
			XMLWriter.WriteAttributeString("value", TLarghezza2.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TMajorPc2");
			XMLWriter.WriteAttributeString("value", TMajorPc2.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TLunghezza3");
			XMLWriter.WriteAttributeString("value", TLunghezza3.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TLarghezza3");
			XMLWriter.WriteAttributeString("value", TLarghezza3.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TMajorPc3");
			XMLWriter.WriteAttributeString("value", TMajorPc3.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// PreImpasto
			XMLWriter.WriteStartElement("PreImpastoDurata");
			XMLWriter.WriteAttributeString("value", PreimpOre.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PreImpastoFarina");
			XMLWriter.WriteAttributeString("value", PreimpFarina.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PreImpastoAcqua");
			XMLWriter.WriteAttributeString("value", PreimpAcqua.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PreImpastoSale");
			XMLWriter.WriteAttributeString("value", PreimpSale.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("TipoPreImpasto");
			XMLWriter.WriteAttributeString("value", TypePreimpasto.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PreimpTemperatura");
			XMLWriter.WriteAttributeString("value", PreimpTemperatura.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("PreImpLievKiloPc");
			XMLWriter.WriteAttributeString("value", PreImpLievKiloPc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			// additifs
			XMLWriter.WriteStartElement("Additivo1Pc");
			XMLWriter.WriteAttributeString("value", Additivo1Pc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Additivo2Pc");
			XMLWriter.WriteAttributeString("value", Additivo2Pc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Additivo3Pc");
			XMLWriter.WriteAttributeString("value", Additivo3Pc.ToString());
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Additivo1Nome");
			XMLWriter.WriteAttributeString("value", Additivo1Nome);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Additivo2Nome");
			XMLWriter.WriteAttributeString("value", Additivo2Nome);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteStartElement("Additivo3Nome");
			XMLWriter.WriteAttributeString("value", Additivo3Nome);
			XMLWriter.WriteEndElement();
			nNbParam++;
			//
			XMLWriter.WriteEndElement();
			XMLWriter.WriteEndDocument();
		}

		// Retourne le tableau des résultats sous forme de chaine.
		public void GenererExportTexte(ref string strRes, ref string strMsg)
		{
			Calcola(ref strMsg);
			GenererTableResult();
			
			foreach(DataRow row in TblResult.Rows)
			{
				if ((eTypeLigne)Convert.ToInt16(row["Type"]) == eTypeLigne.Titre)
					continue;
				
				strRes += row["Libelle"].ToString();
				strRes += "   ";
				strRes += row["Valeur"].ToString();
				strRes += Environment.NewLine;
			}
		}

		// Détermine la durée optimale de la Biga par interpolation linéaire des résultats de MasterBiga.
		public string GetDureeBiga()
		{
			string strMsg = LocStr["DurataMaturConsigliata"] + " {0} h";
			double dblRes = 0;
			string strRes="???";
			double x1=40, y1=0, x3=60, y3=0;

            switch (PreimpTemperatura)
			{
				case 12:
				y1 = 30.83;
				y3 = 21.15;
				break;

				case 13:
				y1 = 28.47;
				y3 = 19.53;
				break;

				case 14:
				y1 = 26.43;
				y3 = 18.13;
				break;

				case 15:
				y1 = 24.67;
				y3 = 16.92;
				break;

				case 16:
				y1 = 23.13;
				y3 = 15.87;
				break;

				case 17:
				y1 = 21.77;
				y3 = 14.93;
				break;

				case 18:
				y1 = 20.55;
				y3 = 14.1;
				break;

				case 19:
				y1 = 19.47;
				y3 = 13.37;
				break;

				case 20:
				y1 = 18.5;
				y3 = 12.68;
				break;

				case 21:
				y1 = 17.62;
				y3 = 12.08;
				break;

				case 22:
				y1 = 16.82;
				y3 = 11.53;
				break;

				case 23:
				y1 = 16.08;
				y3 = 11.03;
				break;

				case 24:
				y1 = 15.42;
				y3 = 10.57;
				break;
			}
			if (PreimpAcqua >=40 && PreimpAcqua <=60 && y1 + y3 > 0)
			{
				dblRes = GetInterpolationLineaire(x1, y1, PreimpAcqua, x3, y3);
				dblRes = Math.Round(dblRes,1);
			}

			if (dblRes > 0)
				strRes = string.Format("{0}", Math.Round(dblRes, 2));

			return string.Format(strMsg, strRes);
		}

		// Retourne y2 calculé par interpolation linéaire.
		private double GetInterpolationLineaire(double x1, double y1, double x2, double x3, double y3)
		{
			double y2;

			//y2 = ((x3 - x2) / (x3 - x1)) * y1 + ((x2 - x1) / (x3 - x1)) * y3;

			y2 = (x2 - x1) * (y3 - y1) / (x3 - x1);
			y2 += y1;

			return y2;
		}
    }
}
