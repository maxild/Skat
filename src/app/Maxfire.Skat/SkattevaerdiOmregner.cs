using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public interface ISkattevaerdiOmregner
	{
		decimal BeregnFradragsbeloeb(decimal skattevaerdi);
	}

	public class SkattevaerdiOmregner : ISkattevaerdiOmregner
	{
		private readonly decimal _skattesats;

		public SkattevaerdiOmregner(decimal skattesats)
		{
			_skattesats = skattesats;
		}

		public decimal BeregnFradragsbeloeb(decimal skattevaerdi)
		{
			var underskud = skattevaerdi / _skattesats;
			return underskud.RoundMoney();
		}

		public decimal BeregnSkattevaerdi(decimal fradragsbeloeb)
		{
			var skattevaerdi = fradragsbeloeb * _skattesats;
			return skattevaerdi.RoundMoney();
		}
	}
}