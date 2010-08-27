namespace Maxfire.Skat.UnitTests
{
	public abstract class AbstractFakeSkattelovRegistry : ISkattelovRegistry
	{
		public virtual decimal GetAktieIndkomstLavesteProgressionsgraense(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstHoejesteProgressionsgraense(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstLavesteSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstMellemsteSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAktieIndkomstHoejesteSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetAMBidragSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetSundhedsbidragSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetBundSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetMellemSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetTopSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetSkatteloftSkattesats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetPersonfradrag(int skatteAar, int alder, bool gift)
		{
			return 0m;
		}

		public virtual decimal GetBundLettelseBundfradrag(int skatteAar, int alder, bool gift)
		{
			return 0m;
		}

		public virtual decimal GetMellemLettelseBundfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetTopLettelseBundfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetPersonfradragSkaerpelse(int skatteAar, int alder, bool gift)
		{
			return 0m;
		}

		public virtual decimal GetMellemskatBundfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetTopskatBundfradrag(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetPositivNettoKapitalIndkomstGrundbeloeb(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetNegativNettoKapitalIndkomstGrundbeloeb(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetNegativNettoKapitalIndkomstSats(int skatteAar)
		{
			return 0m;
		}

		public virtual decimal GetBeskaeftigelsesfradragGrundbeloeb(int skatteAar)
		{
			return 0;
		}

		public virtual decimal GetBeskaeftigelsesfradragSats(int skatteAar)
		{
			return 0;
		}

		public virtual decimal GetGroenCheckPrVoksen(int skatteAar)
		{
			return 0;
		}

		public virtual decimal GetGroenCheckPrBarn(int skatteAar)
		{
			return 0;
		}

		public virtual decimal GetGroenCheckAftrapningssats(int skatteAar)
		{
			return 0;
		}

		public virtual decimal GetGroenCheckBundfradrag(int skatteAar)
		{
			return 0;
		}
	}
}