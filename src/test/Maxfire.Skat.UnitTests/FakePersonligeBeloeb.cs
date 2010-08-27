namespace Maxfire.Skat.UnitTests
{
	public class FakePersonligeBeloeb : IPersonligeBeloeb
	{
		private SkatteIndkomster _selvangiven;
		public ISkatteIndkomster Selvangiven
		{
			get
			{
				return _selvangiven ?? (_selvangiven = new SkatteIndkomster(
														PersonligIndkomstAMIndkomst,
														PersonligIndkomst,
														NettoKapitalIndkomst,
														LigningsmaessigeFradrag,
														SkattepligtigIndkomst,
														KapitalPensionsindskud,
														AktieIndkomst));
			}
		}

		private SkatteIndkomster _skattegrundlag;
		public ISkatteIndkomster Skattegrundlag
		{
			get
			{
				return _skattegrundlag ?? (_skattegrundlag = new SkatteIndkomster(
																PersonligIndkomstAMIndkomst,
																PersonligIndkomstSkattegrundlag,
																NettoKapitalIndkomstSkattegrundlag,
																LigningsmaessigeFradrag,
																SkattepligtigIndkomstSkattegrundlag,
																KapitalPensionsindskudSkattegrundlag,
																AktieIndkomst));
			}
		}

		public decimal PersonligIndkomstAMIndkomst { get; set; }
		public decimal PersonligIndkomst { get; set; }

		public decimal FremfoertUnderskudPersonligIndkomst { get; set; }
		public decimal UnderskudPersonligIndkomstTilFremfoersel { get; set; }

		private decimal _modregnetUnderskudPersonligIndkomst;
		public decimal ModregnetUnderskudPersonligIndkomst
		{
			get { return _modregnetUnderskudPersonligIndkomst; }
			set
			{
				_modregnetUnderskudPersonligIndkomst = value;
				_skattegrundlag = null;
			}
		}

		public decimal PersonligIndkomstSkattegrundlag
		{
			get { return PersonligIndkomst - ModregnetUnderskudPersonligIndkomst; }
		}

		public decimal NettoKapitalIndkomst { get; set; }

		private decimal _modregnetUnderskudNettoKapitalIndkomst;
		public decimal ModregnetUnderskudNettoKapitalIndkomst
		{
			get { return _modregnetUnderskudNettoKapitalIndkomst; }
			set
			{
				_modregnetUnderskudNettoKapitalIndkomst = value;
				_skattegrundlag = null;
			}
		}

		public decimal NettoKapitalIndkomstSkattegrundlag
		{
			get { return NettoKapitalIndkomst - ModregnetUnderskudNettoKapitalIndkomst; }
		}

		public decimal LigningsmaessigeFradrag { get; set; }

		public decimal SkattepligtigIndkomst
		{
			get { return PersonligIndkomst + NettoKapitalIndkomst - LigningsmaessigeFradrag; }
		}

		public decimal FremfoertUnderskudSkattepligtigIndkomst { get; set; }
		public decimal UnderskudSkattepligtigIndkomstTilFremfoersel { get; set; }

		private decimal _modregnetUnderskudSkattepligtigIndkomst;
		public decimal ModregnetUnderskudSkattepligtigIndkomst
		{
			get { return _modregnetUnderskudSkattepligtigIndkomst; }
			set
			{
				_modregnetUnderskudSkattepligtigIndkomst = value;
				_skattegrundlag = null;
			}
		}
		public decimal SkattepligtigIndkomstSkattegrundlag
		{
			get { return SkattepligtigIndkomst - ModregnetUnderskudSkattepligtigIndkomst; }
		}

		public decimal KapitalPensionsindskud { get; set; }

		private decimal _modregnetUnderskudKapitalPensionsindskud;
		public decimal ModregnetUnderskudKapitalPensionsindskud
		{
			get { return _modregnetUnderskudKapitalPensionsindskud; }
			set
			{
				_modregnetUnderskudKapitalPensionsindskud = value;
				_skattegrundlag = null;
			}
		}
		
		public decimal KapitalPensionsindskudSkattegrundlag
		{
			get { return KapitalPensionsindskud - ModregnetUnderskudKapitalPensionsindskud; }
		}

		public decimal AktieIndkomst { get; set; }
	}
}