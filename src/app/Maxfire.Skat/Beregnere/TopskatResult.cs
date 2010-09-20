using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public class TopskatResult
	{
		private readonly ISkattelovRegistry _skattelovRegistry;
		private readonly int _skatteAar;
		private readonly IKommunaleSatser _kommunaleSatser;

		public TopskatResult(
			ISkattelovRegistry skattelovRegistry,
			int skatteAar,
			IKommunaleSatser kommunaleSatser,
			decimal grundlagAfPersonligIndkomst, 
			decimal grundlagAfPositivNettoKapitalIndkomst)
		{
			_skattelovRegistry = skattelovRegistry;
			_skatteAar = skatteAar;
			_kommunaleSatser = kommunaleSatser;
			GrundlagAfPersonligIndkomst = grundlagAfPersonligIndkomst;
			GrundlagAfPositivNettoKapitalIndkomst = grundlagAfPositivNettoKapitalIndkomst;
		}

		/// <summary>
		/// Topskattegrundlag af personlig indkomst.
		/// </summary>
		public decimal GrundlagAfPersonligIndkomst { get; private set; }
		
		/// <summary>
		/// Topskattegrundlag af positiv nettokapitalindkomst.
		/// </summary>
		public decimal GrundlagAfPositivNettoKapitalIndkomst { get; private set; }
		
		/// <summary>
		/// Samlet topskattegrundlag.
		/// </summary>
		public decimal Grundlag { get { return GrundlagAfPersonligIndkomst + GrundlagAfPositivNettoKapitalIndkomst; } }

		/// <summary>
		/// Topskat af personlig indkomst.
		/// </summary>
		public decimal TopskatAfPersonligIndkomst
		{
			get
			{
				decimal topskattesats = _skattelovRegistry.GetTopSkattesats(_skatteAar);
				return topskattesats * GrundlagAfPersonligIndkomst;
			}
		}

		/// <summary>
		/// Topskat af positiv nettokapitalindkomst.
		/// </summary>
		public decimal TopskatAfPositivNettoKapitalIndkomst
		{
			get
			{
				decimal topskattesats = _skattelovRegistry.GetTopSkattesats(_skatteAar);
				return topskattesats * GrundlagAfPositivNettoKapitalIndkomst;
			}
		}

		/// <summary>
		/// Samlet topskat uden hensyn til skatteloft.
		/// </summary>
		public decimal Topskat
		{
			get { return (TopskatAfPersonligIndkomst + TopskatAfPositivNettoKapitalIndkomst).RoundMoney(); }
		}

		/// <summary>
		/// Nedslag i topskatten som følge af (det skrå) skatteloft.
		/// </summary>
		public decimal SkatteloftNedslag
		{
			get
			{
				decimal bundskattesats = _skattelovRegistry.GetBundSkattesats(_skatteAar);
				decimal mellemskattesats = _skattelovRegistry.GetMellemSkattesats(_skatteAar);
				decimal topskattesats = _skattelovRegistry.GetTopSkattesats(_skatteAar);
				decimal sundhedsbidragsats = _skattelovRegistry.GetSundhedsbidragSkattesats(_skatteAar);
				decimal kommuneskattesats = _kommunaleSatser != null ? _kommunaleSatser.Kommuneskattesats : 0;

				var skattesats = bundskattesats + mellemskattesats + topskattesats + sundhedsbidragsats + kommuneskattesats;

				decimal skatteloftsats = _skattelovRegistry.GetSkatteloftSkattesats(_skatteAar);
				var nedslagssats = skattesats - skatteloftsats;

				if (nedslagssats <= 0)
				{
					return 0m;
				}

				if (nedslagssats > topskattesats)
				{
					nedslagssats = topskattesats;
				}

				return (nedslagssats * Grundlag).RoundMoney();
			}
		}

		/// <summary>
		/// Topskat efter nedslag som følge af (det skrå) skatteloft.
		/// </summary>
		public decimal TopskatEfterNedslag
		{
			get { return (Topskat - SkatteloftNedslag).RoundMoney(); }
		}
	}
}