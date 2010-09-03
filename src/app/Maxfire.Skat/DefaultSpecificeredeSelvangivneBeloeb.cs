using System.Linq;

namespace Maxfire.Skat
{
	/// <summary>
	/// Adapter that wraps an ISelvangivneBeloeb so that i behaves like an ISpecficeredeSelvangivneBeloeb
	/// </summary>
	internal class DefaultSpecificeredeSelvangivneBeloeb : ISpecficeredeSelvangivneBeloeb
	{
		private readonly ISelvangivneBeloeb _selvangivneBeloeb;

		public DefaultSpecificeredeSelvangivneBeloeb(ISelvangivneBeloeb selvangivneBeloeb)
		{
			_selvangivneBeloeb = selvangivneBeloeb;
		}

		public decimal PersonligIndkomstAMIndkomst
		{
			get { return _selvangivneBeloeb.PersonligIndkomstAMIndkomst; }
		}

		public decimal PersonligIndkomstEjAMIndkomst
		{
			get { return _selvangivneBeloeb.PersonligIndkomstEjAMIndkomst; }
		}

		public decimal PersonligIndkomstFremfoertUnderskud
		{
			get { return _selvangivneBeloeb.PersonligIndkomstFremfoertUnderskud; }
		}

		public decimal NettoKapitalIndkomst
		{
			get { return _selvangivneBeloeb.NettoKapitalIndkomst; }
		}

		public decimal LigningsmaessigtFradragMinusBeskaeftigelsesfradrag
		{
			get { return _selvangivneBeloeb.LigningsmaessigtFradragMinusBeskaeftigelsesfradrag; }
		}

		public decimal SkattepligtigIndkomstFremfoertUnderskud
		{
			get { return _selvangivneBeloeb.SkattepligtigIndkomstFremfoertUnderskud; }
		}

		public decimal KapitalPensionsindskud
		{
			get { return _selvangivneBeloeb.KapitalPensionsindskud; }
		}

		public decimal PrivatTegnetPensionsindskud
		{
			get { return _selvangivneBeloeb.PrivatTegnetPensionsindskud; }
		}

		public decimal AktieIndkomst
		{
			get { return _selvangivneBeloeb.AktieIndkomst; }
		}

		private IBeloebCollection _personligeIndkomsterAMIndkomster;
		public IBeloebCollection PersonligeIndkomsterAMIndkomster
		{
			get
			{
				return _personligeIndkomsterAMIndkomster ??
				       (_personligeIndkomsterAMIndkomster =
					   PersonligIndkomstAMIndkomst != 0 ? SelvangivneBeloeb.Create(Beloeb.Create("AM-Indkomst", PersonligIndkomstAMIndkomst)) : BeloebCollection.Empty);
			}
		}

		private IBeloebCollection _personligeIndkomsterEjAMIndkomster;
		public IBeloebCollection PersonligeIndkomsterEjAMIndkomster
		{
			get
			{
				return _personligeIndkomsterEjAMIndkomster ??
					   (_personligeIndkomsterEjAMIndkomster =
					   PersonligIndkomstEjAMIndkomst != 0 ? SelvangivneBeloeb.Create(Beloeb.Create("Ej AM-Indkomst", PersonligIndkomstEjAMIndkomst)) : BeloebCollection.Empty);
			}
		}

		private IBeloebCollection _personligeIndkomster;
		public IBeloebCollection PersonligeIndkomster
		{
			get
			{
				return _personligeIndkomster ??
				       (_personligeIndkomster =
				        SelvangivneBeloeb.Create(PersonligeIndkomsterAMIndkomster.Concat(PersonligeIndkomsterEjAMIndkomster)));
			}
		}

		private IBeloebCollection _nettoKapitalIndkomster;
		public IBeloebCollection KapitalIndkomster
		{
			get
			{
				return _nettoKapitalIndkomster ??
				       (_nettoKapitalIndkomster =
					   NettoKapitalIndkomst != 0 ? SelvangivneBeloeb.Create(Beloeb.Create("Nettokapitalindkomst", NettoKapitalIndkomst)) : BeloebCollection.Empty);
			}
		}

		private IBeloebCollection _ligningsmaessigeFradragMinusBeskaeftigelsesfradrag;
		public IBeloebCollection LigningsmaessigeFradragMinusBeskaeftigelsesfradrag
		{
			get
			{
				return _ligningsmaessigeFradragMinusBeskaeftigelsesfradrag ??
				       (_ligningsmaessigeFradragMinusBeskaeftigelsesfradrag =
				        LigningsmaessigtFradragMinusBeskaeftigelsesfradrag != 0 ? SelvangivneBeloeb.Create(Beloeb.Create("Ligningsmæssige fradrag, ekskl. beskæftigelsesfradrag",
						LigningsmaessigtFradragMinusBeskaeftigelsesfradrag)) : BeloebCollection.Empty);
			}
		}


	}
}