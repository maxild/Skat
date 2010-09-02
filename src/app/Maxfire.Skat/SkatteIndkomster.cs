﻿using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class SkatteIndkomster : ISpecificeredeSkatteIndkomster
	{
		private readonly List<ITextValuePair<PersonligIndkomstValue>> _personligeIndkomsterAM = new List<ITextValuePair<PersonligIndkomstValue>>();
		private readonly List<ITextValuePair<PersonligIndkomstValue>> _personligeIndkomsterEjAM = new List<ITextValuePair<PersonligIndkomstValue>>();
		private readonly List<ITextValuePair<PersonligIndkomstValue>> _personligeIndkomsterFremfoertUnderskud = new List<ITextValuePair<PersonligIndkomstValue>>();
		private readonly List<ITextValuePair<PersonligIndkomstValue>> _personligeIndkomsterModregninger = new List<ITextValuePair<PersonligIndkomstValue>>();
		private readonly List<ITextValuePair<PersonligIndkomstValue>> _personligeIndkomsterUnderskudTilFremfoersel = new List<ITextValuePair<PersonligIndkomstValue>>();

		private readonly List<ITextValuePair<decimal>> _kapitalPensionsindskudModregninger = new List<ITextValuePair<decimal>>();
		
		private readonly List<ITextValuePair<decimal>> _kapitalIndkomster = new List<ITextValuePair<decimal>>();
		private readonly List<ITextValuePair<decimal>> _kapitalIndkomstModregninger = new List<ITextValuePair<decimal>>();

		private readonly List<ITextValuePair<decimal>> _ligningsmaessigeFradrag = new List<ITextValuePair<decimal>>();
		
		private readonly List<ITextValuePair<decimal>> _skattepligtigeIndkomster = new List<ITextValuePair<decimal>>();
		private readonly List<ITextValuePair<decimal>> _skattepligtigeIndkomsterFremfoertUnderskud = new List<ITextValuePair<decimal>>();
		private readonly List<ITextValuePair<decimal>> _skattepligtigeIndkomsterModregninger = new List<ITextValuePair<decimal>>();
		private readonly List<ITextValuePair<decimal>> _skattepligtigeIndkomsterUnderskudTilFremfoersel = new List<ITextValuePair<decimal>>();

		public SkatteIndkomster(
			ISpecficeredeSelvangivneBeloeb selvangivneBeloeb, 
			IAMIndkomstSkatteberegner amBidragBeregner, 
			IAMIndkomstSkatteberegner beskaeftigelsesfradragBeregner,
			int skatteAar)
		{
			selvangivneBeloeb.ThrowIfNull("selvangivneBeloeb");
			
			foreach (var item in selvangivneBeloeb.PersonligeIndkomsterAMIndkomster)
			{
				decimal amIndkomst = item.Value;
				decimal amBidrag = amBidragBeregner.Beregn(amIndkomst, skatteAar);
				_personligeIndkomsterAM.AddTextValue(item.Text, amIndkomst, amBidrag);
			}
			
			foreach (var item in selvangivneBeloeb.PersonligeIndkomsterEjAMIndkomster)
			{
				_personligeIndkomsterAM.AddTextValue(item.Text, item.Value);
			}

			if (selvangivneBeloeb.PersonligIndkomstFremfoertUnderskud > 0)
			{
				_personligeIndkomsterFremfoertUnderskud.AddTextValue(
					"Underskud i personlig indkomst fra tidligere år", 
					selvangivneBeloeb.PersonligIndkomstFremfoertUnderskud);
			}

			foreach (var item in selvangivneBeloeb.KapitalIndkomster)
			{
				_kapitalIndkomster.Add(item);
			}

			foreach (var item in selvangivneBeloeb.LigningsmaessigeFradragMinusBeskaeftigelsesfradrag)
			{
				_ligningsmaessigeFradrag.Add(item);
			}
			decimal beskaeftigelsesfradrag = beskaeftigelsesfradragBeregner.Beregn(selvangivneBeloeb.PersonligIndkomstAMIndkomst, skatteAar);
			_ligningsmaessigeFradrag.AddTextValue("Beskæftigelsesfradrag", beskaeftigelsesfradrag);

			_skattepligtigeIndkomster.AddTextValue("Personlig indkomst", PersonligIndkomst);
			_skattepligtigeIndkomster.AddTextValue("Nettokapitalindkomst", NettoKapitalIndkomst);
			_skattepligtigeIndkomster.AddTextValue("Ligningsmæssigt fradrag", -LigningsmaessigtFradrag);

			if (selvangivneBeloeb.SkattepligtigIndkomstFremfoertUnderskud > 0)
			{
				_skattepligtigeIndkomsterFremfoertUnderskud.AddTextValue(
					"Underskud i skattepligtig indkomst fra tidligere år.",
					selvangivneBeloeb.SkattepligtigIndkomstFremfoertUnderskud);
			}

			AktieIndkomst = selvangivneBeloeb.AktieIndkomst;
			KapitalPensionsindskud = selvangivneBeloeb.KapitalPensionsindskud;
		}

		/// <summary>
		/// Den del af den personlige indkomst (før AM-bidrag), der er 
		/// grundlag for beregningen af arbejdsmarkedsbidrag (AM-bidrag).
		/// </summary>
		private decimal? _amIndkomst;
		public decimal AMIndkomst
		{
			get { return _amIndkomst ?? (_amIndkomst = _personligeIndkomsterAM.Sum(x => x.Value.FoerAMBidrag)).Value; }
		}

		/// <summary>
		/// Personlig indkomst (før AM-bidrag), der bygger på de selvangivne beløb (dvs. før modregning).
		/// </summary>
		private decimal? _personligIndkomstFoerAMBidrag;
		public decimal PersonligIndkomstFoerAMBidrag
		{
			get { return _personligIndkomstFoerAMBidrag ?? (_personligIndkomstFoerAMBidrag = _personligeIndkomsterAM.Sum(x => x.Value.FoerAMBidrag) + _personligeIndkomsterEjAM.Sum(x => x.Value.FoerAMBidrag)).Value; }
		}

		/// <summary>
		/// Personlig indkomst (efter AM-bidrag), der bygger på de selvangivne beløb (dvs. før modregning).
		/// </summary>
		private decimal? _personligIndkomst;
		public decimal PersonligIndkomst
		{
			get { return _personligIndkomst ?? (_personligIndkomst = _personligeIndkomsterAM.Sum(x => x.Value.EfterAMBidrag) + _personligeIndkomsterEjAM.Sum(x => x.Value.FoerAMBidrag)).Value; }
		}

		public decimal PersonligIndkomstAaretsUnderskud
		{
			get { return (-PersonligIndkomstSkattegrundlag).NonNegative(); }
		}

		public decimal PersonligIndkomstFremfoertUnderskud
		{
			get { return _personligeIndkomsterFremfoertUnderskud.Sum(x => x.Value.FoerAMBidrag); }
		}
		
		/// <summary>
		/// Modregninger i den personlige indkomst, der har betydning for skatteberegningen.
		/// </summary>
		/// <remarks>
		/// Disse modregninger skyldes overførsler mellem ægtefæller 
		/// baseret på negativ personlig indkomst hos en af ægtefællerne.
		/// </remarks>
		public decimal PersonligIndkomstModregninger
		{
			get { return _personligeIndkomsterModregninger.Sum(x => x.Value.FoerAMBidrag); }
		}

		/// <summary>
		/// Personlig indkomst (efter AM-bidrag) vedr. bund-, mellem og topskat (dvs. efter modregning).
		/// </summary>
		public decimal PersonligIndkomstSkattegrundlag
		{
			get { return PersonligIndkomst + PersonligIndkomstModregninger; }
		}

		public decimal PersonligIndkomstUnderskudTilFremfoersel
		{
			get { return _personligeIndkomsterUnderskudTilFremfoersel.Sum(x => x.Value.FoerAMBidrag); }
		}

		public void NedbringUnderskudForPersonligIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_personligeIndkomsterModregninger.AddTextValue(text, overfoertUnderskud);
			}
		}

		public void NedbringPersonligIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_personligeIndkomsterModregninger.AddTextValue(text, -overfoertUnderskud);
			}
		}

		public void NedbringFremfoertUnderskudForPersonligIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_personligeIndkomsterFremfoertUnderskud.AddTextValue(text, overfoertUnderskud);
			}
		}

		public void TilfoejUnderskudTilFremfoerselForPersonligIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_personligeIndkomsterUnderskudTilFremfoersel.AddTextValue(text, overfoertUnderskud);
			}
		}

		private decimal? _nettoKapitalIndkomst;
		public decimal NettoKapitalIndkomst
		{
			get { return _nettoKapitalIndkomst ?? (_nettoKapitalIndkomst = _kapitalIndkomster.Sum(x => x.Value)).Value; }
		}
		
		public decimal NettoKapitalIndkomstModregninger
		{
			get { return _kapitalIndkomstModregninger.Sum(x => x.Value); }
		}

		public decimal NettoKapitalIndkomstSkattegrundlag
		{
			get { return NettoKapitalIndkomst + NettoKapitalIndkomstModregninger ; }
		}

		public void NedbringNettoKapitalIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_kapitalIndkomstModregninger.AddTextValue(text, -overfoertUnderskud);
			}
		}

		private decimal? _ligningsmaessigtFradrag;
		public decimal LigningsmaessigtFradrag
		{
			get { return _ligningsmaessigtFradrag ?? (_ligningsmaessigtFradrag = _ligningsmaessigeFradrag.Sum(x => x.Value)).Value; }
		}

		public decimal KapitalPensionsindskud { get; private set; }
		
		public decimal KapitalPensionsindskudSkattegrundlag
		{
			get { return KapitalPensionsindskud + KapitalPensionsindskudModregninger; }
		}

		public decimal KapitalPensionsindskudModregninger
		{
			get { return _kapitalPensionsindskudModregninger.Sum(x => x.Value); }
		}

		public void NedbringKapitalPensionsindskud(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_kapitalPensionsindskudModregninger.AddTextValue(text, -overfoertUnderskud);
			}
		}

		public decimal AktieIndkomst { get; private set; }

		private decimal? _skattepligtigIndkomst;
		public decimal SkattepligtigIndkomst
		{
			get { return _skattepligtigIndkomst ?? (_skattepligtigIndkomst = _skattepligtigeIndkomster.Sum(x => x.Value)).Value; }
		}

		public decimal SkattepligtigIndkomstAaretsUnderskud
		{
			get { return (-SkattepligtigIndkomstSkattegrundlag).NonNegative(); }
		}

		public decimal SkattepligtigIndkomstFremfoertUnderskud
		{
			get { return _skattepligtigeIndkomsterFremfoertUnderskud.Sum(x => x.Value); }
		}

		// TODO: Part of interface and maybe rename to SkattepligtigIndkomstModregnetUnderskud
		public decimal SkattepligtigIndkomstModregninger
		{
			get { return _skattepligtigeIndkomsterModregninger.Sum(x => x.Value); }
		}

		public decimal SkattepligtigIndkomstSkattegrundlag
		{
			get { return SkattepligtigIndkomst + SkattepligtigIndkomstModregninger; }
		}

		public decimal SkattepligtigIndkomstUnderskudTilFremfoersel
		{
			get { return _skattepligtigeIndkomsterUnderskudTilFremfoersel.Sum(x => x.Value); }
		}

		public void NedbringUnderskudForSkattepligtigIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_skattepligtigeIndkomsterModregninger.AddTextValue(text, overfoertUnderskud);
			}
		}

		public void NedbringSkattepligtigIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_skattepligtigeIndkomsterModregninger.AddTextValue(text, -overfoertUnderskud);
			}
		}

		public void NedbringFremfoertUnderskudForSkattepligtigIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_skattepligtigeIndkomsterFremfoertUnderskud.AddTextValue(text, -overfoertUnderskud);
			}
		}

		public void TilfoejUnderskudTilFremfoerselForSkattepligtigIndkomst(string text, decimal overfoertUnderskud)
		{
			if (overfoertUnderskud > 0)
			{
				_skattepligtigeIndkomsterUnderskudTilFremfoersel.AddTextValue(text, overfoertUnderskud);
			}
		}

		public IPersonligIndkomstBeloebCollection PersonligeIndkomster
		{
			get
			{
				return new PersonligIndkomstBeloebCollection(
					_personligeIndkomsterAM
						.Concat(_personligeIndkomsterEjAM)
						.Concat(_personligeIndkomsterModregninger));
			}
		}

		public IBeloebCollection KapitalIndkomster
		{
			get { return null; }
		}

		public IBeloebCollection LigningsmaessigeFradrag
		{
			get { return null; }
		}

		public IBeloebCollection SkattepligtigIndkomster
		{
			get { return null; }
		}
	}

	internal static class TextValuePairCollectionExtensions
	{
		public static void AddTextValue(this IList<ITextValuePair<decimal>> collection, string text, decimal value)
		{
			collection.Add(new TextValuePair<decimal>(text, value));
		}

		public static void AddTextValue(this IList<ITextValuePair<PersonligIndkomstValue>> collection, string text, decimal value, decimal? amBidrag = null)
		{
			collection.Add(new TextValuePair<PersonligIndkomstValue>(text, new PersonligIndkomstValue(value, amBidrag)));
		}
	}
}